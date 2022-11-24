using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.RZ2AWing
    {
        public class WrobieTyce : RZ2AWing
        {
            public WrobieTyce() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Wrobie Tyce",
                    "Dynamic Aerialist",
                    Faction.Resistance,
                    4,
                    3,
                    4,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WrobieTyceAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.AWing
                    },
                    skinName: "Green (HoH)"
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d3/66/d3669149-00da-4abf-9e08-9655e10db166/swz68_wrobie-tyce.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WrobieTyceAbility : CombinedAbility
    {
        public override List<Type> CombinedAbilities => new List<Type>()
        {
            typeof(WrobieTyceAbilityDefense),
            typeof(WrobieTyceAbilityAttack),
        };

        private class WrobieTyceAbilityDefense : TriggeredAbility
        {
            public override TriggerForAbility Trigger => new AfterYouDefend
            (
                attackRangeMin: 1,
                attackRangeMax: 1,
                onlyIfAttackIsModifiedByAttacker: true
            );

            public override AbilityPart Action => new AssignTokenAction
            (
                tokenType: typeof(DepleteToken),
                targetShipRole: ShipRole.Attacker,
                showMessage: ShowDepleteAttackerMessage
            );

            private string ShowDepleteAttackerMessage()
            {
                return "Wrobie Tyce: " + Combat.Attacker.PilotInfo.PilotName + " gains Deplete token";
            }
        }

        private class WrobieTyceAbilityAttack : TriggeredAbility
        {
            public override TriggerForAbility Trigger => new AfterYouAttack
            (
                attackRangeMin: 1,
                attackRangeMax: 1,
                onlyIfDefenseIsModifiedByDefender: true
            );

            public override AbilityPart Action => new AssignTokenAction
            (
                tokenType: typeof(DepleteToken),
                targetShipRole: ShipRole.Defender,
                showMessage: ShowDepleteDefenderMessage
            );

            private string ShowDepleteDefenderMessage()
            {
                return "Wrobie Tyce: " + Combat.Defender.PilotInfo.PilotName + " gains Deplete token";
            }
        }
    }
}