namespace EEMod
{
    public interface IComponentHandler<T> : IUpdateable, IDrawable
    {
        void AddElement(T Object);
    }
}