using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Chipaction;

public class Gamehandler : MonoBehaviour
{
    public enum Turn
    {
        Red,
        Blue,
        None
    }

    public enum SectorStatus
    {
        Empty,
        Red,
        Blue,
    }
    public enum Directions
    {
        UpLeft,
        UpRight,
        DownLeft,
        DownRight,
    }

    public enum Result
    {
        Red,
        Blue,
        Tie
    }

    //spacing values of the gameboard sectors
    public float sectorspacing_x = 2.5f;
    public float sectorspacing_z = 2.125f;
    public float tolerance = 0.05f; //for comparing float values
    public int width = 8;
    public int height = 8;
    public int initialturn; //store the int variable which is get from main menu playerprefs
    public Camerahandler cameraHandler;
    public Turn turn;
    private Result result;
    private bool gameover = false;
    [SerializeField] int alive_reds = 12;
    [SerializeField] int movable_reds = 12;
    [SerializeField] int alive_blues = 12;
    [SerializeField] int movable_blues = 12;
    [SerializeField] List<Component> sectors = new List<Component>(); //only contains playable sectors
    [SerializeField] List<Component> chips = new List<Component>();
    [SerializeField] List<Component> availablesectors = new List<Component>();
    [SerializeField] List<Component> jumpablesectors = new List<Component>();
    [SerializeField] List<Component> opponents = new List<Component>();
    [SerializeField] List<Component> jumpablesectors_on_the_upright_path = new List<Component>();
    [SerializeField] List<Component> jumpablesectors_on_the_upleft_path = new List<Component>();
    [SerializeField] List<Component> jumpablesectors_on_the_downleft_path = new List<Component>();
    [SerializeField] List<Component> jumpablesectors_on_the_downright_path = new List<Component>();
    [SerializeField] List<Component> opponents_on_the_upright_path = new List<Component>();
    [SerializeField] List<Component> opponents_on_the_upleft_path = new List<Component>();
    [SerializeField] List<Component> opponents_on_the_downright_path = new List<Component>();
    [SerializeField] List<Component> opponents_on_the_downleft_path = new List<Component>();

    /*one board sector is size of x=2.5 and z=2.125 units
    so if chip needs to be moved, one move is either - or + x=2.5 and + z=2.125
    (- z=2.125 also if chip is king)
    blue chips and red chips are placed in the 3D space differently. When blue needs to move forward,
     it's z vector position needs to be decremented, and the red chip's z vector position needs to be increased instead*/

