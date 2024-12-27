using System;

namespace wordleGame;

public class Player
{
    public string Name { get; set; }
    public int Attempts { get; set; }
    public string ChosenWord { get; set; }
    public bool IsGameOver { get; set; }

    public Player(string name)
    {
        Name = name;
        Attempts = 0;
        IsGameOver = false;
    }
}
