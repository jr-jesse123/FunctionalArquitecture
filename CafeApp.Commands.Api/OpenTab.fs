module OpenTab
open Domain
open CommandHandler
(*
#r "nuget: FSharp.Data"
*)
open FSharp.Data
open System.IO
open System
open ReadModel

[<Literal>]
let OpenTabJson = """{
"openTab" : {
"tableNumber" : 1
}
}"""

type OpenTabReq = JsonProvider<OpenTabJson>


let (|OpenTabRequest|_|) payload =
   try
      let req = OpenTabReq.Parse(payload).OpenTab
      {Id = Guid.NewGuid(); TableNumber = req.TableNumber}
      |> Some
   with
   | ex -> None


let validateOpenTab (getTableByTablenumber(*: int -> Table option*)) tab = async{
   match! getTableByTablenumber tab.TableNumber with
   | Some table -> return (Choice1Of2 tab)
   | _ -> return Choice2Of2 "Invalid table Number"
}

open Commands


let openTabCommander getTableByTableNumber = {
   Validate = validateOpenTab getTableByTableNumber
   ToCommand = OpenTab
}