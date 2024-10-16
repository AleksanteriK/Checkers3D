using UnityEngine;

public class Gridmanager : MonoBehaviour
{
    public Gamehandler gameHandler;
    [SerializeField] private GameObject Boardsector;
    [SerializeField] private GameObject Redchip;
    [SerializeField] private GameObject Bluechip;

    void GenerateGrid()
    {
        for (int x = 0; x < gameHandler.width; x++)
        {
            for (int z = 0; z < gameHandler.height; z++)
            {
                //calculating the position for each sector
                Vector3 spawnpos = new Vector3(x * gameHandler.sectorspacing_x, 0, z * gameHandler.sectorspacing_z);
                var spawnedsector = Instantiate(Boardsector, spawnpos, Quaternion.identity);
                Sector sector = spawnedsector.GetComponent<Sector>();
                spawnedsector.name = $"Sector {x}_{z}";

                //if the sector is playable
                bool is_playable = (x % 2 == 0 && z % 2 != 0) || (x % 2 != 0 && z % 2 == 0);

                if (is_playable)
                {
                    sector.is_playable = true;
                    sector.GetComponent<MeshRenderer>().material.color = Color.black;

                    //there should be no white sectors added to gamehandler's list for no reason
                    gameHandler.AddToSectors(sector);
                }

                //spawn red chips only on playable sectors and in the bottom third of the board
                if (is_playable && z < 3)
                {
                    Vector3 chipspawnpos = new Vector3(spawnpos.x, 0.39f, spawnpos.z);
                    var spawned_redparent = Instantiate(Redchip, chipspawnpos, Quaternion.Euler(-90f, 0, 0));

                    Transform soldier = spawned_redparent.transform.Find("Redsoldier");

                    if (soldier != null)
                    {
                        soldier.SetPositionAndRotation(chipspawnpos, Quaternion.Euler(-90f, 0, 0));
                    }

                    Transform king = spawned_redparent.transform.Find("Redking");

                    if (king != null)
                    {
                        //king chips should be rotated 180 degrees to display the crown correclty
                        king.SetPositionAndRotation(chipspawnpos, Quaternion.Euler(-90f, 0, -180f));
                    }

                    Chipaction red = spawned_redparent.GetComponent<Chipaction>();
                    red.AssignTeam(Chipaction.TeamColor.Red);
                    spawned_redparent.name = $"RedChip {x}_{z}";
                    gameHandler.AddToChips(red);

                    //only the front row chips are able to be moved in the beginning
                    if (spawnpos.z == gameHandler.sectorspacing_z * 2)
                    {
                        red.can_move = true;
                    }
                }

                //spawn blue chips only on playable sectors and in the top third of the board
                if (is_playable && z >= (gameHandler.height * 2) / 3)
                {
                    Vector3 chipspawnpos = new Vector3(spawnpos.x, 0.39f, spawnpos.z);
                    var spawned_blueparent = Instantiate(Bluechip, chipspawnpos, Quaternion.Euler(-90f, 0, 0));

                    Transform soldier = spawned_blueparent.transform.Find("Bluesoldier");

                    if (soldier != null)
                    {
                        soldier.SetPositionAndRotation(chipspawnpos, Quaternion.Euler(-90f, 0, 0));
                    }

                    Transform king = spawned_blueparent.transform.Find("Blueking");

                    if (king != null)
                    {
                        king.SetPositionAndRotation(chipspawnpos, Quaternion.Euler(-90f, 0, -180f));
                    }

                    Chipaction blue = spawned_blueparent.GetComponent<Chipaction>();
                    blue.AssignTeam(Chipaction.TeamColor.Blue);
                    spawned_blueparent.name = $"BlueChip {x}_{z}";
                    gameHandler.AddToChips(blue);

                    if (spawnpos.z == gameHandler.sectorspacing_z * 5)
                    {
                        blue.can_move = true;
                    }
                }
            }
        }
    }

    void Start()
    {
        gameHandler = FindObjectOfType<Gamehandler>();

        if (gameHandler == null)
        {
            Debug.LogError("Gamehandler not found in the scene! Ensure there is only one Gamehandler.");
        }

        GenerateGrid();
    }
}
