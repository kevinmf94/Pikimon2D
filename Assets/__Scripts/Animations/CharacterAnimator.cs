using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection { Down, Up, Left, Right}

public class CharacterAnimator : MonoBehaviour
{

    public float MoveX, MoveY;
    public bool isMoving;

    [SerializeField] private List<Sprite> walkDownSprites, walkUpSprites, walkRightSprites, walkLeftSprites;
    [SerializeField] private FacingDirection defaultDirection = FacingDirection.Down;
    
    public FacingDirection DefaultDirection => defaultDirection;

    [SerializeField] private float frameRate = 0.15f;
    
    private CustomAnimator walkDownAnim, walkUpAnim, walkRightAnim, walkLeftAnim;
    private CustomAnimator currentAnimator;
    private SpriteRenderer renderer;
    private bool wasPreviousMoving = false;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        walkDownAnim = new CustomAnimator(renderer, walkDownSprites, frameRate);
        walkUpAnim = new CustomAnimator(renderer, walkUpSprites, frameRate);
        walkRightAnim = new CustomAnimator(renderer, walkRightSprites, frameRate);
        walkLeftAnim = new CustomAnimator(renderer, walkLeftSprites, frameRate);
        SetFacingDirection(defaultDirection);
        
        currentAnimator = walkDownAnim;
    }

    private void Update()
    {
        var previousAnimator = currentAnimator;
        
        if (MoveX == 1)
        {
            currentAnimator = walkRightAnim;
        }
        else if (MoveX == -1)
        {
            currentAnimator = walkLeftAnim;
        }
        else if (MoveY == 1)
        {
            currentAnimator = walkUpAnim;
        }
        else if (MoveY == -1)
        {
            currentAnimator = walkDownAnim;
        }

        if (previousAnimator != currentAnimator || isMoving != wasPreviousMoving)  
        {
            currentAnimator.Start();
        }

        if (isMoving)
        {
            currentAnimator.HandleUpdate();
        }
        else
        {
            renderer.sprite = currentAnimator.AnimFrames[0];
        }

        wasPreviousMoving = isMoving;
    }
    
    public void SetFacingDirection(FacingDirection direction)
    {
        if (direction == FacingDirection.Down)
        {
            MoveY = -1;
        } else if (direction == FacingDirection.Up)
        {
            MoveY = 1;
        }else if (direction == FacingDirection.Left)
        {
            MoveX = -1;
        }else if (direction == FacingDirection.Right)
        {
            MoveX = 1;
        }
    }
    
}
