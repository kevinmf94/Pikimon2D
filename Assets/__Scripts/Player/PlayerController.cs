using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{

    private bool isMoving;
    
    [SerializeField]
    private float speed;

    private Vector2 input;
    private Vector3 target;

    private Animator _anim;
    
    [SerializeField]
    private LayerMask solidObjectLayers, pokemonZone;

    public event Action OnPokemonEncountered;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
            {
                input.y = 0;
            }

            if (input != Vector2.zero)
            {
                target = transform.position;
                target.x += input.x;
                target.y += input.y;
                
                _anim.SetFloat("MoveX", input.x);
                _anim.SetFloat("MoveY", input.y);

                if(IsAvailable(target)){
                    StartCoroutine("MoveTowards");
                }
            }
        }
    }

    IEnumerator MoveTowards()
    {
        isMoving = true;
        _anim.SetBool("isMoving", true);
        
        while (isMoving && Vector3.Distance(transform.position, target) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                target, speed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
        transform.position = target;
        _anim.SetBool("isMoving", false);
        CheckForPokemon();
    }

    private bool IsAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.2f, solidObjectLayers))
        {
            return false;
        }

        return true;
    }

    private void CheckForPokemon()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, pokemonZone) != null)
        {
            if (Random.Range(0, 100) < 10)
            {
                Debug.Log("POKEMON ZONE");
                OnPokemonEncountered();
            }
        }
    }
}
