using System;
using System.Collections.Generic;
using System.Linq;

namespace Ship.CardInfo
{
    public class FactionData
    {
        public Dictionary<Faction, Type> FactionInfo { get; }
        public Faction DefaultFaction => FactionInfo.First().Key;
        public List<Faction> AllFactions => FactionInfo.Keys.ToList();
        public Type IconicPilot(Faction faction) => FactionInfo[faction];

        public FactionData(Dictionary<Faction, Type> factionInfo)
        {
            FactionInfo = factionInfo;
        }
    }
}
