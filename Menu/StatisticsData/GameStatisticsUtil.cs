using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Menu;
using MoreSlugcats;
using UnityEngine;

namespace AllKills.Menu.StatisticsData
{
    public static class GameStatisticsUtil
    {
        /// <summary>
        ///     Add a new entry to a list of kill data and merge the kill counts if applicable.
        /// </summary>
        /// <param name="killList">
        ///     The list of kills.
        /// </param>
        /// <param name="newData">
        ///     The new kill data.
        /// </param>
        public static void AggregateKillData(this List<KillData> killList, KillData newData)
        {
            KillData existingData = killList
                .Find(k =>
                    k.CreatureType == newData.CreatureType &&
                    k.IntData == newData.IntData);

            if (existingData != null)
                existingData.KillCount += newData.KillCount;
            else
                killList.Add(newData);
        }

        /// <summary>
        ///     Adds one list of kills to another.
        /// </summary>
        /// <param name="killList1">
        ///     The first kill list.
        /// </param>
        /// <param name="killList2">
        ///     The second kill list.
        /// </param>
        public static List<KillData> Add(this List<KillData> killList1, List<KillData> killList2)
        {
            List<KillData> result = killList1;
            killList2.ForEach(k => result.AggregateKillData(k));
            return result;
        }

        /// <summary>
        ///     Subtracts one list of kills from another.
        /// </summary>
        /// <param name="killList1">
        ///     The kill list to subtract from.
        /// </param>
        /// <param name="killList2">
        ///     The kills to subtract.
        /// </param>
        public static List<KillData> Subtract(this List<KillData> killList1, List<KillData> killList2)
        {
            return killList1.GroupJoin(
                    killList2,
                    k1 => k1.CreatureType,
                    k2 => k2.CreatureType,
                    (k1, l) => new KillData
                    {
                        CreatureType = k1.CreatureType,
                        IntData = k1.IntData,
                        KillCount = k1.KillCount - l.First(k2 => k2.IntData == k1.IntData).KillCount
                    })
                .Where(k => k.KillCount > 0)
                .ToList();
        }

        /// <summary>
        ///     Add a new entry to a list of eat data and merge the eat counts if applicable.
        /// </summary>
        /// <param name="eatList">
        ///     The list of eats.
        /// </param>
        /// <param name="newData">
        ///     The new eat data.
        /// </param>
        public static void AggregateEatData(this List<EatData> eatList, EatData newData)
        {
            EatData existingData = eatList
                .Find(k =>
                    k.CreatureType == newData.CreatureType &&
                    k.ObjectType == newData.ObjectType &&
                    k.IntData == newData.IntData);

            if (existingData != null)
                existingData.EatCount += newData.EatCount;
            else
                eatList.Add(newData);
        }

        /// <summary>
        ///     Gets the cycle data from a sleep and death screen data package.
        /// </summary>
        /// <param name="package">
        ///     The package.
        /// </param>
        /// <returns>
        ///     The cycle data.
        /// </returns>
        public static Cycle GetCycleData(this KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            Cycle cycle = new Cycle
            {
                CycleNumber = package.saveState.cycleNumber - 1,
                Statistics = new CycleStatistics
                {
                    TotalTimeAlive = package.saveState.totTime,
                    TotalTimeDead = package.saveState.deathPersistentSaveData.deathTime,
                    Kills = new List<KillData>(),
                    Eats = new List<EatData>()
                }
            };

            PlayerSessionRecord sessionRecord = package.sessionRecord;
            sessionRecord?.kills.ForEach(
                k =>
                    cycle.Statistics.Kills
                        .AggregateKillData(new KillData
                        {
                            CreatureType = k.symbolData.critType,
                            IntData = k.symbolData.intData,
                            KillCount = 1
                        })
            );
            sessionRecord?.eats.ForEach(
                e =>
                    cycle.Statistics.Eats
                        .AggregateEatData(new EatData
                        {
                            CreatureType = e.creatureType,
                            ObjectType = e.objType,
                            IntData = 0,
                            EatCount = 4
                        }));

            return cycle;
        }

        /// <summary>
        ///     Gets the kill data from the given data package.
        /// </summary>
        /// <param name="package">
        ///     The data package.
        /// </param>
        /// <returns>
        ///     The Kill data.
        /// </returns>
        public static List<KillData> GetKillData(this KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            return package.saveState.kills
                .Select(k => new KillData
                {
                    CreatureType = k.Key.critType,
                    IntData = k.Key.intData,
                    KillCount = k.Value
                })
                .ToList();
        }

