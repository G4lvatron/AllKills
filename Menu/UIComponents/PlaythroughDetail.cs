using System;
using System.Collections.Generic;
using System.Linq;
using AllKills.Menu.StatisticsData;
using AllKills.Menu.UIComponents.Base;
using Menu;
using UnityEngine;

namespace AllKills.Menu.UIComponents
{
    public class PlaythroughDetail : ScrollBox
    {
        #region Properties

        ///<inheritdoc />
        public override float ContentHeight { get; }

        #endregion

        /// <summary>
        ///     Create a new play-through detail element.
        /// </summary>
        /// <param name="menu"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="owner"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="pos"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="size">
        ///     The size of this element.
        /// </param>
        /// <param name="statistics">
        ///     The campaign statistics.
        /// </param>
        public PlaythroughDetail(
            global::Menu.Menu menu,
            MenuObject owner,
            Vector2 pos,
            Vector2 size,
            CampaignStatistics statistics)
            : base(menu, owner, pos, size)
        {
            owner.Container.AddChild(myContainer = new FContainer());

            for (int i = 0; i < 30; i++)
            {
                PlayThroughDetailLabel label = new PlayThroughDetailLabel(
                    menu,
                    this,
                    new Vector2(float.NaN, float.NaN),
                    "TEST");
                subObjects.Add(label);
                Content.Add(label);
            }

            ContentHeight =
                Content.Aggregate(0f, (n, c) => n + c.GetElementHeight())
                + 5f * (Content.Count - 1)
                + 30f;
        }

        /// <inheritdoc cref="CycleDetail.GrafUpdate"/>
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
        }

        #region Label

        /// <summary>
        ///     A label to display text in a play-though detail box.
        /// </summary>
        public class PlayThroughDetailLabel : ScrollBoxElement
        {
            /// <summary> The label. </summary>
            public MenuLabel Label;

            /// <summary>
            ///     Create a new detail label.
            /// </summary>
            /// <param name="menu"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
            /// <param name="owner"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
            /// <param name="pos"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
            /// <param name="text">
            ///     The text for this label.
            /// </param>
            public PlayThroughDetailLabel(
                global::Menu.Menu menu,
                MenuObject owner,
                Vector2 pos,
                string text)
                : base(menu, owner, pos, 30f)
            {
                subObjects.Add(Label = new MenuLabel(
                    menu,
                    this,
                    text,
                    new Vector2(10f, 0f),
                    new Vector2(30f, 60f),
                    true));
            }

            /// <inheritdoc/>
            public override void Show()
            {
                Label.label.isVisible = true;
            }

            /// <inheritdoc/>
            public override void Hide()
            {
                Label.label.isVisible = false;
            }

            /// <inheritdoc/>
            public override void SetOpacity(float opacity)
            {
                Label.label.alpha = opacity;
            }
        }

        #endregion
    }
}