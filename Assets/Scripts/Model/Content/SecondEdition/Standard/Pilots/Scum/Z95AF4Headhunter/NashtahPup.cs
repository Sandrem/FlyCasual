using Ship;
using SquadBuilderNS;
using System;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class NashtahPup : Z95AF4Headhunter
        {
            public NashtahPup() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Nashtah Pup",
                    "Contingency Plan",
                    Faction.Scum,
                    0,
                    1,
                    0,
                    isLimited: true,
                    seImageNumber: 171,
                    skinName: "Nashtah Pup"
                );

                ShipAbilities.Add(new Abilities.SecondEdition.EscapeCraftSE());
            }

            public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
            {
                if (squadList.HasUpgrade("Hound's Tooth"))
                {
                    return true;
                }
                else
                {
                    Messages.ShowError("You need YV-666 ship with the Hound's Tooth title\nto use Nashtah Pup in a squad");
                    return false;
                }
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
            HostShip.OnDocked += StartDenyUndocking;
            HostShip.OnUndocked += OnUndocked;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDocked -= StartDenyUndocking;
            HostShip.OnUndocked -= OnUndocked;

            if (HostShip.DockingHost != null) HostShip.DockingHost.OnCanReleaseDockedShipRegular -= DenyRelease;
        }

        private void StartDenyUndocking(GenericShip ship)
        {
            ship.OnCanReleaseDockedShipRegular += DenyRelease;
        }

        private void DenyRelease(ref bool canRelease)
        {
            canRelease = false;
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
            dockingHost.OnCanReleaseDockedShipRegular -= DenyRelease;

            HostShip.PilotInfo = new PilotCardInfo(
                dockingHost.PilotInfo.PilotName,
                dockingHost.PilotInfo.Initiative,
                6,
                isLimited: true,
                charges: dockingHost.PilotInfo.Charges,
                regensCharges: dockingHost.PilotInfo.RegensCharges
            );

            Type pilotAbilityType = dockingHost.PilotInfo.AbilityType;
            if (pilotAbilityType != null)
            {
                GenericAbility pilotAbility = (GenericAbility)System.Activator.CreateInstance(pilotAbilityType);
                pilotAbility.Initialize(HostShip);
                HostShip.PilotAbilities.Add(pilotAbility);
            }
            HostShip.InitializeState();

            Roster.UpdateShipStats(HostShip);
        }
    }
}