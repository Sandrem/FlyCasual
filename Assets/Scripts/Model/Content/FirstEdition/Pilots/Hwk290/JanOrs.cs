using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Hwk290
    {
        public class JanOrs : Hwk290
        {
            public JanOrs() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jan Ors",
                    8,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.JanOrsAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class JanOrsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += RegisterJanOrsAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= RegisterJanOrsAbility;
        }

        protected virtual void RegisterJanOrsAbility()
        {
            if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && Combat.Attacker.ShipId != HostShip.ShipId)
            {
                BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(Combat.Attacker, HostShip);
                if (distanceInfo.Range < 4)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskJanOrsAbility);
                }
            }
        }

        protected void AskJanOrsAbility(object sender, System.EventArgs e)
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(AlwaysUseByDefault, UseJanOrsAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseJanOrsAbility(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(StressToken), AllowRollAdditionalDice);
        }

        private void AllowRollAdditionalDice()
        {
            Combat.Attacker.AfterGotNumberOfAttackDice += IncreaseByOne;
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void IncreaseByOne(ref int value)
        {
            value++;
            Combat.Attacker.AfterGotNumberOfAttackDice -= IncreaseByOne;
        }
    }
}