﻿using Menu;
using UnityEngine;

namespace AllKills.Menu.UIComponents.Base
{
    public abstract class ScrollBoxElement : MenuElementBase, IVisibilityControlled
    {
        #region Properties

        /// <summary> The height of this element. </summary>
        public readonly float ElementHeight;

        /// <summary> The available height this element has. </summary>
        public float AvailableHeight;

        #endregion

        /// <summary>
        ///     Create this element.
        /// </summary>
        /// <param name="menu"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="owner"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="elementHeight">
        ///     The fixed height of this element.
        /// </param>
        protected ScrollBoxElement(
            global::Menu.Menu menu,
            MenuObject owner,
            float elementHeight)
            : base(menu, owner, new Vector2(float.NaN, float.NaN))
        {
            ElementHeight = elementHeight;
        }

        /// <inheritdoc cref="CycleDetail.GrafUpdate"/>
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);

            // Update opacity of elements if space is sparse.
            float availableSpace = Mathf.Min(ElementHeight, AvailableHeight);
            float opacity = availableSpace / ElementHeight;
            SetOpacity(opacity);
        }

        /// <summary>
        ///     Get the height required to fully display this element.
        /// </summary>
        /// <returns>
        ///     The required height.
        /// </returns>
        public virtual float GetElementHeight()
        {
            return ElementHeight;
        }

        /// <summary>
        ///     Set the height that this element has available to be displayed.
        /// </summary>
        /// <param name="availableHeight">
        ///     The available height.
        /// </param>
        public virtual void SetAvailableHeight(float availableHeight)
        {
            AvailableHeight = availableHeight;
        }

        /// <inheritdoc/>
        public abstract void Show();

        /// <inheritdoc/>
        public abstract void Hide();

        /// <inheritdoc/>
        public abstract void SetOpacity(float opacity);

        public class Spacer : ScrollBoxElement
        {
            /// <summary>
            ///     Create a spacer, which is a completely blank element used to separate other elements.
            /// </summary>
            /// <inheritdoc/>
            public Spacer(global::Menu.Menu menu, MenuObject owner, float elementHeight) : base(menu,
                owner, elementHeight)
            {
            }

            /// <inheritdoc/>
            public override void Show()
            {
            }

            /// <inheritdoc/>
            public override void Hide()
            {
            }

            /// <inheritdoc/>
            public override void SetOpacity(float opacity)
            {
            }
        }
    }
}