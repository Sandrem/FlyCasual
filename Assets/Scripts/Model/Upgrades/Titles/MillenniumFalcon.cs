using Abilities;
using ActionsList;
using RuleSets;
using Ship;
using Ship.YT1300;
using System.Linq;
using Tokens;
using Upgrade;



namespace UpgradesList
{
    public class MillenniumFalcon : GenericUpgrade, ISecondEditionUpgrade
    {
        public MillenniumFalcon() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Millennium Falcon";
            Cost = 1;
            isUnique = true;

            UpgradeAbilities.Add(new GenericActionBarAbility<EvadeAction>());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 6;

            UpgradeAbilities.Add(new Abilities.SecondEdition.MilleniumFalconAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is YT1300;
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class MilleniumFalconAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                AddDiceModification(
                HostName,
                IsDiceModificationAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Reroll,
                1
            );
            }

            public override void DeactivateAbility()
            {
                RemoveDiceModification();
            }

            private bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (HostShip.Tokens.HasToken(typeof(EvadeToken)) && (Combat.AttackStep == CombatStep.Defence))
                {
                    result = true;
                }
                return result;
            }

            private int GetDiceModificationPriority()
            {
                return 90;
            }

            private int HasTokenPriority(GenericShip ship)
            {
                if (ship.Tokens.HasToken(typeof(FocusAction))) return 100;
                if (ship.ActionBar.HasAction(typeof(EvadeAction)) || ship.Tokens.HasToken(typeof(EvadeAction))) return 50;
                if (ship.ActionBar.HasAction(typeof(TargetLockAction)) || ship.Tokens.HasToken(typeof(TargetLockAction), '*')) return 50;
                return 0;
            }
            private int GetAiPriority(GenericShip ship)
            {
                int result = 0;

                result += HasTokenPriority(ship);
                result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

                return result;
            }
        }
    }
}