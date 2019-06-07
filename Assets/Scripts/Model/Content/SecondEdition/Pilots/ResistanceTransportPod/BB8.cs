using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransportPod
{
    public class BB8 : ResistanceTransportPod
    {
        public BB8()
        {
            PilotInfo = new PilotCardInfo(
                "BB-8",
                3,
                33,
                isLimited: true,
                abilityType: typeof(BB8TransportPodAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ShipInfo.ActionIcons.SwitchToDroidActions();

            ImageUrl = "https://i.imgur.com/tvYZ2Ig.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BB8TransportPodAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsAbilityActivation += RegisterOwnTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivation -= RegisterOwnTrigger;
        }

        private void RegisterOwnTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToPerformReposition);
        }

        private void AskToPerformReposition(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new BarrelRollAction(){Color = Actions.ActionColor.Red},
                    new BoostAction(){Color = Actions.ActionColor.Red}
                },
                Triggers.FinishTrigger
            );
        }
    }
}