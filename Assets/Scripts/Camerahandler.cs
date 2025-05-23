//Copyright (c) 2025 Aleksanteri Koivisto
//Licensed under the MIT License. See LICENSE file in the project root

using System.Collections;
using UnityEngine;

public class Camerahandler : MonoBehaviour
{
    public enum ActiveCamera
    {
        Default,
        Red,
        Blue,
        Spectator
    }

    public bool automode = false;
    public Gamehandler gameHandler;
    public Hudmanager hudManager;
    ActiveCamera activecamera;
    [SerializeField] GameObject DefaultCamera;
    [SerializeField] GameObject CameraBlue;
    [SerializeField] GameObject CameraRed;
    [SerializeField] GameObject SpectatorCamera;

    public ActiveCamera GetActiveCamera()
    {
        return activecamera;
    }

    void SwitchToDefaultCamera()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        DefaultCamera.SetActive(true);
        CameraBlue.SetActive(false);
        CameraRed.SetActive(false);
        SpectatorCamera.SetActive(false);
        activecamera = ActiveCamera.Default;
    }

    void SwitchToRedCamera()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        CameraRed.SetActive(true);
        CameraBlue.SetActive(false);
        DefaultCamera.SetActive(false);
        SpectatorCamera.SetActive(false);
        activecamera = ActiveCamera.Red;
    }

    void SwitchToBlueCamera()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        CameraBlue.SetActive(true);
        CameraRed.SetActive(false);
        DefaultCamera.SetActive(false);
        SpectatorCamera.SetActive(false);
        activecamera = ActiveCamera.Blue;
    }

    public IEnumerator AutoChange()
    {
        yield return new WaitForSeconds(0.25f);

        if (automode == true)
        {
            if (gameHandler.turn == Gamehandler.Turn.Red)
            {
                SwitchToRedCamera();
            }

            else
            {
                SwitchToBlueCamera();
            }
        }
    }

    public void SwitchToSpectatorCamera()
    {
        SpectatorCamera.SetActive(true);
        CameraBlue.SetActive(false);
        CameraRed.SetActive(false);
        DefaultCamera.SetActive(false);
        activecamera = ActiveCamera.Spectator;
    }

    void Awake()
    {
        DefaultCamera.SetActive(true);
        CameraBlue.SetActive(false);
        CameraRed.SetActive(false);
        SpectatorCamera.SetActive(false);
        activecamera = ActiveCamera.Default;
    }

    void Update()
    {
        //if player presses ESC, switch to default view in order to let player use mouse pointer
        if (hudManager.IsExitMenuButtonsOpen() == true)
        {
            SwitchToDefaultCamera();
            automode = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad0) || Input.GetKeyDown(KeyCode.Alpha0))
        {
            SwitchToDefaultCamera();
            automode = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchToRedCamera();
            automode = false;
        }

        if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchToBlueCamera();
            automode = false;
        }

        if (Input.GetKeyDown(KeyCode.F) && gameHandler.GameHasEnded() == false)
        {
            SwitchToSpectatorCamera();
            automode = false;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (automode == false)
            {
                automode = true;

                if (gameHandler.turn == Gamehandler.Turn.Red)
                {
                    SwitchToRedCamera();
                }

                else
                {
                    SwitchToBlueCamera();
                }
            }

            else
            {
                automode = false;
                SwitchToDefaultCamera();
            }
        }
    }
}