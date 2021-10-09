﻿module PrepareFoodFacts

open FsUnit
open Xunit
open Domain 
open States
open Commands
open Events
open TestData
open Errors
open Domain
open CafeAppTestsDSL

[<Fact>] 
let ``Can Prepare Food`` () =
   let order = {order with Foods = [salad]}
   let expected = 
      InProgressOrder.Create(order, [], [],[salad])
   Given (PlacedOrder order)
   |> When (PrepareFood (salad,order.Tab.Id))
   |> ThenStateShouldBe (OrderInProgress expected)
   |> WithEvents [FoodPrepared (salad, order.Tab.Id)]
   


[<Fact>]
let ``Can not prepare a non-ordered food`` () =
   let order = {order with Foods = [pizza]}
   Given (PlacedOrder order)
   |> When (PrepareFood (salad, order.Tab.Id))
   |> ShouldFailWith (CanNotPrepareNonOrderedFood salad)

[<Fact>]
let ``Can not prepare a food for served order`` () =
   Given (ServedOrder order)
   |> When (PrepareFood (pizza, order.Tab.Id))
   |> ShouldFailWith OrderAlreadyServed
[<Fact>]
let ``Can not prepare with closed tab`` () =
   Given (ClosedTab None)
   |> When (PrepareFood (salad, order.Tab.Id))
   |> ShouldFailWith CanNotPrepareWithClosedTab