https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-aspire-integration
https://learn.microsoft.com/en-us/azure/azure-functions/functions-run-local

# Create a new function
func init Project.Functions --worker-runtime dotnet-isolated

func new --template "Http Trigger" --name TodoNotifierF