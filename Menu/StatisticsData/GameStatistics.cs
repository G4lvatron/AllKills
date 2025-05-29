using System.Collections.Generic;
using System.Xml.Serialization;

namespace AllKills.Menu.StatisticsData
{
    /// <summary> The statistics for Rain World used by All Kills. </summary>
    [XmlRoot("gameStats")]
    public class GameStatistics
    {
        /// <summary> The list of campaigns. </summary>
        [XmlArray("campaigns")] [XmlArrayItem("campaign")]
        public List<Campaign> Campaigns;
    }

    #region Campaign

    public class Campaign
    {
        /// <summary> The character as a serializable field. </summary>
        [XmlElement("character")]
        public string CharacterSerializable
        {
            get => Character.ToString();
            set
            {
                bool success = ExtEnumBase.TryParse(typeof(SlugcatStats.Name), value, true, out var result);
                success &= result.GetType() == typeof(SlugcatStats.Name);

                Character = success
                    ? (SlugcatStats.Name)result
                    : null;
            }
        }

        /// <summary> The character. </summary>
        [XmlIgnore] public SlugcatStats.Name Character;

        /// <summary> The statistics for the campaign. </summary>
        [XmlElement("stats")] public CampaignStatistics Statistics;
    }

    /// <summary> The statistics for a campaign. </summary>
    public class CampaignStatistics
    {
        /// <summary> The total time in the campaign. </summary>
        [XmlElement("ttime")] public int TotalTime;

        /// <summary> The total score in the campaign. </summary>
        [XmlElement("tscore")] public int TotalScore;

        /// <summary> The list of cycles. </summary>
        [XmlArray("cycles")] [XmlArrayItem("cycle")]
        public List<Cycle> Cycles;

        /// <summary> The total kill statistics for the campaign. </summary>
        [XmlArray("kills")] [XmlArrayItem("kill")]
        public List<KillData> TotalKills;

        /// <summary> The total eat summary for the campaign.</summary>
        [XmlArray("eats")] [XmlArrayItem("eat")]
        public List<EatData> TotalEats;
    }

    #endregion

    #region Cycle

    /// <summary> Data for a single cycle. </summary>
    public class Cycle
    {
        /// <summary> The number of the cycle. </summary>
        [XmlElement("number")] public int CycleNumber;

        /// <summary> The detailed statistics of the cycle. </summary>
        [XmlElement("stats")] public CycleStatistics Statistics;
    }

    /// <summary> The detailed statistics for a cycle. </summary>
    public class CycleStatistics
    {
        /// <summary> The total time spent in the run so far. </summary>
        [XmlElement("ttime")] public int TotalTime;

        /// <summary> The time spent in the cycle. </summary>
        [XmlElement("ctime")] public int CycleTime;

        /// <summary> The total run score so far. </summary>
        [XmlElement("tscore")] public int TotalScore;

        /// <summary> The score change for the cycle. </summary>
        [XmlElement("cscore")] public int CycleScore;

        /// <summary> The kills for the cycle. </summary>
        [XmlArray("kills")] [XmlArrayItem("kill")]
        public List<KillData> Kills;

        /// <summary> The eats for the cycle. </summary>
        [XmlArray("eats")] [XmlArrayItem("eat")]
        public List<EatData> Eats;
    }

    #endregion

    #region Other Data Structures

    /// <summary> The data for kills of a creature. </summary>
    public class KillData
    {
        /// <summary> The creature type, but serializable into XML. </summary>
        [XmlElement("creature")]
        public string CreatureTypeSerializable
        {
            get => CreatureType.ToString();
            set
            {
                bool success = ExtEnumBase.TryParse(typeof(CreatureTemplate.Type), value, true, out var result);
                success &= result.GetType() == typeof(CreatureTemplate.Type);

                CreatureType = success
                    ? (CreatureTemplate.Type)result
                    : null;
            }
        }

        /// <summary> The creature type. </summary>
        [XmlIgnore] public CreatureTemplate.Type CreatureType;

        /// <summary> Optional data. </summary>
        [XmlElement("intData")] public int IntData;

        /// <summary> The number of times the creature was killed. </summary>
        [XmlElement("count")] public int KillCount;
    }

    /// <summary> The data for a type of food eaten. </summary>
    public class EatData
    {
        /// <summary> The type of the food, serializable into XML. </summary>
        [XmlElement("creature")]
        public string CreatureTypeSerializable
        {
            get => CreatureType.ToString();
            set
            {
                bool success = ExtEnumBase.TryParse(typeof(CreatureTemplate.Type), value, true,
                    out var result);
                success &= result.GetType() == typeof(CreatureTemplate.Type);

                CreatureType = success
                    ? (CreatureTemplate.Type)result
                    : null;
            }
        }

        /// <summary> The type of the object, serializable into XML. </summary>
        [XmlElement("object")]
        public string ObjectTypeSerializable
        {
            get => IsCreature ? CreatureType.ToString() : ObjectType.ToString();
            set
            {
                bool success = ExtEnumBase.TryParse(typeof(AbstractPhysicalObject.AbstractObjectType), value, true,
                    out var result);
                success &= result.GetType() == typeof(AbstractPhysicalObject.AbstractObjectType);

                ObjectType = success
                    ? (AbstractPhysicalObject.AbstractObjectType)result
                    : null;
            }
        }

        /// <summary> The type of the creature. </summary>
        [XmlIgnore] public CreatureTemplate.Type CreatureType;

        /// <summary> The type of the object. </summary>
        [XmlIgnore] public AbstractPhysicalObject.AbstractObjectType ObjectType;

        /// <summary> Optional data. </summary>
        [XmlElement("intData")] public int IntData;

        /// <summary> <c>true</c> if this food is a creature, <c>false</c> otherwise. </summary>
        [XmlElement("isObj")] public bool IsCreature;

        /// <summary> The amount of this food that was eaten. 1 food pip corresponds to 4 food. </summary>
        [XmlElement("count")] public int EatCount;
    }

    #endregion
}