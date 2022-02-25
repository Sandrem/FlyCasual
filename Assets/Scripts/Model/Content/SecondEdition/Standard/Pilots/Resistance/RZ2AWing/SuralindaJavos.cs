using Abilities.Parameters;
using Content;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class SuralindaJavos  : RZ2AWing
        {
            public SuralindaJavos() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Suralinda Javos",
                    "Inquisitive Journalist",
                    Faction.Resistance,
                    3,
                    4,
                    10,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SuralindaJavosAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    skinName: "Blue"
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e1/64/e1644adc-8d8a-4408-90a1-621e0dd4b0c6/swz68_suralinda-javos.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SuralindaJavosAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterManeuver
        (
            onlyIfPartialExecuted: true
        );

        public override AbilityPart Action => new AskToUseAbilityAction
        (
            description: new AbilityDescription
            (
                name: "Suralinda Javos",
                description: "Do you want to gain Strain token to rotate 90 or 180 degrees?",
                imageSource: HostShip
            ),
            onYes: new AssignTokenAction
            (
                tokenType: typeof(StrainToken),
                targetShipRole: ShipRole.HostShip,
                showMessage: GetGainedStrainTokenMessage,
                afterAction: new AskToRotateShipAction
                (
                    description: new AbilityDescription
                    (
                        name: "Suralinda Javos",
                        description: "Choose how do you rotate",
                        imageSource: HostShip
                    ),
                    rotate90allowed: true,
                    rotate180allowed: true
                )
            )
        );

        private string GetGainedStrainTokenMessage()
        {
            return "Suralinda Javos: Gained strain token to rotate";
        }
    }
}