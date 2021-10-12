module Queries

open ReadModel
open Domain

type TableQueries = {
   GetTables : unit -> Async<Table list>
   GetTableByTableNumber : int -> Async<Table option>
}

type ToDoQueries = {
   GetChefToDos : unit -> Async<ChefToDo list>
   GetWaiterToDos : unit -> Async<WaiterToDo list>
   GetCashierToDos : unit -> Async<Payment list>
}

type Queries = {
   Table : TableQueries
   ToDo : ToDoQueries
}