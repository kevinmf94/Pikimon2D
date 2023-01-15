using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyMemberHUD : MonoBehaviour
{
    public Text nameText, lvlText, hpText;
    public HealthBar healthBar;
    public Image image;

    private Pokemon _pokemon;
    [SerializeField] Color selectedColor = Color.blue;


    public void SetPokemonData(Pokemon pokemon)
    {
        _pokemon = pokemon;
        nameText.text = pokemon.Base.Name;
        lvlText.text = $"Lv {pokemon.Level}";
        hpText.text = $"{pokemon.HP} / {pokemon.MaxHP}";
        healthBar.SetHP(pokemon.HP/ (float) pokemon.MaxHP);
        image.sprite = pokemon.Base.FrontSprite;
    }
    
    public void SetSelectedPokemon(bool selected)
    {
        if (selected)
        {
            nameText.color = selectedColor;
        }
        else
        {
            nameText.color = Color.black;
        }
    }
}
