<# Licensed under the MIT license. See LICENSE file in the project root for full license information.#>

# The Path to the OpenFGA CLI, so we don't have to put it into the 
# global PATH. This makes it easier to just download a new version 
# and point the script at it.
$fgaCliExecutable = "C:\Users\philipp\apps\fga_0.2.1_windows_amd64\fga.exe"

# The OpenFGA Model to transform. This is written in the FGA DSL.
$fgaModelFilename = "${PSScriptRoot}\src\Server\RebacExperiments.Server.Api\Resources\task-management-model.fga"

$fgaJsonModelFilename = "${PSScriptRoot}\src\Server\RebacExperiments.Server.Api\Resources\task-management-model.json"

# The Transform Command to transform from FGA to JSON
$fgaModelTransformCmd = "${fgaCliExecutable} model transform --input-format fga --file ${fgaModelFilename}"

# Run the Transform Command, Pretty Print Results, Write to Output File
Invoke-Expression $fgaModelTransformCmd | ConvertFrom-Json | ConvertTo-Json -Depth 100 | Out-File -FilePath $fgaJsonModelFilename