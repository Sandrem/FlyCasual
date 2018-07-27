using ActionsList;
using RuleSets;
using Ship;
using System.Linq;

namespace Ship
{
    namespace TIEFighter
    {
        public class Howlrunner : TIEFighter, ISecondEditionPilot
        {
            public Howlrunner() : base()
            {
                PilotName = "\"Howlrunner\"";
                PilotSkill = 8;
                Cost = 18;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.HowlrunnerAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 40;
            }
        }
    }
}

namespace Abilities
{
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

        private bool IsAvailable()
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

                if (friendlyShip.GetAvailableDiceModifications().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
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