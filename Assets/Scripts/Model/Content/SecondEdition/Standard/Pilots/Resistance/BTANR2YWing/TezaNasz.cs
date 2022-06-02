using Arcs;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.BTANR2YWing
    {
        public class TezaNasz : BTANR2YWing
        {
            public TezaNasz() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Teza Nasz",
                    "Old Soldier",
                    Faction.Resistance,
                    4,
                    4,
                    17,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TezaNaszAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.YWing
                    }
                );

                ImageUrl = "https://i.imgur.com/qyerxjI.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TezaNaszAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification
            (
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                1,
                isGlobal: true
            );
        }

        private bool IsAvailable()
        {
            if (Tools.IsAnotherTeam(Combat.Attacker, HostShip)) return false;
            if (Combat.AttackStep != CombatStep.Attack) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, Combat.Attacker);
            if (distInfo.Range > 2) return false;

            Dictionary<ArcFacing, List<GenericShip>> enemiesInSectors = Combat.Defender.SectorsInfo.GetEnemiesInAllSectors();
            bool hasEnemiesLeft = enemiesInSectors[ArcFacing.Left].Count > 0;
            bool hasEnemiesRight = enemiesInSectors[ArcFacing.Right].Count > 0;
            return hasEnemiesLeft && hasEnemiesRight;
        }

        private int GetAiPriority()
        {
            return 90; // Free rerolls
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
