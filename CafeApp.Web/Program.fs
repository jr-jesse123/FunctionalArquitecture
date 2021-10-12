// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

open System
open Suave.Web
open Suave.Successful
open Suave.RequestErrors
open Suave.Operators
open Suave.Filters
open CommandApi
open InMemory
open System.Text
open Suave.Headers.Fields
open Suave

let commandApihandler eventStore (context: HttpContext) = async{
   let payload = Encoding.UTF8.GetString context.request.rawForm
   let! response = 
      handleCommandRequest inMemoryQueries eventStore payload
   match response with
   | Ok (state,events) -> 
      return! OK (sprintf "%A" state) context

   |Error err -> 
      return! BAD_REQUEST (err.Message) context

}


let  commandApi eventStore = 
   path "/command"
      >=> Filters.POST  
      >=> commandApihandler eventStore


[<EntryPoint>]
let main argv =
   let app = 
      let eventStore = inMemoryEventStore ()
      choose [
         commandApi eventStore
      ]

   let cfg = 
      {defaultConfig with bindings=[HttpBinding.createSimple HTTP "0.0.0.0" 8083]}

   startWebServer cfg app

   0 