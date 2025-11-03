using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelVersion")]
[OutputType(typeof(VersionObject))]
public class GetVersion : IcePanelCmdlet
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

    protected override void ProcessRecord()
    {
        var api = GetApiClient();

        var landscapeIds = LandscapeId != null ? [LandscapeId] : Landscape?.Select(p => p.Id);

        if (landscapeIds == null) throw new ArgumentException("Landscape need to be set");

        foreach (var lid in landscapeIds)
        {
            var result = RunSync(() => api.Landscapes[lid].Versions.GetAsVersionsGetResponseAsync());
            WriteObject(result.Versions);
        }
    }
}