using System.Collections.Generic;
using UnityEngine;

public class PokemonMapArea : MonoBehaviour
{
    [SerializeField] private List<Pokemon> wildPokemons;

    public Pokemon GetRandomWildPokemon()
    {
        Pokemon pokemon = wildPokemons[Random.Range(0, wildPokemons.Count)];
        pokemon.InitPokemon();
        return pokemon;
    }
    
}
