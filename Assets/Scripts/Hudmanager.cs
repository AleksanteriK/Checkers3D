using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Hudmanager : MonoBehaviour
{
    public Gamehandler gameHandler;
    public Camerahandler cameraHandler;
    public GameObject ControlsMenuPanel;
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

    public void ShowControlsInstructions()
    {
        ControlsMenuPanel.SetActive(true);
    }

    public void HideControlsInstructions()
    {
        ControlsMenuPanel.SetActive(false);
    }

    void Awake()
    {
        ControlsMenuPanel.SetActive(false);
        customCursor.gameObject.SetActive(false);
    }

    void Start()
    {
        ControlsMenuPanel.SetActive(false);
        customCursor.gameObject.SetActive(false);
    }

    void Update()
    {
        ShowRed.text = gameHandler.GetAliveReds().ToString();
        ShowBlue.text = gameHandler.GetAliveBlues().ToString();
        ShowCamera.text = cameraHandler.GetActiveCamera().ToString();

        if (cameraHandler.GetComponent<Camerahandler>().automode == true)
        {
            ShowCamera.text = "Auto";
        }
                                                                        
        if (cameraHandler.GetActiveCamera() == Camerahandler.ActiveCamera.Spectator && gameHandler.GameHasEnded() == false)
        {   //don't display the custom cursor if the game has ended in spectator mode
            customCursor.gameObject.SetActive(true);
        }
        else
        {
            customCursor.gameObject.SetActive(false);
        }

        if ((gameHandler.GetResult() == Gamehandler.Result.Red || gameHandler.GetResult() == Gamehandler.Result.Blue ||
             gameHandler.GetResult() == Gamehandler.Result.Tie) && gameHandler.GameHasEnded() == true)
        {
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
