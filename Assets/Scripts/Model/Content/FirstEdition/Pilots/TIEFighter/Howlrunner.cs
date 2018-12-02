using Abilities.FirstEdition;
using Ship;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEFighter
    {
        public class Howlrunner : TIEFighter
        {
            public Howlrunner() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Howlrunner\"",
                    8,
                    18,
                    isLimited: true,
                    abilityType: typeof(HowlrunnerAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    //When another friendly ship at Range 1 is attacking with its primary weapon, it may reroll 1 attack die.
    public class HowlrunnerAbility : GenericAbility
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

            if (Combat.Attacker == HostShip) return false;
            if (Combat.Attacker.Owner != HostShip.Owner) return false;

            if (Combat.ChosenWeapon.GetType() != typeof(PrimaryWeaponClass)) return false;

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

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}