using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterAnimator anim;
    private float offsetY = 0.2f;
    public bool IsMoving { get; private set; }
    
    [SerializeField]
    private float speed;

    public CharacterAnimator Animator => anim;

    private void Awake()
    {
        anim = GetComponent<CharacterAnimator>();
    }

    public IEnumerator MoveTowards(Vector2 moveVector, Action OnMoveFinish)
    {
        anim.MoveX = moveVector.x;
        anim.MoveY = moveVector.y;
        
        var target = transform.position;
        target.x += moveVector.x;
        target.y += moveVector.y;

        if (!IsAvailable(target))
        {
            yield break;
        }
        
        IsMoving = true;
        
        while (IsMoving && Vector3.Distance(transform.position, target) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        IsMoving = false;
        OnMoveFinish?.Invoke();
    }

    public void HandleUpdate()
    {
        anim.isMoving = IsMoving;
    }
    
    private bool IsAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target - new Vector3(0, offsetY, 0), 0.2f, 
                GameLayers.SharedInstance.SolidObjectsLayer | GameLayers.SharedInstance.InteractableLayer))
        {
            return false;
        }

        return true;
    }
    
}
