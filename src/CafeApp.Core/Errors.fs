module Errors

open Domain

type Error=
   |TabAlreadyOpened
   |CanNotPlaceEmptyOrder
   |OrderAlreadyPlaced
   |CanNotOrderWithClosedTab
   | CanNotServeNonOrderedDrink of Drink
   | OrderAlreadyServed
   | CanNotServeForNonPlacedOrder
   | CanNotServeWithClosedTab
   | CanNotPrepareNonOrderedFood of Food
   | CanNotPrepareForNonPlacedOrder
   | CanNotPrepareWithClosedTab