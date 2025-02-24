using System.Text.Json;
using Abyx.AI.EvaluationTests.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.KernelMemory;

namespace Abyx.AI.EvaluationTests.Fixtures;

public class EvaluationFixture : IAsyncLifetime
{
    public IKernelMemory Memory { get; internal set; } = null!;

    public async Task InitializeAsync()
    {
        InitialiseKernelMemory();

        await ImportRagDocuments();
    }

    private async Task ImportRagDocuments()
    {
        await Memory.ImportDocumentAsync(
            filePath: "Data/story.MD",
            documentId: "black-mirror-story-01");
    }

    private void InitialiseKernelMemory()
    {
        var azureOpenAiTextConfig = new AzureOpenAIConfig();
        var azureOpenAiEmbeddingConfig = new AzureOpenAIConfig();
        var azureAiSearchConfig = new AzureAISearchConfig();
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("testsettings.json", optional: false, reloadOnChange: false)
            .Build();    
        
        configuration
            .BindSection("KernelMemory:Services:AzureOpenAIText", azureOpenAiTextConfig)
            .BindSection("KernelMemory:Services:AzureOpenAIEmbedding", azureOpenAiEmbeddingConfig)
            .BindSection("KernelMemory:Services:AzureAISearch", azureAiSearchConfig);
        
        Memory = new KernelMemoryBuilder()
            .With(new KernelMemoryConfig { DefaultIndexName = "rag-evaluation-demo" })
            .WithAzureOpenAITextEmbeddingGeneration(azureOpenAiEmbeddingConfig)
            .WithAzureOpenAITextGeneration(azureOpenAiTextConfig)
            .WithAzureAISearchMemoryDb(azureAiSearchConfig)
            .Build<MemoryServerless>(new KernelMemoryBuilderBuildOptions {
                AllowMixingVolatileAndPersistentData = true
            });
    }

    public async Task DisposeAsync()
    {
        await Memory.DeleteIndexAsync();
    }
}