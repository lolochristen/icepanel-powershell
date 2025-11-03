using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelDiagramGroup")]
[OutputType(typeof(DiagramGroup))]
public class GetDiagramGroup : IcePanelCmdlet
{
    [Parameter(
        Mandatory = false,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public Landscape[]? Landscape { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 0,
        ValueFromPipelineByPropertyName = true)]
    public string? LandscapeId { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 0,
        ValueFromPipelineByPropertyName = true)]
    public string? DiagramGroupId { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 1,
        ValueFromPipelineByPropertyName = true)]
    public string? Version { get; set; } = "latest";

    protected override void ProcessRecord()
    {
        var api = GetApiClient();

        var landscapeIds = LandscapeId != null ? new[] { LandscapeId } : Landscape?.Select(p => p.Id);
        if (landscapeIds == null) throw new ArgumentException("Landscape need to be set");

        foreach (var lid in landscapeIds)
            if (DiagramGroupId != null)
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].DiagramGroups[DiagramGroupId].GetAsWithDiagramGroupGetResponseAsync());
                WriteObject(result.DiagramGroup);
            }
            else
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].DiagramGroups.GetAsDiagramGroupsGetResponseAsync());
                WriteObject(result.DiagramGroups);
            }
    }
}