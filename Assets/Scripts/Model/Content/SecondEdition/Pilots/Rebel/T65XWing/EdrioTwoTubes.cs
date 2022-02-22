using System;
using Abilities.SecondEdition;
using Ship;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class EdrioTwoTubes : T65XWing
        {
            public EdrioTwoTubes() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Edrio Two Tubes",
                    2,
                    42,
                    isLimited: true,
                    abilityType: typeof(EdrioTwoTubesAbility),
                    extraUpgradeIcon: UpgradeType.Illicit,
                    seImageNumber: 9
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EdrioTwoTubesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementActivationStart += CheckAbilityConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivationStart -= CheckAbilityConditions;
        }

        private void CheckAbilityConditions(GenericShip ship)
        {
            if (HostShip.Tokens.HasToken(typeof(FocusToken))) RegisterEdrioTwoTubesTrigger();
        }

        private void RegisterEdrioTwoTubesTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskToPerfromFreeAction);
        }

        private void AskToPerfromFreeAction(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                HostShip.GetAvailableActions(),
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "When you become the active ship during the Activation phase, if you have 1 or more focus tokens, you may perform a free action",
                HostShip
            );
        }
    }
}