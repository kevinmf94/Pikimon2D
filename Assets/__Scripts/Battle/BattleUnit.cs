using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokemonBase _base;
    public int _level;
    public bool isPlayer;
    
    public Pokemon Pokemon { get; set; }

    public void SetupPokemon()
    {
        Pokemon = new Pokemon(_base, _level);
        GetComponent<Image>().sprite = isPlayer ? 
            Pokemon.PokemonBase.BackSprite : Pokemon.PokemonBase.FrontSprite;
    }
}
