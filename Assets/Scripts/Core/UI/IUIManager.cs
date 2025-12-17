namespace Plugins.Core.UI
{
    public interface IUIManager
    {
        public uint OpenedElementsCount { get; }
        T Get<T>() where T : UIElement;
        void IncreaseOpenedUIElementsCounter();
        void DecreaseOpenedUIElementsCounter();
    }
    // ReSharper disable once ClassNeverInstantiated.Global
}