using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AllKills.Menu.StatisticsData;
using AllKills.Menu.UIComponents.Base;
using Menu;
using RWCustom;
using UnityEngine;

namespace AllKills.Menu.UIComponents
{
    public class PlaythroughDetail : ScrollBox
    {
        #region Properties

        /// <summary> The width of a kill/eat counter. </summary>
        public const float CountWidth = 70f;

        /// <summary> The height of a kill/eat counter. </summary>
        public const float CountHeight = 30f;

        ///<inheritdoc />
        public override float ContentHeight { get; }

        #region Sub-Objects

        /// <summary> The character label. </summary>
        public readonly PlayThroughDetailLabel Character;

        /// <summary> The total score label. </summary>
        public readonly PlayThroughDetailLabel TotalScore;

        /// <summary> The total time label. </summary>
        public readonly PlayThroughDetailLabel TotalTime;

        /// <summary> The total cycles label. </summary>
        public readonly PlayThroughDetailLabel TotalCycles;

        /// <summary> The total deaths label. </summary>
        public readonly PlayThroughDetailLabel TotalDeaths;

        /// <summary> The total quits label. </summary>
        public readonly PlayThroughDetailLabel TotalQuits;

        /// <summary> The total kills label. </summary>
        public readonly PlayThroughDetailLabel TotalKills;

        /// <summary> The total eats label. </summary>
        public readonly PlayThroughDetailLabel TotalEats;

        /// <summary> The kill icons. </summary>
        public readonly List<PlaythroughDetailIconInfo<KillData>> KillIcons;

        /// <summary> The eat icons. </summary>
        public readonly List<PlaythroughDetailIconInfo<EatData>> EatIcons;

        #endregion

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
        /// <param name="campaign">
        ///     The campaign statistics.
        /// </param>
        public PlaythroughDetail(
            global::Menu.Menu menu,
            MenuObject owner,
            Vector2 pos,
            Vector2 size,
            Campaign campaign)
            : base(menu, owner, pos, size)
        {
            // Character
            string characterName;
            SlugcatStats.Name slugcat = campaign.Character;
            if (slugcat == SlugcatStats.Name.White)
                characterName = "Survivor";
            else if (slugcat == SlugcatStats.Name.Yellow)
                characterName = "Monk";
            else if (slugcat == SlugcatStats.Name.Red)
                characterName = "Hunter";
            else if (slugcat == SlugcatStats.Name.Night)
                characterName = "Watcher";
            else
                characterName = Regex.Replace(slugcat.ToString(), "(?<!^)(?=[A-Z])", " ");

            Character = new PlayThroughDetailLabel(
                menu,
                this,
                size.x,
                "Character:",
                $"{characterName}");
            subObjects.Add(Character);
            Content.Add(Character);

            TotalScore = new PlayThroughDetailLabel(
                menu,
                this,
                size.x,
                "Score:",
                $"{(ModManager.MSC ? campaign.Statistics.TotalScoreMsc : campaign.Statistics.TotalScore)}");
            subObjects.Add(TotalScore);
            Content.Add(TotalScore);

            TotalTime = new PlayThroughDetailLabel(
                menu,
                this,
                size.x,
                "Time:",
                $"{Custom.SecondsToMinutesAndSecondsString(campaign.Statistics.TotalTimeAlive + campaign.Statistics.TotalTimeDead)}");
            subObjects.Add(TotalTime);
            Content.Add(TotalTime);

            int totalCycles = campaign.Statistics.Cycles.Aggregate(0, (t, c) =>
            {
                if (c.EndCycleNumber > t)
                    return c.EndCycleNumber.Value;
                return c.CycleNumber > t ? c.CycleNumber : t;
            });
            TotalCycles = new PlayThroughDetailLabel(
                menu,
                this,
                size.x,
                "Cycles:",
                $"{totalCycles}");
            subObjects.Add(TotalCycles);
            Content.Add(TotalCycles);

            int iconsPerRow = (int)size.x / (int)CountWidth;

            KillIcons = new List<PlaythroughDetailIconInfo<KillData>>();
            for (int i = 0; i < campaign.Statistics.TotalKills.Count;)
            {
                if (i == 0)
                {
                    PlaythroughDetailIconInfo<KillData> info = new PlaythroughDetailIconInfo<KillData>(
                        menu,
                        this,
                        campaign.Statistics.TotalKills.Take(iconsPerRow - 1).ToList(),
                        isStartKills: true);
                    subObjects.Add(info);
                    Content.Add(info);

                    i += iconsPerRow - 1;
                }
                else
                {
                    PlaythroughDetailIconInfo<KillData> info = new PlaythroughDetailIconInfo<KillData>(
                        menu,
                        this,
                        campaign.Statistics.TotalKills.Skip(i).Take(iconsPerRow).ToList());
                    subObjects.Add(info);
                    Content.Add(info);

                    i += iconsPerRow;
                }
            }

            Content.Add(new ScrollBoxElement.Spacer(menu, this, 10f));

            EatIcons = new List<PlaythroughDetailIconInfo<EatData>>();
            for (int i = 0; i < campaign.Statistics.TotalEats.Count;)
            {
                if (i == 0)
                {
                    PlaythroughDetailIconInfo<EatData> info = new PlaythroughDetailIconInfo<EatData>(
                        menu,
                        this,
                        campaign.Statistics.TotalEats.Take(iconsPerRow - 1).ToList(),
                        isStartEats: true);
                    subObjects.Add(info);
                    Content.Add(info);

                    i += iconsPerRow - 1;
                }
                else
                {
                    PlaythroughDetailIconInfo<EatData> info = new PlaythroughDetailIconInfo<EatData>(
                        menu,
                        this,
                        campaign.Statistics.TotalEats.Skip(i).Take(iconsPerRow).ToList());
                    subObjects.Add(info);
                    Content.Add(info);

                    i += iconsPerRow;
                }
            }

            AddScrollButtons();

            ContentHeight =
                Content.Aggregate(0f, (n, c) => n + c.GetElementHeight())
                + 5f * (Content.Count - 1)
                + 30f;
        }

