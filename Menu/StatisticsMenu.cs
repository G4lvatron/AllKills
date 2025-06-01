using System.Collections.Generic;
using AllKills.Menu.StatisticsData;
using AllKills.Menu.UIComponents;
using Menu;
using UnityEngine;

namespace AllKills.Menu
{
    public class StatisticsMenu : PositionedMenuObject
    {
        #region Constants

        public static Color MenuRed = new Color(153f / 255f, 0f, 0f);

        public static Color MenuGreen = new Color(37f / 255f, 142f / 255f, 37f / 255f);

        #endregion

        /// <summary>
        ///     Create the All Kills statistics menu.
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
        public StatisticsMenu(StatisticsDialog menu, MenuObject owner, Vector2 pos) : base(menu, owner, pos)
        {
            // Setup
            this.menu = menu;

            CycleList list = new CycleList(
                menu,
                this,
                new Vector2(100f, 100f),
                new Vector2(600f, 600f),
                new CampaignStatistics
                {
                    Cycles = new List<Cycle>
                    {
                        new Cycle
                        {
                            CycleNumber = 34,
                            Statistics = new CycleStatistics
                            {
                                TotalScore = 11647,
                                CycleScore = 2456,
                                TotalTimeAlive = 12273,
                                CycleTimeAlive = 337,
                                Kills = new List<KillData>
                                {
                                    new KillData
                                    {
                                        CreatureType = CreatureTemplate.Type.PinkLizard,
                                        KillCount = 3
                                    },
                                    new KillData
                                    {
                                        CreatureType = CreatureTemplate.Type.BlackLizard,
                                        KillCount = 4
                                    },
                                    new KillData
                                    {
                                        CreatureType = CreatureTemplate.Type.Scavenger,
                                        KillCount = 2
                                    }
                                },
                                Eats = new List<EatData>
                                {
                                    new EatData
                                    {
                                        ObjectType = AbstractPhysicalObject.AbstractObjectType.DangleFruit,
                                        EatCount = 12
                                    }
                                }
                            }
                        },
                        new Cycle
                        {
                            CycleNumber = 33,
                            Statistics = new CycleStatistics
                            {
                                TotalScore = 11647 - 2456,
                                CycleScore = 123,
                                TotalTimeAlive = 12273 - 337,
                                CycleTimeAlive = 25,
                                Kills = new List<KillData>
                                {
                                    new KillData
                                    {
                                        CreatureType = CreatureTemplate.Type.CyanLizard,
                                        KillCount = 1
                                    },
                                    new KillData
                                    {
                                        CreatureType = CreatureTemplate.Type.LanternMouse,
                                        KillCount = 11
                                    }
                                },
                                Eats = new List<EatData>
                                {
                                    new EatData
                                    {
                                        ObjectType = AbstractPhysicalObject.AbstractObjectType.SeedCob,
                                        EatCount = 16
                                    }
                                }
                            }
                        },
                        new Cycle
                        {
                            CycleNumber = 32,
                            Statistics = new CycleStatistics
                            {
                                TotalScore = 11647 - 2456 - 123,
                                CycleScore = 345,
                                TotalTimeAlive = 12273 - 337 - 25,
                                CycleTimeAlive = 143,
                                Kills = new List<KillData>
                                {
                                    new KillData
                                    {
                                        CreatureType = CreatureTemplate.Type.Centiwing,
                                        KillCount = 1
                                    },
                                    new KillData
                                    {
                                        CreatureType = CreatureTemplate.Type.PoleMimic,
                                        KillCount = 1
                                    },
                                    new KillData
                                    {
                                        CreatureType = CreatureTemplate.Type.BigNeedleWorm,
                                        KillCount = 3
                                    }
                                },
                                Eats = new List<EatData>
                                {
                                    new EatData
                                    {
                                        ObjectType = AbstractPhysicalObject.AbstractObjectType.SlimeMold,
                                        EatCount = 12
                                    }
                                }
                            }
                        }
                    }
                }
            );
            subObjects.Add(list);

            PlaythroughDetail detail = new PlaythroughDetail(
                menu,
                this,
                new Vector2(720f, 100f),
                new Vector2(600f, 600f),
                null);
            subObjects.Add(detail);

            AddScrollBox();
        }

        private void AddScrollBox()
        {
        }
    }
}