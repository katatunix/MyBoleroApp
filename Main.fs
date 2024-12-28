module MyBoleroApp.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Routing
open Microsoft.JSInterop
open Elmish
open Bolero
open Bolero.Html
open MudBlazor

type Url =
    | NotFound
    | [<EndPoint "/">] Home
    | [<EndPoint "/counter">] Counter
    | [<EndPoint "/random-picture">] RandomPicture
    | [<EndPoint "/peic-result">] PeicResult
    | [<EndPoint "/gallery">] Gallery

type Page =
    | NotFound
    | Home
    | Counter
    | RandomPicture
    | PeicResult
    | Gallery

type Model =
    { currentUrl: Url
      isDarkMode: bool
      isMenuOpen: bool
      counter: Counter.Model option
      randomPicture: RandomPicture.Model option
      gallery: Gallery.Model option
      currentPage: Page }

type Msg =
    | UrlChanged of Url
    | SetDarkMode of bool
    | SetMenuOpen of bool
    | ToggleMenuOpen
    | CounterMsg of Counter.Msg
    | RandomPictureMsg of RandomPicture.Msg
    | GalleryMsg of Gallery.Msg

let router =
    Router.infer UrlChanged _.currentUrl
    |> Router.withNotFound Url.NotFound

let init _ =
    { currentUrl = Url.Home
      isDarkMode = true
      isMenuOpen = true
      counter = None
      randomPicture = None
      gallery = None
      currentPage = Home },
    Cmd.none

let update (_snackbar: ISnackbar) msg model =
    let model =
        match msg with
        | UrlChanged url -> { model with currentUrl = url }
        | _ -> model

    match msg, model.currentPage with
    | UrlChanged Url.NotFound, _ ->
        { model with currentPage = NotFound }, Cmd.none

    | UrlChanged Url.Home, _ ->
        { model with currentPage = Home }, Cmd.none

    | UrlChanged Url.Counter, page when not page.IsCounter ->
        let m, cmd =
            match model.counter with
            | None -> Counter.init()
            | Some m -> m, Cmd.none
        { model with currentPage = Counter; counter = Some m },
        cmd |> Cmd.map CounterMsg

    | UrlChanged Url.RandomPicture, page when not page.IsRandomPicture ->
        let m, cmd =
            match model.randomPicture with
            | None -> RandomPicture.init()
            | Some m -> m, Cmd.none
        { model with currentPage = RandomPicture; randomPicture = Some m },
        cmd |> Cmd.map RandomPictureMsg

    | UrlChanged Url.PeicResult, _ ->
        { model with currentPage = PeicResult }, Cmd.none

    | UrlChanged Url.Gallery, page when not page.IsGallery ->
        let m, cmd =
            match model.gallery with
            | None -> Gallery.init()
            | Some m -> m, Cmd.none
        { model with currentPage = Gallery; gallery = Some m },
        cmd |> Cmd.map GalleryMsg

    | SetDarkMode value, _ ->
        { model with isDarkMode = value }, Cmd.none

    | SetMenuOpen value, _ ->
        { model with isMenuOpen = value }, Cmd.none

    | ToggleMenuOpen, _ ->
        { model with isMenuOpen = not model.isMenuOpen }, Cmd.none

    | CounterMsg msg, _ ->
        match model.counter with
        | Some m ->
            let m, intent = m |> Counter.update msg
            let model = { model with counter = Some m }
            match intent with
            | Counter.Intent.Nope ->
                model, Cmd.none
            | Counter.Intent.NavigateToHome ->
                { model with currentUrl = Url.Home }, Cmd.none
        | None ->
            bug()

    | RandomPictureMsg msg, _ ->
        match model.randomPicture with
        | Some m ->
            let m, cmd = m |> RandomPicture.update msg
            { model with randomPicture = Some m },
            cmd |> Cmd.map RandomPictureMsg
        | None ->
            bug()

    | GalleryMsg msg, _ ->
        match model.gallery with
        | Some m ->
            let m, cmd = m |> Gallery.update msg
            { model with gallery = Some m },
            cmd |> Cmd.map GalleryMsg
        | None ->
            bug()

    | _ ->
        model, Cmd.none

let render model dispatch =
    let title, page =
        match model.currentPage with
        | NotFound ->
            "Not Found", NotFound.render()
        | Home ->
            "Home", Home.render()
        | Counter ->
            "Counter", Counter.render model.counter.Value (CounterMsg >> dispatch)
        | RandomPicture ->
            "Random Picture", RandomPicture.render model.randomPicture.Value (RandomPictureMsg >> dispatch)
        | PeicResult ->
            "PEIC Result", PeicResult.render()
        | Gallery ->
            "Gallery", Gallery.render model.gallery.Value (GalleryMsg >> dispatch)

    let appBar =
        comp<MudAppBar> {
            comp<MudIconButton> {
                attr.Icon (
                    if model.isMenuOpen then Icons.Material.Filled.MenuOpen
                    else Icons.Material.Filled.Menu)
                attr.Color Color.Inherit
                attr.Edge Edge.Start
                on.click (fun _ -> dispatch ToggleMenuOpen)
            }
            comp<MudText> { attr.Typo Typo.h5; title }
        }

    let sideBar =
        comp<MudDrawer> {
            attr.Open model.isMenuOpen
            on.OpenChanged (SetMenuOpen >> dispatch)
            comp<MudDrawerHeader> {
                comp<MudStack> {
                    comp<MudStack> {
                        attr.Row true
                        attr.AlignItems AlignItems.Center
                        comp<MudIcon> {
                            attr.Size Size.Large
                            attr.Icon Icons.Custom.Brands.MudBlazor
                            attr.Color Color.Primary
                        }
                        comp<MudText> { attr.Typo Typo.h4; "App" }
                    }
                    comp<MudSwitch<bool>> {
                        attr.label "Dark Mode"
                        attr.value model.isDarkMode
                        attr.Color Color.Primary
                        on.ValueChanged (SetDarkMode >> dispatch)
                    }
                }
            }
            comp<MudNavMenu> {
                let navLink (url: Url) (text: string) =
                    comp<MudNavLink> {
                        router.HRef url
                        attr.Match NavLinkMatch.All
                        text
                    }
                navLink Url.Home "Home"
                navLink Url.Counter "Counter"
                navLink Url.RandomPicture "Random Picture"
                navLink Url.PeicResult "PEIC Result"
                navLink Url.Gallery "Gallery"
            }
        }

    concat {
        comp<MudThemeProvider> {
            attr.IsDarkMode model.isDarkMode
        }
        comp<MudSnackbarProvider> {  }
        comp<MudLayout> {
            appBar
            sideBar
            comp<MudMainContent> {
                comp<MudContainer> {
                    attr.MaxWidth MaxWidth.ExtraLarge
                    attr.Class "py-6"
                    page
                }
            }
        }
    }

type App() =
    inherit ProgramComponent<Model, Msg>()

    [<Inject>]
    member val JSRuntime = Unchecked.defaultof<IJSRuntime> with get, set
    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set
    [<Inject>]
    member val Snackbar = Unchecked.defaultof<ISnackbar> with get, set

    override this.Program =
        Js.runtime <- this.JSRuntime
        Http.client <- this.HttpClient

        Program.mkProgram init (update this.Snackbar) render
        |> Program.withRouter router
// #if DEBUG
//         |> Program.withConsoleTrace
// #endif
