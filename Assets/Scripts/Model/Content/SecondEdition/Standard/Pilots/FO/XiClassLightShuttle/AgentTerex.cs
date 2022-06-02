using System.Collections.Generic;
using Upgrade;
using Abilities.Parameters;

namespace Ship
{
    namespace SecondEdition.XiClassLightShuttle
    {
        public class AgentTerex : XiClassLightShuttle
        {
            public AgentTerex() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Agent Terex",
                    "Devious Provocateur",
                    Faction.FirstOrder,
                    3,
                    4,
                    13,
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit
                    },
                    abilityType: typeof(Abilities.SecondEdition.AgentTerexPilotAbility)
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/9a/27/9a27c06f-58be-49b7-ab36-63dc2fae3b9a/swz69_a1_ship_terex.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AgentTerexPilotAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterPlacingForces();

        public override AbilityPart Action => new EachUpgradeCanDoAction
        (
            eachUpgradeAction: new SelectShipAction
            (
                action: new TransferUpgradeAction(),
                conditions: new ConditionsBlock
                (
                    new ShipTypeCondition
                    (
                        typeof(Ship.SecondEdition.TIEFoFighter.TIEFoFighter),
                        typeof(Ship.SecondEdition.TIESfFighter.TIESfFighter)
                    )
                ),
                abilityDescription: new AbilityDescription
                (
                    "Agent Terex",
                    "Select a ship to equip",
                    imageSource: HostShip
                ),
                aiSelectShipPlan: new AiSelectShipPlan
                (
                    AiSelectShipTeamPriority.Friendly,
                    AiSelectShipSpecial.None
                )
            ),
            conditions: new ConditionsBlock
            (
                new UpgradeTypeCondition(UpgradeType.Illicit)
            )
        );
    }
}

