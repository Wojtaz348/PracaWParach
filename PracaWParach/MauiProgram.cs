
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace PracaWParach
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Project> Projects { get; set; }

        public MainPage()
        {
            
            Projects = new ObservableCollection<Project>
            {
                new Project { Name = "Project A", HoursWorked = 10, Earnings = 500 },
                new Project { Name = "Project B", HoursWorked = 15, Earnings = 750 }
            };
            BindingContext = this;
        }


        private async void StartPomodoroTimer(object sender, EventArgs e)
        {
            int workTime = 25; 
            int breakTime = 5; 

            await Task.Delay(workTime * 60 * 1000); 
            await DisplayAlert("Pomodoro Timer", "Time for a break!", "OK");

            await Task.Delay(breakTime * 60 * 1000); 
            await DisplayAlert("Pomodoro Timer", "Back to work!", "OK");
        }

        private async void ExportReportToPdf(object sender, EventArgs e)
        {
            var reportPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Report.pdf");

            var document = new TimeTrackingReport(Projects);

            using var fileStream = new FileStream(reportPath, FileMode.Create);
            document.GeneratePdf(fileStream);

            await DisplayAlert("Report Generated", $"Report saved to {reportPath}", "OK");
        }

        private void AddProject(string name)
        {
            Projects.Add(new Project { Name = name });
        }
    }

    public class Project
    {
        public string Name { get; set; }
        public double HoursWorked { get; set; }
        public double Earnings { get; set; }
    }

    public class TimeTrackingReport : IDocument
    {
        private readonly ObservableCollection<Project> _projects;

        public TimeTrackingReport(ObservableCollection<Project> projects)
        {
            _projects = projects;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Header().Text("Time Tracking Report").FontSize(20).Bold();
                page.Content().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(100);
                        columns.RelativeColumn();
                        columns.ConstantColumn(100);
                    });

                    table.Header(header =>
                    {
                        header.Cell().Text("Project").Bold();
                        header.Cell().Text("Time (hrs)").Bold();
                        header.Cell().Text("Earnings").Bold();
                    });

                    foreach (var project in _projects)
                    {
                        table.Cell().Text(project.Name);
                        table.Cell().Text(project.HoursWorked.ToString());
                        table.Cell().Text($"${project.Earnings:F2}");
                    }
                });

                page.Footer().Text($"Generated on {DateTime.Now}").FontSize(10).AlignRight();
            });
        }
    }
}
