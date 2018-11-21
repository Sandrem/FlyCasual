using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace FirstEdition.VCX100
    {
        public class KananJarrus : VCX100
        {
            public KananJarrus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kanan Jarrus",
                    5,
                    38,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.KananJarrusPilotAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class KananJarrusPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckPilotAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckPilotAbility;
        }

        protected virtual void CheckPilotAbility()
        {
            bool IsDifferentPlayer = (HostShip.Owner.PlayerNo != Combat.Attacker.Owner.PlayerNo);
            bool HasFocusTokens = HostShip.Tokens.HasToken(typeof(FocusToken));
            DistanceInfo distanceInfo = new DistanceInfo(HostShip, Combat.Attacker);

            if (IsDifferentPlayer && HasFocusTokens && distanceInfo.Range < 3)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        protected void AskDecreaseAttack(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, DecreaseAttack, null, null, false);
        }

        protected virtual void DecreaseAttack(object sender, System.EventArgs e)
        {
            HostShip.Tokens.SpendToken(typeof(FocusToken), RegisterDecreaseNumberOfAttackDice);
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        protected void RegisterDecreaseNumberOfAttackDice()
        {
            Combat.Attacker.AfterGotNumberOfAttackDice += DecreaseNumberOfAttackDice;
        }

        private void DecreaseNumberOfAttackDice(ref int diceCount)
        {
            diceCount--;
            Combat.Attacker.AfterGotNumberOfAttackDice -= DecreaseNumberOfAttackDice;
        }
    }

}