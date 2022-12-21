using System.Collections;
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

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        
        pokemonName.text = pokemon.Base.Name;
        pokemonLvl.text = $"Lvl {pokemon.Level}";
        UpdatePokemonData(pokemon.HP);
    }

    public void UpdatePokemonData(int oldHPVal)
    {
        StartCoroutine(healthBar.SetSmoothHP(_pokemon.HP / (float) _pokemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPVal));
    }

    private IEnumerator DecreaseHealthPoints(int oldHpVal)
    {
        while (oldHpVal > _pokemon.HP)
        {
            oldHpVal--;
            pokemonHealth.text = $"{oldHpVal} / {_pokemon.MaxHP}";
            yield return new WaitForSeconds(0.1f);
        }
           
        pokemonHealth.text = $"{_pokemon.HP} / {_pokemon.MaxHP}";
    }
}
