module Cashier

open Projections
open Domain
open System.Collections.Generic
open System
open Table
open System.Collections.Generic
open CommandHandlers

let cashierTodos = new Dictionary<Guid,Payment>()
let addTabAmount tabId amount = 
   async{
      match getTableByTabId tabId with
      | Some table ->
         let payment = 
            {Tab = {Id=tabId;TableNumber=table.Number}; Amount =amount }
         cashierTodos.Add(tabId,payment)
      |None -> ()
   }

let remove tabId = 
   async{
      cashierTodos.Remove(tabId) |> ignore
   }

let cachierActions = {
   AddTabAmount = addTabAmount
   Remove= remove
}

let getCashierTodos () =
   async{
      return cashierTodos.Values |> Seq.toList
   }

