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

                PilotAbilities.Add(new PilotAbilitiesNamespace.ColonelVesseryAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class ColonelVesseryAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.OnImmediatelyAfterRolling += RegisterColonelVesseryAbility;
        }

        private void RegisterColonelVesseryAbility(DiceRoll diceroll)
        {
            RegisterAbilityTrigger(TriggerTypes.OnImmediatelyAfterRolling, AskColonelVesseryAbility);
        }

        private void AskColonelVesseryAbility(object sender, System.EventArgs e)
        { 
            if (Combat.AttackStep == CombatStep.Attack && Combat.Defender.HasToken(typeof(Tokens.RedTargetLockToken), '*'))
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
            Actions.AssignTargetLockToPair(Combat.Attacker, Combat.Defender, SubPhases.DecisionSubPhase.ConfirmDecision, SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}
