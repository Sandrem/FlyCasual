using Ship;
using System;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.TIEInterceptor
    {
        public class FelsWrath : TIEInterceptor
        {
            public FelsWrath() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Fel's Wrath",
                    5,
                    23,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.FelsWrathAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class FelsWrathAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnReadyToBeDestroyed += ActivateAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnReadyToBeDestroyed -= ActivateAbility;
        }

        private void ActivateAbility(GenericShip ship)
        {
            HostShip.OnReadyToBeDestroyed -= ActivateAbility;

            HostShip.PreventDestruction = true;

            Phases.Events.OnCombatPhaseEnd_NoTriggers += ProcessFelsWrath;
        }

        public void ProcessFelsWrath()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, CleanUpFelsWrath);
        }

        private void CleanUpFelsWrath(object sender, EventArgs e)
        {
            HostShip.PreventDestruction = false;
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= ProcessFelsWrath;

            Selection.ThisShip = HostShip;
            HostShip.DestroyShipForced(Triggers.FinishTrigger);
        }
    }
}