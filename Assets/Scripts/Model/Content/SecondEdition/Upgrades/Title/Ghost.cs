using Actions;
using ActionsList;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Ghost : GenericUpgrade
    {
        public Ghost() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Ghost",
                UpgradeType.Title,
                cost: 0,
                isLimited: true,
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Rebel),
                    new ShipRestriction(typeof(Ship.SecondEdition.VCX100LightFreighter.VCX100LightFreighter))
                ),
                abilityType: typeof(Abilities.SecondEdition.GhostAbility),
                seImageNumber: 102
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class GhostAbility : GenericAbility
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
                .Where(s => s.Value is Ship.SecondEdition.AttackShuttle.AttackShuttle || s.Value is Ship.SecondEdition.SheathipedeClassShuttle.SheathipedeClassShuttle)
                .Select(n => n.Value)
                .ToList();
        }

        private void AskInitialDocking(object sender, EventArgs e)
        {
            AskToUseAbility(
                AlwaysUseByDefault,
                StartInitialDocking,
                infoText: "Ghost: Do you want to dock a shuttle?"
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