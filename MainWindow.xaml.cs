using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using ClosedXML.Excel;
using Xceed.Words.NET;
using Xceed.Document.NET;
using lab30_31.Models;

namespace lab30_31
{
    public partial class MainWindow : Window
    {
        private readonly UniversityContext db = new UniversityContext();

        public ObservableCollection<Faculty> FacultiesCollection { get; set; }
        public ObservableCollection<Department> DepartmentsCollection { get; set; }
        public ObservableCollection<Teacher> TeachersCollection { get; set; }
        public ObservableCollection<Discipline> DisciplinesCollection { get; set; }
        public ObservableCollection<Workload> WorkloadsCollection { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            db.Faculties.Load();
            db.Departments.Load();
            db.Teachers.Load();
            db.Disciplines.Load();
            db.Workloads.Load();

            FacultiesCollection = db.Faculties.Local.ToObservableCollection();
            DepartmentsCollection = db.Departments.Local.ToObservableCollection();
            TeachersCollection = db.Teachers.Local.ToObservableCollection();
            DisciplinesCollection = db.Disciplines.Local.ToObservableCollection();
            WorkloadsCollection = db.Workloads.Local.ToObservableCollection();

            DataContext = this;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var facNames = db.Faculties.Select(f => f.FacultyName).ToList();
                if (db.Departments.Any(d => !facNames.Contains(d.FacultyName)))
                {
                    MessageBox.Show("Есть кафедры с несуществующим факультетом.");
                    return;
                }

                var depIds = db.Departments.Select(d => d.Id).ToList();
                if (db.Teachers.Any(t => !depIds.Contains(t.DepartmentId)))
                {
                    MessageBox.Show("Есть преподаватели с несуществующей кафедрой.");
                    return;
                }

                var teacherIds = db.Teachers.Select(t => t.Id).ToList();
                var discIds = db.Disciplines.Select(d => d.Id).ToList();
                if (db.Workloads.Any(w => !teacherIds.Contains(w.TeacherId) || !discIds.Contains(w.DisciplineId)))
                {
                    MessageBox.Show("Есть записи нагрузки с несуществующим преподавателем или дисциплиной.");
                    return;
                }

                db.SaveChanges();
                MessageBox.Show("Данные сохранены!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка SaveChanges");
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MainTabControl.SelectedIndex == 0)
                {
                    var f = FacultiesGrid.SelectedItem as Faculty;
                    if (f != null) { db.Faculties.Remove(f); FacultiesCollection.Remove(f); }
                }
                else if (MainTabControl.SelectedIndex == 1)
                {
                    var d = DepartmentsGrid.SelectedItem as Department;
                    if (d != null) { db.Departments.Remove(d); DepartmentsCollection.Remove(d); }
                }
                else if (MainTabControl.SelectedIndex == 2)
                {
                    var t = TeachersGrid.SelectedItem as Teacher;
                    if (t != null) { db.Teachers.Remove(t); TeachersCollection.Remove(t); }
                }
                else if (MainTabControl.SelectedIndex == 3)
                {
                    var disc = DisciplinesGrid.SelectedItem as Discipline;
                    if (disc != null) { db.Disciplines.Remove(disc); DisciplinesCollection.Remove(disc); }
                }
                else if (MainTabControl.SelectedIndex == 4)
                {
                    var w = WorkloadsGrid.SelectedItem as Workload;
                    if (w != null) { db.Workloads.Remove(w); WorkloadsCollection.Remove(w); }
                }

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Ошибка удаления");
            }
        }

        private void ExcelButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel (*.xlsx)|*.xlsx", FileName = "Отчет_ВУЗ" };
            if (sfd.ShowDialog() == true)
            {
                using (var wb = new XLWorkbook())
                {
                    var wsFac = wb.Worksheets.Add("Факультеты");
                    wsFac.Cell(1, 1).Value = "Название"; wsFac.Cell(1, 2).Value = "Декан";
                    int row = 2;
                    foreach (var f in db.Faculties)
                    {
                        wsFac.Cell(row, 1).Value = f.FacultyName; wsFac.Cell(row, 2).Value = f.DeanFullName;
                        row++;
                    }
                    wsFac.Columns().AdjustToContents();

                    var wsWork = wb.Worksheets.Add("Нагрузка");
                    wsWork.Cell(1, 1).Value = "Преподаватель"; wsWork.Cell(1, 2).Value = "Дисциплина";
                    wsWork.Cell(1, 3).Value = "Учебный год"; wsWork.Cell(1, 4).Value = "Семестр";
                    row = 2;
                    foreach (var w in db.Workloads.Include(w => w.Teacher).Include(w => w.Discipline))
                    {
                        wsWork.Cell(row, 1).Value = w.Teacher != null ? w.Teacher.FullName : "";
                        wsWork.Cell(row, 2).Value = w.Discipline != null ? w.Discipline.DisciplineName : "";
                        wsWork.Cell(row, 3).Value = w.AcademicYear; wsWork.Cell(row, 4).Value = w.Semester;
                        row++;
                    }
                    wsWork.Columns().AdjustToContents();
                    wb.SaveAs(sfd.FileName);
                }
                MessageBox.Show("Excel-отчет сохранён!");
            }
        }

        private void WordButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog { Filter = "Word (*.docx)|*.docx", FileName = "Отчет_ВУЗ" };
            if (sfd.ShowDialog() == true)
            {
                using (var doc = DocX.Create(sfd.FileName))
                {
                    doc.InsertParagraph("ОТЧЕТ ПО ИНФОРМАЦИОННОЙ СИСТЕМЕ ВУЗа").Bold().FontSize(16).Alignment = Alignment.center;
                    doc.InsertParagraph("\nУЧЕБНАЯ НАГРУЗКА").Bold().FontSize(14);
                    var works = db.Workloads.Include(w => w.Teacher).Include(w => w.Discipline).ToList();
                    var tWork = doc.AddTable(works.Count + 1, 4);
                    tWork.Design = TableDesign.LightShadingAccent1;
                    tWork.Rows[0].Cells[0].Paragraphs[0].Append("Преподаватель").Bold();
                    tWork.Rows[0].Cells[1].Paragraphs[0].Append("Дисциплина").Bold();
                    for (int i = 0; i < works.Count; i++)
                    {
                        tWork.Rows[i + 1].Cells[0].Paragraphs[0].Append(works[i].Teacher != null ? works[i].Teacher.FullName : "");
                        tWork.Rows[i + 1].Cells[1].Paragraphs[0].Append(works[i].Discipline != null ? works[i].Discipline.DisciplineName : "");
                    }
                    doc.InsertTable(tWork);
                    doc.Save();
                }
                MessageBox.Show("Word-отчет сохранён!");
            }
        }
    }
}