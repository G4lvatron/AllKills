using BepInEx;
using System.Collections.Generic;
using UnityEngine;

namespace AllKills
{
    /// <summary>
    ///     The main class for All Kills.
    /// </summary>
    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]
    public class AllKillsMain : BaseUnityPlugin
    {
        public const string MOD_GUID = "galva.all_kills";
        public const string MOD_NAME = "Show All Kills";
        public const string MOD_VERSION = "0.0.0";

        private bool InStatisticsScreen = false;

        private CreatureTemplate.Type _previousType = null;
        private int _previousIntData = 0;

        /// <summary>
        ///     Run when mod is enabled.
        /// </summary>
        public void OnEnable()
        {
            On.CreatureSymbol.DoesCreatureEarnATrophy += EarnTrophyCheckHook;
            On.Menu.StoryGameStatisticsScreen.KillsTable.KillSort += KillSortHook;

            On.Menu.SleepScreenKills.ctor += SleepScreenConstructorHook;
            On.Menu.StoryGameStatisticsScreen.KillsTable.ctor += KillsTableConstructorHook;

            On.Menu.StoryGameStatisticsScreen.TickerIsDone += DoneHook;

            On.RainWorld.LoadModResources += ResourceLoadHook;
            On.RainWorld.UnloadResources += ResourceUnloadHook;
            On.CreatureSymbol.SpriteNameOfCreature += SpriteNameHook;
        }

        private bool KillSortHook(
            On.Menu.StoryGameStatisticsScreen.KillsTable.orig_KillSort orig,
            Menu.StoryGameStatisticsScreen.KillsTable.KillTicker a,
            Menu.StoryGameStatisticsScreen.KillsTable.KillTicker b)
        {
            return CreatureSort.GetCreatureValue(a.symbol.iconData) < CreatureSort.GetCreatureValue(b.symbol.iconData);
        }

        /// <summary>
        ///     Hook for loading mod texture atlas.
        /// </summary>
        private void ResourceLoadHook(On.RainWorld.orig_LoadModResources orig, RainWorld self)
        {
            Futile.atlasManager.LoadAtlas("Atlases/iconsak");
            orig(self);
        }

        /// <summary>
        ///     Hook for unloading mod texture atlas.
        /// </summary>
        private void ResourceUnloadHook(On.RainWorld.orig_UnloadResources orig, RainWorld self)
        {
            Futile.atlasManager.UnloadAtlas("Atlases/iconsak");
            orig(self);
        }

        /// <summary>
        ///     Hook for finding custom creature kill icons.
        /// </summary>
        /// <returns> The icon. </returns>
        private string SpriteNameHook(On.CreatureSymbol.orig_SpriteNameOfCreature orig, IconSymbol.IconSymbolData iconData)
        {
            if (iconData.critType == CreatureTemplate.Type.TempleGuard)
                return "Kill_Guard";
            if (iconData.critType == CreatureTemplate.Type.SmallCentipede)
                return "Kill_SmallCentipede";

            return orig(iconData);
        }

        /// <summary>
        ///     Hook for activating logic on the sleep screen.
        /// </summary>
        private void SleepScreenConstructorHook(
            On.Menu.SleepScreenKills.orig_ctor orig,
            Menu.SleepScreenKills self,
            Menu.Menu menu,
            Menu.MenuObject owner,
            Vector2 pos,
            List<PlayerSessionRecord.KillRecord> killsData)
        {
            InStatisticsScreen = true;
            orig(self, menu, owner, pos, killsData);
            InStatisticsScreen = false;
        }

        /// <summary>
        ///     Hook for activating logic on the final kill count table. </summary>
        private void KillsTableConstructorHook(
            On.Menu.StoryGameStatisticsScreen.KillsTable.orig_ctor orig,
            Menu.StoryGameStatisticsScreen.KillsTable self,
            Menu.Menu menu,
            Menu.MenuObject owner,
            Vector2 pos,
            List<KeyValuePair<IconSymbol.IconSymbolData, int>> killsData)
        {
            InStatisticsScreen = true;
            orig(self, menu, owner, pos, killsData);
            InStatisticsScreen = false;

            _previousType = null;
            _previousIntData = 0;
        }

        /// <summary>
        ///     Hook for returning scores of added creatures. Prevents a possible softlock on the results screen.
        /// </summary>
        private void DoneHook(On.Menu.StoryGameStatisticsScreen.orig_TickerIsDone orig, Menu.StoryGameStatisticsScreen self, Menu.StoryGameStatisticsScreen.Ticker ticker)
        {
            if(ticker.ID == Menu.StoryGameStatisticsScreen.TickerID.Kill)
            {
                IconSymbol.IconSymbolData iconData = (ticker as Menu.StoryGameStatisticsScreen.KillsTable.KillTicker).symbol.iconData;
                if (iconData.critType == CreatureTemplate.Type.VultureGrub ||
                    iconData.critType == CreatureTemplate.Type.Hazer ||
                    iconData.critType == CreatureTemplate.Type.TubeWorm ||
                    iconData.critType == CreatureTemplate.Type.SmallNeedleWorm ||
                    iconData.critType == CreatureTemplate.Type.SmallCentipede ||
                    iconData.critType == CreatureTemplate.Type.Deer ||
                    iconData.critType == CreatureTemplate.Type.Overseer ||
                    iconData.critType == CreatureTemplate.Type.TempleGuard ||
                    iconData.critType == CreatureTemplate.Type.GarbageWorm ||
                    iconData.critType == CreatureTemplate.Type.StandardGroundCreature ||
                    iconData.critType == CreatureTemplate.Type.Fly ||
                    iconData.critType == CreatureTemplate.Type.Leech ||
                    iconData.critType == CreatureTemplate.Type.SeaLeech ||
                    iconData.critType == CreatureTemplate.Type.Spider ||
                    (ModManager.MSC && iconData.critType == MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.JungleLeech) ||
                    (iconData.critType == _previousType && iconData.intData == _previousIntData)) // Catch-all
                {
                    self.scoreKeeper.AddScoreAdder(0, ticker.getToValue);
                    return;
                }

                _previousType = iconData.critType;
                _previousIntData = iconData.intData;
            }
            orig(self, ticker);
        }

        /// <summary>
        ///     Hook to remove filtering for creature kills.
        /// </summary>
        public bool EarnTrophyCheckHook(On.CreatureSymbol.orig_DoesCreatureEarnATrophy orig, CreatureTemplate.Type creature)
        {
            if (InStatisticsScreen)
                return creature.Index != -1;
            else
                return orig(creature);
        }
    }
}
