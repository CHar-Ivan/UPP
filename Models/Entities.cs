using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace lab30_31.Models
{
    public class Faculty
    {
        [Key]
        public string FacultyName { get; set; }
        public string DeanFullName { get; set; }
        public string RoomNumber { get; set; }
        public string BuildingNumber { get; set; }
        public string Phone { get; set; }
        public List<Department> Departments { get; set; } = new List<Department>();
    }

    public class Department
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string HeadName { get; set; }
        public string RoomNumber { get; set; }
        public string BuildingNumber { get; set; }
        public string Phone { get; set; }
        public int TeachersCount { get; set; }
        public string FacultyName { get; set; }
        public Faculty Faculty { get; set; }
        public List<Teacher> Teachers { get; set; } = new List<Teacher>();
    }

    public class Teacher
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public int BirthYear { get; set; }
        public int StartYear { get; set; }
        public int Experience { get; set; }
        public string Position { get; set; }
        public string Gender { get; set; }
        public string City { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public List<Workload> Workloads { get; set; } = new List<Workload>();

        [NotMapped]
        public string FullName => $"{LastName} {FirstName} {MiddleName}".Trim();
    }

    public class Discipline
    {
        public int Id { get; set; }
        public string DisciplineName { get; set; }
        public int HoursCount { get; set; }
        public string Cycle { get; set; }
        public List<Workload> Workloads { get; set; } = new List<Workload>();
    }

    public class Workload
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }
        public string AcademicYear { get; set; }
        public int Semester { get; set; }
        public string Groups { get; set; }
        public int StudentCount { get; set; }
        public string ControlType { get; set; }
    }
}