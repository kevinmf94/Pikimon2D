using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    PlayerMove,
    Battle
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private Camera worldMainCamera;
    
    private GameState _gameState;

    private void Awake()
    {
        _gameState = GameState.PlayerMove;
    }

    private void Start()
    {
        playerController.OnPokemonEncountered += StartPokemonBattle;
        battleManager.OnBattleFinish += EndPokemonBattle;
    }

    void StartPokemonBattle()
    {
        _gameState = GameState.Battle;
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.gameObject.GetComponent<PokemonParty>();
        var wildPrototype = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();
        var wildInstance = new Pokemon(wildPrototype.Base, wildPrototype.Level);
        
        battleManager.HandleStartBattle(playerParty, wildInstance);
    }

    void EndPokemonBattle(bool win)
    {
        _gameState = GameState.PlayerMove;
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (_gameState == GameState.PlayerMove)
        {
             playerController.HandleUpdate();
        } else if (_gameState == GameState.Battle)
        {
            battleManager.HandleUpdate();
        }
    }
}
