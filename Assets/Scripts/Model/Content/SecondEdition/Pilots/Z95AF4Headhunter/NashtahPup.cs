using Ship;
using SquadBuilderNS;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class NashtahPup : Z95AF4Headhunter
        {
            public NashtahPup() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Nashtah Pup",
                    0,
                    6,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.NdruSuhlakAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;

                ShipAbilities.Add(new Abilities.SecondEdition.EscapeCraftSE());

                SEImageNumber = 171;
            }

            public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
            {
                foreach (var shipHolder in squadList.GetShips())
                {
                    foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                    {
                        if (upgrade.Name == "Hound's Tooth")
                        {
                            return true;
                        }
                    }
                }

                Messages.ShowError("You need YV-666 ship with Hound's Tooth title\nto use Nashtah Pup in squad");
                return false;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EscapeCraftSE : GenericAbility
    {
        public override void ActivateAbility()
        {
            Rules.Docking.Dock(FindHoundsTooth, GetThisShip);
            HostShip.OnUndocked += OnUndocked;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnUndocked -= OnUndocked;
        }

        private GenericShip GetThisShip()
        {
            return this.HostShip;
        }

        private GenericShip FindHoundsTooth()
        {
            GenericShip result = null;

            foreach (var shipHolder in HostShip.Owner.Ships)
            {
                foreach (var upgrade in shipHolder.Value.UpgradeBar.GetUpgradesOnlyFaceup())
                {
                    // TODOREVERT

                    /*if (upgrade.GetType() == typeof(UpgradesList.HoundsTooth))
                    {
                        result = upgrade.Host;
                        break;
                    }*/
                }
            }

            return result;
        }

        private void OnUndocked(GenericShip dockingHost)
        {
            HostShip.PilotInfo.PilotName = dockingHost.PilotInfo.PilotName;
            HostShip.PilotInfo.Initiative = dockingHost.PilotInfo.Initiative;

            HostShip.PilotInfo.RegensCharges = dockingHost.PilotInfo.RegensCharges;
            HostShip.PilotInfo.Charges = dockingHost.PilotInfo.Charges;

            Type pilotAbilityType = dockingHost.PilotInfo.AbilityType;
            GenericAbility pilotAbility = (GenericAbility)System.Activator.CreateInstance(pilotAbilityType);
            pilotAbility.Initialize(HostShip);
            HostShip.PilotAbilities.Add(pilotAbility);

            Roster.UpdateShipStats(HostShip);
        }
    }
}