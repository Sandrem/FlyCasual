using Abilities.SecondEdition;
using System.Linq;
using Upgrade;
using Tokens;
using System.Collections.Generic;
using Content;

namespace Ship
{
    namespace SecondEdition.TIEReaper
    {
        public class CaptainFeroph : TIEReaper
        {
            public CaptainFeroph() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Captain Feroph",
                    "Imperial Courier",
                    Faction.Imperial,
                    3,
                    4,
                    9,
                    isLimited: true,
                    abilityType: typeof(CaptainFerophAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 114
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainFerophAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Change,
                1,
                sideCanBeChangedTo: DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.CurrentDiceRoll.Blanks > 0 || Combat.CurrentDiceRoll.Focuses > 0) result = 100;

            return result;
        }

        private bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (!Combat.Attacker.Tokens.GetAllTokens().Any(n => n.TokenColor == TokenColors.Green))
                {
                    result = true;
                }
            }

            return result;
        }
    }
}