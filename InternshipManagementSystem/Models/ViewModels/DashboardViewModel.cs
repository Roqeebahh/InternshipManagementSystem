namespace InternshipManagementSystem.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalLogs { get; set; }
        public int TotalEvaluations { get; set; }
        public int TotalProjects { get; set; }
        public double AverageScore { get; set; }
        public List<DailyLog> RecentLogs { get; set; } = new List<DailyLog>();
        public List<Evaluation> RecentEvaluations { get; set; } = new List<Evaluation>();
    }

    public class AdminDashboardViewModel
    {
        public int TotalInterns { get; set; }
        public int ActiveInterns { get; set; }
        public int TotalLogs { get; set; }
        public int TotalEvaluations { get; set; }
        public List<Intern> RecentInterns { get; set; } = new List<Intern>();
        public List<DailyLog> RecentLogs { get; set; } = new List<DailyLog>();
    }
}

