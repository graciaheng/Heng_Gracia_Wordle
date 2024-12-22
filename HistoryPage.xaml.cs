using wordleGame;

namespace wordleGame;

public partial class HistoryPage : ContentPage
{
	public PlayerHistory playerHistory { get; set; }
	public PlayerAttempt playerAttempt { get; set; }
	public HistoryPage()
	{
		InitializeComponent();
		playerHistory = new PlayerHistory();
		//playerAttempt = new PlayerAttempt();
		this.BindingContext = playerHistory;
		loadHistory();
	}
	private async void loadHistory()
	{
		try
        {
            await playerHistory.LoadHistoryAsync();

            // If no history found, display a message
            if (playerHistory.Attempts.Count == 0)
            {
                NoHistoryLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading history: {ex.Message}");
            await DisplayAlert("Error", "Failed to load history.", "OK");
        }
	}

	private async void ReturnToGame(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}