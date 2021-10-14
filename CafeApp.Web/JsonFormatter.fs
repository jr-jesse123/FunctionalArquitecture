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
   jobj ["error" .= err]
   |> string |> JSON BAD_REQUEST


open ReadModel
let statusJobj = function
| Open tabId -> 
   "status" .= jobj ["open" .= tabId.ToString()]
| InService tabId ->
   "status" .= jobj [ "inService" .= tabId.ToString() ]
| Closed -> "status" .= "closed"


let tableJObj table =
   jobj [
      "number" .= table.Number
      "waiter" .= table.Waiter
      statusJobj table.Status
   ]

let toReadModelsJson toJObj key models = 
   models
   |> List.map toJObj |> jArray
   |> (.=) key |> List.singleton |> jobj
   |> string |> JSON OK


let toTablesJSON = toReadModelsJson tableJObj "tables"


let chefToDOJObj (todo: ChefToDo)  = 
   jobj [
      "tabId" .= todo.Tab.Id.ToString()
      "tableNumber" .= todo.Tab.TableNumber
      "foods" .= foodJArray todo.Foods
   ]

let toChefToDosJSON = 
   toReadModelsJson chefToDOJObj "chefToDos"

let waiterToDoObj todo = 
   jobj [
      "tabId" .= todo.Tab.Id.ToString()
      "tableNumber" .= todo.Tab.TableNumber
      "foods" .= foodJArray todo.Foods
      "drinks" .= drinkJArray todo.Drinks
   ]

let toWaiterToDosJSON =
   toReadModelsJson waiterToDoObj "waiterToDos"

let cashierToDoJObj (payment: Payment) =
   jobj [
      "tabId" .= payment.Tab.Id.ToString()
      "tableNumber" .= payment.Tab.TableNumber
      "paymentAmount" .= payment.Amount
   ]

let toCashierToDosJSON = 
   toReadModelsJson cashierToDoJObj  "cashierToDos"


let toFoodsJSON =
   toReadModelsJson foodJObj "foods"


let toDrinksJSON =
   toReadModelsJson drinkJObj "drinks"

open Events

let eventJObj =  function
| TabOpened tab ->
   jobj [
      "event" .= "TabOpened"
      "data" .= tabJObj tab
   ]

| OrderPlaced order ->
   jobj [
      "event" .= "OrderPlaced"
      "data" .= jobj [
         "order" .= orderJObj order
      ]
   ]


| DrinkServed (item,tabId) ->
   jobj  [
      "event"   .= "DrinkServed"
      "data" .= jobj [
         "drink".= drinkJObj item
         "tabId" .= tabId
      ]
   ]

| FoodPrepared (item,tabId) ->
   jobj [
      "event" .= "FoodPrepared"
      "data" .= jobj [
         "food" .= foodJObj item
         "tabId" .= tabId
      ]
   ]

| OrderServed (order, payment) ->
   jobj [
      "event" .= "OrderServed"
      "data" .= payment.Tab.Id
      "tableNumber" .= payment.Tab.TableNumber
      "amount" .= payment.Amount
   ]

| TabClosed payment ->
   jobj [
      "event" .= "TabClosed"
      "data" .= jobj [
         "amountPaid" .= payment.Amount
         "tabId" .= payment.Tab.TableNumber
      ]
   ]