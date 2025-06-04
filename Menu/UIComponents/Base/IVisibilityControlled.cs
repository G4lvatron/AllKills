namespace AllKills.Menu.UIComponents.Base
{
    public interface IVisibilityControlled
    {
        /// <summary>
        ///     Show this element.
        /// </summary>
        void Show();

        /// <summary>
        ///     Hide this element.
        /// </summary>
        void Hide();

        /// <summary>
        ///     Set the opacity of this element.
        /// </summary>
        /// <param name="opacity">
        ///     The opacity to set this element to.
        /// </param>
        void SetOpacity(float opacity);
    }
}