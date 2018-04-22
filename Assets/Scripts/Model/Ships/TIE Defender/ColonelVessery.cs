using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEDefender
    {
        public class ColonelVessery : TIEDefender
        {
            public ColonelVessery() : base()
            {
                PilotName = "Colonel Vessery";
                PilotSkill = 6;
                Cost = 35;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.ColonelVesseryAbility());
            }
        }
    }
}

namespace Abilities
{
    public class ColonelVesseryAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling += RegisterColonelVesseryAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling -= RegisterColonelVesseryAbility;
        }

        private void RegisterColonelVesseryAbility(DiceRoll diceroll)
        {
            RegisterAbilityTrigger(TriggerTypes.OnImmediatelyAfterRolling, AskColonelVesseryAbility);
        }

        private void AskColonelVesseryAbility(object sender, System.EventArgs e)
        { 
            if (Combat.AttackStep == CombatStep.Attack && Combat.Defender.Tokens.HasToken(typeof(Tokens.RedTargetLockToken), '*'))
            {
                AskToUseAbility(AlwaysUseByDefault, UseColonelVesseryAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseColonelVesseryAbility(object sender, System.EventArgs e)
        {
            Actions.AcquireTargetLock(Combat.Attacker, Combat.Defender, SubPhases.DecisionSubPhase.ConfirmDecision, SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}
