using System.Management.Automation;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using IcePanel.Api.Models;
using Polly;
using Polly.Retry;

namespace IcePanel.Powershell;

public enum DiagramDownload
{
    Png,
    Svg,
    PngAndSvg,
    NoDownload,
}

public enum DiagramFileName
{
    Name,
    Id
}

[Cmdlet(VerbsLifecycle.Request, "IcePanelDiagramImage")]
[OutputType(typeof(DiagramExportImage))]
public class RequestDiagramImage : IcePanelCmdlet
{
    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public Diagram[] Diagram { get; set; } = null!;

    [Parameter(Mandatory = false)] public Theme Theme { get; set; } = Theme.Light;

    [Parameter(Mandatory = false, Position = 1)]
    public string? OutPath { get; set; }

    [Parameter(Mandatory = false)] public DiagramDownload Download { get; set; } = DiagramDownload.Png;

    [Parameter(Mandatory = false)] public DiagramFileName FileName { get; set; } = DiagramFileName.Name;

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

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("IcePanel.Powershell", "0.0.1"));
        httpClient.DefaultRequestHeaders.Accept.ParseAdd("image/png");
        httpClient.DefaultRequestHeaders.Accept.ParseAdd("image/svg+xml");

        foreach (var diagram in Diagram)
        {
            var export = RunSync(() => api.Landscapes[diagram.LandscapeId]
                .Versions[diagram.VersionId]
                .Diagrams[diagram.Id]
                .Export.Image
                .PostAsImagePostResponseAsync(new DiagramExportImageOptions { Theme = Theme }));

            var diagramExportImage = export.DiagramExportImage;

            do
            {
                Thread.Sleep(100);
                var result = RunSync(() => api.Landscapes[diagram.LandscapeId]
                    .Versions[diagram.VersionId]
                    .Diagrams[diagram.Id]
                    .Export.Image[diagramExportImage.Id]
                    .GetAsWithDiagramExportImageGetResponseAsync());

                diagramExportImage = result.DiagramExportImage;

            } while (string.IsNullOrEmpty(diagramExportImage?.FileUrls?.Png));

            WriteObject(diagramExportImage);

            if (Download != DiagramDownload.NoDownload)
            {
                OutPath ??= "./";

                var task = Task.Run(async () =>
                {
                    var baseFileName = FileName == DiagramFileName.Name ? CleanFileName(diagram.Name) : diagram.Id;
                    if (Download == DiagramDownload.Png || Download == DiagramDownload.PngAndSvg)
                    {
                        await pipeline.ExecuteAsync(async cancellationToken =>
                        {
                            var response = await httpClient.GetAsync(diagramExportImage.FileUrls.Png, cancellationToken);
                            response.EnsureSuccessStatusCode();
                            await using var imageStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                            await using var fsPng = File.Create(Path.Combine(OutPath, baseFileName + ".png"));
                            await imageStream.CopyToAsync(fsPng, cancellationToken);
                        });
                    }

                    if (Download == DiagramDownload.Svg || Download == DiagramDownload.PngAndSvg)
                    {
                        await pipeline.ExecuteAsync(async cancellationToken =>
                        {
                            var response = await httpClient.GetAsync(diagramExportImage.FileUrls.Svg, cancellationToken);
                            response.EnsureSuccessStatusCode();
                            await using var imageStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                            await using var fsPng = File.Create(Path.Combine(OutPath, baseFileName + ".svg"));
                            await imageStream.CopyToAsync(fsPng, cancellationToken);
                        });
                    }
                });

                task.Wait(new TimeSpan(0, 0, 30));
            }
        }
    }

    private static string CleanFileName(string filename)
    {
        foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
            filename = filename.Replace(invalidFileNameChar, '_');
        return filename;
    }
}
