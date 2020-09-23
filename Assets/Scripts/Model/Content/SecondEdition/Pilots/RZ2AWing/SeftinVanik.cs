using Abilities.Parameters;
using ActionsList;
using Arcs;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class SeftinVanik : RZ2AWing
        {
            public SeftinVanik() : base()
            {
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo(
                    "Seftin Vanik",
                    5,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SeftinVanikAbility),
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent }
                );

                ModelInfo.SkinName = "New Republic";

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/73/ef/73ef0cdc-deb6-451d-a76c-0b3d9ef147ec/swz68_seftin-vanik.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SeftinVanikAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterYouPerformAction
        (
            actionType: typeof(BoostAction),
            hasToken: typeof(EvadeToken)
        );

        public override AbilityPart Action => new SelectShipAction
        (
            abilityDescription: new AbilityDescription
            (
                name: "Seftin Vanik",
                description: "You may transfer Evade token to a friendly ship",
                imageSource: HostShip
            ),
            filter: new SelectShipFilter
            (
                minRange: 1,
                maxRange: 1,
                targetTypes: TargetTypes.OtherFriendly                
            ),
            action: new TransferTokenToTargetAction
            (
                tokenType: typeof(EvadeToken),
                showMessage: ShowTransferSuccessMessage
            ),
            aiSelectShipPlan: new AiSelectShipPlan
            (
                aiSelectShipTeamPriority: AiSelectShipTeamPriority.Friendly
            )
        );

        private string ShowTransferSuccessMessage()
        {
            return "Seftin Vanik: Evade Token is transfered to " + TargetShip.PilotInfo.PilotName;
        }
    }
}