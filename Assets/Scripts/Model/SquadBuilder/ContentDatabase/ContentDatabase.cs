using Editions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SquadBuilderNS
{
    public class ContentDatabase
    {
        //TODO: Hide
        public List<ShipRecord> AllShips = new List<ShipRecord>();
        public List<PilotRecord> AllPilots = new List<PilotRecord>();
        public List<UpgradeRecord> AllUpgrades = new List<UpgradeRecord>();

        private Edition DatabaseEdition;

        public ContentDatabase(Edition edition)
        {
            DatabaseEdition = edition;
            GenerateRecords();
            CheckDuplicates();
            ShowDebugData();
        }

        private void GenerateRecords()
        {
            GenerateShipRecords();
            GenerateUpgradesRecords();
        }

        private void GenerateShipRecords()
        {
            List<string> shipNamespaces = Assembly.GetExecutingAssembly().GetTypes()
                .Where(n => n.Namespace != null)
                .Where(n => n.Namespace.StartsWith($"Ship.{DatabaseEdition.NameShort}."))
                .Select(n => n.Namespace)
                .ToList();

            foreach (var ns in shipNamespaces)
            {
                GetShipRecord(ns);
            }
        }

        private void GetShipRecord(string shipNameSpace)
        {
            ShipRecord shipRecord = new ShipRecord(shipNameSpace);

            if (!AllShips.Any(n => n.ShipName == shipRecord.ShipName))
            {
                AllShips.Add(shipRecord);
                GetPilotsList(shipRecord);
            }
        }

        private void GetPilotsList(ShipRecord ship)
        {
            List<Type> pilotTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => String.Equals(t.Namespace, ship.ShipNamespace, StringComparison.Ordinal))
                .ToList();

            foreach (var pilotType in pilotTypes)
            {
                if (pilotType.MemberType == MemberTypes.NestedType) continue;

                PilotRecord pilotRecord = new PilotRecord(ship, pilotType);
                if (pilotRecord.IsAllowedForSquadBuilder) AllPilots.Add(pilotRecord);
            }
        }

        private void GenerateUpgradesRecords()
        {
            List<Type> typelist = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => String.Equals(t.Namespace, $"UpgradesList.{Edition.Current.NameShort}", StringComparison.Ordinal))
                .ToList();

            foreach (Type type in typelist)
            {
                if (type.MemberType == MemberTypes.NestedType) continue;

                UpgradeRecord upgradeRecord = new UpgradeRecord(type);
                if (upgradeRecord.IsAllowedForSquadBuilder) AllUpgrades.Add(upgradeRecord);
            }

            AllUpgrades = AllUpgrades.OrderBy(n => n.Instance.UpgradeInfo.Name).ToList();
        }

        private void CheckDuplicates()
        {
            if (!DebugManager.ReleaseVersion)
            {
                CheckPilotDuplicates();
                CheckUpgradeDuplicates();
            }
        }

        private void CheckPilotDuplicates()
        {
            foreach (PilotRecord pilotRecord in AllPilots)
            {
                int samePilotCount = 0;
                foreach (PilotRecord pilotRecord2 in AllPilots)
                {
                    if (pilotRecord.PilotNameCanonical == pilotRecord2.PilotNameCanonical)
                    {
                        samePilotCount++;
                        if (samePilotCount > 1)
                        {
                            Debug.LogError("Pilot ID " + pilotRecord.PilotNameCanonical + " has duplicate!");
                            break;
                        }
                    }
                }
            }
        }

        private void CheckUpgradeDuplicates()
        {
            foreach (UpgradeRecord upgradeRecord in AllUpgrades)
            {
                int samePilotCount = 0;
                foreach (UpgradeRecord upgradeRecord2 in AllUpgrades)
                {
                    if (upgradeRecord.UpgradeNameCanonical == upgradeRecord2.UpgradeNameCanonical)
                    {
                        samePilotCount++;
                        if (samePilotCount > 1)
                        {
                            Debug.LogError("Upgrade ID " + upgradeRecord.UpgradeNameCanonical + " has duplicate!");
                            break;
                        }
                    }
                }
            }
        }

        private void ShowDebugData()
        {
            Console.Write("Squadbuilder content database is ready", isBold: true);
            Console.Write($"Pilots: {AllPilots.Count}, (Unique: {{AllPilots.Count(n => n.Instance.PilotInfo.IsLimited)}})");
            Console.Write($"Upgrades: {AllUpgrades.Count}");
        }

        public void ClearData()
        {
            AllPilots.Clear();
            AllShips.Clear();
            AllUpgrades.Clear();
            TextureCache.Cache.Clear();
            Resources.UnloadUnusedAssets();
        }
    }    
}
