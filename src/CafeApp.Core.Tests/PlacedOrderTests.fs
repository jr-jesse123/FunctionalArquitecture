module PlacedOrderTests

open FsUnit

open Xunit
open Domain
open System
open States
open CommandHandlers
open Events
open Commands
open CafeAppTestsDSL

let tab = {Id = Guid.NewGuid(); TableNumber = 1}
let coke = 
   Drink {
      MenuNumber = 1
      Name = "Coke"
      Price = 1.5m
      }
let order = {Tab = tab;Foods = [];Drinks = []}

[<Fact>]
let ``Cna place only drinks order`` () =
   let order = {order with Drinks = [coke]}
   Given (OpenedTab tab)
   |> When (PlaceOrder order)
   |> ThenStateShouldBe (PlacedOrder order)
   |> WithEvents [OrderPlaced order]