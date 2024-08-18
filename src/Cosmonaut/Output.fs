module Cosmonaut.Output

open System.Text.Json
open Spectre.Console
open Spectre.Console.Json
open Spectre.Console.Rendering

let ofJsonString (json: string) = JsonText(json) :> IRenderable
let ofJsonElement (json: JsonElement) = json.ToString() |> ofJsonString
let toConsole (content: IRenderable) = AnsiConsole.Write(content)

let toConsoleN content =
    toConsole content
    AnsiConsole.WriteLine()
