using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelOrganization")]
[OutputType(typeof(Organization))]
public class GetOrganization : IcePanelCmdlet
{
    [Parameter(
        Mandatory = false,
        ValueFromPipelineByPropertyName = true)]
    public string? OrganizationId { get; set; }

    protected override void ProcessRecord()
    {
        var api = GetApiClient();
        if (OrganizationId != null)
        {
            var result = RunSync(() => api.Organizations[OrganizationId].GetAsWithOrganizationGetResponseAsync());
            WriteObject(result.Organization);
        }
        else
        {
            var result = RunSync(() => api.Organizations.GetAsOrganizationsGetResponseAsync());
            WriteObject(result.Organizations);
        }
    }
}