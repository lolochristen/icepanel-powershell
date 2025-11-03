using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelLandscape")]
[OutputType(typeof(Landscape))]
public class GetLandscape : IcePanelCmdlet
{
    [Parameter(
        Mandatory = false,
        ValueFromPipelineByPropertyName = true)]
    public string? OrganizationId { get; set; }

    [Parameter(
        Mandatory = false,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public Organization[]? Organization { get; set; }

    [Parameter(
        Mandatory = false,
        ValueFromPipelineByPropertyName = true)]
    public string? LandscapeId { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 0,
        ValueFromPipelineByPropertyName = true)]
    public string? Name { get; set; }

    protected override void ProcessRecord()
    {
        var api = GetApiClient();

        var filter = (Landscape l) => true;
        if (!string.IsNullOrEmpty(Name)) filter = l => l.Name.Equals(Name, StringComparison.CurrentCultureIgnoreCase);

        if (LandscapeId != null)
        {
            var result = RunSync(() => api.Landscapes[LandscapeId].GetAsWithLandscapeGetResponseAsync());
            WriteObject(result.Landscape);
        }
        else if (Organization != null)
        {
            foreach (var organization in Organization)
            {
                var result = RunSync(() => api.Organizations[organization.Id].Landscapes.GetAsLandscapesGetResponseAsync());
                WriteObject(result.Landscapes.Where(filter).ToList());
            }
        }
        else
        {
            OrganizationId ??= GlobalOrganizationId;
            if (OrganizationId == null) throw new ArgumentException("OrganizationId or Organization need to be set");
            var result = RunSync(() => api.Organizations[OrganizationId].Landscapes.GetAsLandscapesGetResponseAsync());
            WriteObject(result.Landscapes.Where(filter).ToList());
        }
    }
}
