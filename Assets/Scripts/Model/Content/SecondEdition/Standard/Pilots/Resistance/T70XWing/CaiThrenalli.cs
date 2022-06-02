using Abilities.Parameters;
using ActionsList;
using Content;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class CaiThrenalli : T70XWing
        {
            public CaiThrenalli() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "C’ai Threnalli",
                    "Tenacious Survivor",
                    Faction.Resistance,
                    4,
                    5,
                    13,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaiThrenalliAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/60/8a/608a4657-6612-417d-bd10-be587c2a208f/swz68_cai-threnalli.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaiThrenalliAbility : TriggeredAbility
    {
        public override TriggerForAbility Trigger => new AfterManeuver
        (
            onlyIfFullyExecuted: true,
            onlyIfMovedThroughFriendlyShip: true
        );

        public override AbilityPart Action => new AskToPerformAction
        (
            new AbilityDescription
            (
                "C’ai Threnalli",
                "You may perform an Evade action",
                HostShip
            ),
            new ActionInfo
            (
                typeof(EvadeAction)
            )
        );
    }
}