using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using Tokens;
using UnityEngine;
using Upgrade;
using RuleSets;

namespace UpgradesList
{
	public class JammingBeam : GenericSecondaryWeapon, ISecondEditionUpgrade
	{
		public JammingBeam()
		{
            Types.Add(UpgradeType.Cannon);

			Name = "Jamming Beam";
			Cost = 1;

			MinRange = 1;
			MaxRange = 2;
			AttackValue = 3;

            UpgradeAbilities.Add(new JammingBeamAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 2;

            UpgradeAbilities.RemoveAll(a => a is JammingBeamAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.JammingBeamAbilitySE());

            SEImageNumber = 29;
        }
    }
}

namespace Abilities
{
    public class JammingBeamAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterJammingBeamEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnShotHitAsAttacker -= RegisterJammingBeamEffect;
        }

        private void RegisterJammingBeamEffect()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShotHit, JammingBeamEffect);
            }
        }

        private void JammingBeamEffect(object sender, System.EventArgs e)
        {
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignToken(typeof(JamToken), Triggers.FinishTrigger);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JammingBeamAbilitySE : JammingBeamAbility
    {

        protected void JammingBeamEffect(object sender, System.EventArgs e)
        {
            int jammingBeamTokens = Combat.DiceRollAttack.Successes;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(
                () => new JamToken(Combat.Defender),
                jammingBeamTokens,
                Triggers.FinishTrigger
            );
        }
    }
}