using UnityEngine;

public class Chipaction : MonoBehaviour
{
    public enum TeamColor
    {
        Red,
        Blue
    }

    public enum CanDo
    {
        None,
        Jump,
        Move,
        Both
    }

    public bool is_selected = false;
    public bool can_move = true;
    public bool can_jump;
    public bool has_jumped = false;
    public Gamehandler gameHandler;
    private bool is_king = false;
    float current_x;
    float current_z;
    float current_y;
    private TeamColor teamcolor;
    private CanDo possible_action;
    private GameObject soldiermodel;
    private GameObject kingmodel;
    
    public void SelectChip()
    {
        is_selected = true;
        gameHandler.ResetSectors();
        possible_action = gameHandler.CheckPossibilities(current_x, current_z, is_king, teamcolor, has_jumped);

        if (possible_action == CanDo.None)
        {
            can_move = false;
            can_jump = false;
        }

        if (possible_action == CanDo.Jump)
        {
            can_jump = true;
        }

        if (possible_action == CanDo.Move)
        {
            can_move = true;
        }
    }

    public void DeselectChip()
    {
        is_selected = false;
        gameHandler.ResetSectors();
        
        /*check if there is a jump going on, then if is then by deselecting the chip, player
          can decide to not perform a double or triple or quadriple jump*/
        if (has_jumped == true)
        {
            can_jump = false;
            has_jumped = false;
            UpdateCurrentPositions();
            gameHandler.CheckIsKing();
            gameHandler.UpdateGameScore();
            gameHandler.EndTurn();
        }
    }

    public void AssignTeam(TeamColor team)
    {
        teamcolor = team;
    }

    public void MakeKing()
    {
        is_king = true;
        kingmodel.SetActive(true);
        soldiermodel.SetActive(false);
    }

    public TeamColor GetTeamColor()
    {
        return teamcolor;
    }

    public void UpdateCurrentPositions()
    {
        current_x = transform.position.x;
        current_z = transform.position.z;
        current_y = transform.position.y;
    }

    void Awake()
    {
        //on initialization, set the soldier model active first
        soldiermodel = transform.GetChild(1).gameObject;
        kingmodel = transform.GetChild(0).gameObject;

        soldiermodel.SetActive(true);
        kingmodel.SetActive(false);
    }

    void Start()
    {
        UpdateCurrentPositions();
    }

    void Update()
    {
        if (is_selected && gameHandler.GetAvailableSectors().Count > 0 && (possible_action == CanDo.Both || possible_action == CanDo.Move || possible_action == CanDo.Jump))
        {
            bool sector_found = false;

            for (int i = 0; i < gameHandler.GetAvailableSectors().Count; i++)
            {
                for (int j = 0; j < gameHandler.GetSectors().Count; j++)
                {
                    Sector sector = gameHandler.GetSectors()[j].GetComponent<Sector>();

                    if (sector.isclicked &&
                       Mathf.Abs(sector.transform.position.x - gameHandler.GetAvailableSectors()[i].transform.position.x) < gameHandler.tolerance &&
                       Mathf.Abs(sector.transform.position.z - gameHandler.GetAvailableSectors()[i].transform.position.z) < gameHandler.tolerance)
                    {
                        //move the chip to the selected sector
                        Vector3 target_position = new Vector3(gameHandler.GetAvailableSectors()[i].transform.position.x, current_y, gameHandler.GetAvailableSectors()[i].transform.position.z);
                        
                        if (transform.position != target_position)
                        {
                            transform.position = target_position;
                            sector_found = true;
                            has_jumped = false;
                            UpdateCurrentPositions();
                            gameHandler.ResetSectors();
                        }
                        
                        for (int k = 0; k < gameHandler.GetJumpableSectors().Count; k++)
                        {
                            if (Mathf.Abs(gameHandler.GetJumpableSectors()[k].transform.position.x - sector.transform.position.x) < gameHandler.tolerance &&
                                Mathf.Abs(gameHandler.GetJumpableSectors()[k].transform.position.z - sector.transform.position.z) < gameHandler.tolerance)
                            {
                                has_jumped = true;
                            }
                        }

                        if (has_jumped == true)
                        {
                            gameHandler.RemoveEatedOpponents(current_x, current_z);
                            gameHandler.ResetSectors();

                            //check for additional jumps
                            possible_action = gameHandler.CheckPossibilities(current_x, current_z, is_king, teamcolor, has_jumped);

                            if (possible_action == CanDo.Jump || possible_action == CanDo.Both)
                            {
                                can_jump = true;
                                break;
                            }
                        }

                        possible_action = CanDo.None;
                    }
                }
            }

            if (sector_found == true && (possible_action == CanDo.Move || possible_action == CanDo.None))
            {
                sector_found = false;
                can_jump = false;
                has_jumped = false;
                UpdateCurrentPositions();
                gameHandler.ResetSectors();
                gameHandler.CheckIsKing();
                gameHandler.UpdateGameScore();
                gameHandler.EndTurn();
                DeselectChip();
            }
        }
    }
}