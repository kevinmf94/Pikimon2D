public class Move
{
    private MoveBase _base;
    private int _pp;

    public MoveBase Base => _base;

    public int Pp
    {
        get => _pp;
        set => _pp = value;
    }

    public Move(MoveBase moveBase)
    {
        _base = moveBase;
        _pp = moveBase.PP;
    }
}
