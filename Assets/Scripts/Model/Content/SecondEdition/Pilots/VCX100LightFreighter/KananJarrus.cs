using BoardTools;
using System.Collections;
using System.Collections.Generic;
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