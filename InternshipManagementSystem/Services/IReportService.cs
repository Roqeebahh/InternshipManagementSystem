using InternshipManagementSystem.Models;

namespace InternshipManagementSystem.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerateInternshipReport(Intern intern);
        Task<byte[]> GenerateLogsReport(List<DailyLog> logs, DateTime? startDate, DateTime? endDate);
        Task<byte[]> GenerateEvaluationsReport(List<Evaluation> evaluations);
    }
}