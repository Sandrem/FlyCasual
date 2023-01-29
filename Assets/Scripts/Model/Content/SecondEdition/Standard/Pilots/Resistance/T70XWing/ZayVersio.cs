using ActionsList;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class ZayVersio : T70XWing
        {
            public ZayVersio() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Zay Versio",
                    "Her Father's Daughter",
                    Faction.Resistance,
                    3,
                    4,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ZayVersioAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/b6b11bba-184a-4b05-9f79-a3d77effa82b/SWZ97_ZayVersiolegal+%281%29.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ZayVersioAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostName,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                1
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private bool IsAvailable()
        {
            return Combat.Defender == HostShip
                && Combat.AttackStep == CombatStep.Defence
                && Combat.Attacker.Damage.IsDamaged;
        }
    }
}