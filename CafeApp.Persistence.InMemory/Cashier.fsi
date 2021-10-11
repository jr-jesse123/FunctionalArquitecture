module Cashier
open Projections
open Domain

val cachierActions : CachierActions
val getCashierTodos: unit -> Async<Payment list>