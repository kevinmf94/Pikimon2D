using System;
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
    [SerializeField] private MoveType moveType;
    [SerializeField] private MoveStatEffect moveStatEffect;
    [SerializeField] private MoveTarget moveTarget;

    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int PP => pp;

    public MoveStatEffect Effects => moveStatEffect;
    public MoveTarget MoveTarget => moveTarget;

    public MoveType MoveType => moveType;

    /*
    PokemonType.Fire, PokemonType.Water, PokemonType.Grass, PokemonType.Ice, PokemonType.Electric, 
    PokemonType.Dragon, PokemonType.Dark, PokemonType.Psychic*/
    public bool IsEspecialMove => moveType == MoveType.Special;
}

public enum MoveType {
    Physical, Special, Stats
}

[Serializable]
public class MoveStatEffect
{
    [SerializeField] private List<StatBoosting> boostings;
    public List<StatBoosting> Boostings => boostings;
}

[Serializable]
public class StatBoosting
{
    public Stat stat;
    public int boost;
    public MoveTarget target;
}

public enum MoveTarget
{
    Self, Other
}