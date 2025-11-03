Import-Module .\IcePanel.Powershell.psd1

# store your API key in a file named .icepanel in your user profile folder
$apikey = Get-Content -Path "$($env:USERPROFILE)\.icepanel"
$org = Connect-IcePanel $apiKey

#$org = Get-IcePanelOrganization
Write-Host "Organization: $($org.Name)"

Get-IcePanelLandscape | Format-Table

$ls = Get-IcePanelLandscape -Name "Isolutions Quickstart (Blazor) Template"

$diag = Get-IcePanelDiagram -LandscapeId $ls.Id

$diag | Format-Table

Request-IcePanelDiagramImage $diag[0]

#Get-IcePanelModelObject -Landscape $ls | ConvertTo-Json -Depth 10 | Out-File "modelobjects.json"

pause