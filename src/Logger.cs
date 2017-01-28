namespace Richmond
{
    public interface ILogger
    {
        void Log(string message);
    }

    public class Logger : ILogger
    {
        public void Log(string message)
        {
            System.Console.Out.WriteLine(message);
        }
    }
}