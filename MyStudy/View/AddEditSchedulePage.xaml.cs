using MyStudy.Service;
using MyStudy.Model;
using MyStudy.View;


namespace MyStudy.View;

public partial class AddEditSchedulePage : ContentPage
{
    private readonly DatabaseService _databaseService;
    private int? _scheduleId;

    public AddEditSchedulePage(int? scheduleId = null)
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
        _scheduleId = scheduleId;

        if (_scheduleId.HasValue)
        {
            LoadScheduleAsync();
        }
    }

    private async void LoadScheduleAsync()
    {
        var schedule = await _databaseService.GetScheduleAsync(_scheduleId.Value);

        TaskNameEntry.Text = schedule.TaskName;
        TaskDescriptionEditor.Text = schedule.TaskDescription;
        DayOfWeekPicker.SelectedItem = schedule.DayOfWeek;
        StartTimePicker.Time = schedule.StartTime;
        EndTimePicker.Time = schedule.EndTime;
        PriorityPicker.SelectedItem = schedule.Priority;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // Validate the user input
        if (string.IsNullOrWhiteSpace(TaskNameEntry.Text) || StartTimePicker.Time >= EndTimePicker.Time)
        {
            await DisplayAlert("Error", "Task name is required and start time must be before end time.", "OK");
            return;
        }

        var schedule = new Schedule
        {
            TaskName = TaskNameEntry.Text,
            TaskDescription = TaskDescriptionEditor.Text,
            StartTime = StartTimePicker.Time,
            EndTime = EndTimePicker.Time,
            DayOfWeek = DayOfWeekPicker.SelectedItem?.ToString(),
            Priority = PriorityPicker.SelectedItem?.ToString()
        };

        // Save or update the schedule
        if (_scheduleId.HasValue)
        {
            schedule.Id = _scheduleId.Value;
            await _databaseService.UpdateScheduleAsync(schedule);
        }
        else
        {
            await _databaseService.AddScheduleAsync(schedule);
        }

        // Notify MainPage to reload the schedule list
        MessagingCenter.Send(this, "RefreshSchedules");

        // Navigate back to MainPage
        await Navigation.PopAsync();
    }
}