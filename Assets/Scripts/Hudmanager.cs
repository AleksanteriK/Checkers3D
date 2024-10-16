using UnityEngine;
using UnityEngine.UI;
public class Hudmanager : MonoBehaviour
{
    public Gamehandler GameHandler;
    public Text ShowRed;
    public Text ShowBlue;
    public Text ShowResult;
    private string blue_winner = "Blue Win!";
    private string red_winner = "Red Win!";
    private string tie_game = "Tie Game!";

    void Update()
    {
        ShowRed.text = GameHandler.GetAliveReds().ToString();
        ShowBlue.text = GameHandler.GetAliveBlues().ToString();

        if ((GameHandler.GetResult() == Gamehandler.Result.Red || GameHandler.GetResult() == Gamehandler.Result.Blue ||
             GameHandler.GetResult() == Gamehandler.Result.Tie) && GameHandler.GameHasEnded() == true)
        {
            if (GameHandler.GetResult() == Gamehandler.Result.Red)
            {
                ShowResult.color = Color.red;
                ShowResult.text = red_winner;
            }
            
            if (GameHandler.GetResult() == Gamehandler.Result.Blue)
            {
                ShowResult.color = Color.blue;
                ShowResult.text = blue_winner;
            }

            if (GameHandler.GetResult() == Gamehandler.Result.Tie)
            {
                ShowResult.color = Color.green;
                ShowResult.text = tie_game;
            }
        }
    }
}
