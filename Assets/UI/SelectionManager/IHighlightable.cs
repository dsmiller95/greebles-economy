namespace Assets.UI.SelectionManager
{
    public enum HighlightState
    {
        CanBeSelected,
        Selected,
        None
    }
    public interface IHighlightable
    {
        void SetHighlighted(HighlightState highlighted);
    }
}
