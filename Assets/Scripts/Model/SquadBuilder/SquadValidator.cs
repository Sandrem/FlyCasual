using Editions;
using Players;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace SquadBuilderNS
{
    public class SquadValidator
    {
        public bool IsSquadValid(SquadList squad)
        {
            if (!ValidateShipsCount(squad)) return false;
            if (!ValidateMaxSameShipsCount(squad)) return false;
            if (!ValidateLimitedCards(squad)) return false;
            if (!ValidateSquadCost(squad)) return false;
            if (!ValidateLoadoutCost(squad)) return false;
            if (!ValidateSolitaryCards(squad)) return false;
            if (!ValidateStandardizedCards(squad)) return false;
            if (!ValidateUpgradePostChecks(squad)) return false;
            if (!ValidateSpecialSlotsRequirements(squad)) return false;

            return true;
        }

        private bool ValidateShipsCount(SquadList squad)
        {
            if (!DebugManager.DebugNoSquadPointsLimit)
            {
                if (squad.Ships.Count < Edition.Current.MinShipsCount)
                {
                    Messages.ShowError($"The minimum number of pilots required is: {Edition.Current.MinShipsCount}");
                    return false;
                }
            }

            if (squad.Ships.Count > 10)
            {
                Messages.ShowError("Current version of the game supports only 10 ships per player. Please, wait for support of epic mode.");
                return false;
            }

            return true;
        }

        private bool ValidateMaxSameShipsCount(SquadList squad)
        {
            Dictionary<string, int> shipTypesCount = new Dictionary<string, int>();

            foreach (GenericShip ship in squad.Ships.Select(n => n.Instance))
            {
                if (!shipTypesCount.ContainsKey(ship.ShipInfo.ShipName))
                {
                    shipTypesCount.Add(ship.ShipInfo.ShipName, 1);
                }
                else
                {
                    shipTypesCount[ship.ShipInfo.ShipName]++;
                    if (shipTypesCount[ship.ShipInfo.ShipName] > Edition.Current.MaxShipsCount)
                    {
                        Messages.ShowError($"Too many ships of type {ship.ShipInfo.ShipName}");
                        Messages.ShowError($"The maximum allowed number of same ships in squad is: {Edition.Current.MaxShipsCount}");
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ValidateLimitedCards(SquadList squad)
        {
            Dictionary<string, int> uniqueCards = new Dictionary<string, int>();
            foreach (var shipConfig in squad.Ships)
            {
                if (shipConfig.Instance.PilotInfo.IsLimited)
                {
                    string cleanName = shipConfig.Instance.PilotInfo.GetCleanName();
                    if (uniqueCards.ContainsKey(cleanName)) uniqueCards[cleanName]--; else uniqueCards.Add(cleanName, shipConfig.Instance.PilotInfo.Limited - 1);
                }

                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (upgrade.UpgradeInfo.IsLimited)
                    {
                        string cleanName = upgrade.UpgradeInfo.GetCleanName();
                        if (uniqueCards.ContainsKey(cleanName)) uniqueCards[cleanName]--; else uniqueCards.Add(cleanName, upgrade.UpgradeInfo.Limited - 1);
                    }
                }
            }

            foreach (var uniqueCardInfo in uniqueCards)
            {
                if (uniqueCardInfo.Value < 0)
                {
                    Messages.ShowError($"You have too many limited cards with the name {uniqueCardInfo.Key}");
                    return false;
                }
            }

            return true;
        }

        private bool ValidateSquadCost(SquadList squad)
        {
            if (!DebugManager.DebugNoSquadPointsLimit)
            {
                if (squad.Points > Edition.Current.MaxPoints)
                {
                    Messages.ShowError("The cost of your squadron cannot be more than " + Edition.Current.MaxPoints);
                    return false;
                }
            }

            return true;
        }

        private bool ValidateLoadoutCost(SquadList squad)
        {
            if (!DebugManager.DebugNoSquadPointsLimit)
            {
                foreach (SquadListShip ship in squad.Ships)
                {
                    if (ship.Instance.UpgradeBar.GetTotalUsedLoadoutCost() > (ship.Instance.PilotInfo as PilotCardInfo25).LoadoutValue)
                    {
                        Messages.ShowError($"The cost of loadout of {ship.Instance.PilotInfo.PilotName} cannot be more than {(ship.Instance.PilotInfo as PilotCardInfo25).LoadoutValue}");
                        return false;
                    }
                }
            }

            return true;
        }

        private bool ValidateSolitaryCards(SquadList squad)
        {
            int solitaryCards = 0;

            foreach (var shipConfig in squad.Ships)
            {
                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (upgrade.UpgradeInfo.IsSolitary)
                    {
                        if (solitaryCards == 0)
                        {
                            solitaryCards++;
                        }
                        else
                        {
                            Messages.ShowError("Only one Solitary upgrade can be equipped");
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool ValidateStandardizedCards(SquadList squad)
        {
            Dictionary<string, GenericUpgrade> standardizedUpgradesFound = new Dictionary<string, GenericUpgrade>();

            foreach (var shipConfig in squad.Ships)
            {
                foreach (var upgrade in shipConfig.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (upgrade.UpgradeInfo.IsStandardazed)
                    {
                        if (standardizedUpgradesFound.ContainsKey(shipConfig.Instance.ShipInfo.ShipName))
                        {
                            if (standardizedUpgradesFound[shipConfig.Instance.ShipInfo.ShipName].GetType() != upgrade.GetType())
                            {
                                Messages.ShowError($"All {shipConfig.Instance.ShipInfo.ShipName} ships must have the same Standardized upgrade installed");
                                return false;
                            }
                        }
                        else
                        {
                            standardizedUpgradesFound.Add(shipConfig.Instance.ShipInfo.ShipName, upgrade);
                        }
                    }
                }
            }

            foreach (var standardizedPair in standardizedUpgradesFound)
            {
                foreach (var shipConfig in squad.Ships)
                {
                    if (shipConfig.Instance.ShipInfo.ShipName == standardizedPair.Key)
                    {
                        if (!shipConfig.Instance.UpgradeBar.HasUpgradeInstalled(standardizedPair.Value.GetType()))
                        {
                            Messages.ShowError($"All {shipConfig.Instance.ShipInfo.ShipName} ships must have the Standardized upgrade {standardizedPair.Value.UpgradeInfo.Name} installed");
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool ValidateUpgradePostChecks(SquadList squad)
        {
            bool result = true;

            foreach (var shipHolder in squad.Ships)
            {
                if (!shipHolder.Instance.IsAllowedForSquadBuilderPostCheck(squad)) return false;

                foreach (var upgradeHolder in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                {
                    if (!upgradeHolder.IsAllowedForSquadBuilderPostCheck(squad)) return false;
                }
            }

            return result;
        }

        private bool ValidateSpecialSlotsRequirements(SquadList squad)
        {
            bool result = true;

            foreach (var shipHolder in squad.Ships)
            {
                foreach (var upgradeSlot in shipHolder.Instance.UpgradeBar.GetUpgradeSlots())
                {
                    if (!upgradeSlot.IsEmpty)
                    {
                        if (upgradeSlot.MustBeDifferent)
                        {
                            int countDuplicates = shipHolder.Instance.UpgradeBar.GetUpgradesAll().Count(n => n.UpgradeInfo.Name == upgradeSlot.InstalledUpgrade.UpgradeInfo.Name);
                            if (countDuplicates > 1)
                            {
                                Messages.ShowError($"You cannot have more than one copy of {upgradeSlot.InstalledUpgrade.UpgradeInfo.Name} on one ship");
                                return false;
                            }
                        }
                        if (upgradeSlot.InstalledUpgrade.UpgradeInfo.Cost > upgradeSlot.MaxCost)
                        {
                            Messages.ShowError($"The upgrade must costs less than {upgradeSlot.MaxCost}: {upgradeSlot.InstalledUpgrade.UpgradeInfo.Name}");
                            return false;
                        }
                        if (upgradeSlot.MustBeUnique && !upgradeSlot.InstalledUpgrade.UpgradeInfo.IsLimited)
                        {
                            Messages.ShowError($"The upgrade must be unique : {upgradeSlot.InstalledUpgrade.UpgradeInfo.Name}");
                            return false;
                        }
                    }
                }
            }

            return result;
        }
    }
}
