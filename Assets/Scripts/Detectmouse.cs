using UnityEngine;

public class Detectmouse : MonoBehaviour
{
    public Camera Defaultcamera;
    public Camera Camerared;
    public Camera Camerablue;
    public Camera Spectator;
    public Gamehandler gamehandler;
    public Camerahandler camerahandler;
    private Chipaction selectedchip = null; //track the currently selected chip

    void Start()
    {
        gamehandler = FindObjectOfType<Gamehandler>();
        camerahandler = FindObjectOfType<Camerahandler>();

        if (gamehandler == null)
        {
            Debug.LogError("Gamehandler not found in the scene! Ensure there is only one Gamehandler.");
        }

        if (camerahandler == null)
        {
            Debug.LogError("Camerahandler not found in the scene! Ensure there is only one Camerahandler.");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Camerahandler.ActiveCamera currentcamera = camerahandler.GetActiveCamera();

            Ray ray = new Ray();
            RaycastHit hit;

            if (currentcamera == Camerahandler.ActiveCamera.Default)
            {
                ray = Defaultcamera.ScreenPointToRay(Input.mousePosition);
            }

            else if (currentcamera == Camerahandler.ActiveCamera.Red)
            {
                ray = Camerared.ScreenPointToRay(Input.mousePosition);
            }

            else if (currentcamera == Camerahandler.ActiveCamera.Blue)
            {
                ray = Camerablue.ScreenPointToRay(Input.mousePosition);
            }

            else if (currentcamera == Camerahandler.ActiveCamera.Spectator)
            {
                ray = Spectator.ScreenPointToRay(Input.mousePosition);
            }

            else
            {
                Debug.LogWarning("No valid camera is active.");
                return;
            }

            if (Physics.Raycast(ray, out hit, 100))
            {
                Chipaction chip = hit.transform.GetComponent<Chipaction>();

                if (chip != null)
                {
                    if ((chip.GetTeamColor() == Chipaction.TeamColor.Red && gamehandler.turn == Gamehandler.Turn.Red) ||
                        (chip.GetTeamColor() == Chipaction.TeamColor.Blue && gamehandler.turn == Gamehandler.Turn.Blue))
                    {
                        //if there is already a selected chip and it's different from the clicked chip
                        if (selectedchip != null && selectedchip != chip)
                        {
                            selectedchip.DeselectChip();
                        }

                        if (!chip.is_selected)
                        {
                            chip.SelectChip();
                            selectedchip = chip;
                            
                        }

                        else
                        {
                            //if the clicked chip is already selected, deselect it
                            chip.DeselectChip();
                            selectedchip = null;
                        }
                    }
                }
            }
        }
    }
}
