using System.ClientModel;
using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.AI.Evaluation;
using Microsoft.Extensions.AI.Evaluation.Quality;

IList<ChatMessage> chatMessages = [
        new ChatMessage(
            ChatRole.System,
            """
            You are an AI assistant that can answer questions related to astronomy.
            Keep your responses concise staying under 100 words as much as possible.
            Use the imperial measurement system for all measurements in your response.
            """),
        new ChatMessage(
            ChatRole.User,
            "How far is the planet Venus from the Earth at its closest and furthest points?")];

IChatClient client =
            new AzureOpenAIClient(
                new Uri("https://oai-rocketmind-master-ai.openai.azure.com/"),
                new ApiKeyCredential("9196a00999d04092a63e16e9380ffdaa"))
                .AsChatClient(modelId: "gpt-4o-mini");

var chatConfiguration = new ChatConfiguration(client);

var chatOptions =
            new ChatOptions
            {
                Temperature = 0.0f,
                ResponseFormat = ChatResponseFormat.Text
            };

var response = await chatConfiguration.ChatClient.GetResponseAsync(chatMessages, chatOptions);
var modelResponse = response.Message;

var equivalenceEvaluator = new EquivalenceEvaluator();
var baselineResponseForEquivalenceEvaluator =
            new EquivalenceEvaluatorContext(
                """
                The distance between Earth and Venus varies significantly due to the elliptical orbits of both planets
                around the Sun. At their closest approach, known as inferior conjunction, Venus can be about 23.6
                million miles away from Earth. At their furthest point, when Venus is on the opposite side of the Sun
                from Earth, known as superior conjunction, the distance can be about 162 million miles. These distances
                can vary slightly due to the specific orbital positions of the planets at any given time.
                """);

var result = await equivalenceEvaluator.EvaluateAsync(
    modelResponse,
    chatConfiguration,
    [baselineResponseForEquivalenceEvaluator]);

var equivalence = result.Get<NumericMetric>(EquivalenceEvaluator.EquivalenceMetricName);

Console.WriteLine($"Equivalence: {equivalence.Value}");

