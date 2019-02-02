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
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NdruSuhlakAbility),
                    extraUpgradeIcon: UpgradeType.Illicit,
                    factionOverride: Faction.Scum,
                    seImageNumber: 171
                );

                ShipAbilities.Add(new Abilities.SecondEdition.EscapeCraftSE());
            }

            public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
            {
                foreach (var shipHolder in squadList.GetShips())
                {
                    foreach (var upgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                    {
                        if (upgrade.UpgradeInfo.Name == "Hound's Tooth")
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
                    if (upgrade.GetType() == typeof(UpgradesList.SecondEdition.HoundsTooth))
                    {
                        result = upgrade.HostShip;
                        break;
                    }
                }
            }

            return result;
        }

        private void OnUndocked(GenericShip dockingHost)
        {
            HostShip.PilotInfo = new PilotCardInfo(
                dockingHost.PilotInfo.PilotName,
                dockingHost.PilotInfo.Initiative,
                6,
                isLimited: true,
                charges: dockingHost.PilotInfo.Charges,
                regensCharges: dockingHost.PilotInfo.RegensCharges
            );

            Type pilotAbilityType = dockingHost.PilotInfo.AbilityType;
            GenericAbility pilotAbility = (GenericAbility)System.Activator.CreateInstance(pilotAbilityType);
            pilotAbility.Initialize(HostShip);
            HostShip.PilotAbilities.Add(pilotAbility);

            Roster.UpdateShipStats(HostShip);
        }
    }
}