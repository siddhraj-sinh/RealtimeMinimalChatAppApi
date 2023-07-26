using MinimalChatAppApi.Models;

namespace MinimalChatAppApi.Interfaces
{
    public interface ILogService
    {
        Task<IEnumerable<LogModel>> GetLogsAsync(DateTime? startTime = null, DateTime? endTime = null);
    }
}
