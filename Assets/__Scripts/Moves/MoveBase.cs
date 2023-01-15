using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Nuevo movimiento")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;
    [TextArea] [SerializeField] private string description;

    public string Name => name;
    public string Description => description;

    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;

    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;

    public static List<PokemonType> specials = new()
    {
        PokemonType.Fire, PokemonType.Water, PokemonType.Grass, PokemonType.Ice, PokemonType.Electric, 
        PokemonType.Dragon, PokemonType.Dark, PokemonType.Psychic
    };
    public bool IsEspecialMove
    {
        get => specials.Contains(type);
    }
}
