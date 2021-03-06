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
 
module CommandParser

open FParsec
open Types
open Events
open Parser

let file_name = many1CharsTill anyChar (skipNewline <|> eof)
let term_ws = pterm

let show_simple_beliefs = (str_ws "SHOW_SIMPLE_BELIEFS" <|> str_ws "SSB") >>. term_ws |>> fun t -> Show_Simple_Beliefs(t)
let show_temporal_beliefs = (str_ws "SHOW_TEMPORAL_BELIEFS" <|> str_ws "STB") >>. term_ws |>> fun t -> Show_Temporal_Beliefs(t)
let show_hypothesis_beliefs = (str_ws "SHOW_HYPOTHESIS_BELIEFS" <|> str_ws "SHB") >>. term_ws |>> fun t -> Show_Hypothesis_Beliefs(t)
let show_generalised_beliefs = (str_ws "SHOW_GENERALISED_BELIEFS" <|> str_ws "SGB") >>. term_ws |>> fun t -> Show_Generalised_Beliefs(t)
let show_goals = (str_ws "SHOW_GOALS" <|> str_ws "SGO") |>> fun _ -> Show_Goals
let show_node = (str_ws "SHOW_NODE" <|> str_ws "SN") >>. term_ws |>> fun t -> Show_Node(t)

let node_count = (str_ws "NODE_COUNT" <|> str_ws "NC") |>> fun _ -> Node_Count
let enable_trace = (str_ws "ENABLE_TRACE" <|> str_ws "ET") >>. term_ws |>> fun t -> Enable_Trace(t)
let disable_trace = (str_ws "DISABLE_TRACE" <|> str_ws "DT") >>. term_ws  |>> fun t -> Disable_Trace(t)
let save_graph = (str_ws "SAVE_GRAPH" <|> str_ws "SGR") |>> fun _ -> Save_Graph
let pause = (str_ws "PAUSE" <|> str_ws "P") |>> fun _ -> Pause
let continue_flow = (str_ws "CONTINUE" <|> str_ws "C") |>> fun _ -> Continue
let load = (str_ws "LOAD" <|> str_ws "L") >>. file_name |>> fun file -> Load(file)
let save = (str_ws "SAVE" <|> str_ws "S") >>. file_name |>> fun file -> Save(file)
let reset = (str_ws "RESET" <|> str_ws "R") |>> fun _ -> Reset

let pcmd = show_simple_beliefs 
           <|> show_temporal_beliefs 
           <|> show_hypothesis_beliefs
           <|> show_generalised_beliefs
           <|> show_goals
           <|> show_node 
           <|> node_count 
           <|> enable_trace 
           <|> disable_trace 
           <|> save_graph
           <|> pause 
           <|> continue_flow 
           <|> load 
           <|> save 
           <|> reset

// General Parser entry point
let commandParser(command:string) =
    vNum <- 0; vMap.Clear(); 
    match run pcmd command with
    | Success(result, _, _)   -> 
        result
    | Failure(errorMsg, e, s) -> 
        raiseParseErrorEvent errorMsg
        Command.Unknown
        