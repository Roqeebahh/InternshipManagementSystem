using iTextSharp.text;
using iTextSharp.text.pdf;
using InternshipManagementSystem.Models;
using InternshipManagementSystem.Services;

namespace InternshipManagementSystem.Services
{
    public class ReportService : IReportService
    {
        public async Task<byte[]> GenerateInternshipReport(Intern intern)
        {
            return await Task.Run(() =>
            {
                using var memoryStream = new MemoryStream();
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, memoryStream);

                document.Open();

                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
                var title = new Paragraph("INTERNSHIP COMPLETION REPORT", titleFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                };
                document.Add(title);

                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.BLACK);

                document.Add(new Paragraph("INTERN INFORMATION", headerFont) { SpacingAfter = 10 });

                var internTable = new PdfPTable(2) { WidthPercentage = 100, SpacingAfter = 15 };
                internTable.SetWidths(new float[] { 30, 70 });

                AddTableRow(internTable, "Full Name:", intern.FullName, headerFont, normalFont);
                AddTableRow(internTable, "Email:", intern.Email, headerFont, normalFont);
                AddTableRow(internTable, "Phone:", intern.PhoneNumber, headerFont, normalFont);
                AddTableRow(internTable, "Institution:", intern.Institution, headerFont, normalFont);
                AddTableRow(internTable, "Course of Study:", intern.CourseOfStudy, headerFont, normalFont);
                AddTableRow(internTable, "Start Date:", intern.StartDate.ToString("MMM dd, yyyy"), headerFont, normalFont);
                AddTableRow(internTable, "End Date:", intern.EndDate.ToString("MMM dd, yyyy"), headerFont, normalFont);

                document.Add(internTable);

                if (intern.Evaluations.Any())
                {
                    document.Add(new Paragraph("PERFORMANCE SUMMARY", headerFont) { SpacingAfter = 10 });

                    var avgPunctuality = intern.Evaluations.Average(e => e.Punctuality);
                    var avgTeamwork = intern.Evaluations.Average(e => e.Teamwork);
                    var avgSkillLevel = intern.Evaluations.Average(e => e.SkillLevel);
                    var overallAvg = (avgPunctuality + avgTeamwork + avgSkillLevel) / 3;

                    var perfTable = new PdfPTable(2) { WidthPercentage = 100, SpacingAfter = 15 };
                    perfTable.SetWidths(new float[] { 40, 60 });

                    AddTableRow(perfTable, "Average Punctuality:", $"{avgPunctuality:F1}/10", headerFont, normalFont);
                    AddTableRow(perfTable, "Average Teamwork:", $"{avgTeamwork:F1}/10", headerFont, normalFont);
                    AddTableRow(perfTable, "Average Skill Level:", $"{avgSkillLevel:F1}/10", headerFont, normalFont);
                    AddTableRow(perfTable, "Overall Average:", $"{overallAvg:F1}/10", headerFont, normalFont);
                    AddTableRow(perfTable, "Total Evaluations:", intern.Evaluations.Count.ToString(), headerFont, normalFont);

                    document.Add(perfTable);
                }

                document.Add(new Paragraph("ACTIVITY SUMMARY", headerFont) { SpacingAfter = 10 });

                var activityTable = new PdfPTable(2) { WidthPercentage = 100, SpacingAfter = 15 };
                activityTable.SetWidths(new float[] { 40, 60 });

                AddTableRow(activityTable, "Total Daily Logs:", intern.DailyLogs.Count.ToString(), headerFont, normalFont);
                AddTableRow(activityTable, "Total Projects Submitted:", intern.Projects.Count.ToString(), headerFont, normalFont);

                if (intern.DailyLogs.Any())
                {
                    var firstLog = intern.DailyLogs.OrderBy(d => d.LogDate).First().LogDate;
                    var lastLog = intern.DailyLogs.OrderByDescending(d => d.LogDate).First().LogDate;
                    AddTableRow(activityTable, "First Log Entry:", firstLog.ToString("MMM dd, yyyy"), headerFont, normalFont);
                    AddTableRow(activityTable, "Last Log Entry:", lastLog.ToString("MMM dd, yyyy"), headerFont, normalFont);
                }

                document.Add(activityTable);

                if (intern.Evaluations.Any())
                {
                    document.Add(new Paragraph("RECENT EVALUATIONS", headerFont) { SpacingAfter = 10 });

                    var evalTable = new PdfPTable(5) { WidthPercentage = 100, SpacingAfter = 15 };
                    evalTable.SetWidths(new float[] { 20, 15, 15, 15, 35 });

                    evalTable.AddCell(new PdfPCell(new Phrase("Date", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    evalTable.AddCell(new PdfPCell(new Phrase("Punctuality", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    evalTable.AddCell(new PdfPCell(new Phrase("Teamwork", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    evalTable.AddCell(new PdfPCell(new Phrase("Skill Level", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                    evalTable.AddCell(new PdfPCell(new Phrase("Comments", headerFont)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                    foreach (var evaluation in intern.Evaluations.OrderByDescending(e => e.EvaluationDate).Take(5))
                    {
                        evalTable.AddCell(new Phrase(evaluation.EvaluationDate.ToString("MMM dd"), normalFont));
                        evalTable.AddCell(new Phrase(evaluation.Punctuality.ToString(), normalFont));
                        evalTable.AddCell(new Phrase(evaluation.Teamwork.ToString(), normalFont));
                        evalTable.AddCell(new Phrase(evaluation.SkillLevel.ToString(), normalFont));
                        evalTable.AddCell(new Phrase(evaluation.Comments ?? "N/A", normalFont));
                    }

                    document.Add(evalTable);
                }

                document.Add(new Paragraph($"\nReport generated on {DateTime.Now:MMM dd, yyyy}",
                    FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY))
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingBefore = 20
                });

                document.Close();
                writer.Close();

                return memoryStream.ToArray();
            });
        }

        public async Task<byte[]> GenerateLogsReport(List<DailyLog> logs, DateTime? startDate, DateTime? endDate)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4, 25, 25, 30, 30);
            var writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
            var title = new Paragraph("DAILY LOGS REPORT", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            if (startDate.HasValue || endDate.HasValue)
            {
                var dateRange = "Period: ";
                if (startDate.HasValue) dateRange += startDate.Value.ToString("MMM dd, yyyy");
                if (startDate.HasValue && endDate.HasValue) dateRange += " - ";
                if (endDate.HasValue) dateRange += endDate.Value.ToString("MMM dd, yyyy");

                document.Add(new Paragraph(dateRange, FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY))
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 20
                });
            }

            var logsTable = new PdfPTable(4) { WidthPercentage = 100 };
            logsTable.SetWidths(new float[] { 20, 25, 15, 40 });

            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.WHITE);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 9, BaseColor.BLACK);

            logsTable.AddCell(new PdfPCell(new Phrase("Date", headerFont))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Padding = 8
            });
            logsTable.AddCell(new PdfPCell(new Phrase("Intern Name", headerFont))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Padding = 8
            });
            logsTable.AddCell(new PdfPCell(new Phrase("Submitted", headerFont))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Padding = 8
            });
            logsTable.AddCell(new PdfPCell(new Phrase("Activity", headerFont))
            {
                BackgroundColor = BaseColor.DARK_GRAY,
                Padding = 8
            });

            foreach (var log in logs)
            {
                logsTable.AddCell(new PdfPCell(new Phrase(log.LogDate.ToString("MMM dd, yyyy"), normalFont)) { Padding = 5 });
                logsTable.AddCell(new PdfPCell(new Phrase(log.Intern?.FullName ?? "N/A", normalFont)) { Padding = 5 });
                logsTable.AddCell(new PdfPCell(new Phrase(log.CreatedAt.ToString("MMM dd"), normalFont)) { Padding = 5 });
                logsTable.AddCell(new PdfPCell(new Phrase(log.Activity, normalFont)) { Padding = 5 });
            }

            document.Add(logsTable);

            document.Add(new Paragraph($"\nTotal Logs: {logs.Count}",
                FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK))
            {
                SpacingBefore = 20
            });

            document.Add(new Paragraph($"\nReport generated on {DateTime.Now:MMM dd, yyyy HH:mm}",
                FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY))
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingBefore = 20
            });

