# Semantic Kernel Yandex GPT Integration
This repository provides a Yandex GPT connector for Semantic Kernel.

## Getting Started
### Prerequisites
  1. Yandex Cloud account
  2. Access to Yandex GPT API

## Setup Yandex Cloud Credentials
### Get folderId
  1. Go to Yandex Cloud Console
  2. Navigate to your cloud folder
  3. Copy the folder ID from the URL or dashboard (it looks like b1gxxxxxxxxxxxxxxxxxxx)

### Get API Key
  1. Create a service account:
    - Go to "Service accounts" section
    - Click "Create service account"
    - Fill in the name and description  
  2. Assign the *[ai.languageModels.user]* role to the service account
  4. Create an API key:
    - Select your service account
    - Go to "API keys" tab
    - Click "Create API key"
    - Copy the key value (this is your secret API key)

## Usage Example
```csharp
var apiKey = "Your secret key";
var folderId = "Your folder ID";

var kernel = Kernel.CreateBuilder()
    .AddYandexGptChatCompletion(
        apiKey: apiKey,
        folderId: folderId)
    .Build();

var ideaPlugin = kernel.CreatePluginFromFunctions(
    pluginName: "IdeaPlugin",
    functions: new[]
    {
        kernel.CreateFunctionFromPrompt(
            functionName: "GenerateIdeas",
            description: "Generates creative ideas",
            promptTemplate: "Generate {{$count}} ideas on '{{$topic}}'.\n" +
                            "Output format:\n1. Idea name - short description\n" +
                            "2. Idea name - short description")
    });

var result = await kernel.InvokeAsync(
    ideaPlugin["GenerateIdeas"],
    new KernelArguments
    {
        { "topic", "eco-friendly home technologies" },
        { "count", "3" }
    });

Console.WriteLine("Generated ideas:\n");
Console.WriteLine(result);
```

## Configuration
  - apiKey: Your Yandex Cloud service account API key
  - folderId: ID of your Yandex Cloud folder where the service account has permissions
