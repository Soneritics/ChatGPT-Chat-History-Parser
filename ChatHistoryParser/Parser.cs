using OpenAI.Chat;

namespace ChatHistoryParser;

public class Parser(string chatGptApiKey, Chat chat)
{
    private readonly ChatClient _chatClient = new(model: "gpt-4o", apiKey: chatGptApiKey);
    
    public async Task<IEnumerable<Event>> Parse()
    {
        var result = new List<Event>();
        var currentChunk = 0;
        
        var chunk = chat.GetNextChunk();
        while (chunk != null)
        {
            Console.WriteLine($"Parsing chunk {++currentChunk}");

            var text = string.Join(Environment.NewLine, chunk);
            result.AddRange(await ParseTrunk(text));
            
            chunk = chat.GetNextChunk();
        }
        
        return result.OrderBy(r => r.Date);
    }
    
    private async Task<List<Event>> ParseTrunk(string text)
    {
        var result = new List<Event>();
       
        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(
                "You extract events from a chat history.\n" +
                "Include events like:\n" +
                " - What events they attended\n" +
                " - What holidays they went on\n" +
                " - What trips they went on\n" +
                " - What bad things happened\n" +
                " - What good things happened\n" +
                "\n" +
                "You focus on bigger life events that happened.\n" +
                "You don't include items that they buy, only when they buy it together.\n" +
                "Don't include eventss that they are making plans for, only include the events that have actually taken place.\n" +
                "In the response you include the place where the event happened.\n" +
                "\n" +
                $"The persons in the chat are {chat.Names}.\n" +
                $"This chat is about {chat.ChatDescription}.\n" +
                $"You describe the events in the language: {chat.Language}.\n" +
                "You respond only one event per line, in the format: {date}|{event}\n" +
                "The date format is YYYY-MM-DD."),
            new UserChatMessage(text)
        };
        
        var response = await _chatClient.CompleteChatAsync(messages);
        var parseableResult = response.Value?.Content?[0].Text;

        if (parseableResult != null)
        {
            result = parseableResult
                .Split('\n')
                .Select(line => line.Split('|'))
                .Where(parts => parts.Length == 2)
                .Select(parts => new Event(parts[0], parts[1]))
                .ToList();
        }
        
        return result;
    }
}

public class Event(string date, string text)
{
    public string Date { get; } = date;
    public string Text { get; } = text;
}
