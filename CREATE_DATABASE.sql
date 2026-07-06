-- СОЗДАНИЕ БД СИСТЕМЫ УПРАВЛЕНИЯ ЖЭК - ВЕРСИЯ 5.0 (ЧИСТАЯ)

USE master;
GO

-- Удаляем старую БД если существует
IF EXISTS (SELECT 1 FROM sys.databases WHERE name = 'JEK_DB')
BEGIN
    ALTER DATABASE JEK_DB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE JEK_DB;
END
GO

-- Создаем новую БД с UTF-8 кодировкой
CREATE DATABASE JEK_DB COLLATE Latin1_General_100_CI_AS_SC_UTF8;
GO

USE JEK_DB;
GO

-- Устанавливаем UTF-8 кодировку по умолчанию для БД
ALTER DATABASE JEK_DB COLLATE Latin1_General_100_CI_AS_SC_UTF8;
GO

-- ============================================================
-- Таблица HOUSES (Дома)
-- ============================================================
CREATE TABLE Houses (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Address NVARCHAR(500) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    BuildingNumber NVARCHAR(50) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    FloorsCount INT NOT NULL,
    ApartmentsCount INT NOT NULL,
    BuildYear INT NOT NULL,
    Material NVARCHAR(100) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- ============================================================
-- Таблица APARTMENTS (Квартиры)
-- ============================================================
CREATE TABLE Apartments (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HouseId INT NOT NULL,
    ApartmentNumber INT NOT NULL,
    Floor INT NOT NULL,
    Area DECIMAL(10, 2) NOT NULL,
    RoomsCount INT NOT NULL,
    OwnerName NVARCHAR(200) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    OwnerPhone NVARCHAR(20) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    OwnerEmail NVARCHAR(100) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    MoveInDate DATETIME NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Apartments_Houses FOREIGN KEY (HouseId) REFERENCES Houses(Id) ON DELETE CASCADE
);

-- ============================================================
-- Таблица MAINTENANCE_TYPES (Виды обслуживания/услуги)
-- ============================================================
CREATE TABLE MaintenanceTypes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(200) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    Description NVARCHAR(500) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    Unit NVARCHAR(50) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    TariffPerUnit DECIMAL(15, 2) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE()
);

-- ============================================================
-- Таблица UTILITIES (Показания коммунальных услуг)
-- ============================================================
CREATE TABLE Utilities (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ApartmentId INT NOT NULL,
    MaintenanceTypeId INT NOT NULL,
    PreviousReading DECIMAL(10, 2) NOT NULL,
    CurrentReading DECIMAL(10, 2) NOT NULL,
    Consumption DECIMAL(10, 2) NOT NULL,
    Amount DECIMAL(15, 2) NOT NULL,
    ReadingDate DATETIME NOT NULL,
    PaymentDueDate DATETIME NOT NULL,
    IsPaid BIT NOT NULL DEFAULT 0,
    PaidDate DATETIME NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Utilities_Apartments FOREIGN KEY (ApartmentId) REFERENCES Apartments(Id) ON DELETE CASCADE,
    CONSTRAINT FK_Utilities_MaintenanceTypes FOREIGN KEY (MaintenanceTypeId) REFERENCES MaintenanceTypes(Id) ON DELETE CASCADE
);

-- ============================================================
-- Таблица MAINTENANCE_REQUESTS (Заявки на обслуживание)
-- ============================================================
CREATE TABLE MaintenanceRequests (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ApartmentId INT NOT NULL,
    MaintenanceTypeId INT NOT NULL,
    Description NVARCHAR(1000) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    Status NVARCHAR(50) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL DEFAULT N'Новая',
    Priority NVARCHAR(50) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL DEFAULT N'Обычная',
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ScheduledDate DATETIME NULL,
    CompletedDate DATETIME NULL,
    AssignedTo NVARCHAR(200) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    Result NVARCHAR(1000) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    Cost DECIMAL(15, 2) NOT NULL DEFAULT 0,
    UpdatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_MaintenanceRequests_Apartments FOREIGN KEY (ApartmentId) REFERENCES Apartments(Id) ON DELETE CASCADE,
    CONSTRAINT FK_MaintenanceRequests_MaintenanceTypes FOREIGN KEY (MaintenanceTypeId) REFERENCES MaintenanceTypes(Id) ON DELETE CASCADE
);

