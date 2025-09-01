using System.Linq;

public class WordFinder
{
    private readonly string[] _scan;

    /// <summary>
    /// Initializes a new instance of the <see cref="WordFinder"/> class using the specified matrix of characters.
    /// </summary>
    /// <param name="matrix">A collection of strings representing the rows of the character matrix. Each string must have the same length,
    /// and the matrix must contain at least one row and one column.</param>
    public WordFinder(IEnumerable<string> matrix)
    {
        ArgumentNullException.ThrowIfNull(matrix);

        string[] rows = [.. matrix];
        if (rows.Length == 0)
        {
            throw new ArgumentException("Must have rows.");
        }

        int cols = rows[0].Length;
        if (cols == 0)
        {
            throw new ArgumentException("Rows must have at least one char.");
        }

        for (int r = 1; r < rows.Length; r++)
        {
            if (rows[r].Length != cols)
                throw new ArgumentException("All rows must be the same length.");
        }

        string[] colStrings = new string[cols];
        for (int c = 0; c < cols; c++)
        {
            char[] chars = new char[rows.Length];
            for (int r = 0; r < rows.Length; r++)
                chars[r] = rows[r][c];
            colStrings[c] = new string(chars);
        }

        _scan = rows.Concat(colStrings).ToArray();
    }

    /// <summary>
    /// Finds the top 10 most frequently occurring unique words from the specified word stream.
    /// </summary>
    /// <param name="wordstream">An enumerable collection of words to analyze. Cannot be <see langword="null"/>.</param>
    public IEnumerable<string> Find(IEnumerable<string> wordstream)
    {
        ArgumentNullException.ThrowIfNull(wordstream);

        HashSet<string> unique = new(StringComparer.Ordinal);
        foreach (string? w in wordstream.Where(w => !string.IsNullOrEmpty(w)))
        {
            unique.Add(w);
        }

        if (unique.Count == 0) yield break;

        List<(string word, int count)> counts = new(unique.Count);
        foreach (string w in unique)
        {
            int total = 0;
            foreach (string line in _scan)
                total += CountOccurrences(line, w);

            if (total > 0) counts.Add((w, total));
        }

        foreach ((string word, int _) in counts
                    .OrderByDescending(t => t.count)
                    .ThenBy(t => t.word, StringComparer.Ordinal)
                    .Take(10))
        {
            yield return word;
        }
    }

    /// <summary>
    /// Counts the number of non-overlapping occurrences of a specified substring within a given string.
    /// </summary>
    /// <param name="text">The string in which to search for the substring. Cannot be <see langword="null"/>.</param>
    /// <param name="pattern">The substring to search for. Cannot be <see langword="null"/> or empty.</param>
    private static int CountOccurrences(string text, string pattern)
    {
        if (string.IsNullOrEmpty(pattern) || pattern.Length > text.Length) return 0;

        int count = 0, i = 0;
        while (true)
        {
            int idx = text.IndexOf(pattern, i, StringComparison.Ordinal);
            if (idx < 0) break;
            count++;
            i = idx + 1;
        }

        return count;
    }
}
