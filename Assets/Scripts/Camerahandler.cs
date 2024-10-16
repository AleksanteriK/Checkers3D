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
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            SwitchToDefaultCamera();
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            SwitchToRedCamera();
        }

        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            SwitchToBlueCamera();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            SwitchToSpectatorCamera();
        }
    }
}