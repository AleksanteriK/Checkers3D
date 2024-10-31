using UnityEngine;
using UnityEngine.UI;

public class Hudmanager : MonoBehaviour
{
    public Gamehandler gameHandler;
    public Text ShowRed;
    public Text ShowBlue;
    public Text ShowResult;
    private string blue_winner = "Blue Win!";
    private string red_winner = "Red Win!";
    private string tie_game = "Tie Game!";

    void Update()
    {
        ShowRed.text = gameHandler.GetAliveReds().ToString();
        ShowBlue.text = gameHandler.GetAliveBlues().ToString();

        if ((gameHandler.GetResult() == Gamehandler.Result.Red || gameHandler.GetResult() == Gamehandler.Result.Blue ||
             gameHandler.GetResult() == Gamehandler.Result.Tie) && gameHandler.GameHasEnded() == true)
        {
            if (gameHandler.GetResult() == Gamehandler.Result.Red)
            {
                ShowResult.color = Color.red;
                ShowResult.text = red_winner;
            }
            
            if (gameHandler.GetResult() == Gamehandler.Result.Blue)
            {
                ShowResult.color = Color.blue;
                ShowResult.text = blue_winner;
            }

            if (gameHandler.GetResult() == Gamehandler.Result.Tie)
            {
                ShowResult.color = Color.green;
                ShowResult.text = tie_game;
            }
        }
    }
}
