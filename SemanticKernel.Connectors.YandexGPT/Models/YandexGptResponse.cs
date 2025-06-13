namespace SemanticKernel.Connectors.YandexGPT.Models;

using System.Collections.Generic;

public class YandexGptResponse
{
    public YandexGptResult Result { get; set; }
}

public class YandexGptResult
{
    public List<YandexGptAlternative> Alternatives { get; set; }
}

public class YandexGptAlternative
{
    public YandexGptMessage Message { get; set; }
}

public class YandexGptMessage
{
    public string Text { get; set; }
}