using IcePanel.Api.Models;
using System.Management.Automation;
using System.Security.Cryptography;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelShareLink")]
[OutputType(typeof(ShareLink))]
public class GetShareLink : IcePanelCmdlet
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
        Position = 1,
        ValueFromPipelineByPropertyName = true)]
    public string? Version { get; set; } = "latest";

    [Parameter(
        Mandatory = false,
        ValueFromPipelineByPropertyName = true)]
    public string? ShareLinkId { get; set; }


    protected override void ProcessRecord()
    {
        var api = GetApiClient();

        if (ShareLinkId != null)
        {
            var result = RunSync(() => api.ShareLinks[ShareLinkId].GetAsWithShortGetResponseAsync());
            WriteObject(result.ShareLink);
            return;
        }

        var landscapeIds = LandscapeId != null ? new[] { LandscapeId } : Landscape?.Select(p => p.Id);
        if (landscapeIds == null) throw new ArgumentException("Landscape need to be set");

        foreach (var lid in landscapeIds)
        {
            var result = RunSync(() => api.Landscapes[lid].Versions[Version].ShareLink.GetAsShareLinkGetResponseAsync());
            WriteObject(result.ShareLink);
        }
    }
}