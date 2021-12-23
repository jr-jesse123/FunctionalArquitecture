module Table
open Domain
open System.Collections.Generic
open ReadModel
open Projections
open Queries

//TODO: COMO SE RELACIONAM OS IN MEMORYS DO NEVEVNTS E AS IMPLEMENTAÇÕES DE DICIONÁRIOS DESTE PROJETO?


//TODO: TROCAR O DICIONARIO PELO MAP
let  tables =
   let dict = new Dictionary<int, Table>()
   dict.Add(1, {Number = 1; Waiter = "X"; Status = Closed})
   dict.Add(2, {Number = 2; Waiter = "y"; Status = Closed})
   dict.Add(3, {Number = 3; Waiter = "Z"; Status = Closed})
   dict

let  openTab tab =
   async {
      let tableNumber = tab.TableNumber
      let table = tables.[tableNumber]   
      tables.[tableNumber] <- {table with Status = Open(tab.Id)}
   }

let closeTab tab =
   let tableNumber = tab.TableNumber
   let table = tables.[tableNumber]
   tables.[tableNumber] <- {table with Status = Closed}
   async.Return ()

//TODO: DESCOBRIR ONDE ESTA FUNÇÃO É UTILIZADA fE PORQUE ELA É SEPARADA DA FUNÇÃO DE BAIXO
let getTableByTabId tabId =
   tables.Values
   |> Seq.tryFind( fun t ->
      match t.Status with
      | (Open id) | (InService id) -> id = tabId
      | _ -> false
   )

let getTableByTabIdAsync tabId = 
   async{
      return getTableByTabId tabId
   }

let  receivedOrder tabId =
   match getTableByTabId tabId with
   | Some table ->
      let tableNumber = table.Number
      tables.[tableNumber] <- {table with Status = InService(tabId)}
   | None -> ()
   async.Return ()


let tableActions = {
   OpenTab = openTab
   ReceivedOrder = receivedOrder
   CloseTab = closeTab
}

let getTables () =
   tables.Values
   |> Seq.toList
   |> async.Return


let getTableByTablenumber tablenumber = 
   async{
      if tables.ContainsKey tablenumber then
         return tables.[tablenumber] |> Some 
      else
         return None 
   }

let tableQueries = {
   GetTables = getTables
   GetTableByTabid = getTableByTabIdAsync
   GetTableByTableNumber = getTableByTablenumber
}
