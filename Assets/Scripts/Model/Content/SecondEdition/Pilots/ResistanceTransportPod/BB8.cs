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
                26,
                isLimited: true,
                abilityType: typeof(BB8TransportPodAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ShipInfo.ActionIcons.SwitchToDroidActions();

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/20/47/20474294-ecdf-4000-8f8c-63ba9ff0c9aa/swz45_bb-8.png";
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