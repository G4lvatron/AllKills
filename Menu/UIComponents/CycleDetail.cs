using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using AllKills.Menu.StatisticsData;
using AllKills.Menu.UIComponents.Base;
using Menu;
using RWCustom;
using UnityEngine;

namespace AllKills.Menu.UIComponents
{
    /// <summary>
    ///     An element for displaying the details of a single cycle.
    /// </summary>
    public class CycleDetail : ButtonTemplate, IVisibilityControlled
    {
        #region Properties

        /// <summary> The width of a kill/eat counter. </summary>
        public const float CountWidth = 70f;

        /// <summary> The height of a kill/eat counter. </summary>
        public const float CountHeight = 30f;

        /// <summary> The cycle data. </summary>
        public readonly Cycle Cycle;

        #region Sub-Objects

        /// <summary> The border around this element. </summary>
        public readonly RoundedRect Border;

        /// <summary> The title displaying cycle number. </summary>
        public readonly MenuLabel Title;

        /// <summary> The score of the whole run at this cycle. </summary>
        public readonly MenuLabel TotalScore;

        /// <summary> The change in score since last cycle. </summary>
        public readonly MenuLabel CycleScore;

        /// <summary> The total time at this cycle. </summary>
        public readonly MenuLabel TotalTime;

        /// <summary> The time since last cycle. </summary>
        public readonly MenuLabel CycleTime;

        /// <summary> The kills icon. </summary>
        public readonly PlayerResultBox.SymbolAndLabel KillsSymbol;

        /// <summary> All the creatures killed in the cycle. </summary>
        public readonly List<SymbolCounter> KillCounters;

        /// <summary> The eats icon. </summary>
        public readonly PlayerResultBox.SymbolAndLabel EatsSymbol;

        /// <summary> All the food consumed in the cycle. </summary>
        public readonly List<SymbolCounter> EatCounters;

        #endregion

        #region Layout

        /// <summary> The number of kill/eat counters that can fit on one row. </summary>
        public readonly int CountersPerRow;

        /// <summary> If this cycle has any kills. </summary>
        public readonly bool HasKills;

        /// <summary> The number of rows needed to display all kills. </summary>
        public readonly int KillRows;

        /// <summary> If this cycle has any eats. </summary>
        public readonly bool HasEats;

        /// <summary> The number of row needed to display all eats. </summary>
        public readonly int EatRows;

        /// <summary> The max height needed to fully display this element. </summary>
        public readonly float MaxHeight;

        /// <summary> The amount of vertical space this element has within its parent. </summary>
        public float AvailableHeight;

        /// <summary> If this element is currently visible. </summary>
        public bool IsVisible;

        #endregion

        #endregion

        #region Auto-Properties

        /// <summary> If this element can be hovered over in the menu. </summary>
        public override bool CurrentlySelectableNonMouse => !buttonBehav.greyedOut;

        #endregion

