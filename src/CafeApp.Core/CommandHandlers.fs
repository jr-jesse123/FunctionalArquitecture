module CommandHandlers
open States
open Events
open System
open Domain
open Commands
open Errors

let (|ServeDrinkCompletesIPOorder|_|) ipo drink = 
   match isServinDrinkCompletesIPOorder ipo drink with
   |true -> Some drink
   |false -> None


let (|ServeFoodCompletesIPOorder|_|) ipo food = 
   match isServinFoodCompletesIPOorder ipo food with
   |true -> Some food
   |false -> None


let (|NonPreparedFood|_|) ipo food = 
  match List.contains food ipo.PreparedFoods with
  | true -> None
  | false -> Some food

let (|AlreadyServedFood|_|) ipo food =
  match List.contains food ipo.ServedFoods with
  | true -> Some food
  | false -> None


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


let (|AlreadyServedDrink|_|) ipo drink =
   match List.contains drink ipo.ServedDrinks with
   | true -> Some drink
   | false -> None

let (|AlreadyPreparedFood|_|) ipo food =
   match List.contains food ipo.PreparedFoods with
   | true -> Some food
   | false -> None

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

let payment (order :Order) = 
   {Tab = order.Tab; Amount = orderAmount order}

let handleServeDrink drink tabid state= 
   match state with
   | PlacedOrder order ->
      let event = DrinkServed (drink,tabid)
      match drink with
         |NonOrderedDrink order _ ->
            CanNotServeNonOrderedDrink drink |> Error

         |ServeDrinCompletesOrder order _ ->
            let payment = {Tab= order.Tab; Amount = orderAmount order}
            event :: [OrderServed (order,payment)] |> Ok
         | _ -> [event] |> Ok

      //[DrinkServed (drink, tabid)] |> Ok
   | ServedOrder _ -> OrderAlreadyServed |> Error
   | OpenedTab _ -> CanNotServeForNonPlacedOrder |> Error
   | ClosedTab _ -> CanNotServeWithClosedTab |> Error
   | OrderInProgress ipo -> 
      let drinkdServed = DrinkServed (drink, ipo.PlacedOrder.Tab.Id)
      let order = ipo.PlacedOrder
      match drink with
      |AlreadyServedDrink ipo drink -> 
         CanNotServeAlreadyServedDrink drink |> Error
      |NonOrderedDrink order _ ->
         CanNotServeNonOrderedDrink drink |> Error

      | ServeDrinkCompletesIPOorder ipo drink ->
            //orderAmount order
         [drinkdServed; OrderServed (ipo.PlacedOrder, payment order )]|> Ok

      //| ServeFoodCompletesIPOorder
      | _ -> [drinkdServed] |> Ok
   
   | _ -> 
      sprintf "HandleDrink não implementado para o estado %A" state
      |> failwith 



let handlePrepareFood food tabId state =
   match state with 
   | PlacedOrder order -> 
      match food with
      |NonOrderedFood order food' -> CanNotPrepareNonOrderedFood food' |> Error
      | _ ->  [FoodPrepared (food, tabId)] |> Ok

   //| OpenedTab _ -> CanNotPrepareForNonPlacedOrder |> Error
   | ClosedTab _ -> CanNotPrepareWithClosedTab |> Error
   | ServedOrder _ -> OrderAlreadyServed |> Error
   | OrderInProgress ipo ->
      let order = ipo.PlacedOrder
      match food with
      | NonOrderedFood order _ ->
         CanNotPrepareNonOrderedFood food |> Error
      | AlreadyPreparedFood ipo _ ->
         CanNotPrepareAlreadyPreparedFood food |> Error
      | _ -> [FoodPrepared (food, tabId)] |> Ok
   | outroEstado -> 
      sprintf "Comando preparar Comida não implementado para o stado %A" state
      |> failwith 
  
let handleServeFood food tabId state =
   match state with
   | OrderInProgress iop ->
      let events =  [FoodServed (food , tabId)]
      match food with
      | NonOrderedFood iop.PlacedOrder food ->
         CanNotServeNonOrderedFood food |> Error
      | NonPreparedFood iop food -> 
         CanNotServeNonPreparedFood food |> Error
      | AlreadyServedFood iop food ->
         CanNotServeAlreadyServedFood food |> Error
      | ServeFoodCompletesIPOorder iop food ->
         events @ [(OrderServed (iop.PlacedOrder, payment iop.PlacedOrder))] //TODO: COLOCAR TESTE PARA ORDEM INVERSA
         |> Ok

      | _ -> events |> Ok
   | _ ->
      sprintf "handleServeFood não implementado para o stado %A" state
           |> failwith 

let handleCloseTab payment' state =
   match state with
   |State.ServedOrder Order ->
      match (payment Order).Amount = payment'.Amount with
      | true ->  [TabClosed payment'] |> Ok
      | false ->  InvalidPayment ((payment Order).Amount,  payment'.Amount  ) |> Error
      //[TabClosed payment'] |> Ok
   | _ -> 
      CanNotPayForNonServedOrder |> Error
   //|  _ -> 
   //   sprintf "Close tab não implementado para o stado %A" state
   //   |> failwith
   

//TODO: APÓS FINALIZADO O LIVRO, VERIFICAR A POSSIBILIDADE DE RETIRAR O A CCHESSIE LIB
///Executa o comando solicitado e responde com os eventos consequentes
let execute state command =
   match command with
   | OpenTab tab -> handleOpenTab tab state
   | PlaceOrder order -> HandlePlaceOrder order state
   | ServeDrink (drink, tabId) -> handleServeDrink  drink tabId state
   | PrepareFood (food, tabId) -> handlePrepareFood food tabId state
   | ServeFood (food, tabId) -> handleServeFood food tabId state
   |  CloseTab pay -> handleCloseTab pay state
   | _ -> 
      sprintf "O Comando %A não foii implementado para o estado %A" command state
      |> failwith 



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
