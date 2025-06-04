using AllKills.Menu.UIComponents.Base;
using Menu;
using UnityEngine;

namespace AllKills.Menu.UIComponents
{
    /// <summary>
    ///     A creature/item symbol along with a counter next to it.
    /// </summary>
    public sealed class SymbolCounter : PositionedMenuObject, IVisibilityControlled
    {
        #region Properties

        /// <summary> The number. </summary>
        public readonly MenuLabel NumberLabel;

        /// <summary> The creature/item symbol. </summary>
        public readonly IconSymbol Symbol;

        /// <summary> The numerator of the fraction. </summary>
        public readonly MenuLabel FracN;

        /// <summary> The denominator of the fraction. </summary>
        public readonly MenuLabel FracD;

        /// <summary> <c>true</c> if this element is visible. </summary>
        public bool Visible;

        #endregion

        /// <summary>
        ///     Create a new symbol counter.
        /// </summary>
        /// <param name="menu"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="owner"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="pos"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="creatureType">
        ///     The type to use for the symbol.
        /// </param>
        /// <param name="objectType">
        ///     The type of the icon being used.
        /// </param>
        /// <param name="intData">
        ///     Optional data.
        /// </param>
        /// <param name="count">
        ///     The number to be displayed alongside the symbol.
        /// </param>
        /// <param name="fracCount">
        ///     The fractional count to be displayed in quarters.
        /// </param>
        public SymbolCounter(
            global::Menu.Menu menu,
            MenuObject owner,
            Vector2 pos,
            CreatureTemplate.Type creatureType,
            AbstractPhysicalObject.AbstractObjectType objectType,
            int intData,
            int count,
            int fracCount)
            : base(menu, owner, pos)
        {
            fracCount %= 4;

            //Icon
            Symbol = IconSymbol.CreateIconSymbol(
                new IconSymbol.IconSymbolData(creatureType, objectType, intData),
                Container);

            //Number
            subObjects.Add(NumberLabel = new MenuLabel(
                menu,
                this,
                count.ToString(),
                new Vector2(0f, 0f),
                new Vector2(60f, 20f),
                true));

            float width;
            float scaleMod = 1f;
            if ((width = NumberLabel.label.textRect.width + (fracCount > 0 ? 5f : 0f)) > 25f)
                scaleMod = 25f / width;

            NumberLabel.pos = new Vector2(NumberLabel.label.textRect.width * scaleMod / 2 - 8f, -10f);
            NumberLabel.label.scale = scaleMod;

            // Fraction Part
            if (fracCount > 0)
            {
                float fracPosX = NumberLabel.pos.x + 5f + NumberLabel.label.textRect.width * scaleMod / 2f;
                subObjects.Add(FracN = new MenuLabel(
                    menu,
                    this,
                    fracCount < 3 ? "1" : "3",
                    new Vector2(fracPosX, -10f + 5f * scaleMod),
                    new Vector2(60f, 20f),
                    true)
                {
                    label = { scale = 0.5f * scaleMod }
                });
                subObjects.Add(FracD = new MenuLabel(
                    menu,
                    this,
                    fracCount == 2 ? "2" : "4",
                    new Vector2(fracPosX, -10f - 5f * scaleMod),
                    new Vector2(60f, 20f),
                    true)
                {
                    label = { scale = 0.5f * scaleMod }
                });
            }

            Symbol.Show(false);
            Symbol.showFlash = 0f;
            Symbol.lastShowFlash = 0f;
        }

        /// <inheritdoc cref="MenuElementBase.Update"/>
        public override void Update()
        {
            base.Update();
            Symbol.Update();
        }

        /// <inheritdoc cref="MenuElementBase.GrafUpdate"/>
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);

            NumberLabel.label.isVisible = Visible;
            if (Visible)
            {
                if (Symbol.symbolSprite != null)
                {
                    NumberLabel.label.color = Symbol.symbolSprite.color;
                    if (FracN != null)
                    {
                        FracN.label.color = Symbol.symbolSprite.color;
                        FracD.label.color = Symbol.symbolSprite.color;
                    }
                }

                Symbol.Draw(timeStacker, DrawPos(timeStacker));
                return;
            }

            Symbol.Draw(timeStacker, new Vector2(-1000f, -1000f));
        }

        #region Visibility

        /// <inheritdoc/>
        public void Show()
        {
            Visible = true;
            Symbol.symbolSprite.isVisible = true;
            NumberLabel.label.isVisible = true;
            if (FracN == null) return;
            FracN.label.isVisible = true;
            FracD.label.isVisible = true;
        }

        /// <inheritdoc/>
        public void Hide()
        {
            Visible = false;
            Symbol.symbolSprite.isVisible = false;
            NumberLabel.label.isVisible = false;
            if (FracN == null) return;
            FracN.label.isVisible = false;
            FracD.label.isVisible = false;
        }

        /// <inheritdoc/>
        public void SetOpacity(float opacity)
        {
            Symbol.symbolSprite.alpha = opacity;
            NumberLabel.label.alpha = opacity;
            if (FracN == null) return;
            FracN.label.alpha = opacity;
            FracD.label.alpha = opacity;
        }

        #endregion
    }
}