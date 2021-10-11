module Chef

open System.Collections.Generic
open System
open ReadModel
open Table
open Domain
open Projections


let chefToDos = new Dictionary<Guid,ChefToDo>()

let addFoodsToPrepare tabId foods = 
   async{
      match getTableByTabId tabId with
      |Some table -> 
         let tab = {Id=tabId; TableNumber = table.Number}
         let todo :ChefToDo = {Tab = tab ; Foods = foods}
         chefToDos.Add(tabId,todo)
      |None -> ()
   }

let removeFood tabId food = 
   async{
      let todo = chefToDos.[tabId]
      let chefeTodo = {todo with Foods = List.except [food] todo.Foods}
      chefToDos.[tabId] <- chefeTodo
   }
   
let  remove tabId =
   async{
      chefToDos.Remove tabId   |> ignore
   }

   

let chefActions = {
   AddFoodsToPrepare = addFoodsToPrepare
   RemoveFood = removeFood
   Remove = remove
}

let getChefToDos () =
   async{
      return chefToDos.Values |> Seq.toList
   }
