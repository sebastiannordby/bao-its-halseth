using CIS.Application.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CIS.Application.Shared.Infrastructure
{
    public class DatabaseSink : ILogEventSink, IDisposable
    {
        private readonly ConcurrentQueue<LogEvent> _queue = new ConcurrentQueue<LogEvent>();
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DatabaseSink(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Emit(LogEvent logEvent)
        {
            _queue.Enqueue(logEvent);
        }

        private void FlushQueue(object state)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CISDbContext>();

                var logEvents = new List<LogEvent>();
                while (_queue.TryDequeue(out var logEvent))
                {
                    logEvents.Add(logEvent);
                }

                if (logEvents.Any())
                {
                    var logEntries = logEvents.Select(e => new LogEntry
                    {
                        Timestamp = e.Timestamp.DateTime,
                        Level = e.Level.ToString(),
                        Message = e.RenderMessage()
                    });

                    dbContext.LogEntries.AddRange(logEntries);
                    dbContext.SaveChanges();
                }
            }
        }

        public void Dispose()
        {
        }
    }
}
