using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public enum BattleState
{
    StartBattle,
    ActionSelection,
    MovementSelection,
    PerformMovement,
    Busy,
    PartySelectScreen,
    ItemSelectScreen,
    LoseTurn,
    FinishBattle
}

public enum ActionState
{
    Fight,
    Pokemon,
    Bag,
    Run
}

public enum BattleType
{
    WildPokemon,
    Trainer
}

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleUnit enemyUnit;

    [SerializeField] private BattleDialogBox battleDialogBox;
    [SerializeField] private PartyHUD partyHUD;

    [SerializeField] private GameObject pokeball;

    public BattleState state;
    public BattleType battleType;
    private int escapeAttempts;
    
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
        battleType = BattleType.WildPokemon;
        
        StartCoroutine(SetupBattle());
    }

    public void HandleStartTrainerBattle(PokemonParty playerParty, Pokemon trainerParty)
    {
        battleType = BattleType.Trainer;
        //TODO: Batalla NPC
    }

    private IEnumerator SetupBattle()
    {
        state = BattleState.StartBattle;

        escapeAttempts = 0;
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
        //TODO: Implementar inventario y logica de items
        battleDialogBox.ToggleActions(false);
        StartCoroutine(ThrowPokeball());
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
            case BattleState.LoseTurn:
                StartCoroutine(PerformEnemyMovement());
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
                    StartCoroutine(TryToEscapeFromBattle());
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
            yield return HandlePokemonFainted(target);
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

    IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (battleType != BattleType.WildPokemon)
        {
            yield return battleDialogBox.SetDialog($"No puedes robar los pokemons de otros entrenadores");
            state = BattleState.LoseTurn;
            yield break;
        }

        yield return battleDialogBox.SetDialog($"Has lanzado una {pokeball.name}!");
        var pokeballInst = Instantiate(pokeball, playerUnit.transform.position - new Vector3(2, 2), 
            Quaternion.identity);

        var pokeballSpt = pokeballInst.GetComponent<SpriteRenderer>();
        yield return pokeballSpt.transform
            .DOLocalJump(enemyUnit.transform.position + 
                         new Vector3(0, 2), 2, 1, 1f)
            .WaitForCompletion();

        yield return enemyUnit.PlayCapturedAnimation();
        yield return pokeballSpt.transform
            .DOLocalMoveY(enemyUnit.transform.position.y - 1.8f, 0.6f)
            .WaitForCompletion();

        var numberOfShakes = TryToCatchPokemon(enemyUnit.Pokemon);
        Debug.Log(numberOfShakes);
        for (int i = 0; i < Mathf.Min(numberOfShakes, 3); i++)
        {
            yield return new WaitForSeconds(1.2f);
            yield return pokeballSpt.transform.DOPunchRotation(new Vector3(0, 0, 15f), 0.6f);
        }

        if (numberOfShakes == 4)
        {
            yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.name} capturado!");
            yield return pokeballSpt.DOFade(0, 1f).WaitForCompletion();
            _pokemonParty.AddPokemonParty(enemyUnit.Pokemon);

            Destroy(pokeballInst);
            BattleFinish(true);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
            pokeballSpt.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (numberOfShakes < 2)
            {
                yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.name} ha escapado!");
            }
            else
            {
                yield return battleDialogBox.SetDialog($"{enemyUnit.Pokemon.Base.name} ha escapado por poco!");
            }
            
            Destroy(pokeballInst);
            state = BattleState.LoseTurn;
        }
        
        
    }
    
    int TryToCatchPokemon(Pokemon pokemon)
    {
        float bonusBall = 1f;
        float bonusStatus = 1f;
        float a = (3 * pokemon.MaxHP - 2 * pokemon.HP) * pokemon.Base.CatchRate * bonusBall * bonusStatus
            / (3 * pokemon.MaxHP);

        if (a >= 255)
        {
            return 4;
        }

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (Random.Range(0, 65535) >= b)
            {
                break;
            }

            shakeCount++;
        }

        return shakeCount;
    }

    IEnumerator TryToEscapeFromBattle()
    {
        state = BattleState.Busy;

        if (battleType != BattleType.WildPokemon)
        {
            yield return battleDialogBox.SetDialog("No puedes huir de combates contra entrenadores");
            state = BattleState.LoseTurn;
            yield break;
        }

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if (playerSpeed >= enemySpeed)
        {
            yield return battleDialogBox.SetDialog("Has escapado con exito!");
            yield return new WaitForSeconds(1f);
            OnBattleFinish(false);
        }
        else
        {
            int oddsScape = (Mathf.FloorToInt(playerSpeed * 128 / enemySpeed) + 30 * escapeAttempts) % 256;
            if (Random.Range(0, 256) < oddsScape)
            {
                yield return battleDialogBox.SetDialog("Has escapado con exito!");
                
            }
            else
            {
                yield return battleDialogBox.SetDialog("No puedes escapar");
                state = BattleState.LoseTurn;
            }
        }
    }

    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return battleDialogBox.SetDialog($"{faintedUnit.Pokemon.Base.Name} se ha debilitado");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(1.5f);

        if (!faintedUnit.IsPlayer)
        {
            //EXP++ y Checquear new level
            int expBase = faintedUnit.Pokemon.Base.ExpBase;
            int level = faintedUnit.Pokemon.Level;
            float multiplier = (battleType == BattleType.WildPokemon) ? 1f : 1.5f;
            int wonExp = Mathf.FloorToInt(expBase * level * multiplier / 7);
            playerUnit.Pokemon.Exp += wonExp;
            yield return battleDialogBox.SetDialog($"{playerUnit.Pokemon.Base.Name} ha ganado {wonExp} de experiencia");
            yield return new WaitForSeconds(0.5f);
        }
            
        CheckForBattleFinish(faintedUnit);
    }
    
}
