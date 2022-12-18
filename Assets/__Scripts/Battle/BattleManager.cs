using System;
using System.Collections;
using UnityEngine;

public enum BattleState
{
    StartBattle,
    PlayerSelectAction,
    PlayerMove,
    EnemyMove,
    Busy
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;
    
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox battleDialogBox;

    public BattleState state;
    private int currentSelectedAction;
    private int currentSelectedMovement;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;
        playerUnit.SetupPokemon();
        playerHUD.SetPokemonData(playerUnit.Pokemon);
        battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
        
        enemyUnit.SetupPokemon();
        enemyHUD.SetPokemonData(enemyUnit.Pokemon);
        
        yield return battleDialogBox.SetDialog($"Un {enemyUnit._base.Name} salvaje a aparecido!");
        yield return new WaitForSeconds(1.0f);

        PlayerAction();
    }

    private void PlayerAction()
    {
        state = BattleState.PlayerSelectAction;
        StartCoroutine(battleDialogBox.SetDialog("Selecciona una acción"));
        battleDialogBox.ToggleActions(true);
        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleMovements(false);
        currentSelectedAction = 0;
        battleDialogBox.SelectAction(currentSelectedAction);
    }

    void PlayerMovement()
    {
        state = BattleState.PlayerMove;
        battleDialogBox.ToggleDialogText(false);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(true);
        currentSelectedMovement = 0;
        battleDialogBox.SelectMovement(currentSelectedMovement, 
            playerUnit.Pokemon.Moves[currentSelectedMovement]);
    }

    private void EnemyAction()
    {
        
    }

    private void Update()
    {
        if (state == BattleState.PlayerSelectAction)
        {
            HandlePlayerActionSelection();
        } else if (state == BattleState.PlayerMove)
        {
            HandlePlayerMovementSelection();
        }
    }
    private void HandlePlayerActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelectedAction = (++currentSelectedAction) % 2;
            battleDialogBox.SelectAction(currentSelectedAction);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentSelectedAction == 0)
            {
                PlayerMovement();
            }
            else if (currentSelectedAction == 1)
            {
                //todo: Huir
            }
        }
    }
    
    private void HandlePlayerMovementSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelectedMovement = (currentSelectedMovement + 2) % 4;
        } else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSelectedMovement =
                currentSelectedMovement % 2 == 0 ? currentSelectedMovement + 1 : currentSelectedMovement - 1;
        }

        if (Input.anyKeyDown)
        {
            currentSelectedMovement %= playerUnit.Pokemon.Moves.Count;
            battleDialogBox.SelectMovement(currentSelectedMovement,
                playerUnit.Pokemon.Moves[currentSelectedMovement]);
        }
    }
}