    public CanDo CheckPossibilities(float current_x, float current_z, bool is_king, Chipaction.TeamColor teamcolor, bool has_jumped, bool is_selected)
    {
        availablesectors.Clear();
        jumpablesectors.Clear();
        opponents.Clear();
        jumpablesectors_on_the_upright_path.Clear();
        jumpablesectors_on_the_upleft_path.Clear();
        jumpablesectors_on_the_downleft_path.Clear();
        jumpablesectors_on_the_downright_path.Clear();
        opponents_on_the_upright_path.Clear();
        opponents_on_the_upleft_path.Clear();
        opponents_on_the_downleft_path.Clear();
        opponents_on_the_downright_path.Clear();

        //directional offsets for regular chips
        float move_x_left = -sectorspacing_x;
        float move_x_right = sectorspacing_x;
        float move_z_forward = (teamcolor == Chipaction.TeamColor.Red) ? sectorspacing_z : -sectorspacing_z;
        float move_z_backward = (teamcolor == Chipaction.TeamColor.Red) ? -sectorspacing_z : sectorspacing_z;

        if (teamcolor == TeamColor.Red)
        {
            CheckAndAddSector(current_x + move_x_left, current_z + move_z_forward, teamcolor, Directions.UpLeft, has_jumped, is_selected);
            CheckAndAddSector(current_x + move_x_right, current_z + move_z_forward, teamcolor, Directions.UpRight, has_jumped, is_selected);
        }

        if (teamcolor == TeamColor.Blue)
        {
            CheckAndAddSector(current_x + move_x_left, current_z + move_z_forward, teamcolor, Directions.UpRight, has_jumped, is_selected);
            CheckAndAddSector(current_x + move_x_right, current_z + move_z_forward, teamcolor, Directions.UpLeft, has_jumped, is_selected);
        }

        if (is_king)
        {
            if (teamcolor == TeamColor.Red)
            {
                CheckAndAddSector(current_x + move_x_left, current_z + move_z_backward, teamcolor, Directions.DownLeft, has_jumped, is_selected);
                CheckAndAddSector(current_x + move_x_right, current_z + move_z_backward, teamcolor, Directions.DownRight, has_jumped, is_selected);
            }

            if (teamcolor == TeamColor.Blue)
            {
                CheckAndAddSector(current_x + move_x_left, current_z + move_z_backward, teamcolor, Directions.DownRight, has_jumped, is_selected);
                CheckAndAddSector(current_x + move_x_right, current_z + move_z_backward, teamcolor, Directions.DownLeft, has_jumped, is_selected);
            }
        }

        if (teamcolor == TeamColor.Red)
        {
            CheckForJump(current_x, current_z, move_x_left, move_z_forward, is_king, teamcolor, Directions.UpLeft, has_jumped, is_selected);
            CheckForJump(current_x, current_z, move_x_right, move_z_forward, is_king, teamcolor, Directions.UpRight, has_jumped, is_selected);
        }

        if (teamcolor == TeamColor.Blue)
        {
            CheckForJump(current_x, current_z, move_x_left, move_z_forward, is_king, teamcolor, Directions.UpRight, has_jumped, is_selected);
            CheckForJump(current_x, current_z, move_x_right, move_z_forward, is_king, teamcolor, Directions.UpLeft, has_jumped, is_selected);
        }

        if (is_king)
        {
            if (teamcolor == TeamColor.Red)
            {
                CheckForJump(current_x, current_z, move_x_left, move_z_backward, is_king, teamcolor, Directions.DownLeft, has_jumped, is_selected);
                CheckForJump(current_x, current_z, move_x_right, move_z_backward, is_king, teamcolor, Directions.DownRight, has_jumped, is_selected);
            }

            if (teamcolor == TeamColor.Blue)
            {
                CheckForJump(current_x, current_z, move_x_left, move_z_backward, is_king, teamcolor, Directions.DownRight, has_jumped, is_selected);
                CheckForJump(current_x, current_z, move_x_right, move_z_backward, is_king, teamcolor, Directions.DownLeft, has_jumped, is_selected);
            }
        }

        if (jumpablesectors.Count > 0)
        {
            for (int i = 0; i < jumpablesectors.Count; i++)
            {
                float jump_x = jumpablesectors[i].transform.position.x;
                float jump_z = jumpablesectors[i].transform.position.z;

                if (jump_x > current_x && jump_z > current_z && teamcolor == TeamColor.Red)
                {
                    jumpablesectors_on_the_upright_path.Add(jumpablesectors[i]);
                    CheckAndAddSector(jump_x, jump_z, teamcolor, Directions.UpRight, has_jumped, is_selected);
                }

                else if (jump_x < current_x && jump_z > current_z && teamcolor == TeamColor.Red)
                {
                    jumpablesectors_on_the_upleft_path.Add(jumpablesectors[i]);
                    CheckAndAddSector(jump_x, jump_z, teamcolor, Directions.UpLeft, has_jumped, is_selected);
                }

                else if (jump_x < current_x && jump_z < current_z && teamcolor == TeamColor.Red)
                {
                    jumpablesectors_on_the_downleft_path.Add(jumpablesectors[i]);
                    CheckAndAddSector(jump_x, jump_z, teamcolor, Directions.DownLeft, has_jumped, is_selected);
                }

                else if (jump_x > current_x && jump_z < current_z && teamcolor == TeamColor.Red)
                {
                    jumpablesectors_on_the_downright_path.Add(jumpablesectors[i]);
                    CheckAndAddSector(jump_x, jump_z, teamcolor, Directions.DownRight, has_jumped, is_selected);
                }
            }
        }

        //determine return based on available moves and jumps
        if (availablesectors.Count > 0 && (jumpablesectors.Count == 0 &&
            jumpablesectors_on_the_upright_path.Count == 0 &&
            jumpablesectors_on_the_upleft_path.Count == 0 &&
            jumpablesectors_on_the_downleft_path.Count == 0 &&
            jumpablesectors_on_the_downright_path.Count == 0))
        {
            return CanDo.Move;
        }

        if (availablesectors.Count == 0 && (jumpablesectors.Count > 0 ||
            jumpablesectors_on_the_upright_path.Count > 0 ||
            jumpablesectors_on_the_upleft_path.Count > 0 ||
            jumpablesectors_on_the_downleft_path.Count > 0 ||
            jumpablesectors_on_the_downright_path.Count > 0))
        {
            return CanDo.Jump;
        }

        if (availablesectors.Count > 0 && (jumpablesectors.Count > 0 ||
            jumpablesectors_on_the_upright_path.Count > 0 ||
            jumpablesectors_on_the_upleft_path.Count > 0 ||
            jumpablesectors_on_the_downleft_path.Count > 0 ||
            jumpablesectors_on_the_downright_path.Count > 0))
        {
            return CanDo.Both;
        }

        return CanDo.None;
    }

