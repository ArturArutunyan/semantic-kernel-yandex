namespace SemanticKernel.Connectors.YandexGPT.Services;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Models;

/// <summary>
/// A connector for YandexGPT chat completion service.
/// </summary>
public class YandexGptChatCompletionService : IChatCompletionService
{
    private readonly string _apiKey;
    private readonly string _folderId;
    private readonly string _model;
    private readonly string _apiUrl;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="YandexGptChatCompletionService"/> class.
    /// </summary>
    /// <param name="apiKey">The API key for Yandex Cloud authentication.</param>
    /// <param name="folderId">The folder ID in Yandex Cloud.</param>
    /// <param name="model">The model name to use (default: "yandexgpt-lite").</param>
    /// <param name="apiUrl">The API endpoint URL (default: Yandex foundation models endpoint).</param>
    /// <param name="httpClient">Optional HttpClient instance.</param>
    public YandexGptChatCompletionService(
        string apiKey,
        string folderId,
        string model = "yandexgpt-lite",
        string apiUrl = "https://llm.api.cloud.yandex.net/foundationModels/v1/completion",
        HttpClient httpClient = null)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(paramName: nameof(apiKey));
        _folderId = folderId ?? throw new ArgumentNullException(paramName: nameof(folderId));
        _model = model;
        _apiUrl = apiUrl;
        _httpClient = httpClient ?? new HttpClient();
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, object> Attributes => new Dictionary<string, object>();

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings executionSettings = null,
        Kernel kernel = null,
        CancellationToken cancellationToken = default)
    {
        var settings = OpenAIPromptExecutionSettings.FromExecutionSettings(executionSettings);

        var requestData = new
        {
            modelUri = $"gpt://{_folderId}/{_model}",
            messages = PrepareMessages(chatHistory),
            completionOptions = new
            {
                temperature = settings?.Temperature ?? 0.7,
                maxTokens = settings?.MaxTokens ?? 1000,
            }
        };

        var json = JsonSerializer.Serialize(value: requestData);
        var content = new StringContent(
            content: json,
            encoding: Encoding.UTF8,
            mediaType: "application/json");

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
            scheme: "Api-Key",
            parameter: _apiKey);

        var response = await _httpClient.PostAsync(
            requestUri: _apiUrl,
            content: content,
            cancellationToken: cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<YandexGptResponse>(
            json: responseJson,
            options: _options);

        return new List<ChatMessageContent>
        {
            new ChatMessageContent(
                role: AuthorRole.Assistant,
                content: responseData?.Result?.Alternatives?[0]?.Message?.Text ?? "")
        };
    }

    /// <inheritdoc/>
    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
        ChatHistory chatHistory,
        PromptExecutionSettings executionSettings = null, 
        Kernel kernel = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    /// <summary>
    /// Prepares chat messages for the YandexGPT API request.
    /// </summary>
    /// <param name="chatHistory">The chat history to prepare.</param>
    /// <returns>A list of formatted message objects.</returns>
    private List<object> PrepareMessages(ChatHistory chatHistory)
    {
        var messages = new List<object>();

        foreach (var message in chatHistory)
        {
            messages.Add(new
            {
                role = message.Role.ToString().ToLower(),
                text = message.Content
            });
        }

        return messages;
    }
}