        /// <inheritdoc/>
        public override void Update()
        {
            base.Update();
        }

        /// <inheritdoc/>
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
            /// <summary> The left label. </summary>
            public readonly MenuLabel LeftLabel;

            /// <summary> The right label. </summary>
            public readonly MenuLabel RightLabel;

            /// <summary>
            ///     Create a new detail label.
            /// </summary>
            /// <param name="menu"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
            /// <param name="owner"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
            /// <param name="width">
            ///     The width of the label.
            /// </param>
            /// <param name="leftText">
            ///     The left text for this label.
            /// </param>
            /// <param name="rightText">
            ///     The right text for this label.
            /// </param>
            public PlayThroughDetailLabel(
                global::Menu.Menu menu,
                MenuObject owner,
                float width,
                string leftText,
                string rightText)
                : base(menu, owner, 30f)
            {
                subObjects.Add(LeftLabel = new MenuLabel(
                    menu,
                    this,
                    leftText,
                    new Vector2(10f, 0f),
                    new Vector2(30f, 60f),
                    true));
                LeftLabel.pos = new Vector2(
                    LeftLabel.label.textRect.width / 2 + 10f,
                    0f);
                LeftLabel.label.color = global::Menu.Menu.MenuRGB(global::Menu.Menu.MenuColors.MediumGrey);

                subObjects.Add(RightLabel = new MenuLabel(
                    menu,
                    this,
                    rightText,
                    new Vector2(10f, 0f),
                    new Vector2(30f, 60f),
                    true));
                RightLabel.pos = new Vector2(
                    width - RightLabel.label.textRect.width / 2f - 10f,
                    0f);
            }

            /// <inheritdoc/>
            public override void Show()
            {
                LeftLabel.label.isVisible = true;
                RightLabel.label.isVisible = true;
            }

            /// <inheritdoc/>
            public override void Hide()
            {
                LeftLabel.label.isVisible = false;
                RightLabel.label.isVisible = false;
            }

