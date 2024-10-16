using System;
using System.IO;
using System.Threading.Tasks;

public class TextLoaderService
{
    private readonly string filePath = "SampleTexts.txt";

    public async Task<string> LoadRandomTextFromFileAsync()
    {
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (StreamReader reader = new StreamReader(fs))
            {
                var lines = await reader.ReadToEndAsync();
                var lineArray = lines.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                var random = new Random();

                if (lineArray.Length > 0)
                {
                    int index = random.Next(0, lineArray.Length);
                    return lineArray[index];
                }
            }
            return null;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return null;
        }
    }
}

