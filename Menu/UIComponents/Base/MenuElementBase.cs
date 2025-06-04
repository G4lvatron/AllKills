using System.Diagnostics.CodeAnalysis;
using Menu;
using UnityEngine;

namespace AllKills.Menu.UIComponents.Base
{
    [SuppressMessage("ReSharper", "RedundantOverriddenMember")]
    public class MenuElementBase : PositionedMenuObject
    {
        /// <summary>
        ///     Base class for all menu element in All Kills.
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
        public MenuElementBase(
            global::Menu.Menu menu,
            MenuObject owner,
            Vector2 pos)
            : base(menu, owner, pos)
        {
        }

        /// <summary>
        ///     Update this element.
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        ///     Update the graphics of this element.
        /// </summary>
        /// <param name="timeStacker">
        ///     A value representing time passed, used for interpolation.
        /// </param>
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
        }
    }
}