module CommandHandlers
open States
open Events
open System
open Domain
open Commands

let execute state command =
   match command with
   //| _ -> [TabOpened {Id = Guid.NewGuid(); TableNumber = 1}]
   | OpenTab tab -> [TabOpened tab]
   //| OpenTab tab -> TabOpened tab
   | _ -> failwith "não implementado"

let evolve state command =
   let events = execute state command
   let newState = List.fold apply state events //TODO: ASSEGURAR QUE ESTE S ESTÁ CORRETO
   (newState, events)
