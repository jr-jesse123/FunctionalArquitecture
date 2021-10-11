module Table
open Projections
open System
open ReadModel
open Queries

val tableActions :TableActions
val getTableByTabId : Guid -> Table option
val getTables : unit -> Async<Table list>
val tableQueries : TableQueries
