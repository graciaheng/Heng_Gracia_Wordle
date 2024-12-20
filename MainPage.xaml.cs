﻿namespace wordleGame;

public partial class MainPage : ContentPage
{
	private readonly FileService _fileService;
	private WordleViewModel wordleViewModel;
	private int attempts = 0;
    private string guess;
    private bool isGridCreated = false;
    private bool isGameOver = false;
    private string[] feedback = new string[5];

	public MainPage()
	{
		InitializeComponent();
		_fileService = new FileService();
		wordleViewModel = new WordleViewModel();
		this.BindingContext = wordleViewModel;
        CreateTheGrid();
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		
        try 
        {
			var words = await _fileService.ReadWords();
            if (words == null || words.Length == 0)
            {
				throw new Exception("No words available in the file.");
            }
			
            string chosenWord = words[new Random().Next(words.Length)];
            wordleViewModel.ChosenWord = chosenWord;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load words: {ex.Message}", "OK");
        }

	}

	private void CreateTheGrid() 
	{
        if (isGridCreated == true)
        {
            return;
        }

        for (int i = 0; i < 6; ++i) 
		{
            GridPageContent.AddRowDefinition(new RowDefinition());
        }

		for (int i = 0; i < 5; ++i) 
		{
            GridPageContent.AddColumnDefinition(new ColumnDefinition());
        }

        GridPageContent.RowSpacing = 10;
        GridPageContent.ColumnSpacing = 10;

        for (int i = 0; i < 6; ++i) {
            for (int j = 0; j < 5; ++j) {
                
                Frame styledFrame = new Frame
                {
                    BackgroundColor = Colors.GhostWhite,
                    BorderColor = Colors.Grey,
                    CornerRadius = 10
                };

                Label styledLabel = new Label
                {
                    Text = "", // Initially no text
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = 24,
                    FontFamily = "Karnak Condensed",
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.White
                };

                styledFrame.Content = styledLabel;

				GridPageContent.Add(styledFrame, j, i);
            }
        }

        isGridCreated = true;
    }

    private async void showHistory(object sender, EventArgs args) 
	{
        await Navigation.PushAsync(new HistoryPage());
    }

	private void submitGuess(object sender, EventArgs args) 
	{
		guess = $"{Letter1.Text}{Letter2.Text}{Letter3.Text}{Letter4.Text}{Letter5.Text}";

		//check if user has inputted correctly
        if (guess.Length != 5 || !guess.All(char.IsLetter))
        {
            DisplayAlert("Invalid Guess", "Please enter a valid 5-letter word.", "OK");
            return;
        }

		checkGuess();
        UpdateGrid();

        //correct word
        if (guess.Equals(wordleViewModel.ChosenWord, StringComparison.OrdinalIgnoreCase))
        {
            DisplayStatus.Text = "You Won!";
        }

        //wrong word
        else
        {
            attempts++;

            if (attempts == 6)
            {
                DisplayStatus.Text = $"Game Over! The word was {wordleViewModel.ChosenWord}";
                isGameOver = true;
                savePlayerAttempt(isGameOver, wordleViewModel.ChosenWord, attempts);
                DisableSubmitButton();
            }
        }

        ClearEntryFields();
	}

    private void ClearEntryFields()
    {
        Letter1.Text = string.Empty;
        Letter2.Text = string.Empty;
        Letter3.Text = string.Empty;
        Letter4.Text = string.Empty;
        Letter5.Text = string.Empty;
    }

    private void checkGuess() 
    {
        Array.Fill(feedback, string.Empty);
        bool[] matched = new bool[5];

        for (int i = 0; i < 5; i++)
        {
            char guessedLetter = char.ToUpper(guess[i]);
            char correctLetter = char.ToUpper(wordleViewModel.ChosenWord[i]);

            if (guessedLetter == correctLetter)
            {
                feedback[i] = "Correct";
                matched[i] = true;
            }
        }

        for (int i = 0; i < 5; i++) 
        {
            if (feedback[i] == "Correct")
            continue; //skip if already matched

            char guessedLetter = char.ToUpper(guess[i]);

            for (int j = 0; j < 5; j++)
            {
                //only updates if this letter has not been matched yet
                if (!matched[j] && char.ToUpper(wordleViewModel.ChosenWord[j]) == guessedLetter)
                {
                    feedback[i] = "WrongPlace"; 
                    matched[j] = true;
                    break;
                }
            }

            if (feedback[i] == string.Empty)
            {
                feedback[i] = "Incorrect";  
            }
        }
    }

	private void UpdateGrid()
	{
		for (int i = 0; i < 5; i++) {
			 
			  // Get the correct Border from the grid
			int index = i + (attempts * 5);

            if (GridPageContent.Children[index] is Frame frame)
			{
                if (frame.Content is Label label)
                {
                    label.Text = guess[i].ToString().ToUpper();

				    switch (feedback[i])
			        {
				        case "Correct":
                        frame.BackgroundColor = Colors.YellowGreen;
                        break;
                
				        case "WrongPlace":
                        frame.BackgroundColor = Colors.Gold;
                        break;

                        case "Incorrect":
                        frame.BackgroundColor = Colors.Gray;
                        break;
                    }
                }
			}
			else
			{
				Console.WriteLine($"Unexpected type: {GridPageContent.Children[index].GetType()}");
			}
		}
	}

    private void DisableSubmitButton()
    {
        Letter1.IsEnabled = false;
        Letter2.IsEnabled = false;
        Letter3.IsEnabled = false;
        Letter4.IsEnabled = false;
        Letter5.IsEnabled = false;
        submitButton.IsEnabled = false; // Disable the submit button after game over
    }

    private void savePlayerAttempt(bool isWordGuessed, string correctWord, int numOfGuesses)
    {
        var attempt = new PlayerAttempt(isWordGuessed, correctWord, numOfGuesses);
        wordleViewModel.PlayerHistory.Attempts.Add(attempt);
        wordleViewModel.PlayerHistory.SaveHistoryAsync();
    }
}

