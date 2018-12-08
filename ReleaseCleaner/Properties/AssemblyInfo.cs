using System.Runtime.CompilerServices;

// expose internals to test project
[assembly: InternalsVisibleTo("ReleaseCleanerTests")]
// expose internals to Moq for Mocking them
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]