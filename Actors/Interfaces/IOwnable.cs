namespace CoolTools.Actors
{
    public interface IOwnable
    {
        public bool HasOwner { get; }
        public Actor Owner { get; set; }
    }
}