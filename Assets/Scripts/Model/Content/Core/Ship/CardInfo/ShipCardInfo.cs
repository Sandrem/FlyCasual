using System;
using System.Collections.Generic;

namespace Ship
{
    public class ShipCardInfo
    {
        public string ShipName { get; set; }
        public BaseSize BaseSize { get; set; }
        public Faction DefaultShipFaction { get; set; }

        public ShipArcsInfo ArcInfo { get; private set; }
        public int Firepower { get {return ArcInfo.Firepower; } }
        public int Agility { get; private set; }
        public int Hull { get; set; }
        public int Shields { get; set; }

        public ShipActionsInfo ActionIcons { get; private set; }
        public ShipUpgradesInfo UpgradeIcons { get; private set; }

        public char Icon { get; private set; }
        public List<Faction> FactionsAll { get; set; }

        public Faction SubFaction { get; set; }

        public ShipCardInfo(string shipName, BaseSize baseSize, Faction faction, ShipArcsInfo arcInfo, int agility, int hull, int shields, ShipActionsInfo actionIcons, ShipUpgradesInfo upgradeIcons, char icon = ' ', Faction subFaction = Faction.None, List<Faction> factionsAll = null)
        {
            ShipName = shipName;
            BaseSize = baseSize;

            DefaultShipFaction = faction;
            if (subFaction != Faction.None) SubFaction = subFaction;

            ArcInfo = arcInfo;

            Agility = agility;
            Hull = hull;
            Shields = shields;

            ActionIcons = actionIcons;
            UpgradeIcons = upgradeIcons;

            Icon = icon;

            FactionsAll = (factionsAll != null) ? factionsAll : new List<Faction>() { faction };
        }

        public ShipCardInfo(JSONObject ship)
        {
            ShipName = ship["name"].str;
            switch (ship["size"].str)
            {
                case "Small":
                    BaseSize = BaseSize.Small;
                    break;
                case "Medium":
                    BaseSize = BaseSize.Medium;
                    break;
                case "Large":
                    BaseSize = BaseSize.Large;
                    break;
                default:
                    Console.Write("Ship size unknown: " + ship["size"].str);
                    break;
            }

            //DefaultShipFaction = faction;
            FactionsAll = new List<Faction>();
            foreach (string faction in ship["factions"].keys)            {                switch (faction)                {                    case "Galactic Empire":                        FactionsAll.Add(Faction.Imperial);                        break;                    case "Rebel Alliance":                        FactionsAll.Add(Faction.Rebel);                        break;                    case "Scum and Villainy":                        FactionsAll.Add(Faction.Scum);                        break;                    default:                        Console.Write("Faction type unknown: " + faction);                        break;                }            }

            foreach (JSONObject stat in ship["stats"].list)            {                switch (stat["type"].str)                {                    case "hull":                        Hull = (int)stat["value"].i;                        break;                    case "shields":                        Shields = (int)stat["value"].i;                        break;                    case "agility":                        Agility = (int)stat["value"].i;                        break;                    case "attack":                        Arcs.ArcType arc = Arcs.ArcType.Primary;                        if (stat["arc"].str == "Double Turret Arc") { arc = Arcs.ArcType.Primary; }                        if (stat["arc"].str == "Rear Arc") { arc = Arcs.ArcType.Primary; }                        new ShipArcsInfo(arc, (int)stat["value"].i);                        break;                    default:                        Console.Write("Stat type unknown: " + stat["type"].str);                        break;                }            }            //if (ShipBaseArcsType == Arcs.BaseArcsType.ArcRear && Firepower > rear_firepower) { ShipAbilities.Add(new Abilities.SecondEdition.WeakNonPrimaryArc()); }

            ActionIcons = new ShipActionsInfo();
            foreach (JSONObject action in ship["actions"].list)            {                System.Type type = typeof(ActionsList.FocusAction); ;                Actions.ActionColor color = Actions.ActionColor.White;                if (action["difficulty"].str == "Red") { color = Actions.ActionColor.Red; }                switch (action["type"].str)                {                    case "Focus":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.FocusAction), color));                        type = typeof(ActionsList.FocusAction);                        break;
                    case "Calculate":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.CalculateAction), color));                        type = typeof(ActionsList.CalculateAction);                        break;                    case "Evade":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.EvadeAction), color));                        type = typeof(ActionsList.EvadeAction);                        break;                    case "Lock":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.TargetLockAction), color));                        type = typeof(ActionsList.TargetLockAction);                        break;                    case "Barrel Roll":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.BarrelRollAction), color));                        type = typeof(ActionsList.BarrelRollAction);                        break;                    case "Boost":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.BoostAction), color));                        type = typeof(ActionsList.BoostAction);                        break;                    case "Reinforce":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.ReinforceForeAction), color));                        type = typeof(ActionsList.ReinforceForeAction);                        break;                    case "Reload":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.ReloadAction), color));                        type = typeof(ActionsList.ReloadAction);                        break;                    case "Coordinate":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.CoordinateAction), color));                        type = typeof(ActionsList.CoordinateAction);                        break;                    case "Jam":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.JamAction), color));                        type = typeof(ActionsList.JamAction);                        break;
                    case "Cloak":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.CloakAction), color));                        type = typeof(ActionsList.CloakAction);                        break;                    case "Rotate Arc":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.RotateArcAction), color));                        type = typeof(ActionsList.RotateArcAction);                        break;                    case "SLAM":                        ActionIcons.AddActions(new Actions.ActionInfo(typeof(ActionsList.SlamAction), color));                        type = typeof(ActionsList.SlamAction);                        break;                    default:                        Console.Write("Action type unknown: " + action["type"].str);                        break;                }                if (action.HasField("linked"))                {                    Actions.ActionColor colorLink = Actions.ActionColor.White;                    if (action["linked"]["difficulty"].str == "Red") { colorLink = Actions.ActionColor.Red; }                    switch (action["linked"]["type"].str)                    {                        case "Focus":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.FocusAction), colorLink));                            break;                        case "Calculate":
                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.CalculateAction), colorLink));
                            break;                         case "Evade":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.EvadeAction), colorLink));                            break;                         case "Lock":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.TargetLockAction), colorLink));                            break;                         case "Barrel Roll":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.BarrelRollAction), colorLink));                            break;                         case "Boost":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.BoostAction), colorLink));                            break;                         case "Reinforce":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.ReinforceForeAction), colorLink));                            break;                         case "Reload":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.ReloadAction), colorLink));                            break;                         case "Coordinate":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.CoordinateAction), colorLink));                            break;                         case "Jam":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.JamAction), colorLink));                            break; 
                        case "Cloak":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.CloakAction), colorLink));                            break;                         case "Rotate Arc":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.RotateArcAction), colorLink));                            break;                         case "SLAM":                            ActionIcons.AddLinkedAction(new Actions.LinkedActionInfo(type, typeof(ActionsList.SlamAction), colorLink));                            break;                         default:                            Console.Write("Linked action type unknown: " + action["type"].str);                            break;                    }                }            }
            UpgradeIcons = new ShipUpgradesInfo();

           Icon = ' ';
        }
    }
}
