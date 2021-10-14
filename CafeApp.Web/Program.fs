﻿// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp

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
open Events 
open Projections
open JsonFormatter

let eventsStream = new Event<Event list>()

let commandApihandler eventStore (context: HttpContext) = async{
   let payload = Encoding.UTF8.GetString context.request.rawForm
   let! response = 
      handleCommandRequest inMemoryQueries eventStore payload
   match response with
   | Ok (state,events) -> 
      do! eventStore.SaveEvents state events
      eventsStream.Trigger events //emitt events
      //return! OK (sprintf "%A" state) context
      return! toStateJson state context

   |Error err -> 
      //return! BAD_REQUEST (err.Message) context
      return! toErrorJson err context

}


let  commandApi eventStore = 
   path "/command"
      >=> Filters.POST  
      >=> commandApihandler eventStore

let project event = 
   projectReadModel inMemoryActions event
   |> Async.RunSynchronously |> ignore

let projectEvents = List.iter project


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

   eventsStream.Publish.Add(projectEvents)
   
   0 