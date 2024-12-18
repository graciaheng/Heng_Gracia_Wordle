namespace wordleGame;

public partial class MainPage : ContentPage
{
	private readonly FileService _fileService;
	private WordleViewModel wordleViewModel;

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
		
        var words = await _fileService.ReadWordsAsync();

		Random random = new Random();
		string chosenWord = words[random.Next(words.Length)];

		wordleViewModel.ChosenWord = chosenWord;
    
        Console.WriteLine($"Chosen Word: {chosenWord}");
	}

}

