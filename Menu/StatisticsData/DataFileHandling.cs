using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace AllKills.Menu.StatisticsData
{
    /// <summary>
    ///     Data file handling for the statistics data used by AllKills.
    /// </summary>
    public class DataFileHandling
    {
        public const string StatisticsFilePath = "allKills/allKillsMainData.xml";

        /// <summary>
        ///     Load the game statistics for AllKills.
        /// </summary>
        public static GameStatistics LoadGameStatistics()
        {
            string filePath =
                $"{UnityEngine.Application.persistentDataPath}/{StatisticsFilePath}";

            try
            {
                using (StreamReader reader = new StreamReader(filePath, Encoding.Unicode))
                {
                    string data = reader.ReadToEnd();

                    GameStatistics statistics = Json.Deserialize(data) as GameStatistics;
                    return statistics;
                }
            }
            catch (Exception)
            {
                return new GameStatistics();
            }
        }

        /// <summary>
        ///     Save the game statistics for AllKills.
        /// </summary>
        public static bool SaveGameStatistics(GameStatistics statistics)
        {
            string filePath =
                $"{UnityEngine.Application.persistentDataPath}/{StatisticsFilePath}";

            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                if (!fileInfo.Exists)
                    if (fileInfo.Directory != null)
                        Directory.CreateDirectory(fileInfo.Directory.FullName);

                using (StreamWriter writer = new StreamWriter(fileInfo.FullName, false))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(GameStatistics));
                    serializer.Serialize(writer, statistics);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log("AllKills failed to save data!!");
                UnityEngine.Debug.LogException(ex);
                return false;
            }

            return true;
        }
    }
}