module CommandHandlers
open States
open Events
open System
open Domain
open Commands
open Errors
open Chessie.ErrorHandling


//TODO: APÓS FINALIZADO O LIVRO, VERIFICAR A POSSIBILIDADE DE RETIRAR O A CCHESSIE LIB
let execute state command =
   match command with
   | OpenTab tab -> 
      match state with
      |ClosedTab _ -> [TabOpened tab] |> ok
      | _ -> TabAlreadyOpened |> fail
   | _ -> failwith "não implementado"

let evolve state command =
   match execute state command with
   | Ok (events,_) -> 
      let newState = List.fold States.apply state events //TODO: ASSEGURAR QUE ESTE S ESTÁ CORRETO
      (newState, events) |> ok
   | Bad err -> Bad err
