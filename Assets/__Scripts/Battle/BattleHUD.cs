using UnityEngine;
using UnityEngine.UI;

public class BattleHUD : MonoBehaviour
{
    [SerializeField]
    private Text pokemonName;
    
    [SerializeField]
    private Text pokemonLvl;

    [SerializeField]
    private HealthBar healthBar;

    [SerializeField] private Text pokemonHealth;

    public void SetPokemonData(Pokemon pokemon)
    {
        pokemonName.text = pokemon.PokemonBase.Name;
        pokemonLvl.text = $"Lvl {pokemon.Level}";
        healthBar.SetHP(pokemon.Hp / pokemon.MaxHP);
        pokemonHealth.text = $"{pokemon.Hp} / {pokemon.MaxHP}";
    }
}
