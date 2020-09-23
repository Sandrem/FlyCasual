using Abilities.Parameters;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class DGS286 : HMPDroidGunship
        {
            public DGS286() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "DGS-286",
                    3,
                    42,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Crew, UpgradeType.Device },
                    abilityType: typeof(Abilities.SecondEdition.DGS286Ability)
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/26/c0/26c041f8-90e4-4cc6-8fa4-219b87ac502b/swz71_card_dgs286.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DGS286Ability : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new BeforeYouEngage();

        public override AbilityPart Action => new SelectShipAction
        (
            abilityDescription: new AbilityDescription
            (
                name: "DGS-286",
                description: "Choose another friendly ship to transfer Calculate token to you",
                imageSource: HostShip
            ),
            filter: new SelectShipFilter
            (
                minRange: 0,
                maxRange: 1,
                targetTypes: TargetTypes.OtherFriendly,
                hasToken: typeof(CalculateToken)
            ),
            action: new TransferTokenFromTargetAction
            (
                tokenType: typeof(CalculateToken),
                showMessage: GetMessageToShow
            ),
            aiSelectShipPlan: new AiSelectShipPlan
            (
                aiSelectShipTeamPriority: AiSelectShipTeamPriority.Friendly,
                aiSelectShipSpecial: AiSelectShipSpecial.Worst
            )
        );

        private string GetMessageToShow()
        {
            return "DGS-286: Calculate Token is transfered from " + TargetShip.PilotInfo.PilotName;
        }
    }
}