module Projections

open Events
open Domain
open System

//ACTIONS SÃO COMO COMANDOS POR MUDAREM O ESTADO DO SISTEMA. 
//PORÉM OS COMANDOS SÃO MENSAGENS QUE SERÃO INTERPRETADOS DENTRO DO SSITEMA
//JÁ OS ACTIONS sÃO DELEGATES QUE RECEBEM A INFORMAÇÃO DE UM EVENTO PARA ALTERAR O ESTADO DO SISTEMA

type TableActions = {
   OpenTab : Tab -> Async<unit>
   ReceivedOrder : Guid -> Async<unit>
   CloseTab: Tab -> Async<unit>
}


type CachierActions = {
   AddTabAmount: Guid -> decimal -> Async<unit>
   Remove: Guid -> Async<unit>
}

type ChefActions = {
   AddFoodsToPrepare : Guid -> Food list -> Async<unit>
   RemoveFood: Guid -> Food  -> Async<unit>
   Remove: Guid -> Async<unit>
}

type WaiterActions = {
   AddDrinksToServe : Guid -> Drink list -> Async<unit>
   MarkDrinkServed: Guid -> Drink -> Async<unit>
   AddFoodToServe: Guid -> Food -> Async<unit>
   MarkFoodServed: Guid -> Food -> Async<unit>
   Remove:  Guid -> Async<unit>
}


type ProjectionActions = {
   Table : TableActions
   Waiter: WaiterActions
   Chef: ChefActions
   Cachier: CachierActions
}


let Parallel = Async.Parallel


let projectReadModel actions = function
   | TabOpened tab ->
      [actions.Table.OpenTab tab] |> Parallel
   
   | OrderPlaced order -> 
      let tabId = order.Tab.Id
      [
         actions.Table.ReceivedOrder tabId
         actions.Chef.AddFoodsToPrepare tabId order.Foods
         actions.Waiter.AddDrinksToServe tabId order.Drinks
      ] |> Parallel

   | DrinkServed (drink, tabId) ->
      [actions.Waiter.MarkDrinkServed tabId drink]|> Parallel

   | FoodPrepared (food, tabId) -> 
      [
         actions.Chef.RemoveFood tabId food
         actions.Waiter.AddFoodToServe  tabId food
      ] |> Parallel

   | FoodServed (food, tabId) ->
      [actions.Waiter.MarkFoodServed tabId food] |> Parallel

   |OrderServed (order, payment) ->
      let tabId = order.Tab.Id
      [
         actions.Waiter.Remove tabId
         actions.Chef.Remove tabId
         actions.Cachier.AddTabAmount tabId payment.Amount
      ] |> Parallel
   
   | TabClosed payment ->
      let tabId = payment.Tab.Id
      [
         actions.Cachier.Remove tabId
         actions.Table.CloseTab payment.Tab
      ] |> Parallel

   | _ -> failwith "Todo"