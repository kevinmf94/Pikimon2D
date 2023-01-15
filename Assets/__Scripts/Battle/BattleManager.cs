using System;
using System.Collections;
using UnityEngine;

public enum BattleState
{
    StartBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    FinishBattle
}

public enum ActionState
{
    Fight,
    Pokemon,
    Bag,
    Run
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogBox battleDialogBox;
    [SerializeField] private PartyHUD partyHUD;

    public BattleState state;
    private int currentSelectedAction;
    private int currentSelectedMovement;
    private int currentSelectedPokemon;

    public event Action<bool> OnBattleFinish;

    private PokemonParty _pokemonParty;
    private Pokemon _wildPokemon;
    
    // Start is called before the first frame update
    public void HandleStartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        _pokemonParty = playerParty;
        _wildPokemon = wildPokemon;
        
        StartCoroutine(SetupBattle());
    }

    private IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;
        
        playerUnit.SetupPokemon(_pokemonParty.GetFirstHealthyPokemon());
        battleDialogBox.SetPokemonMovements(playerUnit.Pokemon.Moves);
        
        enemyUnit.SetupPokemon(_wildPokemon);
        
        partyHUD.InitPartyHUD();
        
        yield return battleDialogBox.SetDialog($"Un {enemyUnit._base.Name} salvaje a aparecido!");

        if (enemyUnit.Pokemon.Speed > playerUnit.Pokemon.Speed)
        {
            battleDialogBox.ToggleActions(false);
            battleDialogBox.ToggleDialogText(true);
            battleDialogBox.ToggleMovements(false);
            yield return battleDialogBox.SetDialog("El enemigo ataca primero!");
            yield return PerformEnemyMovement();
        }
        else
        {
            PlayerActionSelection();
        }
    }

    void BattleFinish(bool playerHasWon)
    {
        state = BattleState.FinishBattle;
        OnBattleFinish(playerHasWon);
    }

    private void PlayerActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(battleDialogBox.SetDialog("Selecciona una acción"));
        battleDialogBox.ToggleActions(true);
        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleMovements(false);
        currentSelectedAction = 0;
        battleDialogBox.SelectAction(currentSelectedAction);
    }

    void PlayerMovementSelection()
    {
        state = BattleState.MovementSelection;
        battleDialogBox.ToggleDialogText(false);
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleMovements(true);
        currentSelectedMovement = 0;
        battleDialogBox.SelectMovement(currentSelectedMovement, 
            playerUnit.Pokemon.Moves[currentSelectedMovement]);
    }
    
    void OpenPartySelectionScreen()
    {
        state = BattleState.PartySelectScreen;
        partyHUD.SetPartyData(_pokemonParty.Pokemons);
        partyHUD.gameObject.SetActive(true);

        currentSelectedPokemon = 0;
        partyHUD.UpdateSelectedPokemon(currentSelectedPokemon);
    }
    
    void OpenInventoryScreen()
    {
        
    }

    public void HandleUpdate()
    {
        switch (state)
        {
            case BattleState.ActionSelection:
                HandlePlayerActionSelection();
                break;
            case BattleState.MovementSelection:
                HandlePlayerMovementSelection();
                break;
            case BattleState.PartySelectScreen:
                HandlePartyScreen();
                break;
        }
    }
    private void HandlePlayerActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentSelectedAction > 0)
        {
            currentSelectedAction = (--currentSelectedAction) % 4;
            battleDialogBox.SelectAction(currentSelectedAction);
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            currentSelectedAction = (++currentSelectedAction) % 4;
            battleDialogBox.SelectAction(currentSelectedAction);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            switch (currentSelectedAction)
            {
                case (int) ActionState.Fight:
                    PlayerMovementSelection();
                    break;
                case (int) ActionState.Pokemon:
                    OpenPartySelectionScreen();
                    break;
                case (int) ActionState.Bag:
                    OpenInventoryScreen();
                    break;
                case (int) ActionState.Run:
                    //todo: Huir
                    break;
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

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || 
            Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSelectedMovement %= playerUnit.Pokemon.Moves.Count;
            battleDialogBox.SelectMovement(currentSelectedMovement,
                playerUnit.Pokemon.Moves[currentSelectedMovement]);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            battleDialogBox.ToggleMovements(false);
            battleDialogBox.ToggleDialogText(true);
            StartCoroutine(PerformPlayerMovement());
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerActionSelection();
        }
    }
    
    void HandlePartyScreen()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentSelectedPokemon = (currentSelectedPokemon + 2) % _pokemonParty.Pokemons.Count;
        } else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSelectedPokemon =
                currentSelectedPokemon % 2 == 0 ? currentSelectedPokemon + 1 : currentSelectedPokemon - 1;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || 
            Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentSelectedPokemon %= _pokemonParty.Pokemons.Count;
            partyHUD.UpdateSelectedPokemon(currentSelectedPokemon);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            var selectedPokemon = _pokemonParty.Pokemons[currentSelectedPokemon];
            if (selectedPokemon.HP <= 0)
            {
                partyHUD.SetMessage("No puedes enviar un pokemon debilitado");
                return;
            } else if (selectedPokemon == playerUnit.Pokemon)
            {
                partyHUD.SetMessage("No puedes seleccionar en batalla");
                return;
            }

            partyHUD.gameObject.SetActive(false);
            state = BattleState.Busy;
            StartCoroutine(SwitchPokemon(selectedPokemon));
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            partyHUD.gameObject.SetActive(false);
            PlayerActionSelection();
        }
    }
    
    private IEnumerator PerformEnemyMovement()
    {
        state = BattleState.PerformMovement;
        battleDialogBox.ToggleActions(false);
        battleDialogBox.ToggleDialogText(true);
        battleDialogBox.ToggleMovements(false);
        
        Move move = enemyUnit.Pokemon.RandomMove();
        yield return RunMovement(enemyUnit, playerUnit, move);

        if (state == BattleState.PerformMovement)
        {
            PlayerActionSelection();    
        }
    }

    IEnumerator PerformPlayerMovement()
    {
        state = BattleState.PerformMovement;
        Move move = playerUnit.Pokemon.Moves[currentSelectedMovement];
        
        if (move.Pp <= 0)
        {
            PlayerMovementSelection();
            yield break;
        }
        
        yield return RunMovement(playerUnit, enemyUnit, move);

        if (state == BattleState.PerformMovement)
        {
            StartCoroutine(PerformEnemyMovement());   
        }
    }

    IEnumerator RunMovement(BattleUnit attacker, BattleUnit target, Move move)
    {
        move.Pp--;
        yield return battleDialogBox.SetDialog($"{attacker.Pokemon.Base.Name} " +
                                               $"ha usado {move.Base.Name}!");
        
        var oldHpValue = target.Pokemon.HP;
        
        attacker.PlayAttackAnimation();
        yield return new WaitForSeconds(1.0f);
        target.PlayReceiveAttackAnimation();
        
        DamageDescription damageDesc = target.Pokemon.ReceiveDamage(attacker.Pokemon, move);
        
        StartCoroutine(target.Hud.UpdatePokemonData(oldHpValue));
        yield return ShowDamageDescription(damageDesc);
        
        if (damageDesc.Fainted)
        {
            yield return battleDialogBox.SetDialog($"{target.Pokemon.Base.Name} se ha debilitado");
            target.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
            
            CheckForBattleFinish(target);
        }
    }

    void CheckForBattleFinish(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayer)
        {
            var nextPokemon = _pokemonParty.GetFirstHealthyPokemon();
            if (nextPokemon != null) 
            {
                OpenPartySelectionScreen();
            }
            else
            {
                BattleFinish(false);
            }
        }
        else
        {
            BattleFinish(true);
        }
    }

    IEnumerator ShowDamageDescription(DamageDescription desc)
    {
        if (desc.Critical > 1f)
        {
            yield return battleDialogBox.SetDialog("¡Un golpe crítico!");
        }

        if (desc.Type > 1)
        {
            yield return battleDialogBox.SetDialog("¡Es super efectivo!");
        } else if (desc.Type < 1)
        {
            yield return battleDialogBox.SetDialog("No es muy efectivo...");
        }
    }

    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0){
            yield return battleDialogBox.SetDialog($"Vuelve {playerUnit.Pokemon.Base.name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(1.5f);
        }
        
        playerUnit.SetupPokemon(newPokemon);
        battleDialogBox.SetPokemonMovements(newPokemon.Moves);
        
        yield return battleDialogBox.SetDialog($"Te elijo a ti {newPokemon.Base.name}");
        StartCoroutine(PerformEnemyMovement());
    }
    
}
