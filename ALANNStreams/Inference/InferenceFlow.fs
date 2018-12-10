﻿ (*
 * The MIT License
 *
 * Copyright 2018 The ALANN2018 authors.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 *)

module InferenceFlow

open System
open Akka.Streams.Dsl
open Akka.Streams
open Akkling.Streams
open Types
open InferenceFlowModules
open TermUtils
open TermFormatters
open ALANNSystem
open Akkling

let inferenceFlow = GraphDsl.Create(fun builder ->
    let broadcast = builder.Add(Broadcast<EventBelief>(2))
    let merge = builder.Add(Merge<Event>(2))


    let firstOrderFilter =
        Flow.Create<EventBelief>()
        |> Flow.filter (fun eb -> isFirstOrder (eb.Event.Term))

    let higherOrderFilter =
        Flow.Create<EventBelief>()
        |> Flow.filter (fun eb -> (not (isFirstOrder (eb.Event.Term)) && (isFirstOrder (eb.Belief.Term))))

    let groupAndDedupe =
        builder.Add(
            Flow.Create<Event>()
            |> Flow.groupedWithin (Params.MINOR_BLOCK_SIZE) (TimeSpan.FromMilliseconds(Params.GROUP_DELAY_MS))
            |> Flow.map Seq.distinct
            |> Flow.collect (fun events -> events))    

    builder
        .From(broadcast.Out(0))
        .Via(firstOrderFilter)
        .Via((inferenceFlowModules firstOrderModules).Async())
        .To(merge.In(0))
        .From(broadcast.Out(1))
        .Via(higherOrderFilter)
        .Via((inferenceFlowModules higherOrderModules).Async())
        .To(merge.In(1))
        .From(merge)
        .To(groupAndDedupe)
        |> ignore

    FlowShape<EventBelief, Event>(broadcast.In, groupAndDedupe.Outlet))