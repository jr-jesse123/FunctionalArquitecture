﻿module CafeAppTestsDSL
open FsUnit
open Xunit
open CommandHandlers
open States
open Chessie.ErrorHandling
open Errors
// CafeAppTestsDSL.fs
let Given (state : State) = state
let When command state = (command, state)

let ThenStateShouldBe expectedState (command, state) =
   match evolve state command with
   |Ok((actualState, events),_) -> 
      actualState |> should equal expectedState
      events |> Some
   |Bad errs ->
      sprintf "Expected : %A, But Actual : %A" expectedState errs.Head
      |> failwith
      None

let WithEvents expectedEvents actualEvents =
   match actualEvents with
   |Some (events) -> 
      events |> should equal expectedEvents
   |None -> None |> should equal expectedEvents

let ShouldFailWith (expectedError:Error) (command,state)  =
   match evolve state command with
   | Bad errs -> errs.Head |> should equal expectedError
   | Ok (r,_) -> 
      sprintf "Expected : %A, But Actual : %A" expectedError r
      |> failwith


