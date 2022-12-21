using System;
using System.Buffers;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokemonBase _base;
    public int _level;
    public bool isPlayer;
    
    public Pokemon Pokemon { get; set; }
    private Image pokemonImage;
    private Vector3 initialPosition;
    private Color initialColor;

    private float startTimeAnim = 1.0f, attackTimeAnim = 0.3f, dieTimeAnim = 1f, hitTimeAnimation = 0.15f;

    private void Awake()
    {
        pokemonImage = GetComponent<Image>();
        initialPosition = pokemonImage.transform.localPosition;
        initialColor = pokemonImage.color;
    }

    public void SetupPokemon()
    {
        Pokemon = new Pokemon(_base, _level);
        pokemonImage.sprite = isPlayer ? 
            Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite;
        PlayStartAnimation();
    }

    public void PlayStartAnimation()
    {
        pokemonImage.transform.localPosition = new Vector3(initialPosition.x + (isPlayer ? -1 : 1) * 400
            , initialPosition.y);

        pokemonImage.transform.DOLocalMoveX(initialPosition.x, startTimeAnim);
    }

    public void PlayAttackAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x + 
                                                       (isPlayer ? 1 : -1) * 50, attackTimeAnim));
        seq.Append(pokemonImage.transform.DOLocalMoveX(initialPosition.x, attackTimeAnim));
        seq.Play();
    }

    public void PlayReceiveAttackAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.DOColor(Color.grey, hitTimeAnimation));
        seq.Append(pokemonImage.DOColor(initialColor, hitTimeAnimation));
        seq.Play();
    }

    public void PlayFaintAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.transform.DOLocalMoveY(initialPosition.y - 200, dieTimeAnim));
        seq.Join(pokemonImage.DOFade(0, dieTimeAnim));
        seq.Play();
    }
    
}
