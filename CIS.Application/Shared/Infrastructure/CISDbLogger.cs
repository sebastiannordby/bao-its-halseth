using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CIS.Application.Shared.Infrastructure
{
    public class CISDbLoggerProvider : ILoggerProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public CISDbLoggerProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CISDbLogger(_serviceProvider);
        }

        public void Dispose()
        {
            // Dispose any resources used by the logging provider
        }
    }

    public class CISDbLogger : ILogger
    {
        private readonly IServiceProvider _serviceProvider;

        public CISDbLogger(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel < LogLevel.Information)
                return;

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CISDbContext>();
                var message = formatter(state, exception);
                var logEntry = new LogEntry
                {
                    Level = logLevel.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Message = message
                };

                dbContext.LogEntries.Add(logEntry);
                dbContext.SaveChanges();
            }
        }
    }
}
