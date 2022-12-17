using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Nuevo pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] private int ID;
    
    [SerializeField] private string name;
    [TextArea] [SerializeField] private string description;
    
    public string Name => name;
    public string Description => description;

    [SerializeField] private Sprite frontSprite;
    [SerializeField] private Sprite backSprite;

    [SerializeField] private PokemonType type1;
    [SerializeField] private PokemonType type2;
    
    public PokemonType Type1 => type1;
    public PokemonType Type2 => type2;

    //Stats
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;

    public int MaxHp => maxHP;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;

    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;

    [SerializeField] private List<LearnableMove> _learnableMoves;
    public List<LearnableMove> LearnableMoves => _learnableMoves;
}

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Fight,
    Ice,
    Poison,
    Ground,
    Fly,
    Psychic,
    Rock,
    Bug,
    Ghost,
    Dragon,
    Dark,
    Fairy,
    Steel
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase _move;
    [SerializeField] private int level;

    public MoveBase Move => _move;
    public int Level => level;
}
