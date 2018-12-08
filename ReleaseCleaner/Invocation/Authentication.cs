using System;
using Octokit;
using ReleaseCleaner.Input;

namespace ReleaseCleaner.Invocation
{
    using Console = ReleaseCleaner.Input.Console;
    internal class Authentication
    {
        public Credentials Credentials { get; private set; }

        public Authentication(AuthenticationBuilder builder, IConsole console)
        {
            if (!builder.IsValid())
            {
                // BAD PROGRAMMER
                throw new InvalidOperationException();
            }
            Credentials = ExtractOrReadCredentials(builder, console);
        }

        private Credentials ExtractOrReadCredentials(AuthenticationBuilder builder, IConsole console)
        {
            // prefer token auth
            if (builder.IsTokenAuth)
            {
                return new Credentials(builder.Token);
            }
            if (builder.IsPasswordAuth)
            {
                if (builder.Password == default)
                {
                    return new Credentials(builder.Username, console.ReadPassword(builder.Username));
                }
                return new Credentials(builder.Username, builder.Password);
            }
            // no authentication provided through arguments, read it from the user
            string user = console.ReadUsername();
            return new Credentials(user, console.ReadPassword(user));
        }
    }
    internal class AuthenticationBuilder
    {
        public bool IsTokenAuth { get; internal set; }
        public string Token { get; internal set; }
        public bool IsPasswordAuth { get; internal set; }
        public string Password { get; internal set; }
        public string Username { get; internal set; }

        private readonly IConsole console;

        public AuthenticationBuilder()
        {
            this.console = new ReleaseCleaner.Input.Console();
        }

        public AuthenticationBuilder(IConsole console) 
        {
            this.console = console;
        }

        internal bool IsValid()
        {
            // Cannot be both. If neither Token nor Password, user must be prompted
            return !(this.IsTokenAuth && this.IsPasswordAuth)
                // if password auth, username must be set
                && !(this.IsPasswordAuth && this.Username == default);
        }

        public void SetToken(string token)
        {
            if (Token != default)
            {
                // disallow setting that multiple times
                throw new InvalidOperationException();
            }
            IsTokenAuth = true;
            Token = token;
        }

        public void SetUsername(string username)
        {
            if (Username != default)
            {
                // disallow setting that multiple times
                throw new InvalidOperationException();
            }
            IsPasswordAuth = true;
            Username = username;
        }

        public void SetPassword(string password)
        {
            if (Password != default)
            {
                // disallow setting that multiple times
                throw new InvalidOperationException();
            }
            IsPasswordAuth = true;
            Password = password;
        }

        public Authentication Build()
        {
            if (!IsValid())
            {
                throw new InvalidOperationException();
            }
            return new Authentication(this, console);
        }
    }
}