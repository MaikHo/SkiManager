namespace SkiManager.Engine.Interfaces
{
    public interface IConditionalChildAccess
    {
        bool CanEnter(Entity newChild, Reason reason);
    }
}
