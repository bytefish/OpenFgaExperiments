<# Licensed under the MIT license. See LICENSE file in the project root for full license information.#>

# The Path to the OpenFGA CLI, so we don't have to put it into the 
# global PATH. This makes it easier to just download a new version 
# and point the script at it.
$fgaCliExecutable = "C:\Users\philipp\apps\fga_0.2.1_windows_amd64\fga.exe"

# The OpenFGA Model to transform. This is written in the FGA DSL.
$fgaModelFilename = "${PSScriptRoot}\src\Server\RebacExperiments.Server.Api\Resources\task-management-model.fga"

# The Transform Command to transform from FGA to JSON
$fgaCreateStoreCmd = "${fgaCliExecutable} store create --name ""Task Management Application"" --model ${fgaModelFilename}"

# Run the Transform Command, Pretty Print Results, Write to Output File
$fgaCreateStoreResponse = Invoke-Expression $fgaCreateStoreCmd

# Transform Response to a PSObject ...
$fgaStoreJson = $fgaCreateStoreResponse | ConvertFrom-Json

# ... extract the StoreID
$fgaStoreId = $fgaStoreJson.store.id
$fgaAuthorizationModelId = $fgaStoreJson.model.authorization_model_id

# ... write it to the "FGA_STORE_ID" environment variable
$env:OpenFGA__StoreId=$fgaStoreId
$env:OpenFGA__AuthorizationModelId=$fgaAuthorizationModelId

# ... and output the StoreID for Copy and Pasting
Write-Output "OpenFGA StoreId:                  ${fgaStoreId}"
Write-Output "OpenFGA AuthorizationModelId:     ${fgaAuthorizationModelId}"

# ... and output the raw JSON Reponse for Copy and Pasting
Write-Output "JSON Response:"
Write-Output "$($fgaStoreJson | ConvertTo-Json)"
