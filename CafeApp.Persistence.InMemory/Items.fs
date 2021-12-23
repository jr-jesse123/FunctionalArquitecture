module Items
open System.Collections.Generic
open Domain
open System.Collections.Generic
open System.Collections.Generic


let private Foods = 
   let dict = new Dictionary<int,Food>()
   dict.Add(8, Food{MenuNumber = 8;Price=5m;Name="Salad"} )
   dict.Add(9, Food{MenuNumber = 9;Price=10m;Name="Pizza"} )
   dict

let private Drinks = 
   let dict = new Dictionary<int,Drink>()
   dict.Add(10, Drink{MenuNumber = 10; Price = 2.5m; Name = "Coke"})
   dict.Add(11, Drink{MenuNumber = 11; Price = 1.5m; Name = "Lemonade"})
   dict

let private getItems<'a> (dict: Dictionary<int,'a>) keys =
   let invalidKeys = keys |> Array.except dict.Keys
   if Array.isEmpty invalidKeys then
      keys
      |>Array.map (fun n -> dict.[n])
      |> Array.toList
      |> Choice1Of2
   else
      invalidKeys |> Choice2Of2

let getItem<'a> (dict: Dictionary<int,'a>) key = 
   if dict.ContainsKey key then
      dict.[key] |> Some
   else
      None

let getDrinksByMenuNumber key = 
   getItem Drinks key |> async.Return

let getFoodByMenuNumber key = 
   getItem Foods key |> async.Return

let getFoodsBymenuNumbers keys = 
   getItems Foods keys |> async.Return

   

let getDrinksByMenuNumbers keys = 
   printfn "chaves: %A" keys
   getItems Drinks keys |> async.Return




open Queries

let getFoods () = 
   Foods.Values |> Seq.toList |> async.Return

let getDrinks () =
   Drinks.Values |> Seq.toList |> async.Return

let foodQueries = {
   GetFoodsByMenuNumbers = getFoodsBymenuNumbers 
   GetFoodByMenuNumber = getFoodByMenuNumber
   GetFoods = getFoods
}
   

let drinkQueries = {
   GetDrinksByMenuNumbers = getDrinksByMenuNumbers
   getDrinkByMenuNumber = getDrinksByMenuNumber
   GetDrinks = getDrinks
}
