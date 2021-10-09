module Errors

type Error=
   |TabAlreadyOpened
   |CanNotPlaceEmptyOrder
   |OrderAlreadyPlaced
   |CanNotOrderWithClosedTab