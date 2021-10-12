module CommandHandler
open Queries
open Commands
open CommandHandlers
open Errors
open CommandHandlers
open NEventStore
open EventStore

type Commander<'a,'b> = {
   Validate : 'a -> Async<Choice<'b,string>> //TODO: TROCAR PRA RESULT
   ToCommand : 'b -> Command
}

type ErrorResponse = {
   Message: string
}

let err msg = {Message = msg}

let getTabIdFromCommand = function
| OpenTab  tab -> tab.Id
| PlaceOrder order -> order.Tab.Id
| ServeDrink (_,tabId) -> tabId
| PrepareFood (_,tabId) -> tabId
| ServeFood (_,tabId) -> tabId
| CloseTab (payment) -> payment.Tab.Id



let handleCommand (eventStore:EventStore) commandData commander = async {
   let! validationResult = commander.Validate commandData
   match validationResult with
   | Choice1Of2 validatedCommandData ->
      let command = commander.ToCommand validatedCommandData
      let! state = eventStore.GetState (getTabIdFromCommand command)
      match evolve state command with
      | Ok(newState, events) ->
         return (newState,events) |> Ok
      | Error (error) ->
         return error |> toErrorString |> err |> Error
   | Choice2Of2 errorMessage ->
   return errorMessage |> err |> Error
}