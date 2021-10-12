module CommandApi
open  System.Text
open CommandHandlers
open OpenTab
open Queries
open CommandHandler

let handleCommandRequest validationQueries eventStore cmdStr=  async{
   match  cmdStr with
   | OpenTabRequest tab ->
      
      let commander = openTabCommander validationQueries.Table.GetTableByTableNumber
      return! handleCommand eventStore tab commander
      
   | _ ->return  err "invalid command" |> Error
   }