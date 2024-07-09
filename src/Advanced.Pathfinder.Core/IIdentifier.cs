namespace Advanced.Pathfinder.Core;

public interface IIdentifier<T> : IEntityId 
{
    T Id { get; init; }
    string IEntityId.EntityId => Id?.ToString() ??  throw new ArgumentException("DUMMKOPF*420");

}
