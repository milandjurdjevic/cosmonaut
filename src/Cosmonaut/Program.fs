open System
open Argu
open Cosmonaut

type Command =
    | [<MainCommand>] Query of string
    | [<AltCommandLine("-c"); ExactlyOnce>] Container of string
    | [<AltCommandLine("-d"); ExactlyOnce>] Database of string
    | [<AltCommandLine("-e"); ExactlyOnce>] Endpoint of string
    | [<AltCommandLine("-k"); ExactlyOnce>] Key of string

    interface IArgParserTemplate with
        member this.Usage =
            match this with
            | Query _ -> "The SQL query to execute against the Cosmos container."
            | Database _ -> "The name of the Cosmos database."
            | Endpoint _ -> "The endpoint URL of the Cosmos account."
            | Key _ -> "The secret key of the Cosmos account."
            | Container _ -> "The name of the Cosmos container."

try
    let command =
        ArgumentParser
            .Create<Command>()
            .ParseCommandLine(Environment.GetCommandLineArgs() |> Array.skip 1)

    use client = Cosmos.client (command.GetResult Key) (command.GetResult Endpoint)

    client
    |> Cosmos.database (command.GetResult Database)
    |> Cosmos.container (command.GetResult Container)
    |> Cosmos.query (Output.ofJsonElement >> Output.toConsoleN) (command.TryGetResult Query)
with :? ArguParseException as error ->
    error.Message |> eprintfn "%s"
