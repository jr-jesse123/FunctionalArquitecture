

module PlacedOrderTests

open CafeAppTestsDSL
open FsUnit
open Errors
open Xunit
open Domain
open System
open States
open CommandHandlers
open Events
open Commands
open CafeAppTestsDSL
open Xunit
open Xunit

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

[<Fact>]
let ``Can not place empty order`` () =
   Given(OpenedTab tab)
   |> When (PlaceOrder order)
   |> ShouldFailWith CanNotPlaceEmptyOrder


[<Fact>]
let ``Can not place order multiplle times`` () =
   let order = {order with Drinks = [coke]}

   Given (PlacedOrder order)
   |> When (PlaceOrder order)
   |> ShouldFailWith OrderAlreadyPlaced
   

