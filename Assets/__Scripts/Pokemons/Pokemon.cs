using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private int _level;
    private List<Move> _moves;
    private int _hp; // Current life
    private int _exp;
    
    public Dictionary<Stat, int> Stats { get; private set; }
    public Dictionary<Stat, int> StatsBoosters { get; private set; }

    public Pokemon(PokemonBase pBase, int pLevel)
    {
        _base = pBase;
        _level = pLevel;
        InitPokemon();
    }

    public void InitPokemon()
    {
        _moves = new List<Move>();
        _hp = MaxHP;
        _exp = _base.GetNeededExperiencieForLevel(_level);

        foreach (var move in _base.LearnableMoves)
        {
            if (move.Level <= _level)
            {
                _moves.Add(new Move(move.Move));
            }

            if (_moves.Count >= PokemonBase.NUMBER_OF_LERNEABLE_MOVES)
            {
                break;
            }
        }
        
        CalculcateStats();
        ResetModifiers();
    }

    public void CalculcateStats()
    {
        Stats = new Dictionary<Stat, int>();
        Stats.Add(Stat.Attack, Mathf.FloorToInt(_base.Attack * _level / 100.0f) + 1);
        Stats.Add(Stat.Defense, Mathf.FloorToInt(_base.Defense * _level / 100.0f) + 1);
        Stats.Add(Stat.SpAttack, Mathf.FloorToInt(_base.SpAttack * _level / 100.0f) + 1);
        Stats.Add(Stat.SpDefense, Mathf.FloorToInt(_base.SpDefense * _level / 100.0f) + 1);
        Stats.Add(Stat.Speed, Mathf.FloorToInt(_base.Speed * _level / 100.0f) + 1);
    }

    public void ResetModifiers()
    {
        StatsBoosters = new Dictionary<Stat, int>()
        {
            {Stat.Attack, 0}, {Stat.Defense, 0}, {Stat.SpAttack, 0}, {Stat.SpDefense, 0}, {Stat.Speed, 0}
        };
    }

    public int GetStat(Stat stat)
    {
        int statValue = Stats[stat];
        int boost = StatsBoosters[stat];
        float multiplier = 1.0f + Mathf.Abs(boost) / 2.0f;
        return boost >= 0 ? Mathf.FloorToInt(statValue * multiplier) : 
                            Mathf.FloorToInt(statValue / multiplier);
    }

    public void ApplyBoosts(StatBoosting boost)
    {
        Stats[boost.stat] = Mathf.Clamp(Stats[boost.stat] + boost.boost, -6, 6);
        Debug.Log($"{boost.stat} se ha modificado a {Stats[boost.stat]}");
    }

    // Getters and setters
    public List<Move> Moves => _moves;
    public PokemonBase Base => _base;

    public int Level
    {
        get => _level;
        set => _level = value;
    }

    public int HP
    {
        get => _hp;
        set
        {
            _hp = value;
            _hp = Mathf.Min(value, MaxHP);
        }
    }

    public int Exp { get => _exp; set => _exp = value; }

    public int MaxHP => Mathf.FloorToInt(_base.MaxHp * _level / 20.0f) + 10;
    public int Attack => GetStat(Stat.Attack);
    public int Defense => GetStat(Stat.Defense);
    public int SpAttack => GetStat(Stat.SpAttack);
    public int SpDefense => GetStat(Stat.SpDefense);
    public int Speed => GetStat(Stat.Speed);

    public DamageDescription ReceiveDamage(Pokemon attacker, Move move)
    {
        // Critical modifier
        float critical = Random.Range(0f, 100f) < 8f ? 2f : 1f;
        
        // Type modifier
        float type1 = TypeMatrix.GetMultEffectivness(move.Base.Type, Base.Type1);
        float type2 = TypeMatrix.GetMultEffectivness(move.Base.Type, Base.Type2);

        // Damage calculator
        var damageDesc = new DamageDescription()
        {
            Critical = critical,
            Type = type1 * type2,
            Fainted = false
        };

        float attack = move.Base.IsEspecialMove ? attacker.SpAttack : attacker.Attack;
        float defense = move.Base.IsEspecialMove ? SpDefense : Defense;
        
        float modifiers = Random.Range(0.85f, 1.0f) * type1 * type2 * critical;
        float baseDamage = (2 * attacker.Level / 5f + 2) * move.Base.Power * (attack / defense) / 50f + 2;

        int totalDamage = Mathf.FloorToInt(baseDamage * modifiers);

        HP -= totalDamage;
        if (HP <= 0)
        {
            HP = 0;
            damageDesc.Fainted = true;
        }
        
        return damageDesc;
    }

    public Move RandomMove()
    {
        var movesWithPP = Moves.Where(m => m.Pp > 0).ToList();
        if (movesWithPP.Count > 0){
            int randId = Random.Range(0, movesWithPP.Count);
            return movesWithPP[randId];
        }

        return null;
    }

    public bool NeedsToLevelUp()
    {
        if (Exp > Base.GetNeededExperiencieForLevel(_level + 1))
        {
            int currentMaxHP = MaxHP;
            _level++;
            HP += MaxHP - currentMaxHP;
            return true;
        }

        return false;
    }

    public LearnableMove GetLearneableMoveAtCurrentLevel()
    {
        return Base.LearnableMoves.Where(item => item.Level == _level).FirstOrDefault(null);
    }

    public void LearnMove(LearnableMove learnableMove)
    {
        if (Moves.Count >= PokemonBase.NUMBER_OF_LERNEABLE_MOVES)
        {
            return;
        }

        Moves.Add(new Move(learnableMove.Move));
    }

    public void OnBattleFinish()
    {
        ResetModifiers();
    }
}

public struct DamageDescription
{
    public float Critical { get; set; }
    public float Type { get; set; }
    public bool Fainted { get; set; }
}