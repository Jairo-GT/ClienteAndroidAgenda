namespace ClienteAndroidAgenda.Models;

public partial class CustomCalendar : ContentView
{
    private DateTime _currentDate;
    public CustomCalendar()
    {
        InitializeComponent();

        _currentDate = DateTime.Now;
        UpdateCalendar();

        PreviousMonthButton.Clicked += (s, e) => ChangeMonth(-1);
        NextMonthButton.Clicked += (s, e) => ChangeMonth(1);
    }

    private void ChangeMonth(int increment)
    {
        _currentDate = _currentDate.AddMonths(increment);
        UpdateCalendar();
    }

    private void UpdateCalendar()
    {
        // Update header
        MonthYearLabel.Text = _currentDate.ToString("MMMM yyyy");

        // Clear previous calendar
        DaysGrid.Children.Clear();
        DaysGrid.RowDefinitions.Clear();

        // Add rows and columns
        DaysGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Weekdays row
        for (int i = 0; i < 6; i++) // Max 6 weeks in a month
        {
            DaysGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        // Add weekdays labels
        var weekdays = new[] { "Dom", "Lun", "Mar", "Mié", "Jue", "Vie", "Sáb" };
        for (int i = 0; i < weekdays.Length; i++)
        {
            DaysGrid.Add(new Label
            {
                Text = weekdays[i],
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            }, i, 0);
        }

        // Fill days of the month
        var firstDay = new DateTime(_currentDate.Year, _currentDate.Month, 1);
        int startColumn = (int)firstDay.DayOfWeek;
        int row = 1, column = startColumn;

        for (int day = 1; day <= DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month); day++)
        {
            var dayButton = new Button { Text = day.ToString() };
            dayButton.BackgroundColor = Colors.Transparent;
            dayButton.BorderColor = Colors.Black;
            dayButton.BorderWidth = 1;
            dayButton.TextColor = Colors.Black;
            dayButton.Clicked += DayButton_Clicked;

            DaysGrid.Add(dayButton, column, row);

            column++;
            if (column == 7) // New row
            {
                column = 0;
                row++;
            }
        }
    }

    private void DayButton_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            int day = int.Parse(button.Text);
            var selectedDate = new DateTime(_currentDate.Year, _currentDate.Month, day);

            // Handle day selection here
            Console.WriteLine($"Selected Date: {selectedDate.ToShortDateString()}");
        }
    }
}