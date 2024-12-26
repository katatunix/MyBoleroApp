module MyBoleroApp.MultiImages

type Model =
    { ImageUrls: string list }

let private random = System.Random()

let private width, height = 1500, 850

let init () =
    { ImageUrls = [
        for _ = 1 to 200 do
            $"https://picsum.photos/id/{random.Next(500)}/{width}/{height}"
    ]}

open Bolero
open Bolero.Html
open MudBlazor

let render (model: Model) =
    comp<MudStack> {
        for url in model.ImageUrls do
            comp<MudImage> {
                attr.Src url
                attr.style $"aspect-ratio: {width} / {height};"
            }
    }
