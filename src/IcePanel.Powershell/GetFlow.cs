using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelFlow")]
[OutputType(typeof(Flow))]
public class GetFlow : IcePanelCmdlet
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
    public string? FlowId { get; set; }

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
            if (FlowId != null)
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].Flows[FlowId].GetAsWithFlowGetResponseAsync());
                WriteObject(result.Flow);
            }
            else
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].Flows.GetAsFlowsGetResponseAsync());
                WriteObject(result.Flows);
            }
    }
}