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
        private bool IsAbilityActivated = false;

        public override void ActivateAbility()
        {
            HostShip.OnCheckPreventDestruction += ActivateAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckPreventDestruction -= ActivateAbility;
        }

        private void ActivateAbility(GenericShip ship, ref bool preventDestruction)
        {
            preventDestruction = true;

            if (!IsAbilityActivated)
            {
                IsAbilityActivated = true;
                Phases.Events.OnCombatPhaseEnd_NoTriggers += ProcessFelsWrath;
            }
        }

        public void ProcessFelsWrath()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, CleanUpFelsWrath);
        }

        private void CleanUpFelsWrath(object sender, EventArgs e)
        {
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= ProcessFelsWrath;

            Selection.ChangeActiveShip(HostShip);
            HostShip.DestroyShipForced(Triggers.FinishTrigger);
        }
    }
}