# Az PowerShell module must be install and you must login into azure using Connect-AzAccount

$randomNumber = Get-Random

$rg = "ARM-FA-Service-Bus-RG-" + $randomNumber

$deploymentName = $rg + '-deployment'

New-AzResourceGroup -Name $rg -Location northeurope -Force

# Validate the template before running the deployment
# Test-AzResourceGroupDeployment -ResourceGroupName $rg -TemplateFile '03-function-app.json'

New-AzResourceGroupDeployment -Name $deploymentName -resourceGroupname $rg -TemplateFile '03-function-app.json'