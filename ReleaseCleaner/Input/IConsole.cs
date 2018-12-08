namespace ReleaseCleaner.Input
{
    internal interface IConsole
    {
        string ReadUsername();
        string ReadPassword(string username);
    }
}