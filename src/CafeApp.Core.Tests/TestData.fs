
module TestData
open Domain
open System
let tab = {Id=Guid.NewGuid();TableNumber=1}
let coke = 
   Drink {
      MenuNumber =1
      Name="Coke"
      Price = 1.5m
   }
let lemonade = 
   Drink {
      MenuNumber = 3 
      Name= "Lemonade"
      Price=1.0m
   }
let appleJuice = 
   Drink <| Item.Create 5 1.0M "Apple Juice"

let order = {Tab = tab; Foods=[];Drinks=[]}
let salad = Food <| Item.Create 2 2.5M "Salada"
let pizza = Food (Item.Create 4 6.5M  "Pizza")
let foodPrice (Food food) = food.Price
let drinkPrice (Drink drink) = drink.Price




