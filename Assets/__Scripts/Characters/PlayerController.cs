using System;
using System.Collections;
using Mono.CompilerServices.SymbolWriter;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterAnimator))]
public class PlayerController : MonoBehaviour
{
    private float offsetY = 0.2f;

    private Vector2 input;
    private Vector3 target;

    private Character character;

    public event Action OnPokemonEncountered;

    private void Awake()
    { 
        character = GetComponent<Character>();
    }

    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
            {
                input.y = 0;
            }

            if (input != Vector2.zero)
            {
                StartCoroutine(character.MoveTowards(input, CheckForPokemon));
            }
        }
        
        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            Interact();
        }
    }

    private void Interact()
    {
        var facingDirection = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPosition = transform.position + facingDirection;
        
        Debug.DrawLine(transform.position, interactPosition, Color.magenta, 1f);
        var collider = Physics2D.OverlapCircle(interactPosition, offsetY, GameLayers.SharedInstance.InteractableLayer);
        if (collider != null)
        {
            Debug.DrawLine(transform.position, interactPosition, Color.yellow, 1f);
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    private void CheckForPokemon()
    {
        if (Physics2D.OverlapCircle(transform.position, offsetY, GameLayers.SharedInstance.PokemonLayer) != null)
        {
            if (Random.Range(0, 100) < 10)
            {
                OnPokemonEncountered();
            }
        }
    }
}
