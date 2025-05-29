using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllKills.Menu.StatisticsData;
using AllKills.Menu.UIComponents.Base;
using Menu;
using UnityEngine;

namespace AllKills.Menu.UIComponents
{
    internal class CycleDetailScrollElement : ScrollBoxElement
    {
        #region Properties

        /// <summary>
        ///     The inner cycle detail contained in this element.
        /// </summary>
        public readonly CycleDetail CycleDetailElement;

        #endregion

        /// <summary>
        ///     Create a new cycle detail element that can be placed in a scroll box.
        /// </summary>
        /// <param name="menu"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="owner"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="pos"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="width"><inheritdoc cref="CycleDetail(Menu, MenuObject, Vector2, Vector2, Cycle)"/></param>
        /// <param name="cycle"><inheritdoc cref="CycleDetail(Menu, MenuObject, Vector2, Vector2, Cycle)"/></param>
        public CycleDetailScrollElement(
            global::Menu.Menu menu,
            MenuObject owner,
            Vector2 pos,
            float width,
            Cycle cycle)
            : base(menu, owner, pos, _getHeight(width, cycle))
        {
            subObjects.Add(CycleDetailElement = new CycleDetail(
                menu,
                this,
                new Vector2(0f, 0f),
                new Vector2(width, ElementHeight),
                cycle));
        }

        #region Visibility

        /// <inheritdoc/>
        public override void Show()
        {
            CycleDetailElement.Show();
        }

        /// <inheritdoc/>
        public override void Hide()
        {
            CycleDetailElement.Hide();
        }

        /// <inheritdoc/>
        public override void SetOpacity(float opacity)
        {
            CycleDetailElement.SetOpacity(opacity);
        }

        #endregion

        #region Helper

        /// <summary>
        ///     Get the required dimensions of a detail box.
        /// </summary>
        /// <param name="width">
        ///     The available width.
        /// </param>
        /// <param name="cycle">
        ///     The cycle details.
        /// </param>
        /// <returns>
        ///     The dimensions of the detail box with the required height.
        /// </returns>
        private static float _getHeight(float width, Cycle cycle)
        {
            int perRow = ((int)width - 50) / (int)CycleDetail.CountWidth;
            if (perRow == 0)
                return 0f; // Width is too small

            int killRows = (cycle?.Statistics?.Kills?.Count ?? 0) / perRow;
            if (cycle?.Statistics?.Kills?.Count % perRow > 0)
                killRows++;
            int eatRows = (cycle?.Statistics?.Eats?.Count ?? 0) / perRow;
            if (cycle?.Statistics?.Eats?.Count % perRow > 0)
                eatRows++;

            return 85f + CycleDetail.CountHeight * (eatRows + killRows);
        }

        #endregion
    }
}