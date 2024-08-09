/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================
using System.Net.Http.Json;

namespace Blazr.OneWayStreet.Infrastructure;

public sealed class CommandAPIHandler
    : ICommandHandler
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    public CommandAPIHandler(IServiceProvider serviceProvider, IHttpClientFactory httpClientFactory)
    {
        _serviceProvider = serviceProvider;
        _httpClientFactory = httpClientFactory;
    }

    /// <summary>
    /// Uses a specific handler if one is configured in DI
    /// If not, uses a generic handler using the APIInfo attributes to configure the HttpClient request  
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    /// <param name="request"></param>
    /// <returns></returns>
    public async ValueTask<CommandResult> ExecuteAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        ICommandHandler<TRecord>? _customHandler = null;

        _customHandler = _serviceProvider.GetService<ICommandHandler<TRecord>>();

        // Get the custom handler
        if (_customHandler is not null)
            return await _customHandler.ExecuteAsync(request);

        return await CommandAsync<TRecord>(request);
    }

    public async ValueTask<CommandResult> CommandAsync<TRecord>(CommandRequest<TRecord> request)
        where TRecord : class
    {
        var attribute = Attribute.GetCustomAttribute(typeof(TRecord), typeof(APIInfo));

        if (attribute is null || !(attribute is APIInfo apiInfo))
            throw new DataPipelineException($"No API attribute defined for Record {typeof(TRecord).Name}");

        using var http = _httpClientFactory.CreateClient(apiInfo.ClientName);

        var apiRequest = CommandAPIRequest<TRecord>.FromRequest(request);

        var httpResult = await http.PostAsJsonAsync<CommandAPIRequest<TRecord>>($"/API/{apiInfo.PathName}/Command", apiRequest, request.Cancellation)
            .ConfigureAwait(ConfigureAwaitOptions.None); ;

        if (!httpResult.IsSuccessStatusCode)
            return CommandResult.Failure($"The server returned a status code of : {httpResult.StatusCode}");

        var commandAPIResult = await httpResult.Content.ReadFromJsonAsync<CommandAPIResult<Guid>>()
            .ConfigureAwait(ConfigureAwaitOptions.None); 

        CommandResult? commandResult = null;

        if (commandAPIResult is not null)
            commandResult = commandAPIResult.ToCommandResult();

        return commandResult ?? CommandResult.Failure($"No data was returned");
    }
}
