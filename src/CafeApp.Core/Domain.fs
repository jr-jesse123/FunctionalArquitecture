module Domain

open System

type Tab = {
Id : Guid
TableNumber : int
}

type Item = {
MenuNumber : int
Price : decimal
Name : string
} with 
   static member Create menuNr preco nome = 
         {MenuNumber =menuNr; Price=preco; Name=nome}

type Food = Food of Item

type Drink = Drink of Item

type Payment = {
Tab : Tab
Amount : decimal
}

type Order = {
Foods : Food list
Drinks : Drink list
Tab : Tab
}

type InProgressOrder = {
PlacedOrder : Order
ServedDrinks : Drink list
ServedFoods : Food list
PreparedFoods : Food list
} 
with 
   static member Create(order, drinkList, servedFoodList, preparedFoodList) =
      {
         PlacedOrder = order;
         ServedDrinks = drinkList;
         ServedFoods = servedFoodList; 
         PreparedFoods = preparedFoodList
      }


let isServinDrinkCompletesOrder order drink =
   List.isEmpty order.Foods && order.Drinks = [drink]

let orderAmount order = 
   let foodAmoun = 
      order.Foods |> List.map (fun (Food f) -> f.Price) |> List.sum

   let drinksAmount = 
      order.Drinks |> List.map (fun (Drink d) -> d.Price) |> List.sum

   foodAmoun + drinksAmount
