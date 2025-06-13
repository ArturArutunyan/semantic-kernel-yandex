namespace SemanticKernel.Connectors.YandexGPT.Extensions;

using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Services;

/// <summary>
/// Extension methods for adding YandexGPT connector to KernelBuilder.
/// </summary>
public static class YandexGptKernelBuilderExtensions
{
    /// <summary>
    /// Adds YandexGPT chat completion service to the KernelBuilder.
    /// </summary>
    /// <param name="builder">The KernelBuilder instance.</param>
    /// <param name="apiKey">The API key for Yandex Cloud authentication.</param>
    /// <param name="folderId">The folder ID in Yandex Cloud.</param>
    /// <param name="model">The model name to use (default: "yandexgpt-lite").</param>
    /// <param name="apiUrl">The API endpoint URL (default: Yandex foundation models endpoint).</param>
    /// <param name="httpClient">Optional HttpClient instance.</param>
    /// <returns>The KernelBuilder instance for chaining.</returns>
    public static IKernelBuilder AddYandexGptChatCompletion(
        this IKernelBuilder builder,
        string apiKey,
        string folderId,
        string model = "yandexgpt-lite",
        string apiUrl = "https://llm.api.cloud.yandex.net/foundationModels/v1/completion",
        HttpClient httpClient = null)
    {
        builder.Services.AddKeyedSingleton<IChatCompletionService>(
            serviceKey: "yandex-gpt",
            implementationFactory: (_, _) => new YandexGptChatCompletionService(
                apiKey: apiKey,
                folderId: folderId,
                model: model,
                apiUrl: apiUrl,
                httpClient: httpClient));

        return builder;
    }
}