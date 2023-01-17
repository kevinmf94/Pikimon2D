using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty: MonoBehaviour
{
    [SerializeField] private List<Pokemon> pokemons;
    public const int NUM_MAX_POKEMON_IN_PARTY = 6;
    
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

    public void AddPokemonParty(Pokemon pokemon)
    {
        if (pokemons.Count < NUM_MAX_POKEMON_IN_PARTY)
        {
            pokemons.Add(pokemon);
        }
        else
        {
            //TODO: Añadir la funcionalidad de enviar al PC de Bill
        }
    }
}