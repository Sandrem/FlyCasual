using Abilities.SecondEdition;
using ActionsList;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.ResistanceTransportPod
{
    public class BB8 : ResistanceTransportPod
    {
        public BB8()
        {
            PilotInfo = new PilotCardInfo25
            (
                "BB-8",
                "Full of Surprises",
                Faction.Resistance,
                3,
                2,
                4,
                isLimited: true,
                abilityType: typeof(BB8TransportPodAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Tech
                },
                tags: new List<Tags>
                {
                    Tags.Droid 
                }
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
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsAbilityActivation -= RegisterOwnTrigger;
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            flag = true;
        }

        private void RegisterOwnTrigger(GenericShip ship)
        {
            // Always register
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToPerformReposition);
        }

        private void AskToPerformReposition(object sender, EventArgs e)
        {
            Sounds.PlayShipSound("BB-8-Sound");

            HostShip.AskPerformFreeAction(
                new List<GenericAction>()
                {
                    new BarrelRollAction(){Color = Actions.ActionColor.Red},
                    new BoostAction(){Color = Actions.ActionColor.Red}
                },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "During the System Phase, you may perform a red Barrel Roll or Boost action",
                HostShip
            );
        }
    }
}