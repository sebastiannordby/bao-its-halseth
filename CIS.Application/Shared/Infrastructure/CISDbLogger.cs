using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS.Application.Shared.Models;
using Microsoft.Extensions.Logging;

namespace CIS.Application.Shared.Infrastructure
{
    public class CISDbLoggerProvider : ILoggerProvider
    {
        private readonly CISDbContext _dbContext;

        public CISDbLoggerProvider(CISDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new CISDbLogger(_dbContext);
        }

        public void Dispose()
        {
            // Dispose any resources used by the logging provider
        }
    }

    public class CISDbLogger : ILogger
    {
        private readonly CISDbContext _dbContext;

        public CISDbLogger(CISDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true; // Log all levels

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (logLevel < LogLevel.Information)
                return;

            Task.Run(async () =>
            {
                var message = formatter(state, exception);
                var logEntry = new LogEntry
                {
                    Level = logLevel.ToString(),
                    Timestamp = DateTime.UtcNow,
                    Message = message
                };

                await _dbContext.LogEntries.AddAsync(logEntry);
                await _dbContext.SaveChangesAsync();
            });
        }
    }
}
