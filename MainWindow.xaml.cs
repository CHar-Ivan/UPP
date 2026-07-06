using System;
using System.Windows;
using ла629._1.Data;

namespace ла629._1
{
    public partial class MainWindow : Window
    {
        private JekDatabaseService _dbService;

        public MainWindow()
        {
            InitializeComponent();
            _dbService = new JekDatabaseService();
            LoadData();
        }

        private async void LoadData()
        {
            bool connected = await _dbService.ConnectToServerAsync();

            if (connected)
            {
                await _dbService.SyncWithServerAsync();

                // Ниже — пример привязки данных к вашему DataGrid (проверьте имя HousesGrid в XAML)
                try
                {
                    var houses = await _dbService.GetHousesAsync();
                    HousesGrid.ItemsSource = houses;
                }
                catch { }
            }
            else
            {
                _dbService.InitializeOfflineData();
                MessageBox.Show("Не удалось подключиться к базе данных. Приложение запущено в автономном режиме (Offline).", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);

                try
                {
                    var houses = await _dbService.GetHousesAsync();
                    HousesGrid.ItemsSource = houses;
                }
                catch { }
            }
        }

        private async void AddHouseButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(HouseAddressBox.Text) || string.IsNullOrWhiteSpace(HouseBuildingBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните основные текстовые поля (Адрес и Номер здания)!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int.TryParse(HouseFloorsBox.Text, out int floors);
            int.TryParse(HouseApartmentsBox.Text, out int apartments);
            int.TryParse(HouseYearBox.Text, out int year);

            var newHouse = new Models.House
            {
                Address = HouseAddressBox.Text.Trim(),
                BuildingNumber = HouseBuildingBox.Text.Trim(),
                FloorsCount = floors,
                ApartmentsCount = apartments,
                BuildYear = year,
                Material = HouseMaterialBox.Text.Trim()
            };

            try
            {
                await _dbService.AddHouseAsync(newHouse);

                if (_dbService.IsConnected)
                {
                    if (_dbService.SaveChangesToServer())
                    {
                        MessageBox.Show("Дом успешно добавлен и синхронизирован с базой данных SQL!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Данные сохранены локально, но произошла ошибка при обновлении таблиц на SQL Server.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else
                {
                    MessageBox.Show("Дом успешно добавлен в локальный список (Offline режим).", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Обновляем отображение в таблице
                var houses = await _dbService.GetHousesAsync();
                HousesGrid.ItemsSource = null;
                HousesGrid.ItemsSource = houses;

                // Очищаем форму ввода
                HouseAddressBox.Clear();
                HouseBuildingBox.Clear();
                HouseFloorsBox.Clear();
                HouseApartmentsBox.Clear();
                HouseYearBox.Clear();
                HouseMaterialBox.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении операции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}