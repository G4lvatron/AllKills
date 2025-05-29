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
    public abstract class ScrollBox : MenuElementBase, Slider.ISliderOwner
    {
        #region Properties

        /// <summary> The size of this element. </summary>
        public Vector2 Size;

        /// <summary> The height in pixels of the content in this scroll box. </summary>
        public abstract float ContentHeight { get; }

        /// <summary> The number of pixels from the top of the content the scroll box has be scrolled through. </summary>
        public float ScrollDistance;

        #region Sub-Objects

        /// <summary> The scrollbar on the left of this element. </summary>
        public readonly VerticalSlider Scrollbar;

        /// <summary> The cycle detail elements displayed in the scroll box. </summary>
        public readonly List<ScrollBoxElement> Content;

        #endregion

        #endregion

        /// <summary>
        ///     Create a new cycle list.
        /// </summary>
        /// <param name="menu"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="owner"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="pos"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="size">
        ///     The size of this element
        /// </param>
        protected ScrollBox(
            global::Menu.Menu menu,
            MenuObject owner,
            Vector2 pos,
            Vector2 size)
            : base(menu, owner, pos)
        {
            Size = size;
            Content = new List<ScrollBoxElement>();

            subObjects.Add(Scrollbar = new VerticalSlider(
                menu,
                this,
                "",
                new Vector2(-20f, 0f),
                new Vector2(30f, size.y),
                Slider.SliderID.LevelsListScroll,
                true));
        }

        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();

            float currentBasePosition = Size.y + ScrollDistance;
            foreach (ScrollBoxElement element in Content)
            {
                float elementHeight = element.GetElementHeight();

                currentBasePosition -= elementHeight;

                // Check if the element is too high to be drawn
                if (
                    currentBasePosition < Size.y - 10f
                    && currentBasePosition > 10f - elementHeight)
                {
                    element.pos = new Vector2(10f, currentBasePosition);

                    // Tell the element how much space it has
                    if (currentBasePosition + elementHeight > Size.y)
                        element.SetAvailableHeight(Size.y - currentBasePosition);
                    else if (currentBasePosition < 0)
                        element.SetAvailableHeight(elementHeight + currentBasePosition);
                    else
                        element.SetAvailableHeight(elementHeight);

                    element.Show();
                }
                else element.Hide();

                currentBasePosition -= 5f;
            }
        }

        /// <inheritdoc/>
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);
        }

        #region Scrollbar Methods

        /// <summary>
        ///     Return how far the scrollable body has moved.
        /// </summary>
        /// <param name="slider">
        ///     The slider.
        /// </param>
        /// <returns>
        ///     A value between 0 and 1 representing how far down the user has scrolled.
        /// </returns>
        public float ValueOfSlider(Slider slider)
        {
            return 1f - ScrollDistance / (ContentHeight - Size.y);
        }

        /// <summary>
        ///     Set the scroll distance based on the position of the scrollbar.
        /// </summary>
        /// <param name="slider">
        ///     The scrollbar sending the height.
        /// </param>
        /// <param name="setValue">
        ///     The value to set the scroll distance to.
        /// </param>
        public void SliderSetValue(Slider slider, float setValue)
        {
            ScrollDistance = (ContentHeight - Size.y) * (1f - setValue);
        }

        #endregion
    }
}