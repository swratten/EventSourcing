namespace Overs.Over;
public class Batsman
{
    public Guid BatsmanId { get; }
    public int BatOrder { get; }
    public bool InPlay { get; }
    public Batsman(Guid batsmanId, int batOrder, bool inPlay)
    {
        BatsmanId = batsmanId;
        BatOrder = batOrder;
        InPlay = inPlay;
    }
}