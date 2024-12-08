using MyStudy.Service;
using System.Collections.ObjectModel;
using MyStudy.Model;
using MyStudy.View;

namespace MyStudy
{
    public partial class MainPage : ContentPage
    {
        private readonly DatabaseService _databaseService;

        // Property to track button selection state
        public bool IsAddButtonSelected { get; set; }

        public ObservableCollection<Schedule> Schedules { get; set; }

        public MainPage()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
            Schedules = new ObservableCollection<Schedule>();
            BindingContext = this;

            // Subscribe to the RefreshSchedules message
            MessagingCenter.Subscribe<AddEditSchedulePage>(this, "RefreshSchedules", (sender) =>
            {
                LoadSchedules(); // Refresh schedules when the message is received
            });
            LoadSchedules();
        }

        private async void LoadSchedules()
        {
            // Clear existing data and reload from the database
            var schedules = await _databaseService.GetSchedulesAsync();
            Schedules.Clear();
            foreach (var schedule in schedules)
            {
                Schedules.Add(schedule);
            }
        }

        private async void OnAddScheduleClicked(object sender, EventArgs e)
        {
            // Toggle the selection state of the button
            IsAddButtonSelected = !IsAddButtonSelected;

            // Navigate to the Add/Edit Schedule page
            await Navigation.PushAsync(new AddEditSchedulePage());
            LoadSchedules(); // Refresh after adding
        }

        private async void OnEditSchedule(object sender, EventArgs e)
        {
            var id = (int)((Button)sender).CommandParameter;
            await Navigation.PushAsync(new AddEditSchedulePage(id));
            LoadSchedules(); // Refresh after editing
        }

        private async void OnDeleteSchedule(object sender, EventArgs e)
        {
            var id = (int)((Button)sender).CommandParameter;
            await _databaseService.DeleteScheduleAsync(id);
            LoadSchedules(); // Refresh after deleting
        }
    }
}