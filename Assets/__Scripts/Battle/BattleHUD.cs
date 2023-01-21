using System.Collections;
using DG.Tweening;
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
    [SerializeField] private GameObject expBar;

    private Pokemon _pokemon;

    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        
        pokemonName.text = pokemon.Base.Name;
        healthBar.SetHP(_pokemon.HP / (float) _pokemon.MaxHP);
        SetLevelText();
        SetExp();
        StartCoroutine(UpdatePokemonData(pokemon.HP));
    }

    public IEnumerator UpdatePokemonData(int oldHPVal)
    {
        StartCoroutine(healthBar.SetSmoothHP(_pokemon.HP / (float) _pokemon.MaxHP));
        StartCoroutine(DecreaseHealthPoints(oldHPVal));
        yield return null;
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

    public void SetExp()
    {
        if (expBar == null)
        {
            return;
        }

        expBar.transform.localScale = new Vector3(NormalizedExp(), 1f, 1f);
    }

    public IEnumerator SetSmoothExp(bool needsToReset = false)
    {
        if (expBar == null)
        {
            yield break;
        }

        if (needsToReset)
        {
            expBar.transform.localScale = new Vector3(0f, 1f, 1f);
        }
        
        yield return expBar.transform.DOScaleX(NormalizedExp(), 1f).WaitForCompletion();
    }

    float NormalizedExp()
    {
        float currentLevelExp = _pokemon.Base.GetNeededExperiencieForLevel(_pokemon.Level);
        float nextLevelExp = _pokemon.Base.GetNeededExperiencieForLevel(_pokemon.Level + 1);
        float normalizedExp = (_pokemon.Exp - currentLevelExp) / (nextLevelExp - currentLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }

    public void SetLevelText()
    {
        pokemonLvl.text = $"Lvl {_pokemon.Level}";
    }
}
