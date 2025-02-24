using System.Text.Json;
using Abyx.AI.EvaluationTests.Fixtures;
using Abyx.AI.EvaluationTests.Models;
using FluentAssertions;
using Microsoft.KernelMemory;

namespace Abyx.AI.EvaluationTests.Tests;

public class RagAskShould : IClassFixture<EvaluationFixture>
{
    private readonly IKernelMemory? _memory;

    public RagAskShould(
        EvaluationFixture fixture)
    {
        _memory = fixture.Memory;
    }

    public static IEnumerable<object[]> GetQuestionsToEvaluate()
    {
        var datasetFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/evaluation-dataset.json");
        var jsonContent = File.ReadAllText(datasetFullPath);
        var data = JsonSerializer.Deserialize<IEnumerable<EvaluationItem>>(jsonContent);
        return data!.Select(item => new object[] { item });
    }

    [Theory]
    [MemberData(nameof(GetQuestionsToEvaluate))]
    public void EvaluateQuestion(EvaluationItem question)
    {
        question.Query.Should().NotBeNull();
    }
}