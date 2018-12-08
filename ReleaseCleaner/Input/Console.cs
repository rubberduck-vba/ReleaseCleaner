namespace ReleaseCleaner.Input
{
    using System;
    using System.Text;
    using IO = System.Console;
    internal class Console : IConsole
    {
        public string ReadUsername()
        {
            IO.WriteLine("Please enter your Github username");
            // simple ReadLine, because we don't need to mask
            return IO.ReadLine();
        }

        // SecureString is recommended to not be used, see platform-compat/DE0001
        public string ReadPassword(string username)
        {
            IO.WriteLine($"Please enter the Github password for {username}");
            var password = new StringBuilder();
            ConsoleKeyInfo keyInfo;
            do
            {
                // true intercepts the keypress, effectively masking the password
                keyInfo = IO.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control))
                    {
                        password.Clear();
                    }
                    else
                    {
                        password.Remove(password.Length - 1, 1);
                    }
                }
                // for all other cases
                password.Append(keyInfo.KeyChar);
            } while (keyInfo.Key != ConsoleKey.Enter);
            IO.WriteLine();
            return password.ToString();
        }
    }

}