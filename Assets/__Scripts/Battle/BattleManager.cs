using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private BattleUnit playerUnit;
    [SerializeField] private BattleHUD playerHUD;
    
    [SerializeField] private BattleUnit enemyUnit;
    [SerializeField] private BattleHUD enemyHUD;

    [SerializeField] private BattleDialogBox battleDialogBox;

    // Start is called before the first frame update
    void Start()
    {
        SetupBattle();
    }

    private void SetupBattle()
    {
        playerUnit.SetupPokemon();
        playerHUD.SetPokemonData(playerUnit.Pokemon);
        
        enemyUnit.SetupPokemon();
        enemyHUD.SetPokemonData(enemyUnit.Pokemon);
        
        StartCoroutine(battleDialogBox.SetDialog($"Un {enemyUnit._base.Name} salvaje a aparecido!"));
    }
}
