module ServeFood
open FSharp.Data


[<Literal>]
let ServeFoodJson = """{
   "serveFood" : {
      "tabId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
      "menuNumber" : 10
   }
}"""


type ServeFoodReq = JsonProvider<ServeFoodJson>

let (|ServeFoodRequest|_|) payload = 
   try
      let req = ServeFoodReq.Parse(payload).ServeFood
      (req.TabId,req.MenuNumber) |> Some
   with ex -> None

let validateServeFood getTableByTabId getFoodByMenuNumber (tabId,foodsMenuNumber) = async{
   let! table = getTableByTabId tabId
   match table with
   | Some _ ->
      let! foods = getFoodByMenuNumber foodsMenuNumber
      match foods with
      |Some d ->
         return Choice1Of2 (d,tabId)
      | _ -> return Choice2Of2 "Invalid Foods Menu Number"
   | _ -> return Choice2Of2 "invalid Tab Id"
}
   
open Commands
open CommandHandler



let servFoodCommander getTableByTabId getFoodssByMenuNumber =
   let validate = 
      getFoodssByMenuNumber 
      |> validateServeFood getTableByTabId
   {
      Validate = validate
      ToCommand = ServeFood
   }

   