-- ============================================================
-- Таблица MAINTENANCE_TASKS (Работы по обслуживанию дома)
-- ============================================================
CREATE TABLE MaintenanceTasks (
    Id INT PRIMARY KEY IDENTITY(1,1),
    HouseId INT NOT NULL,
    MaintenanceTypeId INT NOT NULL,
    Title NVARCHAR(300) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    Description NVARCHAR(1000) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    Status NVARCHAR(50) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL DEFAULT N'Планируется',
    PlannedDate DATETIME NOT NULL,
    StartDate DATETIME NULL,
    EndDate DATETIME NULL,
    Budget DECIMAL(15, 2) NOT NULL,
    ActualCost DECIMAL(15, 2) NOT NULL DEFAULT 0,
    Contractor NVARCHAR(300) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    ResponsiblePerson NVARCHAR(200) COLLATE Latin1_General_100_CI_AS_SC_UTF8 NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_MaintenanceTasks_Houses FOREIGN KEY (HouseId) REFERENCES Houses(Id) ON DELETE CASCADE,
    CONSTRAINT FK_MaintenanceTasks_MaintenanceTypes FOREIGN KEY (MaintenanceTypeId) REFERENCES MaintenanceTypes(Id) ON DELETE CASCADE
);

-- ============================================================
-- ВСТАВКА ДАННЫХ
-- ============================================================

-- Дома
INSERT INTO Houses (Address, BuildingNumber, FloorsCount, ApartmentsCount, BuildYear, Material) VALUES
(N'ул. Ленина, 45', N'45', 5, 20, 1985, N'Кирпич'),
(N'пр. Советский, 120', N'120', 9, 45, 1995, N'Панель'),
(N'ул. Пушкина, 78', N'78', 3, 12, 2010, N'Кирпич'),
(N'ул. Садовая, 33', N'33', 4, 16, 2000, N'Кирпич'),
(N'пр. Октябрьский, 56', N'56', 7, 28, 1990, N'Панель');

-- Квартиры
INSERT INTO Apartments (HouseId, ApartmentNumber, Floor, Area, RoomsCount, OwnerName, OwnerPhone, OwnerEmail, MoveInDate) VALUES
(1, 1, 1, 45, 2, N'Иван Петров', N'+7-950-1000001', N'ivan@mail.ru', DATEADD(YEAR, -10, GETDATE())),
(1, 2, 1, 55, 3, N'Мария Сидорова', N'+7-950-1000002', N'maria@mail.ru', DATEADD(YEAR, -8, GETDATE())),
(1, 3, 2, 50, 2, N'Петр Иванов', N'+7-950-1000003', N'petr@mail.ru', DATEADD(YEAR, -7, GETDATE())),
(1, 4, 2, 60, 3, N'Анна Смирнова', N'+7-950-1000004', N'anna@mail.ru', DATEADD(YEAR, -6, GETDATE())),
(1, 5, 3, 45, 2, N'Сергей Морозов', N'+7-950-1000005', N'sergey@mail.ru', DATEADD(YEAR, -5, GETDATE())),
(2, 10, 1, 65, 2, N'Ольга Волкова', N'+7-950-2000001', N'olga@mail.ru', DATEADD(YEAR, -9, GETDATE())),
(2, 20, 2, 75, 3, N'Дмитрий Соколов', N'+7-950-2000002', N'dmitry@mail.ru', DATEADD(YEAR, -7, GETDATE())),
(2, 30, 3, 70, 2, N'Елена Кузнецова', N'+7-950-2000003', N'elena@mail.ru', DATEADD(YEAR, -6, GETDATE())),
(2, 40, 4, 80, 3, N'Александр Коваленко', N'+7-950-2000004', N'alex@mail.ru', DATEADD(YEAR, -5, GETDATE())),
(3, 1, 1, 50, 2, N'Виктор Новиков', N'+7-950-3000001', N'victor@mail.ru', DATEADD(YEAR, -13, GETDATE())),
(3, 2, 1, 55, 2, N'Ирина Орлова', N'+7-950-3000002', N'irina@mail.ru', DATEADD(YEAR, -12, GETDATE())),
(3, 3, 2, 60, 3, N'Филипп Сафин', N'+7-950-3000003', N'philip@mail.ru', DATEADD(YEAR, -11, GETDATE()));

