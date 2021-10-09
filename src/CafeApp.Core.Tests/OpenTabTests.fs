

module OpenTabTests
open CafeAppTestsDSL
open Domain
open FsUnit
open Xunit

open Events
open CommandHandlers
open Commands
open System
open States
open Errors


[<Fact>]
let ``Can Open a new Tab``() =
   let tab = {Id = Guid.NewGuid(); TableNumber = 1}

   Given (ClosedTab None) // Current State
   |> When (OpenTab tab) // Command
   |> ThenStateShouldBe (OpenedTab tab) // New State
   |> WithEvents [TabOpened tab] // Event Emitted


[<Fact>]
let ``Cannot open an already Opened tab`` () =
   let tab = {Id = Guid.NewGuid(); TableNumber = 1}
   
   Given (OpenedTab tab)
   |> When (OpenTab tab)
   |> ShouldFailWith Error.TabAlreadyOpened

