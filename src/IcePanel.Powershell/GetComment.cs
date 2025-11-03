using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelComment")]
[OutputType(typeof(Comment))]
public class GetComment : IcePanelCmdlet
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
    public string? CommentId { get; set; }

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
            if (CommentId != null)
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].Comments[CommentId].GetAsWithCommentGetResponseAsync());
                WriteObject(result.Comment);
            }
            else
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].Comments.GetAsCommentsGetResponseAsync());
                WriteObject(result.Comments);
            }
    }
}