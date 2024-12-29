using ChatHistoryParser;

var filename = GetString("The full path and filename of the chat");

var names = GetString(
    "Who are in the chat?\n" +
    "Make sure to include the names/phone numbers so ChatGPT can identify them.\n" +
    "Names");

var chatDescription = GetString(
    "What can be found in this chat?" +
    "Who are the persons, how do they know each other?" +
    "This chat is about");

var language = GetString("Output language");

var chat = new Chat(filename, names, chatDescription, language);
Console.WriteLine($"Chat has {chat.GetChunkSize()} chunks");

var parser = new Parser(
    GetString("Chatgpt api key"),
    chat);

var result = await parser.Parse();
Console.WriteLine(
    result
        .Select(r => $"{r.Date};{r.Text}")
        .Aggregate((a, b) => $"{a}\n{b}"));

string GetString(string description)
{
    Console.Write($"{description}: ");
    return Console.ReadLine() ?? string.Empty;
}