using Arcs;
using BoardTools;
using Content;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIERbHeavy
    {
        public class LyttanDree : TIERbHeavy
        {
            public LyttanDree() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Lyttan Dree",
                    "Onyx 2",
                    Faction.Imperial,
                    3,
                    5,
                    14,
                    abilityType: typeof(Abilities.SecondEdition.LyttanDreePilotAbility),
                    isLimited: true,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/6f/37/6f375dcd-61b2-407d-bb3f-0c01cf9491ae/swz67_lyttan-dree.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LyttanDreePilotAbility : GenericAbility
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

        protected virtual bool IsAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack) return false;
            if (Combat.Attacker.Owner != HostShip.Owner) return false;

            DistanceInfo positionInfo = new DistanceInfo(HostShip, Combat.Attacker);
            if (positionInfo.Range > 2) return false;

            if (Combat.Defender.SectorsInfo.IsShipInSector(HostShip, ArcType.Left) || Combat.Defender.SectorsInfo.IsShipInSector(HostShip, ArcType.Right))
            {
                return true;
            }
            else
            {
                return false;
            }
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

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}