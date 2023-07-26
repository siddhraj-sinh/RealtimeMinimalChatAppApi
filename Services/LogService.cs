using MinimalChatAppApi.Interfaces;
using MinimalChatAppApi.Models;

namespace MinimalChatAppApi.Services
{
    public class LogService : ILogService
    {

        private readonly IRepository<LogModel> _logRepository;

        public LogService(IRepository<LogModel> logRepository)
        {
            _logRepository = logRepository;
        }
        public async Task<IEnumerable<LogModel>> GetLogsAsync(DateTime? startTime = null, DateTime? endTime = null)
        {
            var logsQuery = await _logRepository.GetAllAsync();

            // Filter logs based on the provided start and end times, if any
            if (startTime != null)
            {
                logsQuery = logsQuery.Where(l => l.Timestamp >= startTime);
            }

            if (endTime != null)
            {
                logsQuery = logsQuery.Where(l => l.Timestamp <= endTime);
            }

            var logs = logsQuery.ToList();

            logs.ForEach(l =>
            {
                string cleanedString = l.RequestBody.Replace("\r\n", "").Replace(" ", "").Replace("\\", "");
                Console.WriteLine(cleanedString);
                l.RequestBody = cleanedString;
            });

            return logs;
        }
    }
}
