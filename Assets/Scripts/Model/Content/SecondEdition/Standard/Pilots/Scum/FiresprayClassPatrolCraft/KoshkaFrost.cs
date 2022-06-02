using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class KoshkaFrost : FiresprayClassPatrolCraft
        {
            public KoshkaFrost() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Koshka Frost",
                    "Icy Professional",
                    Faction.Scum,
                    3,
                    7,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KoshkaFrostAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    seImageNumber: 152
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KoshkaFrostAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotInfo.PilotName,
                IsAvailable,
                GetPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return ((Combat.AttackStep == CombatStep.Attack && Combat.Defender.IsStressed) ||
               (Combat.AttackStep == CombatStep.Defence && Combat.Attacker.IsStressed));
        }

        private int GetPriority()
        {
            return 90;
        }

    }
}