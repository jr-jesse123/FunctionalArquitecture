module EventStore
open States

let getStateFromEvents events = 
   Seq.fold apply (ClosedTab None)

open System
open NEventStore
open Events
open Domain
open System
open System
open System.Runtime.Intrinsics.Arm
open System

let getTabIdFromState = function
| ClosedTab None -> None
| OpenedTab Tab -> Some Tab.Id
| PlacedOrder po -> Some po.Tab.Id
| OrderInProgress ipo -> Some ipo.PlacedOrder.Tab.Id
| ServedOrder payment -> Some payment.Tab.Id
| ClosedTab (Some tabId) -> Some tabId


let saveEvent (storeEvents: IStoreEvents) state event = 
   
      match getTabIdFromState state with
      |Some tabId -> 
         use stream = storeEvents.OpenStream(tabId.ToString())
         stream.Add(new EventMessage(Body = event))
         stream.CommitChanges(Guid.NewGuid())
      | _ -> failwith "quero saber quando que chega aqui"
   


let saveEvents (storeEvents:IStoreEvents) state events = 
   async {
      return List.iter (saveEvent storeEvents state ) events
   }
   //|> async.Return

let getEvents (StoreEvents:IStoreEvents) tabId = 
   use stream = StoreEvents.OpenStream((tabId:Guid).ToString())
   stream.CommittedEvents
   |> Seq.map (fun msg -> msg.Body)
   |> Seq.cast<Event>
  

let getState storeEvents tabId = 
   async {
      return ( 
         getEvents storeEvents tabId
         |> Seq.fold apply (ClosedTab None)
         )
   }

type EventStore = {
   GetState : Guid -> Async<State>
//   SaveEvent : State -> Event -> Async<unit>
   SaveEvents : State -> Event list -> Async<unit>
}
   
