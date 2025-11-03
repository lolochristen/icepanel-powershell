using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelModelObject")]
[OutputType(typeof(ModelObject))]
public class GetModelObject : IcePanelCmdlet
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
    public string? ModelObjectId { get; set; }

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
        {
            if (ModelObjectId != null)
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].Model.Objects[ModelObjectId].GetAsWithModelObjectGetResponseAsync());
                WriteObject(result.ModelObject);
            }
            else
            {
                var result = RunSync(() => api.Landscapes[lid].Versions[Version].Model.Objects.GetAsObjectsGetResponseAsync());
                WriteObject(result.ModelObjects, true);
            }
        }
    }
}