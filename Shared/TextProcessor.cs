public class TextProcessor<T> where T : class, new()
{
    public T ProcessText(Func<string, T> converter, string inputText)
    {
        if (string.IsNullOrWhiteSpace(inputText))
            throw new ArgumentException("Input text cannot be null or empty.");

        return converter(inputText);
    }
}
