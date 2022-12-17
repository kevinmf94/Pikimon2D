using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Nuevo movimiento")]
public class MoveBase : ScriptableObject
{
    [SerializeField] private string name;
    [TextArea] [SerializeField] private string description;

    public string Name => name;
    public string Description => description;

    [SerializeField] private PokemonType type;
    [SerializeField] private int power;
    [SerializeField] private int accuracy;
    [SerializeField] private int pp;

    public PokemonType Type => type;
    public int Power => power;
    public int Accuracy => accuracy;
    public int Pp => pp;
}
