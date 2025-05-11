using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AllKills.Menu.StatisticsData
{
    [XmlRoot("gameStats")]
    public class GameStatistics
    {
        [XmlArray("campaigns")] [XmlArrayItem("campaign")]
        public List<Campaign> Campaigns;
    }

    #region Campaign

    public class Campaign
    {
        [XmlElement("character")]
        public string CharacterSerializable
        {
            get => Character.ToString();
            set
            {
                bool success = ExtEnumBase.TryParse(typeof(SlugcatStats.Name), value, true, out var result);
                success &= result.GetType() == typeof(SlugcatStats.Name);

                if (success)
                    Character = (SlugcatStats.Name)result;
                else
                    Character = null;
            }
        }

        [XmlIgnore] public SlugcatStats.Name Character;

        [XmlElement("stats")] public CampaignStatistics Statistics;
    }

    public class CampaignStatistics
    {
        [XmlArray("cycles")] [XmlArrayItem("cycle")]
        public List<Cycle> Cycles;

        [XmlArray("kills")] [XmlArrayItem("kill")]
        public List<KillsData> TotalKills;
    }

    #endregion

    #region Cycle

    public class Cycle
    {
        [XmlElement("number")] public int CycleNumber;

        [XmlElement("stats")] public CycleStatistics Statistics;
    }

    public class CycleStatistics
    {
        [XmlArray("kills")] [XmlArrayItem("kill")]
        public List<KillsData> Kills;
    }

    #endregion

    #region Other Data Structures

    public class KillsData
    {
        [XmlElement("creature")]
        public string CreatureTypeSerializable
        {
            get => CreatureType.ToString();
            set
            {
                bool success = ExtEnumBase.TryParse(typeof(CreatureTemplate.Type), value, true, out var result);
                success &= result.GetType() == typeof(CreatureTemplate.Type);

                if (success)
                    CreatureType = (CreatureTemplate.Type)result;
                else
                    CreatureType = null;
            }
        }

        [XmlIgnore] public CreatureTemplate.Type CreatureType;

        [XmlElement("count")] public int KillCount;
    }

    #endregion
}