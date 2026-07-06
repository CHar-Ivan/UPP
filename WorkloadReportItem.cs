// Файл: MainWindow.xaml.cs
namespace lab30_31
{
    internal class WorkloadReportItem
    {
        public int HoursCount { get; internal set; }
        public string TeacherFullName { get; internal set; }
        public string Position { get; internal set; }
        public string DepartmentName { get; internal set; }
        public string DisciplineName { get; internal set; }
        public string Cycle { get; internal set; }
        public string Groups { get; internal set; }
        public int StudentCount { get; internal set; }
        public string ControlType { get; internal set; }
    }
}