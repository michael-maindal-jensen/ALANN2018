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

module TermFormatters

open Types
open ActivePatterns

let truth tv = sprintf "{%.2f %.2f}" tv.F tv.C
let av av = sprintf "[%.2f %.2f]" av.STI av.LTI

let Trail trail = 
    match trail with
    | [] -> "[]"
    | _ -> "[" + List.reduce (fun x y -> x + " " + y) (List.map (fun x -> x.ToString()) trail) + "]" 
        
let rec ft t = 
    let separateList lst sep = 
        match lst with
        | [] -> ""
        | _ -> List.reduce (fun x y -> x + sep + y) (List.map (fun x -> ft x) lst )

    match t with 
    | Not(t) -> "--" + ft t
    | And(lst) -> "(" + separateList lst " && " + ")"
    | Or(lst) -> "(" + separateList lst " || " + ")"
    | Imp(s, p) -> "<" + ft s + " ==> " + ft p + ">"
    | PreImp(s, p, i) -> "[" + i.ToString() + "]<" + ft s + " =+> " + ft p + ">"
    | ConImp(s, p) -> "<" + ft s + " =|> "+ ft p + ">"
    | RetImp(s, p, i) -> "[" + i.ToString() + "]<" + ft s + " =-> " + ft p + ">"
    | Equ(s, p) -> "<" + ft s + " <=> " + ft p + ">"
    | ConEqu(s, p, i) -> "<" + ft s + " <|> "+ ft p + ">"
    | PreEqu(s, p, i) -> "<" + ft s + " <+> " + ft p + ">"
    | Inh(s, p) -> "<" + ft s + " --> " + ft p + ">"
    | Sim(s, p) -> "<" + ft s + " <-> " + ft p + ">"
    | Oper(lst) -> "^(" + separateList lst " " + ")"
    | ExtSet(lst) -> "{" + separateList lst " " + "}"
    | IntSet(lst) -> "[" + separateList lst " " + "]"
    | ExtInt(lst) -> "(" + separateList lst " & " + ")"
    | IntInt(lst) -> "(" + separateList lst " | " + ")"
    | ExtDif(a, b) -> "(" + ft a + " - " + ft b + ")"
    | IntDif(a, b) -> "(" + ft a + " ~ " + ft b + ")"
    | Prod(a, b) -> "(" + separateList [a; b] " * " + ")"
    | Par(a, b, _) -> "(" + separateList [a; b] "; " + ")"
    | Seq(a, b, i) -> "(" + separateList [a; b] ", " + ")"
    | ExtImg(a, b, c) -> "(" + ft a + " / "+ separateList [b; c] " " + ")"
    | IntImg(a, b, c) -> "(" + ft a + " \\ "+ separateList [b; c] " " + ")"
    | Word(c) ->  c
    | Var(IVar, c) -> "$" + c
    | Var(DVar, c) -> "#" + c
    | Var(QVar, c) -> "?" + c
    | _ -> "Unknown type Error: " + t.ToString()

let formatEvent e = sprintf "%A %s %s %s %s" e.EventType (av e.AV) (match e.TV with | Some tv -> truth tv | _ -> "None") (ft e.Term) (Trail e.Stamp.Evidence)

let formatBelief (b : Belief) = sprintf "@%07d %s %s %s" (b.Stamp.OccurenceTime) (truth b.TV) (ft b.Term) (Trail b.Stamp.Evidence) 
let formatGoal (g : Belief) = sprintf "@%07d %s %s! %s" (g.Stamp.OccurenceTime) (truth g.TV) (ft g.Term) (Trail g.Stamp.Evidence) 