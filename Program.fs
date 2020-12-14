open Farmer
open Farmer.Builders
open System

// Create ARM resources here e.g. webApp { } or storageAccount { } etc.
// See https://compositionalit.github.io/farmer/api-overview/resources/ for more details.

let storage = storageAccount {
    name "cmarshall10450farmerst"
    sku Storage.Sku.Standard_GRS
    add_private_container "config"
    add_private_container "inbound"
    add_private_container "sandbox"
    add_private_container "staging"
    add_private_container "warehouse"
}

let workspace = databricksWorkspace {
    name "cmarshall10450-databricks-workspace"
    pricing_tier Databricks.Sku.Standard
    managed_resource_group_id "databricks-managed-rg"
    byov_vnet "cmarshall10450-databricks-vnet"
    byov_public_subnet "databricks-pub-snet"
    byov_private_subnet "databricks-priv-snet"
}

// Add resources to the ARM deployment using the add_resource keyword.
// See https://compositionalit.github.io/farmer/api-overview/resources/arm/ for more details.
let deployment = arm {
    location Location.NorthEurope
    add_resource storage
    add_resource workspace
}

let resourceGroupName =
    match Environment.GetEnvironmentVariable "RESOURCE_GROUP_NAME" with
    | null -> failwith "Missing RESOURCE_GROUP_NAME environment variable"
    | value -> value

// deployment
// |> Deploy.execute "farmer-deployments" Deploy.NoParameters

deployment
|> Writer.quickWrite "azuredeploy"