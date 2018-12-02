using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class KoshkaFrost : FiresprayClassPatrolCraft
        {
            public KoshkaFrost() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Koshka Frost",
                    3,
                    71,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KoshkaFrostAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
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
                "Koshka Frost",
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