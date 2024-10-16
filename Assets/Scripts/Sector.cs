using UnityEngine;
public class Sector : MonoBehaviour
{
    public Material material;
    public Gamehandler gamehandler;
    public bool is_playable = false;
    public bool isclicked = false;

    private void OnMouseDown()
    {
        if (!isclicked)
        {
            isclicked = true;

            //check if there is any other sectors selected, deselect them to avoid conflict
            foreach (var sector in gamehandler.GetSectors())
            {
                if (sector != null && sector.name != name && sector.GetComponent<Sector>().isclicked == true)
                {
                    sector.GetComponent<Sector>().isclicked = false;
                }
            }
        }

        else
        {
            isclicked = false;

            //same here
            foreach (var sector in gamehandler.GetSectors())
            {
                if (sector != null && sector.name != name && sector.GetComponent<Sector>().isclicked == true)
                {
                    sector.GetComponent<Sector>().isclicked = false;
                }
            }
        }
    }

    void Start()
    {
        if (gamehandler == null)
        {
            gamehandler = FindObjectOfType<Gamehandler>();
            if (gamehandler == null)
            {
                Debug.LogError("Gamehandler not found! Ensure there is a Gamehandler in the scene.");
            }
        }

        if (material == null)
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                material = renderer.material;
            }
            else
            {
                Debug.LogError("Renderer or Material not found on the Sector object!");
            }
        }
    }
}
