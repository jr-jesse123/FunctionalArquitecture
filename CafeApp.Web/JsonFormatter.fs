module JsonFormatter
open Newtonsoft.Json.Linq

let (.=) key (value : obj) = new JProperty(key, value)


/// Cria um objeto com base em uma lista de outros objetos que serão propriedades
let jobj (jProperties) =
   let jObject = new JObject()
   jProperties |> List.iter jObject.Add
   jObject

///Cria um json array com base em uma lista de JToken
let jArray jObjects =
   let jArray = new JArray()
   jObjects |> List.iter jArray.Add
   jArray


open Domain


let tabIdJObj tabId = 
   jobj [
      "tabId" .= tabId
   ]

let tabJObj tab = 
   jobj [
      "id" .= tab.Id
      "tableNumber" .= tab.TableNumber
   ]

let itemJObj item = 
   jobj [
      "menuNumber" .= item.MenuNumber
      "name" .= item.Name
   ]

let foodJObj (Food item) = itemJObj item
let drinkJObj (Drink item) = itemJObj item

let foodJArray foods = 
   foods |> List.map foodJObj |> jArray

let drinkJArray drinks = 
   drinks |> List.map drinkJObj |> jArray

let orderJObj (order:Order) =
   jobj [
      "tabId" .= order.Tab.Id
      "tableNumber" .= order.Tab.TableNumber
      "foods" .= foodJArray order.Foods
      "drinks" .= drinkJArray order.Drinks
   ]


let orderInProgressJobj ipo = 
   jobj [
      "tabId" .= ipo.PlacedOrder.Tab.Id
      "TableNumber" .= ipo.PlacedOrder.Tab.TableNumber
      "preparedFoods" .= foodJArray ipo.PreparedFoods
      "servedFoods" .= foodJArray ipo.ServedFoods
      "servedDrinks" .= drinkJArray ipo.ServedDrinks
   ]

[<AbstractClass>]
type JObjFormater =
   static member CreateJobj tabid = 
      tabIdJObj tabid

   static member CreateJobj tab = 
      tabJObj tab

   static member CreateJobj item = 
      itemJObj item

   static member CreateJobj food = 
      foodJObj food

   static member CreateJobj drink = 
      drinkJObj drink

   static member CreateJobj drinks = 
      drinkJArray drinks

   static member CreateJobj foods = 
      foodJArray foods

   static member CreateJobj order = 
      orderJObj order
   
   static member CreateJobj orderInProgres = 
      orderInProgressJobj orderInProgres
   



open States

let stateJObj = function
|ClosedTab tabId ->
   let state = "state" .= "ClosedTab"
   match tabId with 
   | Some id ->
      jobj [state; "data" .= tabIdJObj id]
   | None -> jobj  [state]

| OpenedTab tab ->
   jobj [
      "state" .= "OpenedTab"
      "data" .= tabJObj tab
   ]

| PlacedOrder order ->
   jobj[
      "state" .= "PlacedOrder"
      "data" .= orderJObj order
   ]

| OrderInProgress oip ->
   jobj[
      "sate" .= "OrderInProgress"
      "data" .= OrderInProgress oip
   ]

| ServedOrder order ->
   jobj [
      "state" .= "ServedOrder"
      "data" .= orderJObj order
   ]

open Suave
open Suave.Operators
open Suave.Successful
let JSON webpart jsonString (context:HttpContext) = async{
   let wp = 
      webpart jsonString >=> 
         Writers.setMimeType "application/json; charset=utf-8"

   return! wp context
}

let toStateJson state = 
   state |> stateJObj |> string |> JSON OK
   
   

open CommandHandler
open Suave.RequestErrors

let toErrorJson err = 
   jobj["error" .= err]
   |> string |> JSON BAD_REQUEST