            document.Close();
            writer.Close();

            return memoryStream.ToArray();
        }

        public async Task<byte[]> GenerateEvaluationsReport(List<Evaluation> evaluations)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4.Rotate(), 25, 25, 30, 30); // Landscape for evaluations
            var writer = PdfWriter.GetInstance(document, memoryStream);

            document.Open();

            var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.DARK_GRAY);
            var title = new Paragraph("EVALUATIONS REPORT", titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20
            };
            document.Add(title);

            var evalTable = new PdfPTable(7) { WidthPercentage = 100 };
            evalTable.SetWidths(new float[] { 20, 20, 15, 10, 10, 10, 25 });

            var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 9, BaseColor.WHITE);
            var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK);

            var headers = new[] { "Date", "Intern Name", "Supervisor", "Punctuality", "Teamwork", "Skill Level", "Comments" };
            foreach (var header in headers)
            {
                evalTable.AddCell(new PdfPCell(new Phrase(header, headerFont))
                {
                    BackgroundColor = BaseColor.DARK_GRAY,
                    Padding = 6
                });
            }

            foreach (var evaluation in evaluations)
            {
                evalTable.AddCell(new PdfPCell(new Phrase(evaluation.EvaluationDate.ToString("MMM dd, yyyy"), normalFont)) { Padding = 4 });
                evalTable.AddCell(new PdfPCell(new Phrase(evaluation.Intern?.FullName ?? "N/A", normalFont)) { Padding = 4 });
                evalTable.AddCell(new PdfPCell(new Phrase(evaluation.Supervisor?.FullName ?? "N/A", normalFont)) { Padding = 4 });
                evalTable.AddCell(new PdfPCell(new Phrase($"{evaluation.Punctuality}/10", normalFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_CENTER });
                evalTable.AddCell(new PdfPCell(new Phrase($"{evaluation.Teamwork}/10", normalFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_CENTER });
                evalTable.AddCell(new PdfPCell(new Phrase($"{evaluation.SkillLevel}/10", normalFont)) { Padding = 4, HorizontalAlignment = Element.ALIGN_CENTER });
                evalTable.AddCell(new PdfPCell(new Phrase(evaluation.Comments ?? "No comments", normalFont)) { Padding = 4 });
            }

            document.Add(evalTable);

            if (evaluations.Any())
            {
                document.Add(new Paragraph("\nSUMMARY STATISTICS",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12, BaseColor.BLACK))
                {
                    SpacingBefore = 20,
                    SpacingAfter = 10
                });

                var avgPunctuality = evaluations.Average(e => e.Punctuality);
                var avgTeamwork = evaluations.Average(e => e.Teamwork);
                var avgSkillLevel = evaluations.Average(e => e.SkillLevel);

                document.Add(new Paragraph($"Total Evaluations: {evaluations.Count}", normalFont));
                document.Add(new Paragraph($"Average Punctuality: {avgPunctuality:F1}/10", normalFont));
                document.Add(new Paragraph($"Average Teamwork: {avgTeamwork:F1}/10", normalFont));
                document.Add(new Paragraph($"Average Skill Level: {avgSkillLevel:F1}/10", normalFont));
                document.Add(new Paragraph($"Overall Average: {(avgPunctuality + avgTeamwork + avgSkillLevel) / 3:F1}/10", normalFont));
            }

            document.Add(new Paragraph($"\nReport generated on {DateTime.Now:MMM dd, yyyy HH:mm}",
                FontFactory.GetFont(FontFactory.HELVETICA_OBLIQUE, 8, BaseColor.GRAY))
            {
                Alignment = Element.ALIGN_RIGHT,
                SpacingBefore = 20
            });

            document.Close();
            writer.Close();

            return memoryStream.ToArray();
        }

        private void AddTableRow(PdfPTable table, string label, string value, Font labelFont, Font valueFont)
        {
            table.AddCell(new PdfPCell(new Phrase(label, labelFont)) { Border = Rectangle.NO_BORDER, Padding = 5 });
            table.AddCell(new PdfPCell(new Phrase(value, valueFont)) { Border = Rectangle.NO_BORDER, Padding = 5 });
        }
    }
}