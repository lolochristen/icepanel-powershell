@{
GUID="F2AC5887-E79A-46AC-BBCB-410B1AB19925"
Author="Laurent Christen"
Copyright="Copyright (c) Laurent Christen"
ModuleVersion="0.0.0.1"
CompatiblePSEditions = @("Core")
PowerShellVersion="3.0"
NestedModules="IcePanel.Powershell.dll"
FunctionsToExport = @()
AliasesToExport = @()
CmdletsToExport=@(
    "Connect-IcePanel",
    "Get-IcePanelDiagram",
    "Get-IcePanelComment",
    "Get-IcePanelTag",
    "Get-IcePanelModelObject",
    "Get-IcePanelUser",
    "Get-IcePanelVersion",
    "Get-IcePanelFlow",
    "Get-IcePanelLandscape",
    "Get-IcePanelDraft",
    "Get-IcePanelOrganization",
    "Get-IcePanelShareLink",
    "Get-IcePanelTagGroup",
    "Request-IcePanelDiagramImage",
    "Get-IcePanelDiagramGroup",
    "Get-IcePanelDomain",
    "Get-IcePanelModelConnection"
    )
}