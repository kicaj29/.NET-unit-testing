using Microsoft.Extensions.Logging;

namespace VerifyCallsOrder.Tests
{
    /// <summary>
    /// https://pnguyen.io/posts/verify-ilogger-call-in-dotnet-core/
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class InMemoryFakeLogger<T> : ILogger<T>
    {
        public LogLevel Level { get; private set; }
        public Exception Ex { get; private set; }
        public List<string?> Messages { get; private set; } = new List<string?>();

        public IDisposable BeginScope<TState>(TState state)
        {
            // return NullScope.Instance;
            throw new NotImplementedException();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            //return true;
            throw new NotImplementedException();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Level = logLevel;
            if (state != null)
            {
                Messages.Add(state.ToString());
            }
            Ex = exception;
        }

        /// <summary>
        /// Reference: https://github.com/aspnet/Logging/blob/master/src/Microsoft.Extensions.Logging.Abstractions/Internal/NullScope.cs
        /// </summary>
        public class NullScope : IDisposable
        {
            public static NullScope Instance { get; } = new NullScope();

            private NullScope()
            {
            }

            public void Dispose()
            {
            }
        }
    }
}
