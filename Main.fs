module MyBoleroApp.Main

open System.Net.Http
open Microsoft.AspNetCore.Components
open Microsoft.AspNetCore.Components.Routing
open Microsoft.AspNetCore.Components.Sections
open Elmish
open Bolero
open Bolero.Html
open MudBlazor

type Url =
    | [<EndPoint "/">] Home
    | [<EndPoint "/counter?{start}">] Counter of start: int option
    | NotFound

type Page =
    | Home
    | Counter of Counter.Model
    | NotFound

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

let router =
    Router.infer UrlChanged _.CurrentUrl
    |> Router.withNotFound Url.NotFound

let init _ =
    { CurrentUrl = Url.Home
      IsDarkMode = true
      IsMenuOpen = true
      CurrentPage = Home },
    Cmd.none

let update (_http: HttpClient) msg model =
    match msg, model.CurrentPage with
    | UrlChanged (Url.Home as url), Home ->
        { model with CurrentUrl = url }, Cmd.none
    | UrlChanged (Url.Home as url), _ ->
        { model with CurrentUrl = url; CurrentPage = Home }, Cmd.none

    | UrlChanged (Url.Counter start as url), Counter _ ->
        { model with CurrentUrl = url }, Cmd.none
    | UrlChanged (Url.Counter start as url), _ ->
        { model with CurrentUrl = url; CurrentPage = Counter (Counter.init start) }, Cmd.none

    | UrlChanged (Url.NotFound as url), _ ->
        { model with CurrentUrl = url; CurrentPage = NotFound }, Cmd.none

    | SetDarkMode value, _ ->
        { model with IsDarkMode = value }, Cmd.none

    | SetMenuOpen value, _ ->
        { model with IsMenuOpen = value }, Cmd.none

    | ToggleMenuOpen, _ ->
        { model with IsMenuOpen = not model.IsMenuOpen }, Cmd.none

    | CounterMsg msg, Counter m ->
        match Counter.update msg m with
        | m, Counter.Nope ->
            { model with CurrentPage = Counter m }, Cmd.none
        | _, Counter.NavigateToHome ->
            { model with CurrentUrl = Url.Home; CurrentPage = Home }, Cmd.none

    | _ ->
        failwithf "oops!!! (msg: %A) (model: %A)" msg model

let render model dispatch =
    let appBar =
        comp<MudAppBar> {
            comp<MudIconButton> {
                attr.Icon Icons.Material.Filled.Menu
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
            }
        }

    let main =
        match model.CurrentPage with
        | Home ->
            Home.render ()
        | Counter m ->
            Counter.render (router.Link Url.Home) m (CounterMsg >> dispatch)
        | NotFound ->
            NotFound.render ()

    concat {
        comp<MudThemeProvider> {
            attr.IsDarkMode model.IsDarkMode
        }
        comp<MudLayout> {
            appBar
            sideBar
            comp<MudMainContent> {
                comp<MudContainer> {
                    attr.MaxWidth MaxWidth.ExtraLarge
                    attr.Class "pt-5"
                    main
                }
            }
        }
    }

type App() =
    inherit ProgramComponent<Model, Msg>()

    [<Inject>]
    member val HttpClient = Unchecked.defaultof<HttpClient> with get, set

    override this.Program =
        let update = update this.HttpClient
        Program.mkProgram init update render
        |> Program.withRouter router
        #if DEBUG
        |> Program.withConsoleTrace
        #endif
