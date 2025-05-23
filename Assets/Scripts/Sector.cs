//Copyright (c) 2025 Aleksanteri Koivisto
//Licensed under the MIT License. See LICENSE file in the project root

using UnityEngine;

public class Sector : MonoBehaviour
{
    public Material material;
    public Gamehandler gameHandler;
    public Hudmanager hudManager;
    public bool is_playable = false;
    public bool isclicked = false;

    void OnMouseDown()
    {
        //same here like in Detectmouse.cs, don't detect clicking the sector if controls menu is open
        if (hudManager.IsControlsMenuOpen() == false)
        {
            if (!isclicked)
            {
                isclicked = true;

                //check if there is any other sectors selected, deselect them to avoid conflict
                foreach (var sector in gameHandler.GetSectors())
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
                foreach (var sector in gameHandler.GetSectors())
                {
                    if (sector != null && sector.name != name && sector.GetComponent<Sector>().isclicked == true)
                    {
                        sector.GetComponent<Sector>().isclicked = false;
                    }
                }
            }
        }
    }

    void Start()
    {
        if (gameHandler == null)
        {
            gameHandler = FindObjectOfType<Gamehandler>();
            if (gameHandler == null)
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
