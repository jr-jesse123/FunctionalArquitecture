module ServeDrinksTests
open Domain
open States
open Commands
open Events
open CafeAppTestsDSL
open Xunit
open FsUnit
open TestData
open Errors
open Xunit
open Xunit
open Xunit
open TestData
open Xunit

[<Fact>]
let ``Can Serve Drind`` () =
   let order = {order with Drinks= [coke;lemonade]}
   let expected = {
      PlacedOrder = order
      ServedDrinks = [coke]
      PreparedFoods = []
      ServedFoods =[]
   }
   Given (PlacedOrder order)
   |> When (ServeDrink (coke, order.Tab.Id))
   |> ThenStateShouldBe (OrderInProgress expected)
   |> WithEvents [DrinkServed (coke, order.Tab.Id)]

[<Fact>]
let ``Can not serve non ordered drink`` () =
   let order = {order with Drinks = [coke]}
   Given (PlacedOrder order)
   |> When (ServeDrink (lemonade, order.Tab.Id))
   |> ShouldFailWith (CanNotServeNonOrderedDrink lemonade)

[<Fact>]
let ``Can not serve drink for already served order`` () =
   Given (ServedOrder order)
   |> When (ServeDrink (coke, order.Tab.Id))
   |> ShouldFailWith OrderAlreadyServed

[<Fact>]
let ``Can not serve drinks for non placed order`` () =
   Given (OpenedTab tab)
   |> When (ServeDrink (coke, tab.Id))
   |> ShouldFailWith CanNotServeForNonPlacedOrder

[<Fact>]
let ``Can not serve with closed tab`` () =
   Given (ClosedTab None)
   |> When (ServeDrink (coke, tab.Id))
   |> ShouldFailWith CanNotServeWithClosedTab

[<Fact>]
let ``Can serve drink for order containin only one drink`` () =
   let order = {order with Drinks = [coke]}
   let payment = {Tab= order.Tab; Amount=drinkPrice coke}
   Given (PlacedOrder order)
   |> When (ServeDrink (coke, order.Tab.Id))
   |> ThenStateShouldBe (ServedOrder order)
   |> WithEvents  [
      DrinkServed (coke, order.Tab.Id)
      OrderServed (order, payment)
   ]

[<Fact>]
let ``Remain in order in progress while serving drink`` () =
   let order = {order with Drinks = [coke;lemonade;appleJuice]}
   let orderInProgress = {
   PlacedOrder = order
   ServedDrinks = [coke]
   PreparedFoods = []
   ServedFoods = []
   }
   let expected = {orderInProgress with ServedDrinks = lemonade :: orderInProgress.ServedDrinks}

   Given (OrderInProgress orderInProgress)
   |> When (ServeDrink (lemonade, order.Tab.Id))
   |> ThenStateShouldBe (OrderInProgress expected)
   |> WithEvents [DrinkServed (lemonade, order.Tab.Id)]


[<Fact>]
let ``Can not serve non ordered drinks during order in progress `` () =
   let order = {order with Drinks = [coke;lemonade]}
   let orderInProgress = {
      PlacedOrder = order
      ServedDrinks = [coke]
      PreparedFoods = []
      ServedFoods = []
   }
   Given (OrderInProgress orderInProgress)
   |> When (ServeDrink (appleJuice,order.Tab.Id))
   |> ShouldFailWith (CanNotServeNonOrderedDrink appleJuice)


[<Fact>]
let ``Can not serve an already served drinks`` () =
   let order = {order with Drinks = [coke;lemonade]}
   let orderInProgress = {
      PlacedOrder = order
      ServedDrinks = [coke]
      PreparedFoods = []
      ServedFoods = []
   }
   Given (OrderInProgress orderInProgress)
   |> When (ServeDrink (coke,order.Tab.Id))
   |> ShouldFailWith (CanNotServeAlreadyServedDrink coke)