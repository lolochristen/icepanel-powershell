using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelDiagram")]
[OutputType(typeof(Diagram))]
public class GetDiagram : IcePanelCmdlet
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
    public string? DiagramId { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 1,
        ValueFromPipelineByPropertyName = true)]
    public string? Version { get; set; } = "latest";

    protected override void ProcessRecord()
    {
        var api = GetApiClient();

        var landscapeIds = LandscapeId != null ? [LandscapeId] : Landscape?.Select(p => p.Id);

        if (landscapeIds == null) throw new ArgumentException("Landscape need to be set");

        foreach (var lid in landscapeIds)
            if (DiagramId != null)
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].Diagrams[DiagramId].GetAsWithDiagramGetResponseAsync());
                WriteObject(result.Diagram);
            }
            else
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].Diagrams.GetAsDiagramsGetResponseAsync());
                WriteObject(result.Diagrams);
            }
    }
}