        /// <summary>
        ///     Create a new cycle detail box. TODO Info
        /// </summary>
        /// <param name="menu"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="owner"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="pos"><inheritdoc cref="Base.MenuElementBase(Menu, MenuObject, Vector2)"/></param>
        /// <param name="size">
        ///     The size of this element.
        /// </param>
        /// <param name="cycle">
        ///     The statistics for the cycle.
        /// </param>
        [SuppressMessage("ReSharper", "PossibleLossOfFraction")]
        public CycleDetail(
            global::Menu.Menu menu,
            MenuObject owner,
            Vector2 pos,
            Vector2 size,
            Cycle cycle)
            : base(menu, owner, pos, size)
        {
            //Size = size;
            Cycle = cycle;

            // Border
            subObjects.Add(Border = new RoundedRect(
                menu,
                this,
                new Vector2(0f, 0f),
                size,
                true));

            // Title
            subObjects.Add(Title = new MenuLabel(
                this.menu,
                this,
                $"Cycle {cycle?.CycleNumber}",
                new Vector2(0f, 0f),
                default,
                true));
            Title.pos = new Vector2(
                Title.label.textRect.width / 2 + 15f,
                size.y - 20f);

            // General Info
            subObjects.Add(TotalScore = new MenuLabel(
                this.menu,
                this,
                $"Score: {cycle?.Statistics?.TotalScore}",
                new Vector2(0f, 0f),
                default,
                false));
            TotalScore.pos = new Vector2(
                TotalScore.label.textRect.width / 2 + 15f,
                size.y - 45f);
            TotalScore.label.color = global::Menu.Menu.MenuRGB(global::Menu.Menu.MenuColors.MediumGrey);
            subObjects.Add(CycleScore = new MenuLabel(
                this.menu,
                this,
                (cycle?.Statistics?.CycleScore >= 0 ? "(+" : "(") +
                $"{cycle?.Statistics?.CycleScore})",
                new Vector2(0f, 0f),
                default,
                false));
            CycleScore.pos = new Vector2(
                TotalScore.label.textRect.width + CycleScore.label.textRect.width / 2 + 20f,
                size.y - 45f);
            CycleScore.label.color = cycle?.Statistics?.CycleScore > 0
                ? StatisticsMenu.MenuGreen
                : StatisticsMenu.MenuRed;

            subObjects.Add(TotalTime = new MenuLabel(
                this.menu,
                this,
                $"Time: {Custom.SecondsToMinutesAndSecondsString(cycle?.Statistics?.TotalTimeAlive ?? 0)}",
                new Vector2(0f, 0f),
                default,
                false));
            TotalTime.pos = new Vector2(
                TotalTime.label.textRect.width / 2 + 15f,
                size.y - 60f);
            TotalTime.label.color = global::Menu.Menu.MenuRGB(global::Menu.Menu.MenuColors.MediumGrey);
            subObjects.Add(CycleTime = new MenuLabel(
                this.menu,
                this,
                $"(+{Custom.SecondsToMinutesAndSecondsString(cycle?.Statistics?.CycleTimeAlive ?? 0)})",
                new Vector2(0f, 0f),
                default,
                false));
            CycleTime.pos = new Vector2(
                TotalTime.label.textRect.width
                + CycleTime.label.textRect.width / 2
                + 20f,
                size.y - 60f);
            CycleTime.label.color = global::Menu.Menu.MenuRGB(global::Menu.Menu.MenuColors.MediumGrey);

            // Kills
            CountersPerRow = ((int)size.x - 50) / (int)CountWidth;

            if (cycle?.Statistics?.Kills?.Count > 0)
            {
                HasKills = true;

                subObjects.Add(KillsSymbol = new PlayerResultBox.SymbolAndLabel(
                    this.menu,
                    this,
                    new Vector2(25f, size.y - 90f),
                    "Multiplayer_Bones",
                    " ~",
                    -32f));

                KillRows = cycle.Statistics.Kills.Count / CountersPerRow;
                if (cycle.Statistics.Kills.Count % CountersPerRow > 0)
                    KillRows++;
                KillCounters = new List<SymbolCounter>();
                for (int i = 0; i < cycle.Statistics.Kills.Count; i++)
                {
                    KillData kill = cycle.Statistics.Kills[i];

                    SymbolCounter counter = new SymbolCounter(
                        menu,
                        this,
                        new Vector2(
                            CountWidth * ((i + 1) % CountersPerRow)
                            + 25f,
                            size.y
                            - CountHeight * ((i + 1) / CountersPerRow)
                            - 90f),
                        kill.CreatureType,
                        AbstractPhysicalObject.AbstractObjectType.Creature,
                        kill.IntData,
                        kill.KillCount,
                        0);
                    subObjects.Add(counter);
                    KillCounters.Add(counter);
                }
            }

            // Eats
            if (cycle?.Statistics?.Eats?.Count > 0)
            {
                HasEats = true;

                subObjects.Add(EatsSymbol = new PlayerResultBox.SymbolAndLabel(
                    this.menu,
                    this,
                    new Vector2(
                        25f,
                        size.y
                        - CountHeight * KillRows
                        - 90f),
                    "Multiplayer_Bones",
                    " ~",
                    -32f));

                EatRows = cycle.Statistics.Eats.Count / CountersPerRow;
                if (cycle.Statistics.Eats.Count % CountersPerRow > 0)
                    EatRows++;
                EatCounters = new List<SymbolCounter>();

                for (int i = 0; i < cycle.Statistics.Eats.Count; i++)
                {
                    EatData eat = cycle.Statistics.Eats[i];

                    SymbolCounter counter = new SymbolCounter(
                        menu,
                        this,
                        new Vector2(
                            CountWidth * ((i + 1) % CountersPerRow)
                            + 25f,
                            size.y
                            - CountHeight * ((i + 1) / CountersPerRow)
                            - CountHeight * KillRows
                            - 90f),
                        eat.CreatureType,
                        eat.ObjectType,
                        eat.IntData,
                        eat.EatCount / 4,
                        eat.EatCount % 4);
                    subObjects.Add(counter);
                    EatCounters.Add(counter);
                }
            }

            AvailableHeight = MaxHeight = size.y;
            IsVisible = true;
            SetOpacity(0f);
            Hide();
        }

