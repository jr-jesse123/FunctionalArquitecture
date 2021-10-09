module CommandHandlers
open States
open Events
open System
open Domain
open Commands
open Errors
//open Chessie.ErrorHandling


let handleOpenTab tab = function
| ClosedTab _ -> [TabOpened tab] |> Ok
| _ -> TabAlreadyOpened |> Error

let HandlePlaceOrder order = function
|OpenedTab _ -> [OrderPlaced order] |> Ok
| _ -> failwith "não implementado"

//TODO: APÓS FINALIZADO O LIVRO, VERIFICAR A POSSIBILIDADE DE RETIRAR O A CCHESSIE LIB
let execute state command =
   match command with
   | OpenTab tab -> handleOpenTab tab state
   | PlaceOrder order -> HandlePlaceOrder order state
   | _ -> failwith "não implementado"

let evolve state command =
   match execute state command with
   | Ok (events) -> 
      let newState = List.fold States.apply state events //TODO: ASSEGURAR QUE ESTE S ESTÁ CORRETO
      (newState, events) |> Ok
   | Error err -> Error err
