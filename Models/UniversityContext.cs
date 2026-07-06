using Microsoft.EntityFrameworkCore;

namespace lab30_31.Models
{
    public class UniversityContext : DbContext
    {
        public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Workload> Workloads { get; set; }

        public UniversityContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=DESKTOP-DVM8L4A;
                  Database=UniversityDb;
                  Trusted_Connection=True;
                  TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Указываем первичный текстовый ключ для Факультета
            modelBuilder.Entity<Faculty>().HasKey(f => f.FacultyName);

            // 2. Настраиваем связь «Один-ко-многим» между Faculty и Department явно.
            // Это решает проблему со скрытой колонкой FacultyName1
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Faculty)
                .WithMany(f => f.Departments)
                .HasForeignKey(d => d.FacultyName);

            // ===== ФАКУЛЬТЕТЫ (Данные без числового Id. Ключ — FacultyName) =====
            modelBuilder.Entity<Faculty>().HasData(
                new Faculty { FacultyName = "Факультет информатики", DeanFullName = "Иванов И.И.", RoomNumber = "101", BuildingNumber = "1", Phone = "+7 900 111-11-11" },
                new Faculty { FacultyName = "Факультет физики", DeanFullName = "Petrov P.P.", RoomNumber = "202", BuildingNumber = "2", Phone = "+7 900 222-22-22" },
                new Faculty { FacultyName = "Факультет экономики", DeanFullName = "Сергеев С.С.", RoomNumber = "303", BuildingNumber = "3", Phone = "+7 900 333-33-33" },
                new Faculty { FacultyName = "Факультет математики", DeanFullName = "Кузнецова А.А.", RoomNumber = "404", BuildingNumber = "4", Phone = "+7 900 444-44-44" },
                new Faculty { FacultyName = "Факультет лингвистики", DeanFullName = "Фролов Д.Д.", RoomNumber = "505", BuildingNumber = "5", Phone = "+7 900 555-55-55" }
            );

            // ===== КАФЕДРЫ =====
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, DepartmentName = "Кафедра программирования", HeadName = "Сидоров С.С.", RoomNumber = "110", BuildingNumber = "1", Phone = "+7 900 111-22-33", TeachersCount = 4, FacultyName = "Факультет информатики" },
                new Department { Id = 2, DepartmentName = "Кафедра физики", HeadName = "Кузнецов К.К.", RoomNumber = "210", BuildingNumber = "2", Phone = "+7 900 222-33-44", TeachersCount = 4, FacultyName = "Факультет физики" },
                new Department { Id = 3, DepartmentName = "Кафедра экономики", HeadName = "Фомин О.О.", RoomNumber = "310", BuildingNumber = "3", Phone = "+7 900 333-44-55", TeachersCount = 4, FacultyName = "Факультет экономики" },
                new Department { Id = 4, DepartmentName = "Кафедра макроэкономики", HeadName = "Громов Г.Г.", RoomNumber = "410", BuildingNumber = "4", Phone = "+7 900 444-55-66", TeachersCount = 4, FacultyName = "Факультет математики" },
                new Department { Id = 5, DepartmentName = "Кафедра английского языка", HeadName = "Лебедева Л.Л.", RoomNumber = "510", BuildingNumber = "5", Phone = "+7 900 555-66-77", TeachersCount = 4, FacultyName = "Факультет лингвистики" }
            );

            // ===== ПРЕПОДАВАТЕЛИ =====
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher { Id = 1, LastName = "Смирнов", FirstName = "Алексей", MiddleName = "Игоревич", BirthYear = 1980, StartYear = 2005, Experience = 19, Position = "Доцент", Gender = "М", City = "Москва", DepartmentId = 1 },
                new Teacher { Id = 2, LastName = "Петров", FirstName = "Иван", MiddleName = "Ильич", BirthYear = 1985, StartYear = 2008, Experience = 16, Position = "Старший преподаватель", Gender = "М", City = "Москва", DepartmentId = 2 },
                new Teacher { Id = 3, LastName = "Мина", FirstName = "Нина", MiddleName = "Сергеевна", BirthYear = 1978, StartYear = 2000, Experience = 24, Position = "Профессор", Gender = "Ж", City = "СПб", DepartmentId = 3 },
                new Teacher { Id = 4, LastName = "Андреев", FirstName = "Андрей", MiddleName = "Андреевич", BirthYear = 1985, StartYear = 2015, Experience = 9, Position = "Ассистент", Gender = "М", City = "Казань", DepartmentId = 4 },
                new Teacher { Id = 5, LastName = "Лебедев", FirstName = "Дмитрий", MiddleName = "Олегович", BirthYear = 1982, StartYear = 2008, Experience = 16, Position = "Доцент", Gender = "М", City = "Новосибирск", DepartmentId = 5 }
            );

            // ===== ДИСЦИПЛИНЫ =====
            modelBuilder.Entity<Discipline>().HasData(
                new Discipline { Id = 1, DisciplineName = "Программирование", HoursCount = 120, Cycle = "Профессиональный" },
                new Discipline { Id = 2, DisciplineName = "Физика", HoursCount = 90, Cycle = "Базовый" },
                new Discipline { Id = 3, DisciplineName = "Экономика", HoursCount = 100, Cycle = "Профессиональный" },
                new Discipline { Id = 4, DisciplineName = "Макроэкономика", HoursCount = 110, Cycle = "Профессиональный" },
                new Discipline { Id = 5, DisciplineName = "Английский язык", HoursCount = 80, Cycle = "Базовый" }
            );

            // ===== УЧЕБНАЯ НАГРУЗКА =====
            modelBuilder.Entity<Workload>().HasData(
                new Workload { Id = 1, TeacherId = 1, DisciplineId = 1, AcademicYear = "2024/2025", Semester = 1, Groups = "П-31", StudentCount = 25, ControlType = "Экзамен" },
                new Workload { Id = 2, TeacherId = 2, DisciplineId = 2, AcademicYear = "2024/2025", Semester = 2, Groups = "Ф-21", StudentCount = 18, ControlType = "Зачет" },
                new Workload { Id = 3, TeacherId = 3, DisciplineId = 3, AcademicYear = "2024/2025", Semester = 1, Groups = "Э-11", StudentCount = 20, ControlType = "Экзамен" },
                new Workload { Id = 4, TeacherId = 4, DisciplineId = 4, AcademicYear = "2024/2025", Semester = 2, Groups = "М-41", StudentCount = 30, ControlType = "Зачет" },
                new Workload { Id = 5, TeacherId = 5, DisciplineId = 5, AcademicYear = "2024/2025", Semester = 1, Groups = "Л-31", StudentCount = 22, ControlType = "Экзамен" }
            );
        }
    }
}