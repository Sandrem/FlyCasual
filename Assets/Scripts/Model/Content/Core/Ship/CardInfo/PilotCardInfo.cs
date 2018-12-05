using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    public class PilotCardInfo
    {
        public string PilotName { get; private set; }
        public string PilotTitle { get; private set; }
        public int Initiative { get; private set; }
        public bool IsLimited { get; private set; }

        public Type AbilityType { get; private set; }

        public int Force { get; set; }
        public int Charges { get; private set; }
        public bool RegensCharges { get; private set; }

        public int Cost { get; private set; }

        public List<UpgradeType> ExtraUpgrades { get; private set; }
        public Faction Faction { get; private set; }
        public int SEImageNumber { get; private set; }

        public PilotCardInfo(string pilotName, int initiative, int cost, bool isLimited = false, Type abilityType = null, string pilotTitle = "", int force = 0, int charges = 0, bool regensCharges = false, UpgradeType extraUpgradeIcon = UpgradeType.None, List<UpgradeType> extraUpgradeIcons = null, Faction factionOverride = Faction.None, int seImageNumber = 0)
        {
            PilotName = pilotName;
            PilotTitle = pilotTitle;
            Initiative = initiative;
            IsLimited = isLimited;

            AbilityType = abilityType;

            Force = force;
            Charges = charges;
            RegensCharges = regensCharges;

            Cost = cost;

            SEImageNumber = seImageNumber;

            ExtraUpgrades = new List<UpgradeType>();
            if (extraUpgradeIcon != UpgradeType.None) ExtraUpgrades.Add(extraUpgradeIcon);
            if (extraUpgradeIcons != null) ExtraUpgrades.AddRange(extraUpgradeIcons);
            if (factionOverride != Faction.None) Faction = factionOverride;
        }

        public PilotCardInfo(JSONObject pilot)
        {
            //Console.Write(pilot.ToString());
            PilotName = pilot["name"].str;
            if (!pilot["caption"].IsNull) { PilotTitle = pilot["caption"].str; }
            Initiative = (int)pilot["initiative"].i;
            if (pilot["limited"].i == 1) { IsLimited = true; }

            //Console.Write("Loading Pilot Ability");
            if (pilot.HasField("AbilityType"))
            {
                Type type = Type.GetType(pilot["AbilityType"].str);
                if (type != null) { AbilityType = type; }
                else { Console.Write("Unable to add Pilot Ability: " + pilot["AbilityType"].str); }
            }

            //Console.Write("Loading Pilot Stats");
            if (!pilot["force"].IsNull) { Force = (int)pilot["force"].i; }
            if (!pilot["charges"].IsNull) {
                Charges = (int)pilot["charges"]["value"].i;
                if ((int)pilot["charges"]["recovers"].i == 1) { RegensCharges = true; }
            }

            Cost = (int)pilot["cost"].i;
            SEImageNumber = (int)pilot["ffg"].i;

            //Console.Write("Loading Pilot Upgrade Slots");
            ExtraUpgrades = new List<UpgradeType>();
            foreach (JSONObject slot in pilot["slots"].list)
            {
                switch (slot.str)
                {
                    case "Talent":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Talent);
                        break;
                    case "Force Power":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Force);
                        break;
                    case "Modification":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Modification);
                        break;
                    case "Sensor":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.System);
                        break;
                    case "Cannon":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Cannon);
                        break;
                    case "Missile":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Missile);
                        break;
                    case "Device":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Bomb);
                        break;
                    case "Torpedo":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Torpedo);
                        break;
                    case "Crew":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Crew);
                        break;
                    case "Gunner":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Gunner);
                        break;
                    case "Astromech":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Astromech);
                        break;
                    case "Illicit":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Illicit);
                        break;
                    case "Title":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Title);
                        break;
                    case "Configuration":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Configuration);
                        break;
                    case "Turret":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Turret);
                        break;
                    case "Tech":
                        ExtraUpgrades.Add(Upgrade.UpgradeType.Tech);
                        break;
                    default:
                        Console.Write("Upgrade type unknown: " + slot.str);
                        break;
                } //Need to add the rest
            }

            switch (pilot["faction"].str)
            {
                case "Galactic Empire":
                    Faction = Faction.Imperial;
                    break;
                case "Rebel Alliance":
                    Faction = Faction.Rebel;
                    break;
                case "Scum and Villainy":
                    Faction = Faction.Scum;
                    break;
                default:
                    Console.Write("Faction type unknown: " + pilot["faction"].str);
                    break;
            }
        }
    }
}
