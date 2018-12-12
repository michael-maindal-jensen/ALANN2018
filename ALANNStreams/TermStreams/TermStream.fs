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

module TermStream

open System
open Akkling
open Akkling.Streams
open Akka.Streams.Dsl
open Akka.Streams
open Types
open Node
open InferenceFlow
open NodeFunctions

let termStream (i) =
    GraphDsl.Create(
        fun builder ->
            let processEvent {Term = t; Event = e} = 
                match stores.[i].TryGetValue(t) with
                | (true, node) ->
                    let (node', ebs) = processEvent node e
                    match stores.[i].TryUpdate(t, node', node) with
                    | false -> 
                        failwith "ProcessEvent failed with node update"
                        []
                    | _ -> ebs
                | (false, _) -> 
                    failwith "ProcessEvent failed with node get"
                    []

            let createNode {Term = t; Event = e} = stores.[i].TryAdd(t, createNode (t, e)) |> ignore; {Term = t; Event = e}
            let preferCreatedTermMerge = builder.Add(MergePreferred<TermEvent>(1))
            let partitionExistingTerms = builder.Add(Partition<TermEvent>(2, fun {Term = t} -> if stores.[i].ContainsKey(t) then 1 else 0))
            let partitionEvents = builder.Add(Partition<EventBelief>(2, fun {Event = e} -> match e.ProcessType with | InferenceReq -> 0 | Inference -> 1 | _ -> failwith "partition error"))
            let mergeEvents = builder.Add(Merge<Event>(2))
            let makeEventFromEventBelief eb = 

                match eb.Belief.Stamp.Source with 
                | Virtual -> 
                    eb.Event
                | _ -> 
                    {Term = eb.Belief.Term
                     AV = eb.AV
                     EventType = Belief
                     ProcessType = eb.Event.ProcessType
                     TV = Some eb.Belief.TV
                     Stamp = eb.Belief.Stamp
                     Solution = eb.Event.Solution}

            let converter = 
                Flow.Create<EventBelief>() 
                |> Flow.map (fun eb -> makeEventFromEventBelief eb)

            let createableNode = function 
                | {Term = _; Event = e} when e.EventType = Belief -> true
                | _ -> false
            
            let create = 
                Flow.Create<TermEvent>()
                |> Flow.filter createableNode
                |> Flow.map createNode

            let processEvent = 
                (Flow.Create<TermEvent>()
                |> Flow.map processEvent).Async()
            
            let collector =
                Flow.Create<EventBelief list>()
                |> Flow.collect (fun ebs -> ebs)

            let groupAndDelay =
                Flow.Create<TermEvent>()
                |> Flow.groupedWithin (Params.MINOR_BLOCK_SIZE) (TimeSpan.FromMilliseconds(Params.CYCLE_DELAY_MS))
                |> Flow.delay(System.TimeSpan.FromMilliseconds(Params.CYCLE_DELAY_MS))
                |> Flow.collect (fun events -> events)

            let deriver = builder.Add(inferenceFlow.Async())

            builder
                .From(preferCreatedTermMerge)
                .To(partitionExistingTerms)
                .From(partitionExistingTerms.Out(0))
                .Via(create)
                .To(preferCreatedTermMerge.Preferred)
                .From(partitionExistingTerms.Out(1))
                .Via(groupAndDelay)
                .Via(processEvent)
                .Via(collector)
                .To(partitionEvents)
                .From(partitionEvents.Out(1))
                .Via(deriver)
                .To(mergeEvents.In(1))
                .From(partitionEvents.Out(0))
                .Via(converter)
                .To(mergeEvents.In(0))
                |> ignore

            FlowShape<TermEvent, Event>(preferCreatedTermMerge.In(0), mergeEvents.Out)
        ).Named("TermStream")