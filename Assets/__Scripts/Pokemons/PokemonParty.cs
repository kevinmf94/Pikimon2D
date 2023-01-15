using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty: MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;

    public List<Pokemon> Pokemons
    {
        get => pokemons;
    }
    
    private void Start()
    {
        foreach (var pokemon in pokemons)
        {
            pokemon.InitPokemon();
        }
    }

    public Pokemon GetFirstHealthyPokemon()
    {
        return pokemons.FirstOrDefault(x => x.HP > 0);
    }
}