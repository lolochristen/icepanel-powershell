using IcePanel.Api;
using IcePanel.Api.Models;
using Microsoft.Kiota.Abstractions.Serialization;
using System.Management.Automation;
using System.Text;

namespace IcePanel.Powershell;

[Cmdlet(VerbsData.Import, "IcePanelLandscape")]
public class ImportLandscape : IcePanelCmdlet
{
    [Parameter(
        Mandatory = false,
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
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    public PSObject? InputObject { get; set; }


    [Parameter]
    public SwitchParameter FromExportFormat { get; set; }

    protected override void ProcessRecord()
    {
        var api = GetApiClient();

        if (InputObject  == null)
        {
            return;
        }

        var landscapeIds = LandscapeId != null ? new[] { LandscapeId } : Landscape?.Select(p => p.Id);
        if (landscapeIds == null) throw new ArgumentException("Landscape need to be set");

        foreach (var lid in landscapeIds)
        {
            var result = RunSync(async () => 
            {
                LandscapeImportData? importData;

                if (InputObject.BaseObject is string s)
                {
                    if (FromExportFormat.IsPresent)
                    {
                        importData = await LandscapeImportMapper.FromJson(s);
                    }
                    else
                    {
                        importData = await KiotaJsonSerializer.DeserializeAsync<LandscapeImportData>(s);
                    }
                }
                else if (InputObject.BaseObject is LandscapeImportData data)
                {
                    importData = data;
                }
                else if (InputObject.BaseObject is byte[] bytes)
                {
                    using var ms = new MemoryStream(bytes);
                    if (FromExportFormat.IsPresent)
                    {
                        importData = await LandscapeImportMapper.FromJson(Encoding.UTF8.GetString(ms.ToArray()));
                    }
                    else
                    {
                        importData = await KiotaJsonSerializer.DeserializeAsync<LandscapeImportData>(ms);
                    }
                }
                else
                {
                    throw new ArgumentException("InputObject must be either a string or a LandscapeImportData");
                }

                if (importData == null)
                {
                    throw new ArgumentException("InputObject must be contain valid LandscapeImportData");
                }

                return await api.Landscapes[lid].Versions[Version].Import.PostAsImportPostResponseAsync(importData);
            });
        }
    }
}
