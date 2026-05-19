using IcePanel.Api.Models;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IcePanel.Powershell;

[Cmdlet(VerbsData.Export, "IcePanelLandscape")]
public class ExportLandscape : IcePanelCmdlet
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
        Position = 2,
        ValueFromPipelineByPropertyName = true)]
    public Api.Models.LandscapeExportType? LandscapeExportType { get; set; } = Api.Models.LandscapeExportType.Json;

    [Parameter(Mandatory = false)]
    public string? FilePath { get; set; }

    protected override void ProcessRecord()
    {
        var api = GetApiClient();

        var retryOptions = new RetryStrategyOptions
        {
            MaxRetryAttempts = 3,
            Delay = TimeSpan.FromSeconds(1)
        };
        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(retryOptions)
            .Build();

        var landscapeIds = LandscapeId != null ? new[] { LandscapeId } : Landscape?.Select(p => p.Id);
        if (landscapeIds == null) throw new ArgumentException("Landscape need to be set");

        foreach (var lid in landscapeIds)
        {
            var result = RunSync(() => api.Landscapes[lid].Versions[Version].Export.PostAsExportPostResponseAsync(
                new LandscapeExportOptions(),
                (config) =>
                {
                    config.QueryParameters.TypeAsLandscapeExportType = LandscapeExportType;
                }));

            var landscapeExport = result.LandscapeExport;

            WriteObject(result.LandscapeExport);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("IcePanel.Powershell", "0.0.1"));

            if (!string.IsNullOrEmpty(FilePath))
            {
                var task = Task.Run(async () =>
                {
                    await pipeline.ExecuteAsync(async cancellationToken =>
                    {
                        var response = await httpClient.GetAsync(landscapeExport.FileUrl, cancellationToken);
                        response.EnsureSuccessStatusCode();
                        await using var imageStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                        await using var fileStream = File.Create(FilePath);
                        await imageStream.CopyToAsync(fileStream, cancellationToken);
                    });
                });

                task.Wait(new TimeSpan(0, 0, 30));
            }
        }
    }
}
