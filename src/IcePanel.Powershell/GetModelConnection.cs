using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelModelConnection")]
[OutputType(typeof(ModelConnection))]
public class GetModelConnection : IcePanelCmdlet
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
    public string? ModelConnectionId { get; set; }

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
            if (ModelConnectionId != null)
            {
                var result = api.Landscapes[lid].Versions[Version].Model.Connections[ModelConnectionId].GetAsWithModelConnectionGetResponseAsync().GetAwaiter().GetResult();
                WriteObject(result.ModelConnection);
            }
            else
            {
                var result = api.Landscapes[lid].Versions[Version].Model.Connections.GetAsConnectionsGetResponseAsync().GetAwaiter().GetResult();
                WriteObject(result.ModelConnections);
            }
    }
}