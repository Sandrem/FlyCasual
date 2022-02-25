using Editions;
using Obstacles;
using Players;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace SquadBuilderNS
{
    public class SquadList
    {
        public PlayerNo PlayerNo { get; set; }
        public string Name { get; set; }
        public Type PlayerType { get; set; }
        public Faction SquadFaction { get; set; }
        public List<SquadListShip> Ships { get; } = new List<SquadListShip>();
        public List<GenericObstacle> ChosenObstacles { get; } = new List<GenericObstacle>();
        public int Points => Ships.Sum(n => GetShipCost(n));
        public JSONObject SavedConfiguration { get; set; }

        public bool IsValid
        {
            get
            {
                SquadValidator squadValidator = new SquadValidator();
                return squadValidator.IsSquadValid(this);
            }
        }

        public SquadList(PlayerNo playerNo)
        {
            PlayerNo = playerNo;
            SetDefaultObstacles();
        }

        public void SetDefaultObstacles()
        {
            ChosenObstacles.Clear();
            ChosenObstacles.AddRange
            (
                new List<GenericObstacle>()
                {
                    ObstaclesManager.GetPossibleObstacle("coreasteroid5"),
                    ObstaclesManager.GetPossibleObstacle("core2asteroid5"),
                    ObstaclesManager.GetPossibleObstacle("core2asteroid4")
                }
            );
        }

        public void SetPlayerSquadFromImportedJson(JSONObject squadJson)
        {
            ClearAll();
            SquadJsonHelper.SetPlayerSquadFromImportedJson(this, squadJson);
        }

        public void ReGenerateSquad()
        {
            JSONObject playerJson = SavedConfiguration;
            SetPlayerSquadFromImportedJson(playerJson);
        }

        public JSONObject GetSquadInJson()
        {
            return SquadJsonHelper.GetSquadInJson(this);
        }

        public void CreateSquadFromImportedJson(string squadString)
        {
            SquadJsonHelper.CreateSquadFromImportedJson(this, squadString);
        }

        public void SaveSquadronToFile(string squadName)
        {
            SquadJsonHelper.SaveSquadronToFile(this, squadName);
        }

        public SquadListShip AddShip(GenericShip ship)
        {
            SquadListShip newShip = new SquadListShip(ship, this);
            Ships.Add(newShip);
            return newShip;
        }

        public void RemoveShip(SquadListShip ship)
        {
            Ships.Remove(ship);
        }

        public void CopyShip(SquadListShip shipToCopy)
        {
            if (UniquePilotsLimitIsNotExceeded(shipToCopy.Instance))
            {
                GenericShip newShip = (GenericShip)Activator.CreateInstance(Type.GetType(shipToCopy.Instance.GetType().ToString()));
                Edition.Current.AdaptShipToRules(newShip);
                Edition.Current.AdaptPilotToRules(newShip);

                SquadListShip squadBuilderShip = AddShip(newShip);
                List<GenericUpgrade> copyUpgradesList = new List<GenericUpgrade>(shipToCopy.Instance.UpgradeBar.GetUpgradesAll());
                CopyUpgradesRecursive(squadBuilderShip, copyUpgradesList);
            }
            else
            {
                Messages.ShowInfo(shipToCopy.Instance.PilotInfo.PilotName + " is not copied due to unique cards limit");
            }
        }

        private void CopyUpgradesRecursive(SquadListShip targetShip, List<GenericUpgrade> upgradeList)
        {
            if (upgradeList.Count > 0)
            {
                if (UniqueUpgradesLimitIsNotExceeded(upgradeList.First()))
                {
                    targetShip.InstallUpgrade(upgradeList.First().NameCanonical, upgradeList.First().UpgradeInfo.UpgradeTypes.First());
                }
                else
                {
                    Messages.ShowInfo(upgradeList.First().UpgradeInfo.Name + " is not copied due to unique cards limit");
                }

                upgradeList.Remove(upgradeList.First());

                CopyUpgradesRecursive(targetShip, upgradeList);
            }
        }

        private bool UniqueUpgradesLimitIsNotExceeded(GenericUpgrade upgradeToCopy)
        {
            if (!upgradeToCopy.UpgradeInfo.IsLimited) return true;

            int sameUpgradePresent = 0;

            foreach (var ship in Ships)
            {
                foreach (var upgrade in ship.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (upgrade.UpgradeInfo.GetCleanName() == upgradeToCopy.UpgradeInfo.GetCleanName())
                    {
                        sameUpgradePresent++;
                    }
                }
            }

            return sameUpgradePresent < upgradeToCopy.UpgradeInfo.Limited;
        }

        private bool UniquePilotsLimitIsNotExceeded(GenericShip pilotToCopy)
        {
            if (!pilotToCopy.PilotInfo.IsLimited) return true;

            int samePilotPresent = 0;

            foreach (var ship in Ships)
            {
                if (ship.Instance.PilotInfo.PilotName == pilotToCopy.PilotInfo.PilotName)
                {
                    samePilotPresent++;
                }
            }

            return samePilotPresent < pilotToCopy.PilotInfo.Limited;
        }

        public void ClearShips()
        {
            Ships.Clear();
        }

        private int GetShipCost(SquadListShip shipHolder)
        {
            return shipHolder.Instance.PilotInfo.Cost;
        }

        private int ReduceUpgradeCost(int cost, int decrease)
        {
            if (cost >= 0) cost = Math.Max(cost - decrease, 0);
            return cost;
        }

        public SquadListShip AddPilotToSquad(GenericShip ship, bool isFromUi = false)
        {
            SquadListShip squadBuilderShip = AddShip(ship);

            if (isFromUi)
            {
                foreach (var upgradeType in ship.DefaultUpgrades)
                {
                    GenericUpgrade upgrade = (GenericUpgrade)Activator.CreateInstance(upgradeType);
                    Edition.Current.AdaptUpgradeToRules(upgrade);

                    squadBuilderShip.TryInstallUpgade(upgrade);
                }
            }

            return squadBuilderShip;
        }

        public void ClearAll()
        {
            ClearShips();
            GenerateDefaultNameForSquad();
        }

        public void GenerateDefaultNameForSquad()
        {
            Name = $"Squadron of Player {Tools.PlayerToInt(PlayerNo)}";
        }

        public bool HasPilot(string name)
        {
            return Ships.Any(n => n.Instance.PilotInfo.PilotName == name);
        }

        public bool HasUpgrade(string name)
        {
            foreach (var shipHolder in Ships)
            {
                foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (upgrade.UpgradeInfo.Name == name) return true;
                }
            }

            return false;
        }
    }
}