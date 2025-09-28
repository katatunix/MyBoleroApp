[<AutoOpen>]
module MyBoleroApp.Common

open System

let random = Random()

let exnMsg (ex: exn) = ex.Message

let bug () = failwith "oops"

let (| Int | _ |) (str: string) =
    match Int32.TryParse str with
    | true, number -> Some number
    | _ -> None
