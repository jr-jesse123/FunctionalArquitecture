module Chef
open Projections
open ReadModel

val chefActions : ChefActions
val getChefToDos : unit -> Async<ChefToDo list>