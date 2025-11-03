using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommon.Get, "IcePanelTeam")]
[OutputType(typeof(Team))]
public class GetTeam : IcePanelCmdlet
{
    [Parameter(
        Mandatory = false,
        ValueFromPipelineByPropertyName = true)]
    public string? OrganizationId { get; set; }

    [Parameter(
        Mandatory = false,
        Position = 0,
        ValueFromPipelineByPropertyName = true)]
    public string? TeamId { get; set; }

    protected override void ProcessRecord()
    {
        var api = GetApiClient();

        if (TeamId != null)
        {
            var result = RunSync(() => api.Organizations[OrganizationId ?? GlobalOrganizationId].Teams[TeamId].GetAsWithTeamGetResponseAsync());
            WriteObject(result.Team);
        }
        else
        {
            var result = RunSync(() => api.Organizations[OrganizationId ?? GlobalOrganizationId].Teams.GetAsTeamsGetResponseAsync());
            WriteObject(result.Teams);
        }
    }
}