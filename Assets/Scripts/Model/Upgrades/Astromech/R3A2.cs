using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;

namespace UpgradesList
{

    public class R3A2 : GenericUpgrade
    {
        public R3A2() : base()
        {
            Type = UpgradeType.Astromech;
            Name = "R3-A2";
            Cost = 2;

            isUnique = true;

            UpgradeAbilities.Add(new R3A2Ability());
        }
    }

}

namespace Abilities
{
    public class R3A2Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += CheckR3A2Ability;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= CheckR3A2Ability;
        }

        private void CheckR3A2Ability()
        {
            if (Combat.AttackStep == CombatStep.Attack && Combat.Attacker.ShipId == HostShip.ShipId)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskAssignStress);
            }
        }

        private void AskAssignStress(object sender, System.EventArgs e)
        {
            if (!alwaysUseAbility)
            {
                AskToUseAbility(AlwaysUseByDefault, AssignStressAndFinishDecision, null, null, true);
            }
            else
            {
                AssignStress(Triggers.FinishTrigger);
            }
        }

        private void AssignStressAndFinishDecision(object sender, System.EventArgs e)
        {
            AssignStress(SubPhases.DecisionSubPhase.ConfirmDecision);
        }

        private void AssignStress(Action callback)
        {
            Messages.ShowInfo(Name + " is used");
            Sounds.PlayShipSound("R2D2-Beeping-5");

            HostShip.AssignToken(new Tokens.StressToken(), delegate { AssignStressToDefender(callback); });
        }

        private void AssignStressToDefender(Action callback)
        {
            Combat.Defender.AssignToken(new Tokens.StressToken(), callback);
        }

    }
}
