module CommandHandlers
open States
open Events
open System
open Domain
open Commands
open Errors


let (|ServeDrinCompletesOrder|_|) order drink = 
   if isServinDrinkCompletesOrder order drink then
      Some drink
   else
      None
   

let (|NonOrderedDrink|_|) order drink = 
   match List.contains drink order.Drinks with
   |false -> Some Drink
   | true -> None

let (|NonOrderedFood|_|) order food = 
   match List.contains food order.Foods with
   |false -> Some food
   |true -> None

let handleOpenTab tab = function
| ClosedTab _ -> [TabOpened tab] |> Ok
| _ -> TabAlreadyOpened |> Error

let HandlePlaceOrder order = function
|OpenedTab _ -> 
   match order with
   | {Foods=[]; Drinks=[];} -> Error CanNotPlaceEmptyOrder 
   | _ -> [OrderPlaced order] |> Ok

| ClosedTab _ -> Error CanNotOrderWithClosedTab
| _ -> Error OrderAlreadyPlaced


let handleServeDrink drink tabid state= 
   match state with
   | PlacedOrder Order ->
      let event = DrinkServed (drink,tabid)
      match drink with
         |NonOrderedDrink Order _ ->
            CanNotServeNonOrderedDrink drink |> Error
         |ServeDrinCompletesOrder Order _ ->
            let payment = {Tab= Order.Tab; Amount = orderAmount Order}
            event :: [OrderServed (Order,payment)] |> Ok

         | _ -> [event] |> Ok

      //[DrinkServed (drink, tabid)] |> Ok
   | ServedOrder _ -> OrderAlreadyServed |> Error
   | OpenedTab _ -> CanNotServeForNonPlacedOrder |> Error
   | ClosedTab _ -> CanNotServeWithClosedTab |> Error
   
   | _ -> failwith "TODO"


let handlePrepareFood food tabId state =
   match state with 
   | PlacedOrder order -> 
      match food with
      |NonOrderedFood order food' -> CanNotPrepareNonOrderedFood food' |> Error
      | _ ->  [FoodPrepared (food, tabId)] |> Ok

   //| OpenedTab _ -> CanNotPrepareForNonPlacedOrder |> Error
   | ClosedTab _ -> CanNotPrepareWithClosedTab |> Error
   | ServedOrder _ -> OrderAlreadyServed |> Error
   | outroEstado -> 
      sprintf "Comando preparar Comida não implementado para o stado %A" state
      |> failwith 
   

//TODO: APÓS FINALIZADO O LIVRO, VERIFICAR A POSSIBILIDADE DE RETIRAR O A CCHESSIE LIB
///Executa o comando solicitado e responde com os eventos consequentes
let execute state command =
   match command with
   | OpenTab tab -> handleOpenTab tab state
   | PlaceOrder order -> HandlePlaceOrder order state
   | ServeDrink (drink, tabId) -> handleServeDrink  drink tabId state
   | PrepareFood (food, tabId) -> handlePrepareFood food tabId state
   | _ -> failwith <| sprintf "O Comando %A não foii implementado para o estado %A" command state



/// <summary>
/// Executa o comando, E altera o estado De acordo com os eventos resultantes do Comando
/// </summary>
/// <param name="state"></param>
/// <param name="command"></param>
let evolve state command =
   match execute state command with
   | Ok (events) -> 
      let newState = List.fold States.apply state events //TODO: ASSEGURAR QUE ESTE S ESTÁ CORRETO
      (newState, events) |> Ok
   | Error err -> Error err
