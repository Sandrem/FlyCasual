using Abilities.Parameters;
using Content;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.Delta7Aethersprite
{
    public class PloKoon : Delta7Aethersprite
    {
        public PloKoon()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Plo Koon",
                "Serene Mentor",
                Faction.Republic,
                5,
                5,
                8,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.PloKoonAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                },
                skinName: "Plo Koon"
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6a/6f/6a6fef51-fb5f-49c1-b5cc-8e96b6d09051/swz32_plo-koon.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagement Phase, you may spend 1 force and choose another friendly ship at range 0-2. 
    //If you do, you may transfer 1 green token to it or transfer one orange token from it to you.
    public class PloKoonAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AtTheStartOfPhase(typeof(SubPhases.CombatStartSubPhase));

        public override AbilityPart Action => new AskToUseAbilityAction
        (
            description: new AbilityDescription
            (
                name: "Plo Koon",
                description: "Do you want to spend Force to transfer tokens?",
                imageSource: HostShip
            ),
            conditions: new ConditionsBlock
            (
                new CanSpendForceCondition(),
                new OrCondition
                (
                    new AndCondition
                    (
                        new HasTokenCondition(tokenColor: TokenColors.Green),
                        new HasAnyShipAtRange
                        (
                            new ConditionsBlock
                            (
                                new RangeToHostCondition(0, 2),
                                new TeamCondition(ShipTypes.OtherFriendly)
                            )
                        )
                    ),
                    new HasAnyShipAtRange
                    (
                        new ConditionsBlock
                        (
                            new RangeToHostCondition(0, 2),
                            new HasTokenCondition(tokenColor: TokenColors.Orange),
                            new TeamCondition(ShipTypes.OtherFriendly)
                        )
                    )
                )
            ),
            onYes: new SelectShipAction
            (
                new AbilityDescription
                (
                    name: "Plo Koon",
                    description: "Choose another friendly ship to transfer token",
                    imageSource: HostShip
                ),
                new ConditionsBlock
                (
                    new CanSpendForceCondition(),
                    new AndCondition
                    (
                        new RangeToHostCondition(0, 2),
                        new TeamCondition(ShipTypes.OtherFriendly),
                        new OrCondition
                        (
                            new HasTokenCondition(tokenColor: TokenColors.Orange),
                            new HasTokenCondition(tokenColor: TokenColors.Green, shipRoleToCheck: ShipRole.HostShip)
                        )
                    )
                ),
                new ExchangeToken
                (
                    getByColor: TokenColors.Orange,
                    giveByColor: TokenColors.Green,
                    showMessage: ShowMessage,
                    doNext: new SpendForceAction()
                ),
                aiSelectShipPlan: new AiSelectShipPlan(AiSelectShipTeamPriority.Friendly, AiSelectShipSpecial.None)
            )
        );

        private string ShowMessage()
        {
            return "Message";
        }
    }
}
