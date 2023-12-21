<# Licensed under the MIT license. See LICENSE file in the project root for full license information.#>

# Kiota Executable
$kiota_exe="kiota"

# Parameters for the Code Generator
$param_openapi_schema="https://localhost:5000/odata/openapi.json"
$param_language="csharp"
$param_namespace="RebacExperiments.Shared.ApiSdk"
$param_log_level="Trace"
$param_out_dir="${PSScriptRoot}/src/Shared/RebacExperiments.Shared.ApiSdk"

# Construct the "kiota generate" Command
$cmd="${kiota_exe} generate --openapi ${param_openapi_schema} --language ${param_language} --namespace-name ${param_namespace} --log-level ${param_log_level} --output ${param_out_dir}"

# Run the the "kiota generate" Command
Invoke-Expression $cmd