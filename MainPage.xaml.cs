namespace wordleGame;

public partial class MainPage : ContentPage
{
	private readonly FileService _fileService;
	private WordleViewModel wordleViewModel;
	private int attempts = 0;

	public MainPage()
	{
		InitializeComponent();
		_fileService = new FileService();
		wordleViewModel = new WordleViewModel();
		BindingContext = wordleViewModel;
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

	private void CreateTheGrid(object sender, EventArgs args) 
	{
        for (int i = 0; i < 6; ++i) 
		{
            GridPageContent.AddRowDefinition(new RowDefinition());
        }

		for (int i = 0; i < 5; ++i) 
		{
            GridPageContent.AddColumnDefinition(new ColumnDefinition());
        }

            //Populate the grid with Borders
        for (int i = 0; i < 6; ++i) {
            for (int j = 0; j < 5; ++j) {
                Border styledBorder = new Border
                {
                    BackgroundColor = Colors.White, // Set the background color
                    Stroke = Colors.Grey,
                    StrokeThickness = 3
                };

				GridPageContent.Add(styledBorder, j, i);
            }
        }
    }

	private void submitGuess(object sender, EventArgs args) 
	{
		string guessString = new string(wordleViewModel.Guess);

		//check if user has inputted correctly
        if (guessString.Length != 5 || !guessString.All(char.IsLetter))
        {
            DisplayAlert("Invalid Guess", "Please enter a valid 5-letter word.", "OK");
            return;
        }

		string[] feedback = wordleViewModel.CheckGuess();
		UpdateGrid(feedback);

        //correct word
        if (guessString.Equals(wordleViewModel.ChosenWord, StringComparison.OrdinalIgnoreCase))
        {
            DisplayAlert("Congratulations!", "You have guessed this word!", "OK");
        }

        //wrong word
        else
        {
            wordleViewModel.IncrementAttempts();

            if (wordleViewModel.Attempts == 6)
            {
                DisplayAlert("Game Over!", "You have ran out of attempts!", "OK");
            }
        }
	}

	private void UpdateGrid(string[] feedback)
	{
		for (int i = 0; i < 5; i++) {
			 
			  // Get the correct Border from the grid
			int index = i + (wordleViewModel.Attempts * 5);

            if (GridPageContent.Children[index] is Border border)
			{
				switch (feedback[i])
			    {
				    case "Correct":
                    border.BackgroundColor = Colors.YellowGreen;
                    break;
                
				    case "WrongPlace":
                    border.BackgroundColor = Colors.Gold;
                    break;

                    case "Incorrect":
                    border.BackgroundColor = Colors.Gray;
                    break;
                }
			}
			else
			{
				Console.WriteLine($"Unexpected type: {GridPageContent.Children[index].GetType()}");
			}
		}
	}
}

