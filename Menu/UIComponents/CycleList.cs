using System.Collections.Generic;
using System.Linq;
using AllKills.Menu.StatisticsData;
using Menu;
using UnityEngine;

namespace AllKills.Menu.UIComponents
{
    /// <summary>
    ///     Element for displaying a list of statistics for the cycles in a play-through.
    /// </summary>
    public class CycleList : ScrollBox
    {
        #region Properties

        ///<inheritdoc />
        public override float ContentHeight { get; }

        #endregion

        /// <summary>
        ///     Create a new cycle list.
        /// </summary>
        /// <param name="menu"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="owner"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="pos"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="size">
        ///     The size of this element
        /// </param>
        /// <param name="statistics">
        ///     The campaign statistics to get the data for the cycles from.
        /// </param>
        public CycleList(
            global::Menu.Menu menu,
            MenuObject owner,
            Vector2 pos,
            Vector2 size,
            CampaignStatistics statistics)
            : base(menu, owner, pos, size)
        {
            owner.Container.AddChild(myContainer = new FContainer());
            Size = size;

            List<Cycle> cycleData = statistics?.Cycles ?? new List<Cycle>();
            foreach (var cycleDetail in cycleData.Select(c => new CycleDetailScrollElement(
                         menu,
                         this,
                         size.x - 20f,
                         c)))
            {
                Content.Add(cycleDetail);
                subObjects.Add(cycleDetail);
            }

            Content.Reverse();
            AddScrollButtons();

            ContentHeight =
                Content.Aggregate(0f, (n, c) => n + c.GetElementHeight())
                + 5f * (Content.Count - 1)
                + 30f;
        }

        /// <inheritdoc/>
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
        }
    }
}