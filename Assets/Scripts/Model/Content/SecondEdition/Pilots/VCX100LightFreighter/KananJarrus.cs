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
                    80,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KananJarrusPilotAbility),
                    force: 2,
                    extraUpgradeIcon: UpgradeType.ForcePower,
                    seImageNumber: 74
                );
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
            bool friendly = HostShip.Owner.PlayerNo == Combat.Defender.Owner.PlayerNo;
            bool hasForceTokens = HostShip.State.Force > 0;
            // according to the rules reference FAQ, a ship is never in its own arc
            bool inArc = Board.IsShipInArc(HostShip, Combat.Defender) && HostShip != Combat.Defender;

            if (friendly && hasForceTokens && inArc)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        protected override void AskDecreaseAttack(object sender, System.EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                DecreaseAttack,
                descriptionLong: "Do you want to spend a force token? (If you do, the attacker rolls 1 fewer attack die)",
                imageHolder: HostShip
            );
        }

        protected override void DecreaseAttack(object sender, System.EventArgs e)
        {
            HostShip.State.Force--;
            RegisterDecreaseNumberOfAttackDice();
            SubPhases.DecisionSubPhase.ConfirmDecision();
        }
    }
}