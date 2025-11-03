# IcePanel PowerShell Module

PowerShell module providing convenient access to the IcePanel API.

## Features
- Simple `Connect-IcePanel` cmdlet to store API key and default organization in session
- Cmdlets to fetch organizations, landscapes, versions, diagrams, model objects, flows, domains, tags, comments, drafts, connections, groups, share links
- Export diagram images (PNG/SVG) via `Request-IcePanelDiagramImage`

## Prerequisites
- PowerShell7+ (Core edition) – module manifest targets `CompatiblePSEditions = Core`
- .NET8 SDK (only required if you build from source)
- IcePanel API key (get from IcePanel)

## Build From Source
```powershell
# Clone
git clone https://github.com/lolochristen/icepanel-powershell.git
cd icepanel-powershell

# Build
dotnet build
```

After build the module manifest is at:
```
src/IcePanel.Powershell/IcePanel.Powershell.psd1
```
The compiled assembly is at:
```
src/IcePanel.Powershell/bin/Debug/net8.0/IcePanel.Powershell.dll
```

## Import Module (Local Build)
Option1: Import by manifest path
```powershell
Import-Module ./src/IcePanel.Powershell/IcePanel.Powershell.psd1 -Force
```
Option2: Copy to a module path (e.g. for user scope)
```powershell
$dest = Join-Path $env:USERPROFILE 'Documents\PowerShell\Modules\IcePanel.Powershell'
New-Item -ItemType Directory -Path $dest -Force | Out-Null
Copy-Item -Recurse src/IcePanel.Powershell/* $dest
Import-Module IcePanel.Powershell -Force
```

## Session Connection
Use your API key once per session:
```powershell
$apiKey = '<YOUR_API_KEY>'
Connect-IcePanel -ApiKey $apiKey
```
This stores:
- `IcePanelApiKey` (for subsequent calls)
- `IcePanelOrganizationId` (first organization returned if available)

You can override organization explicitly:
```powershell
Connect-IcePanel -ApiKey $apiKey -OrganizationId 'org_123'
```

## Basic Retrieval Examples
List organizations:
```powershell
Get-IcePanelOrganization
```
Get landscapes (default organization from connect):
```powershell
Get-IcePanelLandscape
```
Filter landscapes by name:
```powershell
Get-IcePanelLandscape -Name 'Production'
```
Pipe landscapes to get diagrams:
```powershell
Get-IcePanelLandscape | Get-IcePanelDiagram -Version latest
```
Get a specific diagram by id:
```powershell
Get-IcePanelDiagram -LandscapeId 'land_123' -DiagramId 'diag_456'
```
Get model objects:
```powershell
Get-IcePanelLandscape -Name 'Core Platform' | Get-IcePanelModelObject
```
Get flows / domains / tags / comments:
```powershell
Get-IcePanelLandscape | Get-IcePanelFlow
Get-IcePanelLandscape | Get-IcePanelDomain
Get-IcePanelLandscape | Get-IcePanelTag
Get-IcePanelLandscape | Get-IcePanelComment
```
Get versions for a landscape:
```powershell
Get-IcePanelLandscape -Name 'Core Platform' | Get-IcePanelVersion
```
Get share link:
```powershell
Get-IcePanelLandscape -Name 'Core Platform' | Get-IcePanelShareLink
```

## Export Diagram Images
```powershell
$diagram = Get-IcePanelLandscape -Name 'Core Platform' | Get-IcePanelDiagram -Version latest | Select-Object -First1
$diagram | Request-IcePanelDiagramImage -OutPath ./export -Download PngAndSvg -Theme Dark
```
Output files will be placed in `./export` with sanitized names.

## Available Cmdlets
```
Connect-IcePanel
Get-IcePanelOrganization
Get-IcePanelLandscape
Get-IcePanelVersion
Get-IcePanelDiagram
Get-IcePanelDiagramGroup
Get-IcePanelDraft
Get-IcePanelFlow
Get-IcePanelDomain
Get-IcePanelTagGroup
Get-IcePanelTag
Get-IcePanelComment
Get-IcePanelModelObject
Get-IcePanelModelConnection
Get-IcePanelUser
Get-IcePanelShareLink
Request-IcePanelDiagramImage
```

## Uninstall (Local Copy)
Remove the module directory from your `$env:PSModulePath` and restart your session.

## Contributing
Issues / PRs welcome. Keep changes small and focused.

## License
MIT (see repository).
