using Editions;
using Ship;
using System;
using UnityEngine;

namespace SquadBuilderNS
{
    public class PilotRecord
    {
        public GenericShip Instance { get; }
        public string PilotName => Instance.PilotInfo.PilotName;
        public string PilotTypeName => Instance.GetType().ToString();
        public string PilotNameCanonical => Instance.PilotNameCanonical;
        public Faction PilotFaction => Instance.Faction;
        public int PilotSkill => Instance.PilotInfo.Initiative;
        public ShipRecord Ship { get; }
        public bool IsAllowedForSquadBuilder => Instance.PilotInfo != null && Instance.IsAllowedForSquadBuilder();

        public PilotRecord(ShipRecord ship, Type type)
        {
            Ship = ship;

            Instance = (GenericShip) System.Activator.CreateInstance(type);
            Edition.Current.AdaptPilotToRules(Instance);
        }
    }
}
