using System;
using System.IO;
using System.Text.Json;

namespace wordleGame;

public partial class StartPage : ContentPage
{
    private Player player1;
    private Player player2;
	public StartPage()
	{
		InitializeComponent();
	}

	private void OnGameModeChanged(object sender, EventArgs e)
    {
        // Enable the "Start Game" button only when both name and game mode are selected
        StartGameButton.IsEnabled = !string.IsNullOrEmpty(PlayerNameEntry.Text) && GameModePicker.SelectedIndex != -1;
    }

	private async void OnStartGameClicked(object sender, EventArgs e)
	{
        //Get the name inputted from entry
		string playerName = PlayerNameEntry.Text;

        if (string.IsNullOrEmpty(playerName))
        {
            await DisplayAlert("Error", "Please enter your name.", "OK");
            return;
        }

        // Load or create player data
        await LoadOrCreatePlayer(playerName);

        // Get the selected game mode
        string selectedGameMode = (string)GameModePicker.SelectedItem;

        if (selectedGameMode == null)
        {
            await DisplayAlert("Error", "Please select a game mode.", "OK");
            return;
        }

        // Navigate to the appropriate game page based on selected mode
        if (selectedGameMode == "1 Player Mode")
        {
            await Navigation.PushAsync(new MainPage(playerName));
        }
        else if (selectedGameMode == "2 Player Mode")
        {
            await Navigation.PushAsync(new DoubleGame(playerName));
        }
	}

	private async Task LoadOrCreatePlayer(string playerName)
    {
        string directory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string playerFilePath = Path.Combine(directory, $"{playerName}.json");

        if (File.Exists(playerFilePath))
        {
            // Load player data from the file
            string json = await File.ReadAllTextAsync(playerFilePath);
            Player player = JsonSerializer.Deserialize<Player>(json);
             // You can display the player details or update the app accordingly
        }
        else
        {
            // Create a new player file
            player1 = new Player(playerName);
            string json = JsonSerializer.Serialize(player1);
            await File.WriteAllTextAsync(playerFilePath, json);
        }
    }
}