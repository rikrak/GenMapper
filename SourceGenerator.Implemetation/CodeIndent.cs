namespace SourceGenerator.Implementation;

internal readonly struct CodeIndent
{
    private const int IndentSize = 4;
    private const char IndentChar = ' ';

    private readonly int _indent = 0;

    public CodeIndent(int level)
    {
        if (level < 0)
        {
            level = 0;
        }
        this._indent = level;
    }

    public static CodeIndent operator ++(CodeIndent value)
    {
        var newIndent = value._indent + 1;
        return new CodeIndent(newIndent);
    }

    public static CodeIndent operator --(CodeIndent value)
    {
        var newIndent = value._indent - 1;
        return new CodeIndent(newIndent);
    }

    public override string ToString()
    {
        var charCount = IndentSize * this._indent;

        return new string(IndentChar, charCount);
    }
}