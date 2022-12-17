using System.Collections.Generic;
using UnityEngine;

public class Pokemon
{
    private PokemonBase _base;
    private int _level;
    public List<Move> _moves;
    private int _hp; // Current life

    public Pokemon(PokemonBase pokemonBase, int level)
    {
        _base = pokemonBase;
        _level = level;
        _moves = new List<Move>();
        _hp = MaxHP;

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
    public PokemonBase PokemonBase => _base;

    public int Level
    {
        get => _level;
        set => _level = value;
    }

    public int Hp
    {
        get => _hp;
        set => _hp = value;
    }
    
    public int MaxHP => Mathf.FloorToInt(_base.MaxHp * _level / 20.0f) + 10;
    public int Attack => Mathf.FloorToInt(_base.Attack * _level / 100.0f) + 1;
    public int Defense => Mathf.FloorToInt(_base.Defense * _level / 100.0f) + 1;
    public int SpAttack => Mathf.FloorToInt(_base.SpAttack * _level / 100.0f) + 1;
    public int SpDefense => Mathf.FloorToInt(_base.SpDefense * _level / 100.0f) + 1;
    public int Speed => Mathf.FloorToInt(_base.Speed * _level / 100.0f) + 1;
    
}
