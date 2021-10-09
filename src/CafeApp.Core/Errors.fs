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
   | CanNotServeAlreadyServedDrink of Drink
   | CanNotPrepareAlreadyPreparedFood of Food
   //| CanNotPrepareAlreadyPreparedFood of Food
   | CanNotServeAlreadyServedFood of Food
   | CanNotServeNonPreparedFood of Food
   | CanNotServeNonOrderedFood of Food