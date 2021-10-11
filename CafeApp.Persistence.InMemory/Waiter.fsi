module Waiter
open Projections
open ReadModel

val waiterActions : WaiterActions
val getWaiterTodos : unit -> Async<WaiterToDo list>
