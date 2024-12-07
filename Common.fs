[<AutoOpen>]
module MyBoleroApp.Common

let random = System.Random()

let exnMsg (ex: exn) = ex.Message

let bug () = failwith "oops"
