using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class LandosMilleniumFalcon : GenericUpgrade
    {
        public LandosMilleniumFalcon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Lando's Millenium Falcon",
                UpgradeType.Title,
                cost: 6,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Scum),
                    new ShipRestriction(typeof(Ship.SecondEdition.CustomizedYT1300LightFreighter.CustomizedYT1300LightFreighter))
                ),
                abilityType: typeof(Abilities.SecondEdition.LandosMilleniumFalconAbility),
                seImageNumber: 164
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class LandosMilleniumFalconAbility : GenericAbility
    {
        private List<GenericShip> DockableShips;

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += CheckInitialDockingAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= CheckInitialDockingAbility;
        }

        private void CheckInitialDockingAbility()
        {
            DockableShips = GetDockableShips();
            if (DockableShips.Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnSetupStart, AskInitialDocking);
            }
        }

        private List<GenericShip> GetDockableShips()
        {
            return HostShip.Owner.Ships
                .Where(s => s.Value is Ship.SecondEdition.EscapeCraft.EscapeCraft)
                .Select(n => n.Value)
                .ToList();
        }

        private void AskInitialDocking(object sender, EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                StartInitialDocking,
                infoText: "Lando's Millenium Falcon: Do you want to dock a shuttle?"
            );
        }

        private void StartInitialDocking(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            if (DockableShips.Count == 1)
            {
                Rules.Docking.Dock(HostShip, DockableShips.First());
                Triggers.FinishTrigger();
            }
            else
            {
                // Ask what ships to dock
            }
        }
    }
}