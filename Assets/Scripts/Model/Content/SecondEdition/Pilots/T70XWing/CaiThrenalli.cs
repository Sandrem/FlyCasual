using Abilities.Parameters;
using ActionsList;
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
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo(
                    "C’ai Threnalli",
                    4,
                    51,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaiThrenalliAbility),
                    extraUpgradeIcon: UpgradeType.Talent
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