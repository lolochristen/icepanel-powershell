using IcePanel.Api;
using IcePanel.Api.Models;
using System.Management.Automation;

namespace IcePanel.Powershell;

public abstract class IcePanelCmdlet : Cmdlet
{
    [Parameter(
        Mandatory = false,
        ValueFromPipelineByPropertyName = true)]
    public string? ApiKey { get; set; }

    protected string? GlobalOrganizationId => CommandRuntime.Host.PrivateData.Properties["IcePanelOrganizationId"]?.Value.ToString();

    protected ApiClient GetApiClient()
    {
        if (string.IsNullOrEmpty(ApiKey))
        {
            var apiKeyVar = CommandRuntime.Host.PrivateData.Properties["IcePanelApiKey"];
            ApiKey = apiKeyVar?.Value.ToString();
        }

        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new InvalidDataException("ApiKey is not set. Use Connect-IcePanel or ApiKey property");
        }

        return GetApiClient(ApiKey);
    }

    internal static ApiClient GetApiClient(string apiKey)
    {
        return new ApiClient(apiKey);
    }

    internal static T RunSync<T>(Func<Task<T?>> taskFactory)
    {
        // Avoids capturing PowerShell SynchronizationContext
        var result = Task.Run(taskFactory).GetAwaiter().GetResult();

        if (result == null)
            throw new InvalidOperationException("Result is empty");

        return result;
    }
}