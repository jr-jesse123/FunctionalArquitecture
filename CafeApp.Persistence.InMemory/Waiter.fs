module Waiter

open Projections
open ReadModel
open System .Collections.Generic
open System
open Table

let waiterTodos = new Dictionary<Guid,WaiterToDo>()

let addDrinksToServe tabId drinksItems =
   async{
      match getTableByTabId tabId with
         | Some table -> 
            let todo = 
               {Tab = {Id= tabId; TableNumber= table.Number}
                Foods= []
                Drinks= drinksItems}
            waiterTodos.Add(tabId,todo)
         | None -> ()
   }

let addFoodToServe tabId food = 
   async{
      if waiterTodos.ContainsKey tabId then
         let todo = waiterTodos.[tabId]
         let watierToDo = {todo with Foods= food:: todo.Foods}
         waiterTodos.[tabId] <- watierToDo
      else
         match getTableByTabId tabId with
         | Some table ->
            let todo = 
               { Tab= {Id = tabId; TableNumber= table.Number}
                 Foods=[food]
                 Drinks=[] }
            waiterTodos.Add(tabId,todo)
         | None -> ()
   }

let markDrinkServed tabId drink = 
   async{
      let todo = waiterTodos.[tabId]
      let waiterToDo = 
         { todo with Drinks =
            List.except [drink] todo.Drinks }
      waiterTodos.[tabId] <- waiterToDo
   }

let markFoodServed tabId food = 
   async{
      let todo = waiterTodos.[tabId]
      let waiterTodo = 
        {todo with Foods = List.except [food] todo.Foods}
      waiterTodos.[tabId] <- waiterTodo
   }

let remove tabId = 
   async{
      waiterTodos.Remove(tabId) |> ignore
   }

let waiterActions = {
   AddDrinksToServe =addDrinksToServe
   MarkDrinkServed = markDrinkServed
   AddFoodToServe = addFoodToServe
   MarkFoodServed = markFoodServed
   Remove=remove
}

let getWaiterTodos () =
   async{
      return Seq.toList waiterTodos.Values 
   }
   