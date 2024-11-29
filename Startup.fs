namespace MyBoleroApp

open Microsoft.AspNetCore.Components.WebAssembly.Hosting
open Microsoft.Extensions.DependencyInjection
open System.Net.Http
open MudBlazor.Services

module Program =

    [<EntryPoint>]
    let main args =
        let builder = WebAssemblyHostBuilder.CreateDefault(args)
        builder.RootComponents.Add<Main.App>("#main")

        builder.Services
            .AddScoped<HttpClient>(fun _ -> new HttpClient())
            .AddMudServices()
        |> ignore

        builder.Build().RunAsync() |> ignore
        0
