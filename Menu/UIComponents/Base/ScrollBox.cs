using System.Collections.Generic;
using AllKills.Menu.UIComponents.Base;
using IL.RWCustom;
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

        /// <summary> The scroll distance that the scrollbox wants to reach. </summary>
        public float TargetScrollDistance;

        /// <summary> If the mouse is over this element. </summary>
        public virtual bool MouseOver
        {
            get
            {
                Vector2 screenPos = ScreenPos;
                return
                    menu.mousePosition.x > screenPos.x
                    && menu.mousePosition.y > screenPos.y
                    && menu.mousePosition.x < screenPos.x + Size.x
                    && menu.mousePosition.y < screenPos.y + Size.y;
            }
        }

        #region Sub-Objects

        /// <summary> The scrollbar on the left of this element. </summary>
        public readonly VerticalSlider Scrollbar;

        /// <summary> The cycle detail elements displayed in the scroll box. </summary>
        public readonly List<ScrollBoxElement> Content;

        public SymbolButton UpButton;

        public SymbolButton DownButton;

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

        /// <summary>
        ///     Adds the scroll buttons. This should be called as late as possible for proper layering
        /// </summary>
        public void AddScrollButtons()
        {
            subObjects.Add(UpButton = new SymbolButton(
                menu,
                this,
                "Menu_Symbol_Arrow",
                "UP",
                new Vector2(Size.x / 2, Size.y + 10f)));
            UpButton.symbolSprite.sortZ = 99f;

            subObjects.Add(DownButton = new SymbolButton(
                menu,
                this,
                "Menu_Symbol_Arrow",
                "DOWN",
                new Vector2(Size.x / 2, -10f)));
            DownButton.symbolSprite.rotation = 180f;
            DownButton.symbolSprite.sortZ = 99f;
        }

        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();

            // Button states
            Scrollbar.buttonBehav.greyedOut = ContentHeight < Size.y;
            if (UpButton != null)
            {
                UpButton.buttonBehav.greyedOut
                    = ContentHeight < Size.y
                      || ScrollDistance <= 0f;
                DownButton.buttonBehav.greyedOut
                    = ContentHeight < Size.y
                      || ScrollDistance >= ContentHeight - Size.y;
            }

            // Smooth scroll
            ScrollDistance = RWCustom.Custom.LerpAndTick(ScrollDistance, TargetScrollDistance, 0.5f, 0.5f);

            // Mousewheel
            if (
                ContentHeight > Size.y
                && MouseOver
                && menu.manager.menuesMouseMode
                && menu.mouseScrollWheelMovement != 0)
            {
                if (menu.mouseScrollWheelMovement < 0)
                {
                    TargetScrollDistance -= 50f;
                    if (TargetScrollDistance < 0)
                        TargetScrollDistance = 0;
                }
                else
                {
                    TargetScrollDistance += 50f;
                    if (TargetScrollDistance > ContentHeight - Size.y)
                        TargetScrollDistance = ContentHeight - Size.y;
                }
            }

            // Position content
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

        /// <summary>
        ///     Handle signals.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="message">
        ///     The message.
        /// </param>
        public override void Singal(MenuObject sender, string message)
        {
            base.Singal(sender, message);

            switch (message)
            {
                case "UP":
                    menu.PlaySound(SoundID.MENU_First_Scroll_Tick);
                    TargetScrollDistance -= 100f;
                    if (TargetScrollDistance < 0)
                        TargetScrollDistance = 0;
                    break;
                case "DOWN":
                    menu.PlaySound(SoundID.MENU_First_Scroll_Tick);
                    TargetScrollDistance += 100f;
                    if (TargetScrollDistance > ContentHeight - Size.y)
                        TargetScrollDistance = ContentHeight - Size.y;
                    break;
            }
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
            ScrollDistance = TargetScrollDistance = (ContentHeight - Size.y) * (1f - setValue);
        }

        #endregion
    }
}