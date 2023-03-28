static class Helper
{
    static string[] _words = new[] { "lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
            "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
            "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat" };

    public static IEnumerable<BsonDocument> GetDocs(int count)
    {
        for (var i = 1; i <= count; i++)
        {
            yield return new BsonDocument
            {
                { "_id", i },
                { "name", Guid.NewGuid().ToString() },
                { "lorem", LoremIpsum(3, 5, 2, 3, 3) }
            };
        }
    }

    static string LoremIpsum(int minWords, int maxWords,
        int minSentences, int maxSentences,
        int numParagraphs)
    {
        var rand = new Random(322);
        var numSentences = rand.Next(maxSentences - minSentences) + minSentences + 1;
        var numWords = rand.Next(maxWords - minWords) + minWords + 1;

        var result = new StringBuilder();

        for (int p = 0; p < numParagraphs; p++)
        {
            for (int s = 0; s < numSentences; s++)
            {
                for (int w = 0; w < numWords; w++)
                {
                    if (w > 0) { result.Append(' '); }
                    result.Append(_words[rand.Next(_words.Length)]);
                }
                result.Append(". ");
            }
            result.AppendLine();
        }

        return result.ToString();
    }
}
