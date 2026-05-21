Import-Module .\IcePanel.Powershell.psd1

# store your API key in a file named .icepanel in your user profile folder
$apikey = Get-Content -Path "$($env:USERPROFILE)\.icepanel"
$org = Connect-IcePanel $apiKey

#$org = Get-IcePanelOrganization
Write-Host "Organization: $($org.Name)"
$ls = Get-IcePanelLandscape

$ls | Format-Table

#$diag = Get-IcePanelDiagram -LandscapeId $ls.Id
#$diag | Request-IcePanelDiagramImage

#Export-IcePanelLandscape $ls[1].Id -FilePath "landscape.json"

#Get-Content -Raw "landscape.json" | Import-IcePanelLandscape $ls[0].Id -FromExportFormat

# Request-IcePanelDiagramImage $diag[0]

#Get-IcePanelModelObject -Landscape $ls | ConvertTo-Json -Depth 10 | Out-File "modelobjects.json"

pause