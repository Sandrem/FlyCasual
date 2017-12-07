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

                PilotAbilities.Add(new AbilitiesNamespace.FelsWrathAbility());
            }
		}
	}
}

namespace AbilitiesNamespace
{
    public class FelsWrathAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnReadyToBeDestroyed += ActivateAbility;
        }

        private void ActivateAbility(GenericShip ship)
        {
            HostShip.OnReadyToBeDestroyed -= ActivateAbility;

            HostShip.PreventDestruction = true;

            Phases.OnCombatPhaseEnd += ProcessFelsWrath;
        }

        public void ProcessFelsWrath()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, CleanUpFelsWrath);
        }

        private void CleanUpFelsWrath(object sender, EventArgs e)
        {
            HostShip.PreventDestruction = false;
            Phases.OnCombatPhaseEnd -= ProcessFelsWrath;

            Selection.ThisShip = HostShip;
            HostShip.DestroyShip(Triggers.FinishTrigger, true);
        }
    }
}