    private void CheckAndAddSector(float target_x, float target_z, Chipaction.TeamColor teamcolor, Directions direction, bool has_jumped, bool is_selected)
    {
        if (target_x >= 0 && target_x < width * sectorspacing_x && target_z >= 0 && target_z < height * sectorspacing_z)
        {
            foreach (var sector in sectors)
            {
                float sector_x = sector.transform.position.x;
                float sector_z = sector.transform.position.z;

                if (Mathf.Abs(sector_x - target_x) < tolerance && Mathf.Abs(sector_z - target_z) < tolerance)
                {
                    SectorStatus occupied = IsOccupied(sector, teamcolor, direction);

                    if (occupied == SectorStatus.Empty)
                    {
                        if (has_jumped == false)
                        {
                            availablesectors.Add(sector);

                            //only highlight the available sector, if the chip is selected, otherwise the sectors will be
                            //colored for no reason when gamehandler is checking the status of each chip can they move
                            if (is_selected == true)
                            {
                                sector.GetComponent<MeshRenderer>().material.color = Color.green;
                            }
                        }
                    }

                    if ((occupied == SectorStatus.Red && teamcolor == Chipaction.TeamColor.Blue) ||
                        (occupied == SectorStatus.Blue && teamcolor == Chipaction.TeamColor.Red))
                    {
                        //check the sector behind the opponent
                        float behind_x = sector_x + (sector_x - target_x); // x behind the opponent
                        float behind_z = sector_z + (sector_z - target_z); // z behind the opponent

                        foreach (var sectorbehindopponent in sectors)
                        {
                            float sectorbehind_x = sectorbehindopponent.transform.position.x;
                            float sectorbehind_z = sectorbehindopponent.transform.position.z;

                            if (Mathf.Abs(sectorbehind_x - behind_x) < tolerance && Mathf.Abs(sectorbehind_z - behind_z) < tolerance)
                            {
                                occupied = IsOccupied(sectorbehindopponent, teamcolor, direction);

                                if (occupied == SectorStatus.Empty)
                                {
                                    availablesectors.Add(sectorbehindopponent);
                                    jumpablesectors.Add(sectorbehindopponent);

                                    //only highlight the available sector, if the chip is selected, otherwise the sectors will be
                                    //colored for no reason when gamehandler is checking the status of each chip can they move
                                    if (is_selected == true)
                                    {
                                        sectorbehindopponent.GetComponent<MeshRenderer>().material.color = Color.green;
                                    }

                                    if (teamcolor == Chipaction.TeamColor.Blue)
                                    {
                                        switch (direction)
                                        {
                                            case Directions.UpLeft:
                                                direction = Directions.UpRight;
                                                break;
                                            case Directions.UpRight:
                                                direction = Directions.UpLeft;
                                                break;
                                            case Directions.DownLeft:
                                                direction = Directions.DownRight;
                                                break;
                                            case Directions.DownRight:
                                                direction = Directions.DownLeft;
                                                break;
                                        }
                                    }

                                    switch (direction)
                                    {
                                        case Directions.UpLeft:
                                            jumpablesectors_on_the_upleft_path.Add(sectorbehindopponent);
                                            break;
                                        case Directions.UpRight:
                                            jumpablesectors_on_the_upright_path.Add(sectorbehindopponent);
                                            break;
                                        case Directions.DownLeft:
                                            jumpablesectors_on_the_downleft_path.Add(sectorbehindopponent);
                                            break;
                                        case Directions.DownRight:
                                            jumpablesectors_on_the_downright_path.Add(sectorbehindopponent);
                                            break;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    break;
                }
            }
        }
    }

    private void CheckForJump(float current_x, float current_z, float move_x, float move_z, bool is_king, Chipaction.TeamColor teamcolor, Directions direction, bool has_jumped, bool is_selected)
    {
        foreach (var sector in sectors)
        {
            float sector_x = sector.transform.position.x;
            float sector_z = sector.transform.position.z;

            if (Mathf.Abs(sector_x - (current_x + move_x)) < tolerance && Mathf.Abs(sector_z - (current_z + move_z)) < tolerance)
            {
                SectorStatus occupied = IsOccupied(sector, teamcolor, direction);

                if ((teamcolor == Chipaction.TeamColor.Red && occupied == SectorStatus.Blue) ||
                    (teamcolor == Chipaction.TeamColor.Blue && occupied == SectorStatus.Red))
                {
                    float behind_x = current_x + 2 * move_x;
                    float behind_z = current_z + 2 * move_z;

                    foreach (var sectorbehind in sectors)
                    {
                        float sectorbehind_x = sectorbehind.transform.position.x;
                        float sectorbehind_z = sectorbehind.transform.position.z;

                        //check if sector behind is free
                        if (Mathf.Abs(sectorbehind_x - behind_x) < tolerance && Mathf.Abs(sectorbehind_z - behind_z) < tolerance)
                        {
                            if (IsOccupied(sectorbehind, teamcolor, direction) == SectorStatus.Empty)
                            {
                                availablesectors.Add(sectorbehind);
                                jumpablesectors.Add(sectorbehind);

                                //only highlight the available sector, if the chip is selected, otherwise the sectors will be
                                //colored for no reason when gamehandler is checking the status of each chip can they move
                                if (is_selected == true)
                                {
                                    sectorbehind.GetComponent<MeshRenderer>().material.color = Color.green;
                                }

                                //add to direction-specific jumpable sector list
                                switch (direction)
                                {
                                    case Directions.UpLeft:
                                        jumpablesectors_on_the_upleft_path.Add(sectorbehind);
                                        break;
                                    case Directions.UpRight:
                                        jumpablesectors_on_the_upright_path.Add(sectorbehind);
                                        break;
                                    case Directions.DownLeft:
                                        jumpablesectors_on_the_downleft_path.Add(sectorbehind);
                                        break;
                                    case Directions.DownRight:
                                        jumpablesectors_on_the_downright_path.Add(sectorbehind);
                                        break;
                                }
                            }

                            break;
                        }
                    }
                }
            }
        }
    }

    private SectorStatus IsOccupied(Component sector, Chipaction.TeamColor teamcolor, Directions direction)
    {
        foreach (var chip in chips)
        {
            if (Mathf.Abs(sector.transform.position.x - chip.transform.position.x) < tolerance &&
                Mathf.Abs(sector.transform.position.z - chip.transform.position.z) < tolerance)
            {
                Chipaction chipAction = chip.GetComponent<Chipaction>();

                if (chipAction.GetTeamColor() == Chipaction.TeamColor.Red)
                {
                    if (teamcolor != Chipaction.TeamColor.Red)
                    {
                        //add to the corresponding opponent's list
                        switch (direction)
                        {
                            case Directions.UpLeft:
                                opponents_on_the_upleft_path.Add(chip);
                                break;
                            case Directions.UpRight:
                                opponents_on_the_upright_path.Add(chip);
                                break;
                            case Directions.DownLeft:
                                opponents_on_the_downleft_path.Add(chip);
                                break;
                            case Directions.DownRight:
                                opponents_on_the_downright_path.Add(chip);
                                break;
                        }
                    }

                    return SectorStatus.Red;
                }

                if (chipAction.GetTeamColor() == Chipaction.TeamColor.Blue)
                {
                    if (teamcolor != Chipaction.TeamColor.Blue)
                    {
                        switch (direction)
                        {
                            case Directions.UpLeft:
                                opponents_on_the_upleft_path.Add(chip);
                                break;
                            case Directions.UpRight:
                                opponents_on_the_upright_path.Add(chip);
                                break;
                            case Directions.DownLeft:
                                opponents_on_the_downleft_path.Add(chip);
                                break;
                            case Directions.DownRight:
                                opponents_on_the_downright_path.Add(chip);
                                break;
                        }
                    }

                    return SectorStatus.Blue;
                }
            }
        }

        return SectorStatus.Empty;
    }

    public void RemoveEatedOpponents(float current_x, float current_z)
    {
        //find the matching jumpable sector in the jumpablesectors list
        Component matchedSector = null;
        int i;

        for (i = 0; i < jumpablesectors.Count; i++)
        {
            if (Mathf.Abs(jumpablesectors[i].transform.position.x - current_x) < tolerance &&
                Mathf.Abs(jumpablesectors[i].transform.position.z - current_z) < tolerance)
            {
                matchedSector = jumpablesectors[i];
                break; //exit once the matching sector is found
            }
        }

        if (matchedSector == null)
        {
            return;
        }

        //determine which direction's jumpable sectors to check
        List<Component> target_opponents = null;
        List<Component> target_sectors = null;

        if (jumpablesectors_on_the_upright_path.Contains(matchedSector))
        {
            target_opponents = opponents_on_the_upright_path;
            target_sectors = jumpablesectors_on_the_upright_path;
        }
        else if (jumpablesectors_on_the_upleft_path.Contains(matchedSector))
        {
            target_opponents = opponents_on_the_upleft_path;
            target_sectors = jumpablesectors_on_the_upleft_path;
        }
        else if (jumpablesectors_on_the_downright_path.Contains(matchedSector))
        {
            target_opponents = opponents_on_the_downright_path;
            target_sectors = jumpablesectors_on_the_downright_path;
        }
        else if (jumpablesectors_on_the_downleft_path.Contains(matchedSector))
        {
            target_opponents = opponents_on_the_downleft_path;
            target_sectors = jumpablesectors_on_the_downleft_path;
        }
		
        else
        {
            return;
        }

        //remove the opponents from the matched direction's list
        for (int j = 0; j < target_opponents.Count; j++)
        {
            GameObject opponent_chip = target_opponents[j].gameObject;
            opponent_chip.SetActive(false);
            chips.Remove(target_opponents[j]);
        }

        target_opponents.Clear();
    }

    public void AddToSectors(Sector sector)
    {
        sectors.Add(sector);
    }

    public void AddToChips(Chipaction chip)
    {
        chips.Add(chip);
    }

    public void ResetSectors()
    {
        if (sectors.Count > 0)
        {
            foreach (var sector in sectors)
            {
                sector.GetComponent<Sector>().isclicked = false;
                sector.GetComponent<MeshRenderer>().material.color = Color.black;
            }
        }
    }

    public void CheckIsKing()
    {
        if (chips.Count > 0)
        {
            foreach (var chip in chips)
            {
                //if a chip reaches the opponents end 
                if ((chip.transform.position.z == (sectorspacing_z * (height - 1))) &&
                    chip.GetComponent<Chipaction>().GetTeamColor() == Chipaction.TeamColor.Red)
                {
                    chip.GetComponent<Chipaction>().MakeKing();
                }

                if ((chip.transform.position.z == 0) &&
                    chip.GetComponent<Chipaction>().GetTeamColor() == Chipaction.TeamColor.Blue)
                {
                    chip.GetComponent<Chipaction>().MakeKing();
                }
            }
        }
    }

    public void UpdateGameScore()
    {
        if (chips.Count > 0)
        {
            alive_reds = 0;
            alive_blues = 0;
            movable_reds = 0;
            movable_blues = 0;

            foreach (var chip in chips)
            {
                Chipaction chipAction = chip.GetComponent<Chipaction>();
                if (chipAction != null && chipAction.isActiveAndEnabled && chip.gameObject.activeSelf)
                {
                    //first check the ability to move to keep track of movable chips
                    IsAbleToMove(chipAction);

                    if (chipAction.GetTeamColor() == Chipaction.TeamColor.Red)
                    {
                        alive_reds++;

                        if (chipAction.can_move)
                        {
                            movable_reds++;
                        }
                    }
                    else if (chipAction.GetTeamColor() == Chipaction.TeamColor.Blue)
                    {
                        alive_blues++;

                        if (chipAction.can_move)
                        {
                            movable_blues++;
                        }
                    }
                }
            }
        }
    }

    public void CheckGame()
    {
        if (alive_blues == 0 || movable_blues == 0)
        {
            result = Result.Red;
            turn = Turn.None;
            gameover = true;
        }
        else if (alive_reds == 0 || movable_reds == 0)
        {
            result = Result.Blue;
            turn = Turn.None;
            gameover = true;
        }
        else if (movable_reds == 0 && movable_blues == 0)
        {
            result = Result.Tie;
            turn = Turn.None;
            gameover = true;
        }
    }

    public void EndTurn()
    {
        if (turn == Turn.Red)
        {
            turn = Turn.Blue;
        }

        else
        {
            turn = Turn.Red;
        }

        StartCoroutine(cameraHandler.AutoChange());
        CheckGame();
    }

    public List<Component> GetSectors()
    {
        return sectors;
    }

    public List<Component> GetAvailableSectors()
    {
        return availablesectors;
    }

    public List<Component> GetJumpableSectors()
    {
        return jumpablesectors;
    }

    public int GetAliveReds()
    {
        return alive_reds;
    }

    public int GetAliveBlues()
    {
        return alive_blues;
    }

    public Result GetResult()
    {
        return result;
    }

    public bool GameHasEnded()
    {
        return gameover;
    }

    //called in UpdateGameScore() to keep track of movable chips after every turn
    void IsAbleToMove(Chipaction chip)
    {
        CanDo possibility = CheckPossibilities(chip.GetCurrentXpos(), chip.GetCurrentZpos(), chip.IsKing(), chip.GetTeamColor(), chip.has_jumped, chip.is_selected);

        if (possibility == CanDo.None)
        {
            chip.can_move = false;
        }

        else
        {
            chip.can_move = true;
        }
    }

    void Awake()
    {
        initialturn = PlayerPrefs.GetInt("Startingside");

        if (initialturn == 1)
        {
            turn = Turn.Red;
        }

        else if (initialturn == 2)
        {
            turn = Turn.Blue;
        }
    }
    void Update()
    {
		//return to menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
}