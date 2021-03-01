using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
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
            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            PilotInfo = new PilotCardInfo(
                "Commandant Goran",
                4,
                42,
                isLimited: true,
                abilityType: typeof(CommandantGoranAbility),
                extraUpgradeIcon: UpgradeType.Talent
            );

            ModelInfo.SkinName = "Vult Skerris";

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