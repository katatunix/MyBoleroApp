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

type Page =
    | NotFound
    | Home
    | Counter
    | RandomPicture
    | PeicResult

type Model =
    { CurrentUrl: Url
      IsDarkMode: bool
      IsMenuOpen: bool
      Counter: Counter.Model option
      RandomPicture: RandomPicture.Model option
      CurrentPage: Page }

type Msg =
    | UrlChanged of Url
    | SetDarkMode of bool
    | SetMenuOpen of bool
    | ToggleMenuOpen
    | CounterMsg of Counter.Msg
    | RandomPictureMsg of RandomPicture.Msg

let router =
    Router.infer UrlChanged _.CurrentUrl
    |> Router.withNotFound Url.NotFound

let init _ =
    { CurrentUrl = Url.Home
      IsDarkMode = true
      IsMenuOpen = true
      Counter = None
      RandomPicture = None
      CurrentPage = Home },
    Cmd.none

let update (_snackbar: ISnackbar) msg model =
    let model =
        match msg with
        | UrlChanged url -> { model with CurrentUrl = url }
        | _ -> model

    match msg, model.CurrentPage with
    | UrlChanged Url.NotFound, _ ->
        { model with CurrentPage = NotFound }, Cmd.none

    | UrlChanged Url.Home, _ ->
        { model with CurrentPage = Home }, Cmd.none

    | UrlChanged Url.Counter, page when page.IsCounter|>not ->
        let m = model.Counter |> Option.defaultWith Counter.init
        { model with CurrentPage = Counter; Counter = Some m }, Cmd.none

    | UrlChanged Url.RandomPicture, page when page.IsRandomPicture|>not ->
        let m, cmd =
            match model.RandomPicture with
            | None -> RandomPicture.init()
            | Some m -> m, Cmd.none
        { model with CurrentPage = RandomPicture; RandomPicture = Some m },
        cmd |> Cmd.map RandomPictureMsg

    | UrlChanged Url.PeicResult, _ ->
        { model with CurrentPage = PeicResult }, Cmd.none

    | SetDarkMode value, _ ->
        { model with IsDarkMode = value }, Cmd.none

    | SetMenuOpen value, _ ->
        { model with IsMenuOpen = value }, Cmd.none

    | ToggleMenuOpen, _ ->
        { model with IsMenuOpen = model.IsMenuOpen|>not }, Cmd.none

    | CounterMsg msg, _ ->
        match model.Counter with
        | Some m ->
            let m, intent = m |> Counter.update msg
            let model = { model with Counter = Some m }
            match intent with
            | Counter.Intent.Nope ->
                model, Cmd.none
            | Counter.Intent.NavigateToHome ->
                { model with CurrentUrl = Url.Home; CurrentPage = Home }, Cmd.none
        | None ->
            model, Cmd.none

    | RandomPictureMsg msg, _ ->
        match model.RandomPicture with
        | Some m ->
            let m, cmd = m |> RandomPicture.update msg
            { model with RandomPicture = Some m },
            cmd |> Cmd.map RandomPictureMsg
        | None ->
            RandomPicture.clean msg
            model, Cmd.none

    | _ ->
        model, Cmd.none

let render model dispatch =
    let title, page =
        match model.CurrentPage with
        | NotFound ->
            "Not Found", NotFound.render ()
        | Home ->
            "Home", Home.render ()
        | Counter ->
            "Counter", Counter.render model.Counter.Value (CounterMsg >> dispatch)
        | RandomPicture ->
            "Random Picture", RandomPicture.render model.RandomPicture.Value (RandomPictureMsg >> dispatch)
        | PeicResult ->
            "PEIC Result", PeicResult.render ()

    let appBar =
        comp<MudAppBar> {
            comp<MudIconButton> {
                attr.Icon (
                    if model.IsMenuOpen then Icons.Material.Filled.MenuOpen
                    else Icons.Material.Filled.Menu)
                attr.Color Color.Inherit
                attr.Edge Edge.Start
                on.click (fun _ -> dispatch ToggleMenuOpen)
            }
            comp<MudText> { attr.Typo Typo.h5; title }
        }

    let sideBar =
        comp<MudDrawer> {
            attr.Open model.IsMenuOpen
            on.OpenChanged (SetMenuOpen >> dispatch)
            attr.Elevation 1
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
                        attr.value model.IsDarkMode
                        attr.Color Color.Primary
                        on.ValueChanged (SetDarkMode >> dispatch)
                    }
                }
            }
            comp<MudNavMenu> {
                comp<MudNavLink> {
                    router.HRef Url.Home
                    attr.Match NavLinkMatch.All
                    "Home"
                }
                comp<MudNavLink> {
                    router.HRef Url.Counter
                    attr.Match NavLinkMatch.All
                    "Counter"
                }
                comp<MudNavLink> {
                    router.HRef Url.RandomPicture
                    attr.Match NavLinkMatch.All
                    "Random Picture"
                }
                comp<MudNavLink> {
                    router.HRef Url.PeicResult
                    attr.Match NavLinkMatch.All
                    "PEIC Result"
                }
            }
        }

    concat {
        comp<MudThemeProvider> {
            attr.IsDarkMode model.IsDarkMode
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
#if DEBUG
        // |> Program.withConsoleTrace
#endif
