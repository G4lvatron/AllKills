using BepInEx;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace AllKills
{
    /// <summary>
    ///     The main class for the Show All Kills mod.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class AllKillsMain : BaseUnityPlugin
    {
        public const string ModGuid = "galva.all_kills";
        public const string ModName = "Show All Kills";
        public const string ModVersion = "1.0.3";

        private bool _inStatisticsScreen;

        private CreatureTemplate.Type _previousType;
        private int _previousIntData;

        /// <summary>
        ///     Run when mod is enabled.
        /// </summary>
        public void OnEnable()
        {
            // Show all Kills
            On.CreatureSymbol.DoesCreatureEarnATrophy += EarnTrophyCheckHook;
            On.Menu.StoryGameStatisticsScreen.TickerIsDone += DoneHook;
            On.Menu.StoryGameStatisticsScreen.KillsTable.KillSort += KillSortHook;

            // Resources
            On.RainWorld.LoadModResources += ResourceLoadHook;
            On.RainWorld.UnloadResources += ResourceUnloadHook;
            On.CreatureSymbol.SpriteNameOfCreature += SpriteNameHook;

            // Enable / Disable
            On.Menu.SleepScreenKills.ctor += SleepScreenConstructorHook;
            On.Menu.StoryGameStatisticsScreen.KillsTable.ctor += KillsTableConstructorHook;
        }

        #region Show All Kills

        /// <summary>
        ///     Hook to remove filtering for creature kills.
        /// </summary>
        ///
        /// <param name="orig"> Reference to original method. </param>
        /// <param name="creature"> The creature template to check. </param>
        ///
        /// <returns>
        ///     If the creature is a trophy kill. For this mod, all creatures with a valid index are accepted.
        /// </returns>
        public bool EarnTrophyCheckHook(On.CreatureSymbol.orig_DoesCreatureEarnATrophy orig,
            CreatureTemplate.Type creature)
        {
            if (_inStatisticsScreen)
                return creature.Index != -1;

            return orig(creature);
        }

        /// <summary>
        ///     Hook for returning scores of added creatures. Prevents a possible soft-lock on the results screen.
        /// </summary>
        private void DoneHook(
            On.Menu.StoryGameStatisticsScreen.orig_TickerIsDone orig,
            Menu.StoryGameStatisticsScreen self,
            Menu.StoryGameStatisticsScreen.Ticker ticker)
        {
            if (ticker.ID == Menu.StoryGameStatisticsScreen.TickerID.Kill)
            {
                IconSymbol.IconSymbolData? iconData =
                    (ticker as Menu.StoryGameStatisticsScreen.KillsTable.KillTicker)?.symbol.iconData;

                if (iconData != null)
                {
                    if (iconData.Value.critType == CreatureTemplate.Type.VultureGrub ||
                        iconData.Value.critType == CreatureTemplate.Type.Hazer ||
                        iconData.Value.critType == CreatureTemplate.Type.TubeWorm ||
                        iconData.Value.critType == CreatureTemplate.Type.SmallNeedleWorm ||
                        iconData.Value.critType == CreatureTemplate.Type.SmallCentipede ||
                        iconData.Value.critType == CreatureTemplate.Type.Deer ||
                        iconData.Value.critType == CreatureTemplate.Type.Overseer ||
                        iconData.Value.critType == CreatureTemplate.Type.TempleGuard ||
                        iconData.Value.critType == CreatureTemplate.Type.GarbageWorm ||
                        iconData.Value.critType == CreatureTemplate.Type.StandardGroundCreature ||
                        iconData.Value.critType == CreatureTemplate.Type.Fly ||
                        iconData.Value.critType == CreatureTemplate.Type.Leech ||
                        iconData.Value.critType == CreatureTemplate.Type.SeaLeech ||
                        iconData.Value.critType == CreatureTemplate.Type.Spider ||
                        (ModManager.MSC && iconData.Value.critType ==
                            DLCSharedEnums.CreatureTemplateType.JungleLeech) ||
                        (iconData.Value.critType == _previousType &&
                         iconData.Value.intData == _previousIntData)) // Catch-all
                    {
                        self.scoreKeeper.AddScoreAdder(0,
                            ticker.getToValue); // The game handles this appropriately by not adding a score adder
                        return;
                    }

                    _previousType = iconData.Value.critType;
                    _previousIntData = iconData.Value.intData;
                }
            }

            orig(self, ticker);
        }

        private static bool KillSortHook(
            On.Menu.StoryGameStatisticsScreen.KillsTable.orig_KillSort orig,
            Menu.StoryGameStatisticsScreen.KillsTable.KillTicker a,
            Menu.StoryGameStatisticsScreen.KillsTable.KillTicker b)
        {
            return CreatureSort.GetCreatureValue(a.symbol.iconData) < CreatureSort.GetCreatureValue(b.symbol.iconData);
        }

        #endregion

        #region Resources

        /// <summary>
        ///     Hook for loading mod texture atlas.
        /// </summary>
        private static void ResourceLoadHook(On.RainWorld.orig_LoadModResources orig, RainWorld self)
        {
            // ReSharper disable once StringLiteralTypo
            Futile.atlasManager.LoadAtlas("Atlases/iconsak");
            orig(self);
        }

        /// <summary>
        ///     Hook for unloading mod texture atlas.
        /// </summary>
        private static void ResourceUnloadHook(On.RainWorld.orig_UnloadResources orig, RainWorld self)
        {
            // ReSharper disable once StringLiteralTypo
            Futile.atlasManager.UnloadAtlas("Atlases/iconsak");
            orig(self);
        }

        /// <summary>
        ///     Hook for finding custom creature kill icons.
        /// </summary>
        /// 
        /// <param name="orig"> Reference to original method. </param>
        /// <param name="iconData"> The creature's icon data. </param>
        /// 
        /// <returns>
        ///     The custom icon if the input is a custom creature, otherwise the result of <c>orig(iconData)</c>.
        /// </returns>
        private static string SpriteNameHook(On.CreatureSymbol.orig_SpriteNameOfCreature orig,
            IconSymbol.IconSymbolData iconData)
        {
            if (iconData.critType == CreatureTemplate.Type.TempleGuard)
                return "Kill_Guard";

            return iconData.critType == CreatureTemplate.Type.SmallCentipede
                ? "Kill_SmallCentipede"
                : orig(iconData);
        }

        #endregion

        #region Enable / Disable

        /// <summary>
        ///     Hook for enabling logic on the sleep screen.
        /// </summary>
        private void SleepScreenConstructorHook(
            On.Menu.SleepScreenKills.orig_ctor orig,
            Menu.SleepScreenKills self,
            Menu.Menu menu,
            Menu.MenuObject owner,
            Vector2 pos,
            List<PlayerSessionRecord.KillRecord> killsData)
        {
            _inStatisticsScreen = true;
            orig(self, menu, owner, pos, killsData);
            _inStatisticsScreen = false;
        }

        /// <summary>
        ///     Hook enabling logic on the final kill count table.
        /// </summary>
        private void KillsTableConstructorHook(
            On.Menu.StoryGameStatisticsScreen.KillsTable.orig_ctor orig,
            Menu.StoryGameStatisticsScreen.KillsTable self,
            Menu.Menu menu,
            Menu.MenuObject owner,
            Vector2 pos,
            List<KeyValuePair<IconSymbol.IconSymbolData, int>> killsData)
        {
            _inStatisticsScreen = true;
            orig(self, menu, owner, pos, killsData);
            _inStatisticsScreen = false;

            _previousType = null;
            _previousIntData = 0;
        }

        #endregion
    }
}