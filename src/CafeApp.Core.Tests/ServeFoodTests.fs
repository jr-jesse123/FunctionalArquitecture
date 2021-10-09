module ServeFoodTests

open Domain
open States
open Commands
open Events
open CafeAppTestsDSL

open TestData
open Errors
open Xunit
open Domain

[<Fact>]
let ``Can maintain the order in progress state by serving food`` () =
   let order = {order with Foods = [salad;pizza]}
   let orderInProgress = {
      PlacedOrder = order
      ServedFoods = []
      ServedDrinks = []
      PreparedFoods = [salad;pizza]
   }
   let expected = {orderInProgress with ServedFoods = [salad]}
   Given (OrderInProgress orderInProgress)
   |> When (ServeFood (salad, order.Tab.Id))
   |> ThenStateShouldBe (OrderInProgress expected)
   |> WithEvents [FoodServed (salad, order.Tab.Id)]



[<Fact>]
let ``Can serve only prepared food`` () =
   let order = {order with Foods =[salad;pizza]}
   let oip = 
      InProgressOrder.Create(order,[],[],[])

   Given (OrderInProgress oip)
   |> When (ServeFood (salad,order.Tab.Id))
   |> ShouldFailWith (CanNotServeNonPreparedFood salad)

   //let order = {order with Foods = [salad;pizza]}
   //let orderInProgress = {
   //   PlacedOrder = order
   //   ServedFoods = []
   //   ServedDrinks = []
   //   PreparedFoods = [salad]
   //}
   //Given (OrderInProgress orderInProgress)
   //|> When (ServeFood (pizza, order.Tab.Id))
   //|> ShouldFailWith (CanNotServeNonPreparedFood pizza)
   
[<Fact>]
let ``Can not serve non-ordered food`` () =
   let order = {order with Foods = [salad;]}
   let orderInProgress = {
      PlacedOrder = order
      ServedFoods = []
      ServedDrinks = []
      PreparedFoods = [salad]
   }
   Given (OrderInProgress orderInProgress)
   |> When (ServeFood (pizza, order.Tab.Id))
   |> ShouldFailWith (CanNotServeNonOrderedFood pizza)

[<Fact>]
let ``Can not serve already served food`` () =
   let order = {order with Foods = [salad;pizza]}
   let orderInProgress = {
      PlacedOrder = order
      ServedFoods = [salad]
      ServedDrinks = []
      PreparedFoods = [pizza;salad]
   }
   Given (OrderInProgress orderInProgress)
   |> When (ServeFood (salad, order.Tab.Id))
   |> ShouldFailWith (CanNotServeAlreadyServedFood salad)