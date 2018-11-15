using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VCX100LightFreighter
    {
        public class KananJarrus : VCX100LightFreighter
        {
            public KananJarrus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kanan Jarrus",
                    3,
                    90,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.KananJarrusPilotAbility),
                    force: 2
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Force);

                SEImageNumber = 74;
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
            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, Combat.Attacker);

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

namespace Abilities.SecondEdition
{
    public class KananJarrusPilotAbility : Abilities.FirstEdition.KananJarrusPilotAbility
    {
        protected override void CheckPilotAbility()
        {
            bool enemy = HostShip.Owner.PlayerNo != Combat.Attacker.Owner.PlayerNo;
            bool hasForceTokens = HostShip.State.Force > 0;
            bool inArc = Board.IsShipInArc(HostShip, Combat.Attacker);

            if (enemy && hasForceTokens && inArc)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        protected override void DecreaseAttack(object sender, System.EventArgs e)
        {
            HostShip.State.Force--;
            RegisterDecreaseNumberOfAttackDice();
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}