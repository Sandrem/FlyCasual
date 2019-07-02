using System.Collections;
using System.Collections.Generic;
using Ship;
using System;
using Tokens;
using Abilities.FirstEdition;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.XWing
    {
        public class EdrioTwoTubes : XWing
        {
            public EdrioTwoTubes() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Edrio Two Tubes",
                    4,
                    24,
                    isLimited: true,
                    abilityType: typeof(EdrioTwoTubesAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Partisan";
            }
        }
    }
}

namespace Abilities.FirstEdition
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