-- Виды обслуживания
INSERT INTO MaintenanceTypes (Name, Description, Unit, TariffPerUnit) VALUES
(N'Водоснабжение', N'Холодное водоснабжение', N'м³', 45.50),
(N'Электричество', N'Электроэнергия', N'кВт', 6.83),
(N'Газ', N'Природный газ', N'м³', 8.50),
(N'Отопление', N'Отопление помещений', N'Гкал', 2200.00),
(N'Водоотведение', N'Канализация и водоотведение', N'м³', 30.00),
(N'Уборка', N'Уборка лестничных пролетов', N'шт', 250.00),
(N'Лифт', N'Техническое обслуживание лифта', N'шт', 400.00),
(N'ТО', N'Общее техническое обслуживание', N'шт', 150.00);

-- Показания коммунальных услуг
INSERT INTO Utilities (ApartmentId, MaintenanceTypeId, PreviousReading, CurrentReading, Consumption, Amount, ReadingDate, PaymentDueDate, IsPaid, PaidDate) VALUES
(1, 1, 150, 165, 15, 682.50, GETDATE(), DATEADD(DAY, 10, GETDATE()), 0, NULL),
(1, 2, 2000, 2150, 150, 1024.50, GETDATE(), DATEADD(DAY, 10, GETDATE()), 0, NULL),
(1, 4, 100, 110, 10, 22000, DATEADD(MONTH, -1, GETDATE()), DATEADD(DAY, -5, GETDATE()), 1, GETDATE()),
(2, 1, 200, 218, 18, 819.00, GETDATE(), DATEADD(DAY, 10, GETDATE()), 1, GETDATE()),
(2, 2, 1500, 1680, 180, 1229.40, GETDATE(), DATEADD(DAY, 10, GETDATE()), 1, GETDATE()),
(3, 1, 120, 135, 15, 682.50, GETDATE(), DATEADD(DAY, 10, GETDATE()), 0, NULL),
(4, 2, 2200, 2400, 200, 1366.00, GETDATE(), DATEADD(DAY, 10, GETDATE()), 0, NULL),
(5, 5, 100, 100, 0, 600, DATEADD(MONTH, -1, GETDATE()), DATEADD(DAY, -5, GETDATE()), 1, GETDATE());

-- Заявки на обслуживание
INSERT INTO MaintenanceRequests (ApartmentId, MaintenanceTypeId, Description, Status, Priority, CreatedDate, ScheduledDate, CompletedDate, AssignedTo, Result, Cost) VALUES
(1, 1, N'Протечка под раковиной на кухне', N'В работе', N'Высокая', DATEADD(DAY, -5, GETDATE()), DATEADD(DAY, 0, GETDATE()), DATEADD(DAY, 5, GETDATE()), N'Иван Сидоров', N'Устранено', 1500),
(2, 2, N'Замена электрической розетки в гостиной', N'Новая', N'Обычная', DATEADD(DAY, -1, GETDATE()), DATEADD(DAY, 1, GETDATE()), DATEADD(DAY, 2, GETDATE()), N'Мастер', N'', 0),
(3, 1, N'Засор в водопроводе ванной', N'Выполнена', N'Срочная', DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -30, GETDATE()), DATEADD(DAY, -28, GETDATE()), N'Петр Морозов', N'Засор устранен', 800),
(4, 5, N'Проверка счетчика газа', N'Новая', N'Обычная', DATEADD(DAY, -2, GETDATE()), DATEADD(DAY, 1, GETDATE()), DATEADD(DAY, 2, GETDATE()), N'Техник', N'', 0),
(5, 2, N'Замена люстры в коридоре', N'В работе', N'Низкая', DATEADD(DAY, -7, GETDATE()), DATEADD(DAY, 0, GETDATE()), DATEADD(DAY, 3, GETDATE()), N'Сергей Волков', N'', 2000),
(6, 1, N'Течь из радиатора отопления', N'Новая', N'Высокая', DATEADD(DAY, -3, GETDATE()), DATEADD(DAY, 0, GETDATE()), DATEADD(DAY, 1, GETDATE()), N'Мастер', N'', 0),
(7, 3, N'Проверка счетчика электричества', N'Выполнена', N'Обычная', DATEADD(DAY, -60, GETDATE()), DATEADD(DAY, -60, GETDATE()), DATEADD(DAY, -59, GETDATE()), N'Алексей Сафин', N'Проверка пройдена', 500);