        /// <summary>
        ///     Updates the campaign totals using the given data package.
        /// </summary>
        /// <param name="campaign">
        ///     The campaign to update.
        /// </param>
        /// <param name="package">
        ///     The new data.
        /// </param>
        public static void UpdateCampaignTotals(this Campaign campaign,
            KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            // Kills and eats
            campaign.Statistics.TotalKills = package.GetKillData();
            if (campaign.Statistics.TotalEats == null)
                campaign.Statistics.TotalEats = new List<EatData>();
            package.sessionRecord.eats.ForEach(
                e =>
                    campaign.Statistics.TotalEats
                        .AggregateEatData(new EatData
                        {
                            CreatureType = e.creatureType,
                            ObjectType = e.objType,
                            IntData = 0,
                            EatCount = 4
                        }));

            // Time
            campaign.Statistics.TotalTimeAlive = package.saveState.totTime;
            campaign.Statistics.TotalTimeDead = package.saveState.deathPersistentSaveData.deathTime;

            // Score
            package.GetCurrentScore(out int score, out int scoreMsc);
            campaign.Statistics.TotalScore = score;
            campaign.Statistics.TotalScoreMsc = scoreMsc;
        }

        /// <summary>
        ///     Gets the current campaign score from the data package.
        /// </summary>
        /// <param name="package">
        ///     The data package containing save data.
        /// </param>
        /// <param name="score">
        ///     The score for when playing vanilla.
        /// </param>
        /// <param name="mscScore">
        ///     The score for when playing with Downpour.
        /// </param>
        public static void GetCurrentScore(
            this KarmaLadderScreen.SleepDeathScreenDataPackage package,
            out int score,
            out int mscScore)
        {
            score = 0;
            score += package.saveState.totFood;
            score += package.saveState.deathPersistentSaveData.survives * 10;
            score -= package.saveState.deathPersistentSaveData.deaths * 3;
            score -= package.saveState.deathPersistentSaveData.quits * 3;
            score -= package.saveState.totTime / 60;
            mscScore = score;

            // Story objectives
            if (package.saveState.saveStateNumber == SlugcatStats.Name.Red)
            {
                if (package.saveState.miscWorldSaveData.moonRevived)
                    score += 100;
                if (package.saveState.miscWorldSaveData.pebblesSeenGreenNeuron)
                    score += 40;
                if (package.saveState.deathPersistentSaveData.ascended)
                    score += 300;
                mscScore = score;
            }
            else if (
                package.saveState.saveStateNumber == MoreSlugcatsEnums.SlugcatStatsName.Artificer)
            {
                mscScore += package.saveState.miscWorldSaveData.SLOracleState.significantPearls.Count * 15;
                if (package.saveState.deathPersistentSaveData.ascended)
                    mscScore += 300;
            }
            else
            {
                mscScore += package.saveState.miscWorldSaveData.SLOracleState.significantPearls.Count * 20;
                mscScore += package.saveState.deathPersistentSaveData.friendsSaved * 15;

                if (package.saveState.miscWorldSaveData.SSaiConversationsHad > 0)
                    mscScore += 40;
                if (package.saveState.miscWorldSaveData.SLOracleState.playerEncounters > 0)
                    mscScore += 40;

                if (package.saveState.deathPersistentSaveData.winState
                            .GetTracker(MoreSlugcatsEnums.EndgameID.Gourmand, false)
                        is WinState.GourFeastTracker gourFeastTracker
                    && gourFeastTracker.GoalFullfilled)
                {
                    mscScore += 300;
                }
            }

            // Creature Kills
            int[] creatureKillScores =
                Enumerable.Repeat(1, MultiplayerUnlocks.SandboxUnlockID.values.Count).ToArray();
            SandboxSettingsInterface.DefaultKillScores(ref creatureKillScores);

            List<KeyValuePair<IconSymbol.IconSymbolData, int>> killsData
                = package.saveState.kills
                    .Where(k => CreatureSymbol.DoesCreatureEarnATrophy(k.Key.critType))
                    .ToList();
            foreach (var kill in killsData)
            {
                if (kill.Key.critType == CreatureTemplate.Type.RedCentipede)
                {
                    score += 19 * kill.Value;
                    mscScore += 25 * kill.Value;
                    continue;
                }

                int creatureScore = StoryGameStatisticsScreen.GetNonSandboxKillscore(kill.Key.critType);
                if (
                    creatureScore == 0
                    && (int)MultiplayerUnlocks.SandboxUnlockForSymbolData(kill.Key) < creatureKillScores.Length
                    && (int)MultiplayerUnlocks.SandboxUnlockForSymbolData(kill.Key) >= 0)
                {
                    creatureScore = creatureKillScores[(int)MultiplayerUnlocks.SandboxUnlockForSymbolData(kill.Key)];
                }

                if (!IsCreatureFromDownpour(kill.Key.critType))
                    score += creatureScore * kill.Value;
                mscScore += creatureScore * kill.Value;
            }

            // Passage multiplier
            int multiplier =
                1 + package.saveState.deathPersistentSaveData.winState.endgameTrackers
                    .Where(c => c.IsPassage())
                    .Count(e => e.GoalFullfilled);
            score *= multiplier;
            mscScore *= multiplier;
        }

