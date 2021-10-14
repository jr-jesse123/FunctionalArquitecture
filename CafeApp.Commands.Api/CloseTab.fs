module CloseTab
open FSharp.Data

[<Literal>]
let CloseTabJson = """{
"closeTab" : {
"tabId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
"amount" : 10.1
}
}"""

type CloseTabReq = JsonProvider<CloseTabJson>

let (|CloseTabRequest|_|) payload =
   try
      let req = CloseTabReq.Parse(payload).CloseTab
      (req.TabId, req.Amount) |> Some
   with
      | ex -> None


open Domain
open ReadModel

let validateCloseTab getTableByTabId (tabId,amout) = async{
   let! table = getTableByTabId tabId
   match table with
   |Some (table:Table) ->
      let tab = {Id=tabId; TableNumber = table.Number}
      return Choice1Of2 {Amount = amout; Tab = tab}
   | _ -> return Choice2Of2 "Iinvalid Tab ID"
}

open CommandHandler
open Commands
let closeTabCommander getTableByTabId = {
   Validate = validateCloseTab getTableByTabId
   ToCommand = CloseTab
}