        /// <inheritdoc cref="MenuElementBase.Update"/>
        public override void Update()
        {
            base.Update();

            // Selection effect
            Border.addSize =
                new Vector2(10f, 6f)
                * (buttonBehav.sizeBump + 0.5f * Mathf.Sin(buttonBehav.extraSizeBump * 3.1415927f))
                * (buttonBehav.clicked ? 0f : 1f);
        }

        /// <inheritdoc cref="MenuElementBase.GrafUpdate"/>
        public override void GrafUpdate(float timeStacker)
        {
            base.GrafUpdate(timeStacker);

            // Update opacity of elements if space is sparse.
            float availableSpace = Mathf.Min(MaxHeight, AvailableHeight);
            float opacity = availableSpace / MaxHeight;
            SetOpacity(opacity);
        }

        #region Visibility

        /// <inheritdoc/>
        public void Show()
        {
            if (IsVisible)
                return;

            buttonBehav.greyedOut = false;

            Border.sprites.ToList().ForEach(s => s.isVisible = true);
            Title.label.isVisible = true;
            TotalScore.label.isVisible = true;
            CycleScore.label.isVisible = true;
            TotalTime.label.isVisible = true;
            CycleTime.label.isVisible = false;

            if (HasKills)
            {
                KillsSymbol.menuLabel.label.isVisible = true;
                KillsSymbol.symbol.symbolSprite.isVisible = true;
                KillCounters.ForEach(c => c.Show());
            }

            if (HasEats)
            {
                EatsSymbol.menuLabel.label.isVisible = true;
                EatsSymbol.symbol.symbolSprite.isVisible = true;
                EatCounters.ForEach(c => c.Show());
            }

            IsVisible = true;
        }

        /// <inheritdoc/>
        public void Hide()
        {
            if (!IsVisible)
                return;

            buttonBehav.greyedOut = true;

            Border.sprites.ToList().ForEach(s => s.isVisible = false);
            Title.label.isVisible = false;
            TotalScore.label.isVisible = false;
            CycleScore.label.isVisible = false;
            TotalTime.label.isVisible = false;
            CycleTime.label.isVisible = false;

            if (HasKills)
            {
                KillsSymbol.menuLabel.label.isVisible = false;
                KillsSymbol.symbol.symbolSprite.isVisible = false;
                KillCounters.ForEach(c => c.Hide());
            }

            if (HasEats)
            {
                EatsSymbol.menuLabel.label.isVisible = false;
                EatsSymbol.symbol.symbolSprite.isVisible = false;
                EatCounters.ForEach(c => c.Hide());
            }

            IsVisible = false;
        }

        /// <inheritdoc/>
        public void SetOpacity(float opacity)
        {
            Border.sprites.ToList().ForEach(s => s.alpha = opacity);
            Title.label.alpha = opacity;
            TotalScore.label.alpha = opacity;
            CycleScore.label.alpha = opacity;
            TotalTime.label.alpha = opacity;
            CycleTime.label.alpha = opacity;

            if (HasKills)
            {
                KillsSymbol.menuLabel.label.alpha = opacity;
                KillsSymbol.symbol.symbolSprite.alpha = opacity;
                KillCounters.ForEach(c => c.SetOpacity(opacity));
            }

            if (!HasEats) return;
            {
                EatsSymbol.menuLabel.label.alpha = opacity;
                EatsSymbol.symbol.symbolSprite.alpha = opacity;
                EatCounters.ForEach(c => c.SetOpacity(opacity));
            }
        }

        #endregion
    }
}