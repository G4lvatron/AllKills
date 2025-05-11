using BepInEx;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Menu;
using MonoMod.Cil;
using UnityEngine;
using System;
using AllKills.Menu;
using Mono.Cecil.Cil;

namespace AllKills
{
    /// <summary>
    ///     The main class for the Show All Kills mod.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [BepInPlugin(ModGuid, ModName, ModVersion)]
    public class AllKillsMain : BaseUnityPlugin
    {
        #region Constants

        public const string ModGuid = "galva.all_kills";
        public const string ModName = "Show All Kills";
        public const string ModVersion = "1.0.3";

        #endregion

        #region Private Properties

        /// <summary>
        ///     The handler for the statistics menu on the sleep and death screen.
        /// </summary>
        private readonly SleepAndDeathScreenStatistics _statistics = new SleepAndDeathScreenStatistics();

        #endregion

        /// <summary>
        ///     The entry point for the mod initialization. All hooking is done here.
        /// </summary>
        public void OnEnable()
        {
            // Earn trophy IL hooks
            IL.Menu.StoryGameStatisticsScreen.KillsTable.ctor += ILHook_EarnTrophy;
            IL.Menu.SleepScreenKills.ctor += ILHook_EarnTrophy;

            // Show all Kills
            On.Menu.StoryGameStatisticsScreen.TickerIsDone += Hook_TickerIsDone;
            On.Menu.StoryGameStatisticsScreen.KillsTable.KillSort += Hook_KillSort;

            // Resources
            On.RainWorld.LoadModResources += Hook_LoadModResources;
            On.RainWorld.UnloadResources += Hook_UnloadResources;
            On.CreatureSymbol.SpriteNameOfCreature += Hook_SpriteNameOfCreature;

            // SleepAndDeathScreenStatistics
            _statistics.Attach();
        }

        #region Show All Kills

        /// <summary>
        ///     IL Hook: Declare all creatures as awarding a trophy. Will apply directly after <see cref="CreatureSymbol.DoesCreatureEarnATrophy"/>.
        /// </summary>
        /// <param name="context">
        ///     The intermediate language context.
        /// </param>
        private static void ILHook_EarnTrophy(ILContext context)
        {
            ILCursor cursor = new ILCursor(context);

            cursor.GotoNext(
                x => x.MatchCall<CreatureSymbol>("DoesCreatureEarnATrophy")
            );
            cursor.Index += 1;
            cursor.Emit(OpCodes.Pop);
            cursor.Emit(OpCodes.Ldc_I4_1);
        }

        /// <summary>
        ///     Hook: This will return scores or 0 for creatures that do not have a kill score specified. This prevents the game from getting stuck
        ///     trying to find the score for creatures that do not award any.
        /// </summary>
        /// <param name="orig">
        ///     The original method being hooked to which is called once a creature counter on the statistics screen has finished counting up.
        ///     This method is called to add the score to the score adder on the right side of the screen.
        /// </param>
        /// <param name="self">
        ///     The statistics screen instance.
        /// </param>
        /// <param name="ticker">
        ///     The ticker that just finished counting up. This will contain the creature data.
        /// </param>
        private static void Hook_TickerIsDone(
            On.Menu.StoryGameStatisticsScreen.orig_TickerIsDone orig,
            StoryGameStatisticsScreen self,
            StoryGameStatisticsScreen.Ticker ticker)
        {
            if (ticker.ID == StoryGameStatisticsScreen.TickerID.Kill)
            {
                IconSymbol.IconSymbolData? iconData =
                    (ticker as StoryGameStatisticsScreen.KillsTable.KillTicker)?.symbol.iconData;

                if (iconData != null)
                {
                    if ((int)MultiplayerUnlocks.SandboxUnlockForSymbolData(iconData.Value) >= self.killScores.Length)
                    {
                        self.scoreKeeper.AddScoreAdder(0, ticker.getToValue);
                        return;
                    }
                }
            }

            orig(self, ticker);
        }

        /// <summary>
        ///     Hook: This will sort the kills in a more sensible order. The original kill sorter will sort the kills based on arena ID which doesn't
        ///     group creatures in a way that entirely makes sense.
        /// </summary>
        /// <param name="orig">
        ///     The original method being hooked to which will be replaced with the new sorting method from this mod.
        /// </param>
        /// <param name="tickerA"> The first ticker. </param>
        /// <param name="tickerB"> The second ticker. </param>
        /// <returns>
        ///     <c>true</c> if <c>tickerA</c> is before <c>tickerB</c>, <c>false</c> otherwise.
        /// </returns>
        private static bool Hook_KillSort(
            On.Menu.StoryGameStatisticsScreen.KillsTable.orig_KillSort orig,
            StoryGameStatisticsScreen.KillsTable.KillTicker tickerA,
            StoryGameStatisticsScreen.KillsTable.KillTicker tickerB)
        {
            return CreatureSort.GetCreatureValue(tickerA.symbol.iconData) <
                   CreatureSort.GetCreatureValue(tickerB.symbol.iconData);
        }

        #endregion

        #region Resources

        /// <summary>
        ///     Hook: Load the texture atlas for the custom textures from this mod.
        /// </summary>
        /// <param name="orig">
        ///     The original method for loading resources.
        /// </param>
        /// <param name="self">
        ///     The rain world instance.
        /// </param>
        private static void Hook_LoadModResources(On.RainWorld.orig_LoadModResources orig, RainWorld self)
        {
            // ReSharper disable once StringLiteralTypo
            Futile.atlasManager.LoadAtlas("Atlases/iconsak");
            orig(self);
        }

        /// <summary>
        ///     Hook: Unload the texture atlas for the custom textures from this mod.
        /// </summary>
        /// <param name="orig">
        ///     The original method for unloading resources.
        /// </param>
        /// <param name="self">
        ///     The rain world instance.
        /// </param>
        private static void Hook_UnloadResources(On.RainWorld.orig_UnloadResources orig, RainWorld self)
        {
            // ReSharper disable once StringLiteralTypo
            Futile.atlasManager.UnloadAtlas("Atlases/iconsak");
            orig(self);
        }

        /// <summary>
        ///     Hook: Lets the game find the sprites for the newly added creature textures.
        /// </summary>
        /// <param name="orig">
        ///     The original method that finds the sprite name for a creature.
        /// </param>
        /// <param name="iconData">
        ///     The data of the creature whose sprite name should be returned.
        /// </param>
        /// <returns>
        ///     The sprite name of any newly added sprite when applicable, otherwise the original return value.
        /// </returns>
        private static string Hook_SpriteNameOfCreature(On.CreatureSymbol.orig_SpriteNameOfCreature orig,
            IconSymbol.IconSymbolData iconData)
        {
            if (iconData.critType == CreatureTemplate.Type.TempleGuard)
                return "Kill_Guard";

            return iconData.critType == CreatureTemplate.Type.SmallCentipede
                ? "Kill_SmallCentipede"
                : orig(iconData);
        }

        #endregion
    }
}