using RuleSets;
using Ship;
using SquadBuilderNS;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace Z95
    {
        public class NashtahPup : Z95, ISecondEditionPilot
        {
            public NashtahPup() : base()
            {
                PilotName = "Nashtah Pup";
                PilotSkill = 0;
                Cost = 6;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                PilotRuleType = typeof(SecondEdition);

                faction = Faction.Scum;

                ShipAbilities.Add(new Abilities.SecondEdition.EscapeCraftSE());

                SEImageNumber = 171;
            }

            public void AdaptPilotToSecondEdition()
            {
               // nah
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
                    if (upgrade.GetType() == typeof(UpgradesList.HoundsTooth))
                    {
                        result = upgrade.Host;
                        break;
                    }
                }
            }

            return result;
        }

        private void OnUndocked(GenericShip dockingHost)
        {
            HostShip.PilotName = dockingHost.PilotName;
            HostShip.PilotSkill = dockingHost.PilotSkill;

            HostShip.UsesCharges = dockingHost.UsesCharges;
            HostShip.MaxCharges = dockingHost.MaxCharges;
            HostShip.Charges = dockingHost.Charges;

            Type pilotAbilityType = dockingHost.PilotAbilities[0].GetType();
            GenericAbility pilotAbility = (GenericAbility)System.Activator.CreateInstance(pilotAbilityType);
            pilotAbility.Initialize(HostShip);
            HostShip.PilotAbilities.Add(pilotAbility);

            Roster.UpdateShipStats(HostShip);
        }
    }
}