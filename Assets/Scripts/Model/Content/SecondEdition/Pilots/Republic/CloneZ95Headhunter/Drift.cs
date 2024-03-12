using Content;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class Drift : CloneZ95Headhunter
        {
            public Drift() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "\"Drift\"",
                    "CT-1020",
                    Faction.Republic,
                    3,
                    3,
                    5,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DriftAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Sensor,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Clone
                    }
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/drift.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DriftAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                AiPriority,
                DiceModificationType.Reroll,
                1,
                isGlobal: true
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        protected virtual bool IsAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (!Tools.IsSameTeam(Combat.Attacker, HostShip)) return false;

            if (HostShip.Tokens.GetNonLockRedOrangeTokens().Count != 1) return false;

            BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(HostShip, Combat.Attacker);
            if (positionInfo.Range > 1) return false;

            return true;
        }

        private int AiPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                var friendlyShip = Combat.Attacker;
                int focuses = Combat.DiceRollAttack.FocusesNotRerolled;
                int blanks = Combat.DiceRollAttack.BlanksNotRerolled;

                if (friendlyShip.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (blanks > 0) result = 90;
                }
                else
                {
                    if (blanks + focuses > 0) result = 90;
                }
            }

            return result;
        }
    }
}