-- Работы по обслуживанию дома
INSERT INTO MaintenanceTasks (HouseId, MaintenanceTypeId, Title, Description, Status, PlannedDate, StartDate, EndDate, Budget, ActualCost, Contractor, ResponsiblePerson) VALUES
(1, 1, N'Проверка водопровода', N'Плановая проверка всех труб в доме', N'В работе', DATEADD(DAY, 10, GETDATE()), GETDATE(), DATEADD(DAY, 5, GETDATE()), 15000, 0, N'ООО Водоканал', N'Алексей Сергеев'),
(2, 4, N'Техническое обслуживание котла', N'Ежегодное ТО котельной', N'Планируется', DATEADD(DAY, 60, GETDATE()), DATEADD(DAY, 60, GETDATE()), DATEADD(DAY, 65, GETDATE()), 25000, 0, N'ООО Тепло Сервис', N'Сергей Иванов'),
(3, 2, N'Ревизия электрощитка', N'Проверка состояния электрооборудования', N'Завершена', DATEADD(DAY, -15, GETDATE()), DATEADD(DAY, -20, GETDATE()), DATEADD(DAY, -15, GETDATE()), 8000, 7500, N'ООО Электро', N'Иван Петров'),
(1, 7, N'Техническое обслуживание лифта', N'Ежемесячное ТО лифта', N'В работе', DATEADD(DAY, 5, GETDATE()), GETDATE(), DATEADD(DAY, 1, GETDATE()), 5000, 0, N'ООО ЛифтСервис', N'Виктор Новиков'),
(2, 6, N'Уборка придомовой территории', N'Генеральная уборка территории', N'Планируется', DATEADD(DAY, 30, GETDATE()), DATEADD(DAY, 30, GETDATE()), DATEADD(DAY, 32, GETDATE()), 12000, 0, N'ООО ЧистоТА', N'Ольга Сидорова'),
(4, 8, N'Покраска фасада дома', N'Обновление защитного слоя фасада', N'Планируется', DATEADD(DAY, 90, GETDATE()), DATEADD(DAY, 90, GETDATE()), DATEADD(DAY, 110, GETDATE()), 200000, 0, N'ООО Строй-Про', N'Владимир Козлов'),
(5, 4, N'Модернизация системы отопления', N'Установка новых запорных кранов', N'В работе', DATEADD(DAY, 20, GETDATE()), GETDATE(), DATEADD(DAY, 20, GETDATE()), 50000, 0, N'ООО ТеплоМонтаж', N'Борис Комаров');

-- Статистика
SELECT N'Дома' AS [Таблица], COUNT(*) AS [Записей] FROM Houses
UNION ALL
SELECT N'Квартиры', COUNT(*) FROM Apartments
UNION ALL
SELECT N'Виды обслуживания', COUNT(*) FROM MaintenanceTypes
UNION ALL
SELECT N'Показания услуг', COUNT(*) FROM Utilities
UNION ALL
SELECT N'Заявки на обслуживание', COUNT(*) FROM MaintenanceRequests
UNION ALL
SELECT N'Работы по обслуживанию', COUNT(*) FROM MaintenanceTasks;

-- Проверка русского текста
SELECT TOP 3 Address FROM Houses;
SELECT TOP 3 OwnerName FROM Apartments;
SELECT TOP 3 Name FROM MaintenanceTypes;