            /// <inheritdoc/>
            public override void SetOpacity(float opacity)
            {
                LeftLabel.label.alpha = opacity;
                RightLabel.label.alpha = opacity;
            }
        }

        #endregion

        #region IconInfoRow

        public class PlaythroughDetailIconInfo<T> : ScrollBoxElement
        {
            /// <summary> The start symbol. </summary>
            public readonly PlayerResultBox.SymbolAndLabel StartSymbol;

            /// <summary> The counters. </summary>
            public readonly List<SymbolCounter> Counters;

            /// <summary>
            ///     Create a new row of icons.
            /// </summary>
            /// <param name="menu"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
            /// <param name="owner"><inheritdoc cref="MenuElementBase(Menu, MenuObject, Vector2)"/></param>
            /// <param name="icons">
            ///     The icons.
            /// </param>
            /// <param name="isStartKills">
            ///     If set to <c>true</c> start with a kills icon.
            /// </param>
            /// <param name="isStartEats">
            ///     If set to <c>true</c> start with an eats icon.
            /// </param>
            public PlaythroughDetailIconInfo(
                global::Menu.Menu menu,
                MenuObject owner,
                List<T> icons,
                bool isStartKills = false,
                bool isStartEats = false)
                : base(menu, owner, CountHeight)
            {
                if (isStartKills)
                {
                    subObjects.Add(StartSymbol = new PlayerResultBox.SymbolAndLabel(
                        this.menu,
                        this,
                        new Vector2(35f, CountHeight / 2),
                        "Multiplayer_Bones",
                        " ~",
                        -32f));
                }
                else if (isStartEats)
                {
                    subObjects.Add(StartSymbol = new PlayerResultBox.SymbolAndLabel(
                        this.menu,
                        this,
                        new Vector2(35f, CountHeight / 2),
                        "Stat_Eats",
                        " ~",
                        -32f));
                }

                Counters = new List<SymbolCounter>();
                for (int i = 0; i < icons.Count; i++)
                {
                    T detail = icons[i];
                    SymbolCounter counter;

                    if (detail.GetType() == typeof(KillData))
                    {
                        KillData kill = (KillData)(object)detail;

                        counter = new SymbolCounter(
                            menu,
                            this,
                            new Vector2(35f + (isStartKills || isStartEats ? i + 1 : i) * 70f, CountHeight / 2),
                            kill.CreatureType,
                            AbstractPhysicalObject.AbstractObjectType.Creature,
                            kill.IntData,
                            kill.KillCount,
                            0);
                    }
                    else if (detail.GetType() == typeof(EatData))
                    {
                        EatData eat = (EatData)(object)detail;

                        counter = new SymbolCounter(
                            menu,
                            this,
                            new Vector2(35f + (isStartKills || isStartEats ? i + 1 : i) * 70f, CountHeight / 2),
                            eat.CreatureType,
                            eat.ObjectType,
                            eat.IntData,
                            eat.EatCount / 4,
                            eat.EatCount % 4);
                    }
                    else
                    {
                        Debug.Log("Invalid object!!");
                        continue;
                    }

                    subObjects.Add(counter);
                    Counters.Add(counter);
                }
            }

            /// <inheritdoc/>
            public override void Show()
            {
                if (StartSymbol != null)
                {
                    StartSymbol.symbol.symbolSprite.isVisible = true;
                    StartSymbol.menuLabel.label.isVisible = true;
                }

                Counters.ForEach(c => c.Show());
            }

            /// <inheritdoc/>
            public override void Hide()
            {
                if (StartSymbol != null)
                {
                    StartSymbol.symbol.symbolSprite.isVisible = false;
                    StartSymbol.menuLabel.label.isVisible = false;
                }

                Counters.ForEach(c => c.Hide());
            }

            /// <inheritdoc/>
            public override void SetOpacity(float opacity)
            {
                if (StartSymbol != null)
                {
                    StartSymbol.symbol.symbolSprite.alpha = opacity;
                    StartSymbol.menuLabel.label.alpha = opacity;
                }

                Counters.ForEach(c => c.SetOpacity(opacity));
            }
        }

        #endregion
    }
}