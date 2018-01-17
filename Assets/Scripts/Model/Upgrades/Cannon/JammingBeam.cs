using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

	public class JammingBeam : GenericSecondaryWeapon
	{
		public JammingBeam() : base()
		{
            Types.Add(UpgradeType.Cannon);

			Name = "Jamming Beam";
			Cost = 2;

			MinRange = 1;
			MaxRange = 2;
			AttackValue = 3;
		}

		public override void AttachToShip(Ship.GenericShip host)
		{
			base.AttachToShip(host);

			SubscribeOnHit();
		}

		private void SubscribeOnHit()
		{
			Host.OnShotHitAsAttacker += RegisterJammingBeamEffect;
		}

		private void RegisterJammingBeamEffect()
		{
			if (Combat.ChosenWeapon == this)
			{
				Triggers.RegisterTrigger(
                    new Trigger(){
					    Name = "Jamming Beam effect",
					    TriggerType = TriggerTypes.OnShotHit,
					    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
					    EventHandler = JammingBeamEffect
                    });
			}
		}

		private void JammingBeamEffect(object sender, System.EventArgs e)
		{
			Combat.DiceRollAttack.CancelAllResults();
			Combat.DiceRollAttack.RemoveAllFailures();

			Combat.Defender.AssignToken(
				new Tokens.JamToken(),
                Triggers.FinishTrigger
            );
		}

	}

}