module PlaceOrder
open Domain
open FSharp.Data
open Queries

[<Literal>]
let PlaceOrderJson = """{
   "placeOrder" : {
      "tabId" : "2a964d85-f503-40a1-8014-2c8ee5ac4a49",
      "foodMenuNumbers"  : [8,9],
      "drinkMenunumbers" : [10,11]
   }
}"""

type PlaceorderReq = JsonProvider<PlaceOrderJson>
let (|PlaceOrderRequest|_|) payload = 
   try
      PlaceorderReq.Parse(payload).PlaceOrder |> Some
   with
      ex -> None
   

let validatePlaceOrder (queries:Queries) (c:PlaceorderReq.PlaceOrder) = async{
   let! table = queries.Table.GetTableByTabid c.TabId
   match table with
   |  Some table ->
      let! foods =  queries.Food.GetFoodsByMenuNumbers c.FoodMenuNumbers
      let! drinks = queries.Drink.GetDrinksByMenuNumbers c.DrinkMenunumbers
      let isEmptyOrder foods drinks = 
         List.isEmpty foods && List.isEmpty drinks 

      match foods, drinks with 
      |Choice1Of2 foods, Choice1Of2 drinks ->
         if isEmptyOrder foods drinks then
            let msg = "order Should Contain atleast 1 food or drinks"
            return Choice2Of2 msg
         else
            let tab = {Id = c.TabId; TableNumber = table.Number}
            return Choice1Of2 (tab,drinks,foods)
      |Choice2Of2 keys, _ ->
         let msg = sprintf "invalid food Keys : %A" keys
         return Choice2Of2 msg

      | _, Choice2Of2 keys ->
         let msg = sprintf "Invalid Drinks Keys : %A" keys
         return Choice2Of2 msg
   | _ -> return Choice2Of2 "Invalid tab Id"
   }
         
open CommandHandler
open Commands
open Domain

let toPlaceorderCommand (tab, drinks, foods) = 
   {
      Tab = tab
      Foods = foods
      Drinks = drinks
   } |> PlaceOrder


let placeOrderCommander queries = {
   Validate = validatePlaceOrder queries
   ToCommand = toPlaceorderCommand
}