using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.TIEInterceptor
{
    public class CommandantGoran : TIEInterceptor
    {
        public CommandantGoran() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Commandant Goran",
                "Skystrike Superintendent",
                Faction.Imperial,
                4,
                4,
                8,
                isLimited: true,
                abilityType: typeof(CommandantGoranAbility),
                extraUpgradeIcons: new List<UpgradeType>()
                {
                    UpgradeType.Talent,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Tie
                },
                skinName: "Skystrike Academy"
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/94/37/94377171-95d9-40e5-99be-8f8d6e52eb28/swz84_pilot_commandantgoran.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CommandantGoranAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishUnsuccessfullyGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishUnsuccessfullyGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Tools.IsSameTeam(HostShip, ship) && HostShip.State.Initiative > ship.State.Initiative)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range <= 3) RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToUseGorransAbility);
            }
        }

        private void AskToUseGorransAbility(object sender, EventArgs e)
        {
            if (Selection.ThisShip != null)
            {
                Selection.ThisShip.AskPerformFreeAction
                (
                    new FocusAction() { HostShip = Selection.ThisShip, Color = Actions.ActionColor.Red },
                    Triggers.FinishTrigger,
                    descriptionShort: HostShip.PilotInfo.PilotName,
                    descriptionLong: "You may perform red Focus acton",
                    imageHolder: HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}