namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class ColonelVessery : TIEDDefender
        {
            public ColonelVessery() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Colonel Vessery",
                    4,
                    88,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ColonelVesseryAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 123;
            }
        }
    }
}

namespace Abilities.FirstEdition
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
            ActionsHolder.AcquireTargetLock(Combat.Attacker, Combat.Defender, SubPhases.DecisionSubPhase.ConfirmDecision, SubPhases.DecisionSubPhase.ConfirmDecision);
        }
    }
}