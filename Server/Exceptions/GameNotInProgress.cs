namespace cognify.Server.Exceptions
{
    public class GameNotInProgress : Exception
    {
        public GameNotInProgress(string message) : base(message)
        {
            LogException(message);
        }

        private void LogException(string message)
        {
            string logFilePath = "Logs.txt";
            string logMessage = $"{DateTime.Now}: {message}";

            try
            {
                using var writer = new StreamWriter(logFilePath, true);
                writer.WriteLine(logMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log exception: {ex.Message}");
            }
        }
    }
}
