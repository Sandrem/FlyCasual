using System.Collections.Generic;
using Ship;

namespace Ship
{
    namespace ARC170
    {
        public class ThaneKyrell : ARC170
		{
			public ThaneKyrell() : base()
			{
				PilotName = "Thane Kyrell";
				PilotSkill = 4;
				Cost = 26;

				IsUnique = true;

				PilotAbilities.Add(new Abilities.ThaneKyrellPilotAbility());
			}
		}
	}
}

namespace Abilities
{
    // After an enemy ship inside your firing arc at Range 1-3 attacks another friendly ship, you may perform a free action.
    public class ThaneKyrellPilotAbility : GenericAbility
	{
		public override void ActivateAbility()
		{
            GenericShip.OnAttackFinishGlobal += RegisterBraylenStrammPilotAbility;
		}

		public override void DeactivateAbility()
		{
            GenericShip.OnAttackFinishGlobal -= RegisterBraylenStrammPilotAbility;
		}

		private void RegisterBraylenStrammPilotAbility(GenericShip ship)
		{
            if (Combat.Attacker == ship && ship.Owner != HostShip.Owner && Combat.Defender != HostShip && Combat.Defender.Owner == HostShip.Owner)
            {
                var arcInfo = new Board.ShipShotDistanceInformation(HostShip, ship);
                if (arcInfo.InArc && arcInfo.Range <= 3)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, PerformFreeAction);
                }
            }
		}

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            HostShip.GenerateAvailableActionsList();

            var previousSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;

            HostShip.AskPerformFreeAction(HostShip.GetAvailableActionsList(), delegate 
            {
                Selection.ThisShip = previousSelectedShip;
                Triggers.FinishTrigger();
            });
        }
	}
}

