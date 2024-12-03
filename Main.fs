module MyBoleroApp.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Routing
open Microsoft.AspNetCore.Components.Sections
open Elmish
open Bolero
open Bolero.Html
open Microsoft.JSInterop
open MudBlazor

type Url =
    | NotFound
    | [<EndPoint "/">] Home
    | [<EndPoint "/counter?{start}">] Counter of start: int option
    | [<EndPoint "/random-picture">] RandomPicture
    | [<EndPoint "/peic-result">] PeicResult

type Page =
    | NotFound
    | Home
    | Counter of Counter.Model
    | RandomPicture of RandomPicture.Model
    | PeicResult

type Model =
    { CurrentUrl: Url
      IsDarkMode: bool
      IsMenuOpen: bool
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
      CurrentPage = Home },
    Cmd.none

let update (js: IJSRuntime, http: HttpClient, snackbar: ISnackbar) msg model =
    let model =
        match msg with
        | UrlChanged url -> { model with CurrentUrl = url }
        | _ -> model

    match model.CurrentPage, msg with
    | RandomPicture m, UrlChanged url when not url.IsRandomPicture -> m |> RandomPicture.dispose
    | _ -> ()

    match msg, model.CurrentPage with
    | UrlChanged Url.NotFound, _ ->
        { model with CurrentPage = NotFound }, Cmd.none

    | UrlChanged Url.Home, Home ->
        model, Cmd.none
    | UrlChanged Url.Home, _ ->
        { model with CurrentPage = Home }, Cmd.none

    | UrlChanged (Url.Counter _), Counter _ ->
        model, Cmd.none
    | UrlChanged (Url.Counter start), _ ->
        { model with CurrentPage = Counter (Counter.init start) }, Cmd.none

    | UrlChanged Url.RandomPicture, RandomPicture _ ->
        model, Cmd.none
    | UrlChanged Url.RandomPicture, _ ->
        let m, cmd = RandomPicture.init (js, http)
        { model with CurrentPage = RandomPicture m }, cmd |> Cmd.map RandomPictureMsg

    | UrlChanged Url.PeicResult, PeicResult ->
        model, Cmd.none
    | UrlChanged Url.PeicResult, _ ->
        { model with CurrentPage = PeicResult }, Cmd.none

    | SetDarkMode value, _ ->
        { model with IsDarkMode = value }, Cmd.none

    | SetMenuOpen value, _ ->
        { model with IsMenuOpen = value }, Cmd.none

    | ToggleMenuOpen, _ ->
        snackbar.Add($"ToggleMenuOpen {System.DateTime.Now}", Severity.Success)  |>ignore
        { model with IsMenuOpen = not model.IsMenuOpen }, Cmd.none

    | CounterMsg msg, Counter m ->
        match Counter.update msg m with
        | m, Counter.Nope ->
            { model with CurrentPage = Counter m }, Cmd.none
        | _, Counter.NavigateToHome ->
            { model with CurrentUrl = Url.Home; CurrentPage = Home }, Cmd.none

    | RandomPictureMsg msg, RandomPicture m ->
        let m, cmd = m |> RandomPicture.update (js, http) msg
        { model with CurrentPage = RandomPicture m },
        cmd |> Cmd.map RandomPictureMsg

    | RandomPictureMsg msg, _ ->
        RandomPicture.clean msg
        model, Cmd.none

    | _ ->
        model, Cmd.none

let render model dispatch =
    let appBar =
        comp<MudAppBar> {
            comp<MudIconButton> {
                attr.Icon (if model.IsMenuOpen then Icons.Material.Filled.MenuOpen else Icons.Material.Filled.Menu)
                attr.Color Color.Inherit
                attr.Edge Edge.Start
                on.click (fun _ -> dispatch ToggleMenuOpen)
            }
            comp<SectionOutlet> {
                attr.SectionName "Title"
            }
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
                        attr.Color Color.Primary
                        attr.value model.IsDarkMode
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
                    router.HRef (Url.Counter None)
                    attr.Match NavLinkMatch.Prefix
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

    let main =
        match model.CurrentPage with
        | NotFound ->
            NotFound.render ()
        | Home ->
            Home.render ()
        | Counter m ->
            Counter.render (router.Link Url.Home) m (CounterMsg >> dispatch)
        | RandomPicture m ->
            RandomPicture.render m (RandomPictureMsg >> dispatch)
        | PeicResult ->
            PeicResult.render ()

    concat {
        comp<MudThemeProvider> {
            attr.IsDarkMode model.IsDarkMode
        }
        comp<MudSnackbarProvider>
        comp<MudLayout> {
            appBar
            sideBar
            comp<MudMainContent> {
                comp<MudContainer> {
                    attr.MaxWidth MaxWidth.ExtraLarge
                    attr.Class "py-5"
                    main
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
        let update = update (this.JSRuntime, this.HttpClient, this.Snackbar)
        let program =
            Program.mkProgram init update render
            |> Program.withRouter router
            #if DEBUG
            |> Program.withConsoleTrace
            #endif
        program
