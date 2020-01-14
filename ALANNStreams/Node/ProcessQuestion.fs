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

module ProcessQuestion

open Types
open Choice
open NodeFunctions
open TermUtils

let (|Selective|NonSelective|) t = if isSelective t then Selective else NonSelective

let processQuestion attention state (event : Event) =

    //if state.Term = event.Term && event.Stamp.Source = User then
    //    state.HostUserQuestion <- Some event

    match event.Term with    
    | NonSelective ->                                      
        if state.Term = event.Term then
            tryPrintAnswer event state.HostBelief
            [makeAnsweredEventBelief attention event state.HostBelief]
        else 
            getInferenceBeliefs attention state event

    | Selective ->
        if state.Term = event.Term && not(containsVars state.HostBelief.Term) && not(containsVars event.Term) then
            tryPrintAnswer event state.HostBelief
            [makeAnsweredEventBelief attention event state.HostBelief]
        else // not host term so send event to host to get host truth
            match selectiveAnswer state event with
            | Some belief ->
                [makeEventBelief attention {event with Term = belief.Term} belief]                
            | _ -> 
                getInferenceBeliefs attention state event
