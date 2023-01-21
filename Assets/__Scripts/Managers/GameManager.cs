using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum GameState
{
    PlayerMove,
    Battle,
    Dialog
}

[RequireComponent(typeof(ColorManager))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private BattleManager battleManager;

    [SerializeField] private Camera worldMainCamera;
    [SerializeField] private Image transitionPanel;
    
    private GameState _gameState;

    public AudioClip worldClip, battleClip;

    private void Awake()
    {
        _gameState = GameState.PlayerMove;
    }

    private void Start()
    {
        SoundManager.SharedInstance.PlayMusic(worldClip);
        playerController.OnPokemonEncountered += StartPokemonBattle;
        battleManager.OnBattleFinish += EndPokemonBattle;
        DialogManager.SharedInstance.OnDialogStart += () =>
        {
            _gameState = GameState.Dialog;
        };

        DialogManager.SharedInstance.OnDialogFinish += () =>
        {
            _gameState = GameState.PlayerMove;
        };
    }

    void StartPokemonBattle()
    {
        StartCoroutine(FadeInBattle());
    }

    IEnumerator FadeInBattle()
    {
        SoundManager.SharedInstance.PlayMusic(battleClip);
        _gameState = GameState.Battle;

        yield return transitionPanel.DOFade(1.0f, 1.0f).WaitForCompletion();
        yield return new WaitForSeconds(0.2f);
        
        battleManager.gameObject.SetActive(true);
        worldMainCamera.gameObject.SetActive(false);

        var playerParty = playerController.gameObject.GetComponent<PokemonParty>();
        var wildPrototype = FindObjectOfType<PokemonMapArea>().GetComponent<PokemonMapArea>().GetRandomWildPokemon();
        var wildInstance = new Pokemon(wildPrototype.Base, wildPrototype.Level);
        
        battleManager.HandleStartBattle(playerParty, wildInstance);
        yield return transitionPanel.DOFade(0.0f, 1.0f).WaitForCompletion();
    }

    void EndPokemonBattle(bool playerHasWon)
    {
        StartCoroutine(FadeOutBattle(playerHasWon));
    }

    IEnumerator FadeOutBattle(bool playerHasWon)
    {
        yield return transitionPanel.DOFade(1.0f, 1.0f).WaitForCompletion();
        yield return new WaitForSeconds(0.2f);
        
        battleManager.gameObject.SetActive(false);
        worldMainCamera.gameObject.SetActive(true);
        if (!playerHasWon)
        {
            
        }
        
        yield return transitionPanel.DOFade(0.0f, 1.0f).WaitForCompletion();
        SoundManager.SharedInstance.PlayMusic(worldClip);
        _gameState = GameState.PlayerMove;
    }

    private void Update()
    {
        if (_gameState == GameState.PlayerMove)
        {
             playerController.HandleUpdate();
        } else if (_gameState == GameState.Battle)
        {
            battleManager.HandleUpdate();
        } else if (_gameState == GameState.Dialog)
        {;
            DialogManager.SharedInstance.HandleUpdate();
        }
    }
}
