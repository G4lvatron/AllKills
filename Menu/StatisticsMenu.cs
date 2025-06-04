using AllKills.Menu.StatisticsData;
using AllKills.Menu.UIComponents;
using Menu;
using UnityEngine;

namespace AllKills.Menu
{
    public class StatisticsMenu : PositionedMenuObject
    {
        #region Constants

        public static Color MenuRed = new Color(153f / 255f, 0f, 0f);

        public static Color MenuGreen = new Color(37f / 255f, 142f / 255f, 37f / 255f);

        #endregion

        #region Properties

        public Vector2 Size;

        #endregion

        /// <summary>
        ///     Create the All Kills statistics menu.
        /// </summary>
        /// <param name="menu">
        ///     The menu this element is a part of.
        /// </param>
        /// <param name="owner">
        ///     The owner of this element.
        /// </param>
        /// <param name="pos">
        ///     The position of this element relative to the owner.
        /// </param>
        /// <param name="size">
        ///     The size of this element.
        /// </param>
        /// <param name="campaign">
        ///     The campaign statistics.
        /// </param>
        public StatisticsMenu(
            StatisticsDialog menu,
            MenuObject owner,
            Vector2 pos,
            Vector2 size,
            Campaign campaign)
            : base(menu, owner, pos)
        {
            // Setup
            this.menu = menu;
            Size = size;

            CycleList list = new CycleList(
                menu,
                this,
                new Vector2(50f, 50f),
                new Vector2(size.x / 2 - 75f, size.y - 100f),
                campaign?.Statistics
            );
            subObjects.Add(list);

            PlaythroughDetail detail = new PlaythroughDetail(
                menu,
                this,
                new Vector2(size.x / 2 + 25f, 50f),
                new Vector2(size.x / 2 - 75f, size.y - 100f),
                campaign);
            subObjects.Add(detail);
        }
    }
}