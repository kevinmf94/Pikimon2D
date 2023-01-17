using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

            if (_moves.Count >= 4)
            {
                break;
            }
        }
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
        set => _hp = value;
    }
    
    public int Exp { get; set; }

    public int MaxHP => Mathf.FloorToInt(_base.MaxHp * _level / 20.0f) + 10;
    public int Attack => Mathf.FloorToInt(_base.Attack * _level / 100.0f) + 1;
    public int Defense => Mathf.FloorToInt(_base.Defense * _level / 100.0f) + 1;
    public int SpAttack => Mathf.FloorToInt(_base.SpAttack * _level / 100.0f) + 1;
    public int SpDefense => Mathf.FloorToInt(_base.SpDefense * _level / 100.0f) + 1;
    public int Speed => Mathf.FloorToInt(_base.Speed * _level / 100.0f) + 1;

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
    
}

public struct DamageDescription
{
    public float Critical { get; set; }
    public float Type { get; set; }
    public bool Fainted { get; set; }
}