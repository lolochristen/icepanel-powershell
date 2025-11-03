using System.Management.Automation;
using IcePanel.Api.Models;

namespace IcePanel.Powershell;

[Cmdlet(VerbsCommunications.Connect, "IcePanel")]
[OutputType(typeof(Organization))]
public class Connect : Cmdlet
{
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipelineByPropertyName = true)]
    public string ApiKey { get; set; } = null!;

    [Parameter(
        Mandatory = false,
        Position = 1,
        ValueFromPipelineByPropertyName = true)]
    public string? OrganizationId { get; set; }

    protected override void ProcessRecord()
    {
        CommandRuntime.Host.PrivateData.Properties.Add(new PSVariableProperty(new PSVariable("IcePanelApiKey", ApiKey, ScopedItemOptions.Private)));

        var api = IcePanelCmdlet.GetApiClient(ApiKey);
        var result = IcePanelCmdlet.RunSync(() => api.Organizations.GetAsOrganizationsGetResponseAsync());

        var org = result.Organizations?.FirstOrDefault();
        if (org != null)
        {
            CommandRuntime.Host.PrivateData.Properties.Add(new PSVariableProperty(new PSVariable("IcePanelOrganizationId", org.Id, ScopedItemOptions.Private)));
            WriteObject(org);
            return;
        }
        else if (OrganizationId != null)
        {
            CommandRuntime.Host.PrivateData.Properties.Add(new PSVariableProperty(new PSVariable("IcePanelOrganizationId", OrganizationId, ScopedItemOptions.Private)));
            WriteObject(result.Organizations.Single(p => p.Id.Equals(OrganizationId)));
            return;
        }
        WriteObject(result.Organizations);
    }
}