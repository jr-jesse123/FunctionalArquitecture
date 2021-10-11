module Table
open Projections
open System
open ReadModel

val tableActions :TableActions
val getTableByTabId : Guid -> Table option
