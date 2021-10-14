module CommandApi
open  System.Text
open CommandHandlers
open OpenTab
open Queries
open CommandHandler
open PlaceOrder
open ServeDrink
open PrepareFood
open ServeFood
open CloseTab
open System.Diagnostics.Tracing


let handleCommandRequest validationQueries eventStore cmdStr=  async{
   match  cmdStr with
   | OpenTabRequest tab ->
      
      let commander = openTabCommander validationQueries.Table.GetTableByTableNumber
      return! handleCommand eventStore tab commander

   | PlaceOrderRequest placeOrder ->
      return!
         placeOrderCommander validationQueries 
         |> handleCommand eventStore placeOrder
      
   | ServeDrinkRequest (tabId,menuNumber) ->
      
         let Commander = 
            servDrinkCommander
               validationQueries.Table.GetTableByTabid 
               validationQueries.Drink.getDrinkByMenuNumber
               
         return! handleCommand eventStore (tabId,menuNumber) Commander
      
   | PrepareFoodRequest (tabId,menuNumber) ->
      
      let commander = 
         prepareFoodCommander 
            validationQueries.Table.GetTableByTabid
            validationQueries.Food.GetFoodByMenuNumber

      return! handleCommand eventStore (tabId,menuNumber) commander


   | ServeFoodRequest (tabId, menuNumber) ->
      let Commander = 
         servFoodCommander
            validationQueries.Table.GetTableByTabid 
            validationQueries.Food.GetFoodByMenuNumber
            
      return! handleCommand eventStore (tabId,menuNumber) Commander
      

   | CloseTabRequest (tabId,amount) ->
      
      let commander = 
         closeTabCommander validationQueries.Table.GetTableByTabid

      return! handleCommand eventStore (tabId,amount) commander


   | _ ->return  err "invalid command" |> Error
}