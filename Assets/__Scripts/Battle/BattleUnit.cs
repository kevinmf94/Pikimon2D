using System;
using System.Buffers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class BattleUnit : MonoBehaviour
{
    public PokemonBase _base;
    public int _level;
    [SerializeField] private bool isPlayer;
    [SerializeField] private BattleHUD hud;
    
    public Pokemon Pokemon { get; set; }
    public bool IsPlayer => isPlayer;
    public BattleHUD Hud => hud;

    private Image pokemonImage;
    private Vector3 initialPosition;
    private Color initialColor;

    private float startTimeAnim = 1.0f, attackTimeAnim = 0.3f, 
        dieTimeAnim = 1f, hitTimeAnim = 0.15f, capturedTimeAnim = 0.4f;

    private void Awake()
    {
        pokemonImage = GetComponent<Image>();
        initialPosition = pokemonImage.transform.localPosition;
        initialColor = pokemonImage.color;
    }

    public void SetupPokemon(Pokemon pokemon)
    {
        Pokemon = pokemon;
        pokemonImage.sprite = isPlayer ? 
            Pokemon.Base.BackSprite : Pokemon.Base.FrontSprite;
        pokemonImage.color = initialColor;
        pokemonImage.transform.position = initialPosition;
        transform.localScale = Vector3.one;
        hud.SetPokemonData(pokemon);
        
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
        seq.Append(pokemonImage.DOColor(Color.grey, hitTimeAnim));
        seq.Append(pokemonImage.DOColor(initialColor, hitTimeAnim));
        seq.Play();
    }

    public void PlayFaintAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.transform.DOLocalMoveY(initialPosition.y - 200, dieTimeAnim));
        seq.Join(pokemonImage.DOFade(0, dieTimeAnim));
        seq.Play();
    }

    public IEnumerator PlayCapturedAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.DOFade(0, capturedTimeAnim));
        seq.Join(pokemonImage.transform.DOScale(new Vector3(0.25f, 0.25f, 1f), capturedTimeAnim));
        seq.Join(pokemonImage.transform.DOLocalMoveY(initialPosition.y + 50f, capturedTimeAnim));
        yield return seq.WaitForCompletion();
    }
    
    public IEnumerator PlayBreakOutAnimation()
    {
        var seq = DOTween.Sequence();
        seq.Append(pokemonImage.DOFade(1f, capturedTimeAnim));
        seq.Join(pokemonImage.transform.DOScale(new Vector3(1f, 1f, 1f), capturedTimeAnim));
        seq.Join(pokemonImage.transform.DOLocalMoveY(initialPosition.y, capturedTimeAnim));
        yield return seq.WaitForCompletion();
    }
    
}
