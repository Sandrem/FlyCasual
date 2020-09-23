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
                RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

                PilotInfo = new PilotCardInfo(
                    "Wrobie Tyce",
                    4,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.WrobieTyceAbility),
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Talent, UpgradeType.Talent }
                );

                ModelInfo.SkinName = "New Republic";

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
                targetShip: GetAttacker,
                showMessage: ShowDepleteAttackerMessage
            );

            private GenericShip GetAttacker()
            {
                return Combat.Attacker;
            }

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
                targetShip: GetDefender,
                showMessage: ShowDepleteDefenderMessage
            );

            private GenericShip GetDefender()
            {
                return Combat.Defender;
            }

            private string ShowDepleteDefenderMessage()
            {
                return "Wrobie Tyce: " + Combat.Defender.PilotInfo.PilotName + " gains Deplete token";
            }
        }
    }
}