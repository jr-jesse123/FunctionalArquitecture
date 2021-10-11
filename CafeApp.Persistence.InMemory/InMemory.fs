module InMemory
open Table
open Chef

open Cashier
open Projections
open Queries
//open Items
open EventStore
open NEventStore

type InMemoryEventStore () =
   static member Instance =
                     Wireup.Init()
                        .UsingInMemoryPersistence()
                        .Build()

let inMemoryEventStore () =
   let eventStoreInstance = InMemoryEventStore.Instance
   {
      GetState = getState eventStoreInstance
      //SaveEvent = saveEvent eventStoreInstance
      SaveEvents = saveEvents eventStoreInstance
   }


let toDoQueries = {
   GetChefToDos = getChefToDos
   GetCashierToDos = getCashierTodos
   GetWaiterToDos = Waiter.getWaiterTodos
}

let inMemoryQueries = {
   Table = tableQueries
   ToDo = toDoQueries
}

let inMemoryActions = {
   Table = tableActions
   Chef = chefActions
   Waiter = Waiter.waiterActions
   Cachier  = cachierActions
}