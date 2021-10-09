module OpenTabTests
open CafeAppTestsDSL
open FsUnit
open Xunit
open Domain
open Events
open CommandHandlers
open Commands
open System
open States

[<Fact>] 
let teste () =
   let tab = {Id = Guid.NewGuid(); TableNumber = 1}
   WithEvents [TabOpened tab] ()
   

[<Fact>]
let ``Can Open a new Tab``() =
   let tab = {Id = Guid.NewGuid(); TableNumber = 1}

   let event = 
      Given (ClosedTab None) // Current State
      |> When (OpenTab tab) // Command
      |> ThenStateShouldBe (OpenedTab tab) // New State

   printfn "%A" event
   //|> WithEvents [TabOpened tab] // Event Emitted