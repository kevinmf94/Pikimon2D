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

    [SerializeField] private int catchRate = 255;

    //Stats
    [SerializeField] private int maxHP;
    [SerializeField] private int attack;
    [SerializeField] private int defense;
    [SerializeField] private int spAttack;
    [SerializeField] private int spDefense;
    [SerializeField] private int speed;
    [SerializeField] private int expBase;
    [SerializeField] private GrowthRate growthRate;

    public int MaxHp => maxHP;
    public int Attack => attack;
    public int Defense => defense;
    public int SpAttack => spAttack;
    public int SpDefense => spDefense;
    public int Speed => speed;
    public int CatchRate => catchRate;
    public int ExpBase => expBase;
    public GrowthRate GrowthRate => growthRate;

    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;

    [SerializeField] private List<LearnableMove> _learnableMoves;
    public List<LearnableMove> LearnableMoves => _learnableMoves;
    public static int NUMBER_OF_LERNEABLE_MOVES { get; }= 4;

    public int GetNeededExperiencieForLevel(int level)
    {
        switch (growthRate)
        {
            case GrowthRate.Fast:
                return Mathf.FloorToInt(4 * Mathf.Pow(level, 3) / 5);
            case GrowthRate.MediumFast:
                return Mathf.FloorToInt(Mathf.Pow(level, 3));
            case GrowthRate.MediumSlow:
                return Mathf.FloorToInt(6 * Mathf.Pow(level, 3) / 5 + 15 * 
                    Mathf.Pow(level, 2) + 100 * level - 140);
            case GrowthRate.Slow:
                return Mathf.FloorToInt(5 * Mathf.Pow(level, 3) / 4);
            case GrowthRate.Erratic:
                if (level < 50)
                { 
                    return Mathf.FloorToInt( Mathf.Pow(level, 3) * (100 - level) / 50);
                } else if (level < 68)
                {
                    return Mathf.FloorToInt( Mathf.Pow(level, 3) * (150 - level) / 100);
                } else if (level < 98)
                {
                    return Mathf.FloorToInt( Mathf.Pow(level, 3) * (1911 - 10*level/3) / 500);
                }
                else
                {
                    return Mathf.FloorToInt( Mathf.Pow(level, 3) * (160 - level) / 100);
                }
            case GrowthRate.Fluctuacting:
                return -1;
         }
        
        return -1;
    }
    
    
}

public enum GrowthRate
{
    Erratic, Fast, MediumFast, MediumSlow, Slow, Fluctuacting
} 

public enum PokemonType
{
    None,
    Normal,
    Fire,
    Water,
    Electric,
    Grass,
    Ice,
    Fight,
    Poison,
    Ground,
    Fly,
    Psychic,
    Bug,
    Rock,
    Ghost,
    Dragon,
    Dark,
    Steel,
    Fairy
}

public enum Stat
{
    Attack, Defense, SpAttack, SpDefense, Speed
}

public class TypeMatrix
{
    private static float S = 1f; //Standard
    private static float D = 2f; //Double
    private static float M = 0.5f; //Mid
    private static float N = 0; // None

    private static float[][] _matrix =
    {
        /*                   NOR FIR WAT ELE GRA ICE FIG POI GRO FLY PSY BUG ROC GHO DRA DAR STE FAI*/
        /*NOR*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, M, N, S, S, M, S},
        /*FIR*/ new float[] {S, M, M, S, D, D, S, S, S, S, S, D, M, S, M, S, D, S},
        /*WAT*/ new float[] {S, D, M, S, M, S, S, S, D, S, S, S, D, S, M, S, S, S},
        /*ELE*/ new float[] {S, S, D, M, M, S, S, S, N, D, S, S, S, S, M, S, S, S},
        /*GRA*/ new float[] {S, M, D, S, M, S, S, M, D, M, S, M, D, S, M, S, M, S},
        /*ICE*/ new float[] {S, M, M, S, D, M, S, S, D, D, S, S, S, S, D, S, M, S},
        //TODO: Matrix...
        /*FIG*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*POI*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*GRO*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*FLY*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*PSY*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*BUG*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*ROC*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*GHO*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*DRA*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*DAR*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*STE*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
        /*FAI*/ new float[] {S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S, S},
    };

    public static float GetMultEffectivness(PokemonType attacker, PokemonType defender)
    {
        if (attacker == PokemonType.None || defender == PokemonType.None)
        {
            return S;
        }
        
        return _matrix[(int) attacker - 1][(int) defender - 1];
    }
}

[Serializable]
public class LearnableMove
{
    [SerializeField] private MoveBase _move;
    [SerializeField] private int level;

    public MoveBase Move => _move;
    public int Level => level;
}