        /// <summary>
        ///     Determines whether this tracker is passage.
        /// </summary>
        /// <param name="tracker">
        ///     The tracker.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified tracker is passage; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPassage(this WinState.EndgameTracker tracker)
        {
            WinState.EndgameID id = tracker.ID;

            return
                id == WinState.EndgameID.Survivor
                || id == WinState.EndgameID.Hunter
                || id == WinState.EndgameID.Saint
                || id == WinState.EndgameID.Traveller
                || id == WinState.EndgameID.Chieftain
                || id == WinState.EndgameID.Monk
                || id == WinState.EndgameID.Outlaw
                || id == WinState.EndgameID.DragonSlayer
                || id == WinState.EndgameID.Scholar
                || id == WinState.EndgameID.Friend
                || id == MoreSlugcatsEnums.EndgameID.Nomad
                || id == MoreSlugcatsEnums.EndgameID.Martyr
                || id == MoreSlugcatsEnums.EndgameID.Pilgrim
                || id == MoreSlugcatsEnums.EndgameID.Mother;
        }

        /// <summary>
        ///     Determines whether the specified creature type is a creature from downpour.
        /// </summary>
        /// <param name="creatureType">
        ///     Type of the creature.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified creature type is a creature from downpour; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCreatureFromDownpour(CreatureTemplate.Type creatureType)
        {
            List<FieldInfo> sharedEnums
                = typeof(DLCSharedEnums.CreatureTemplateType)
                    .GetFields()
                    .Where(f => f.FieldType == typeof(CreatureTemplate.Type))
                    .ToList();

            if (sharedEnums.Any(f => (CreatureTemplate.Type)f.GetValue(null) == creatureType))
                return true;

            List<FieldInfo> downpourEnums
                = typeof(MoreSlugcatsEnums.CreatureTemplateType)
                    .GetFields()
                    .Where(f => f.FieldType == typeof(CreatureTemplate.Type))
                    .ToList();

            return downpourEnums.Any(f => (CreatureTemplate.Type)f.GetValue(null) == creatureType);
        }

        #region Debug Only

#if DEBUG
        /// <summary>
        ///     Logs the package details.
        /// </summary>
        /// <param name="package">
        ///     The package.
        /// </param>
        public static void LogPackageDetails(this KarmaLadderScreen.SleepDeathScreenDataPackage package)
        {
            string message
                = "DATA PACKAGE DETAILS"
                  + $"\nCycle number: {package.saveState.cycleNumber}"
                  + $"\nTotal time alive: {package.saveState.totTime}"
                  + $"\nTotal time dead: {package.saveState.deathPersistentSaveData.deathTime}"
                  + $"\nTotal food: {package.saveState.totFood}"
                  + $"\nSuccessful cycles: {package.saveState.deathPersistentSaveData.survives}"
                  + $"\nDeaths: {package.saveState.deathPersistentSaveData.deaths}"
                  + $"\nQuits: {package.saveState.deathPersistentSaveData.quits}\n";

            StringBuilder builder = new StringBuilder(message);
            builder.AppendLine("Total Kills:");
            package.saveState.kills.ForEach(k =>
                builder.AppendLine($"\t{k.Key.critType} - {k.Value}"));
            builder.AppendLine("Kills:");
            package.sessionRecord.kills.ForEach(k =>
                builder.AppendLine($"\t{k.symbolData.critType} ({k.ID})"));
            builder.AppendLine("Eats:");
            package.sessionRecord.eats.ForEach(e =>
                builder.AppendLine($"\t{e.creatureType}{e.objType} ({e.ID})"));

            Debug.Log(builder.ToString());
        }
#endif

        #endregion
    }
}