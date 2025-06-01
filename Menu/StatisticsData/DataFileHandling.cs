using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace AllKills.Menu.StatisticsData
{
    /// <summary>
    ///     Data file handling for the statistics data used by AllKills.
    /// </summary>
    public static class DataFileHandling
    {
        public const string StatisticsFilePath = "allKills/allKillsMainData_sav{0}.dat";

        /// <summary>
        ///     The currently loaded statistics, so that we don't keep reading from the save file all the time.
        /// </summary>
        public static GameStatistics CurrentlyLoadedStatistics;

        /// <summary>
        ///     The currently loaded save slot.
        /// </summary>
        public static int CurrentlyLoadedSaveSlot;

        /// <summary>
        ///     Temporary storage for cycle data when starving.
        /// </summary>
        public static Dictionary<SlugcatStats.Name, Cycle> MalnourishedStatisticsCache =
            new Dictionary<SlugcatStats.Name, Cycle>();

        /// <summary>
        ///     Load the game statistics for AllKills.
        /// </summary>
        public static GameStatistics LoadGameStatistics(int saveSlot)
        {
            Debug.Log("Load");

            if (CurrentlyLoadedStatistics != null && saveSlot == CurrentlyLoadedSaveSlot)
                return CurrentlyLoadedStatistics;

            string filePath =
                $"{Application.persistentDataPath}/{string.Format(StatisticsFilePath, saveSlot)}";

            MalnourishedStatisticsCache = new Dictionary<SlugcatStats.Name, Cycle>();
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                    return new GameStatistics();

                using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                using (GZipStream compressStream = new GZipStream(fileStream, CompressionMode.Decompress))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameStatistics));
                    GameStatistics statistics = serializer.Deserialize(compressStream) as GameStatistics;

                    return CurrentlyLoadedStatistics = statistics;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"{AllKillsMain.ModName}: Error when trying to load data!");
                Debug.LogException(ex);
                return new GameStatistics();
            }
        }

        /// <summary>
        ///     Save the game statistics for AllKills.
        /// </summary>
        public static bool SaveGameStatistics(GameStatistics statistics, int saveSlot)
        {
            Debug.Log("Save");

            string filePath =
                $"{Application.persistentDataPath}/{string.Format(StatisticsFilePath, saveSlot)}";

            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                {
                    if (fileInfo.Directory != null)
                        Directory.CreateDirectory(fileInfo.Directory.FullName);
                    File.Create(fileInfo.FullName).Close();
                }

                using (MemoryStream memoryStream = new MemoryStream())
                using (FileStream fileStream = new FileStream(fileInfo.FullName, FileMode.Truncate, FileAccess.Write))
                using (GZipStream compressStream = new GZipStream(fileStream, CompressionMode.Compress))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameStatistics));
                    serializer.Serialize(memoryStream, statistics);
                    memoryStream.Flush();
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(compressStream);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"{AllKillsMain.ModName}: Failed to save data!!");
                Debug.LogException(ex);
                return false;
            }

            return true;
        }
    }
}