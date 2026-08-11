namespace Dara.Server.BuildingBlocks.Domain;

//entity snapshot helping keeping encapsulated data
public interface IEntitySnapshot;


public interface IHasSnapshot<out T> where T : class, IEntitySnapshot
{
    public T GetSnapshot();
}