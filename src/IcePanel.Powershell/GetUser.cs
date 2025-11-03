using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelUser")]
[OutputType(typeof(OrganizationUser))]
public class GetUser : IcePanelCmdlet
{
    [Parameter(
        Mandatory = false,
        ValueFromPipelineByPropertyName = true)]
    public string? OrganizationId { get; set; }

    protected override void ProcessRecord()
    {
        var api = GetApiClient();

        var result = RunSync(() => api.Organizations[OrganizationId ?? GlobalOrganizationId].Users.GetAsUsersGetResponseAsync());
        WriteObject(result.OrganizationUsers);
    }
}