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

                PilotAbilities.Add(new PilotAbilitiesNamespace.FelsWrathAbility());
            }
		}
	}
}

namespace PilotAbilitiesNamespace
{
    public class FelsWrathAbility : GenericPilotAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            Host.OnReadyToBeDestroyed += ActivateAbility;
        }

        private void ActivateAbility(GenericShip ship)
        {
            Host.OnReadyToBeDestroyed -= ActivateAbility;

            Host.PreventDestruction = true;

            Phases.OnCombatPhaseEnd += ProcessFelsWrath;
        }

        public void ProcessFelsWrath()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, CleanUpFelsWrath);
        }

        private void CleanUpFelsWrath(object sender, EventArgs e)
        {
            Host.PreventDestruction = false;
            Phases.OnCombatPhaseEnd -= ProcessFelsWrath;

            Selection.ThisShip = Host;
            Host.DestroyShip(Triggers.FinishTrigger, true);
        }
    }
}
