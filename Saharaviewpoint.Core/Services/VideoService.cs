// ========================================================================
// Copyright (c) Kingdom Scripts Technology Solutions. All rights reserved.
// Author: Mordecai Godwin
// Website: https://kingdomscripts.com. Email: mordecai@kingdomscripts.com
// ========================================================================

using Mapster;
using Newtonsoft.Json;
using Saharaviewpoint.Models.ApiVideo.Response;
using Saharaviewpoint.Models.App;
using Saharaviewpoint.Models.App.Constants;
using Saharaviewpoint.Models.Configurations;
using Saharaviewpoint.Models.Input.Task;
using Saharaviewpoint.Models.Utilities;
using Serilog;
using System.Text;

namespace Saharaviewpoint.Core.Services;
internal class VideoService
{
    private readonly SaharaviewpointContext _context;
    private readonly ApiVideoConfig _apiVideoConfig;
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    public async Task<Result> GetUploadToken(string taskId)
    {
        var response = await _client.PostAsync("upload-tokens", null);
        if (!response.IsSuccessStatusCode)
            return new ErrorResult("Failed to get video upload token");

        string content = await response.Content.ReadAsStringAsync();
        var uploadToken = JsonConvert.DeserializeObject<ApiVideoTokenView>(content);

        return new SuccessResult(uploadToken);
    }

    public async Task<Result> SetVideoDetails(VideoDetailModel model)
    {
        ApiVideoDetail request = model.Adapt<ApiVideoDetail>();
        request.playerId = _apiVideoConfig.PlayerId;
        var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        var response = await _client.PatchAsync($"videos/{model.videoId}", content);

        if (!response.IsSuccessStatusCode)
        {
            string contentString = await response.Content.ReadAsStringAsync();
            _logger.Error("Failed to set video details. {@Error}", contentString);
            return new ErrorResult("Failed to set video details.");
        }

        var document = new Document
        {
            Name = model.title,
            Type = DocumentTypes.VIDEO,
            Url = model.assets.,
            ThumbnailUrl = fileType == DocumentTypes.IMAGE
                    ? $"{folder}/{subFolder}/_thumbnail/{fileUploadName}"
                    : $"{folder}/{subFolder}/_thumbnail/{fileType}.png",
            CreatedById = _userSession.UserId
        };

        return new SuccessResult(document);

    }
}
