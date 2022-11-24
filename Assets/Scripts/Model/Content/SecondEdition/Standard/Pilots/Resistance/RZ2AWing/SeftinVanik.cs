using Abilities.Parameters;
using ActionsList;
using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Seftin Vanik",
                    "Skillful Wingmate",
                    Faction.Resistance,
                    5,
                    4,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SeftinVanikAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    skinName: "Green (HoH)"
                );

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
            conditions: new ConditionsBlock
            (
                new RangeToHostCondition(1, 1),
                new TeamCondition(ShipTypes.OtherFriendly)
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