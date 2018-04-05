using System;
using Ship;

namespace Ship
{
	namespace TIEInterceptor
	{
		public class FelsWrath : TIEInterceptor
		{
			protected bool IsDestructionIsDelayed;

            public FelsWrath()
            {
                PilotName = "\"Fel's Wrath\"";
                PilotSkill = 5;
                Cost = 23;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.FelsWrathAbility());
            }
		}
	}
}

namespace Abilities
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

            Phases.OnCombatPhaseEnd_NoTriggers += ProcessFelsWrath;
        }

        public void ProcessFelsWrath()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, CleanUpFelsWrath);
        }

        private void CleanUpFelsWrath(object sender, EventArgs e)
        {
            HostShip.PreventDestruction = false;
            Phases.OnCombatPhaseEnd_NoTriggers -= ProcessFelsWrath;

            Selection.ThisShip = HostShip;
            HostShip.DestroyShipForced(Triggers.FinishTrigger);
        }
    }
}
