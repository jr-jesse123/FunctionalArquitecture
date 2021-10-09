module States

open Domain
open Events
open System

type State =
| ClosedTab of Guid option
| OpenedTab of Tab
| PlacedOrder of Order
| OrderInProgress of InProgressOrder
| ServedOrder of Order

///Computa o novo estado da aplicação de acordo com o estado anterior e o evento recebido
let apply state event =
   match state,event with
   | ClosedTab _ ,  TabOpened tab -> OpenedTab tab
   | OpenedTab _, OrderPlaced order -> PlacedOrder order
   | PlacedOrder order, DrinkServed (item , _) ->
      InProgressOrder.Create(order, [item] ,[] ,[]) |> OrderInProgress
   | PlacedOrder order, FoodPrepared (food, tabId) ->
      InProgressOrder.Create(order, [],
         servedFoodList= [],
         preparedFoodList= [food]) 
         |> OrderInProgress

   | OrderInProgress ipo, DrinkServed (drink, _) -> 
      {ipo with ServedDrinks = drink :: ipo.ServedDrinks  }
      |> OrderInProgress

   

   | OrderInProgress ipo, FoodServed (food, _) ->
      {ipo with ServedFoods = food :: ipo.ServedFoods}
      |> OrderInProgress

   | OrderInProgress ipo, OrderServed (order, _) -> 
      ServedOrder order

   | _ -> 
      sprintf "Não existe um próximo estado para %A quando ocorre o evento %A" state event
      |> failwith 
   | _ -> state

   //| _ -> ClosedTab None