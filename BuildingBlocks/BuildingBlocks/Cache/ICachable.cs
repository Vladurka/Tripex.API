namespace BuildingBlocks.Cache;

public interface ICachable
{
    public bool IsCached { get; }
    public void SetIsCached(bool value);
}
