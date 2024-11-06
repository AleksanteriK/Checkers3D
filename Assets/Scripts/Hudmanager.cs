using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Hudmanager : MonoBehaviour
{
    public Gamehandler gameHandler;
    public Camerahandler cameraHandler;
    public GameObject ControlsMenuPanel;
    public GameObject GameOverExitButtons;
    public GameObject ConfirmToExitMenuButtons;
    public UnityEngine.UI.Image customCursor;
    public Text ShowRed;
    public Text ShowBlue;
    public Text ShowResult;
    public Text ShowCamera;

    //this is used in Detectmouse.cs to check whether the controls menu is open to disable raycasting
    public bool IsControlsMenuOpen()
    {
        return ControlsMenuPanel.activeSelf;
    }

    public bool IsExitMenuButtonsOpen()
    {
        return ConfirmToExitMenuButtons.activeSelf;
    }

    public void ShowControlsInstructions()
    {
        ControlsMenuPanel.SetActive(true);
    }

    public void HideControlsInstructions()
    {
        ControlsMenuPanel.SetActive(false);
    }

    public void ShowConfirmToExitMenuButtons()
    {
        ConfirmToExitMenuButtons.SetActive(true);
    }

    public void HideConfirmToExitMenuButtons()
    {
        ConfirmToExitMenuButtons.SetActive(false);
    }

    public void RestartGame()
    {
        Scene scene = SceneManager.GetActiveScene(); 
        SceneManager.LoadScene(scene.name);
    }

    public void QuitToMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    void Awake()
    {
        ControlsMenuPanel.SetActive(false);
        customCursor.gameObject.SetActive(false);
        GameOverExitButtons.SetActive(false);
        ConfirmToExitMenuButtons.SetActive(false);
    }

    void Start()
    {
        ControlsMenuPanel.SetActive(false);
        customCursor.gameObject.SetActive(false);
        GameOverExitButtons.SetActive(false);
        ConfirmToExitMenuButtons.SetActive(false);
    }

    void Update()
    {
        ShowRed.text = gameHandler.GetAliveReds().ToString();
        ShowBlue.text = gameHandler.GetAliveBlues().ToString();
        ShowCamera.text = cameraHandler.GetActiveCamera().ToString();

        //open prompt to confirm to exit menu or not
        if (gameHandler.GameHasEnded() == false && Input.GetKeyDown(KeyCode.Escape))
        {
            //if player presses ESC again, hide prompt
            if (ConfirmToExitMenuButtons.activeSelf == true)
            {
                ConfirmToExitMenuButtons.SetActive(false);
            }

            else
            {
                 ConfirmToExitMenuButtons.SetActive(true);
            }
        }

        if (cameraHandler.GetComponent<Camerahandler>().automode == true)
        {
            ShowCamera.text = "Auto";
        }
                                                                        
        if (cameraHandler.GetActiveCamera() == Camerahandler.ActiveCamera.Spectator && gameHandler.GameHasEnded() == false)
        {  
            customCursor.gameObject.SetActive(true);
        }
        else
        {   //don't display the custom cursor anymore if the game has ended in spectator mode
            customCursor.gameObject.SetActive(false);
        }

        if ((gameHandler.GetResult() == Gamehandler.Result.Red || gameHandler.GetResult() == Gamehandler.Result.Blue ||
             gameHandler.GetResult() == Gamehandler.Result.Tie) && gameHandler.GameHasEnded() == true)
        {
            GameOverExitButtons.SetActive(true);

            if (gameHandler.GetResult() == Gamehandler.Result.Red)
            {
                ShowResult.color = Color.red;
                ShowResult.text = "Red Win!";
            }
            
            if (gameHandler.GetResult() == Gamehandler.Result.Blue)
            {
                ShowResult.color = Color.blue;
                ShowResult.text = "Blue Win!";
            }

            if (gameHandler.GetResult() == Gamehandler.Result.Tie)
            {
                ShowResult.color = Color.green;
                ShowResult.text = "Tie Game!";
            }
        }
    }
}
