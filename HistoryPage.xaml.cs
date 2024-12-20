using wordleGame;

namespace wordleGame;

public partial class HistoryPage : ContentPage
{
	public PlayerHistory playerHistory { get; set; }
	public HistoryPage()
	{
		InitializeComponent();
		playerHistory = new PlayerHistory();
		this.BindingContext = playerHistory;
		loadHistory();
	}
	private async void loadHistory()
	{
		await playerHistory.LoadHistoryAsync();
	}

	private async void ReturnToGame(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}