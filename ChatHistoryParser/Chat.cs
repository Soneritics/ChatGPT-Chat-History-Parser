public class Chat(string filename, string names, string chatDescription, string language)
{
    private string Filename { get; } = filename;
    public string Names { get; } = names;
    public string ChatDescription { get; } = chatDescription;
    public string Language { get; } = language;
    
    private readonly int _chunkSize = 250;
    private int _currentChunk;
    private string[][]? _chatChunks;
    
    public int GetChunkSize()
    {
        InitCunks();
        return _chatChunks!.Length;
    }
    
    public string[]? GetNextChunk()
    {
        InitCunks();
        
        return _chatChunks!.Length > _currentChunk
            ? _chatChunks[_currentChunk++]
            : null;
    }

    private void InitCunks()
    {
        if (_chatChunks == null)
        {
            var chatLines = File.ReadAllLines(Filename);
            _chatChunks = chatLines
                .Select((value, index) => new { value, index })
                .GroupBy(x => x.index / _chunkSize)
                .Select(g => g.Select(x => x.value).ToArray())
                .ToArray();

            _currentChunk = 0;
        }
    }
}