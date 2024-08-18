module Cosmonaut.Cosmos

open System.IO
open System.Text.Json
open Microsoft.Azure.Cosmos

let container name (database: Database) = database.GetContainer name
let database name (client: CosmosClient) = client.GetDatabase name
let client (key: string) endpoint = new CosmosClient(endpoint, key)

let query iter sql (container: Container) =
    sql
    |> Option.defaultValue "SELECT TOP 10 * FROM c ORDER BY c._ts DESC"
    |> container.GetItemQueryStreamIterator
    |> fun feed ->
        async {
            while feed.HasMoreResults do
                let! response = feed.ReadNextAsync() |> Async.AwaitTask
                use reader = new StreamReader(response.Content)
                let! content = reader.ReadToEndAsync() |> Async.AwaitTask
                let json = JsonSerializer.Deserialize<JsonElement>(content)
                iter json
        }
    |> Async.RunSynchronously
