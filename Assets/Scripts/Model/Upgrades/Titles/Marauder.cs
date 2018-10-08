using Ship;
using Ship.Firespray31;
using Upgrade;
using System.Collections.Generic;
using RuleSets;
using Abilities;
using System.Linq;
using Abilities.SecondEdition;

namespace UpgradesList
{
    class Marauder : GenericUpgradeSlotUpgrade, ISecondEditionUpgrade
    {
        public Marauder() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Marauder";
            Cost = 3;
            UpgradeRuleType = typeof(SecondEdition);
            isUnique = true;
            AddedSlots = new List<UpgradeSlot>
            {
                new UpgradeSlot(UpgradeType.Gunner),
            };

            UpgradeAbilities.Add(new MarauderAbility());

            SEImageNumber = 150;
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //Not Needed, only in SE
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Firespray31;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MarauderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += MarauderDiceAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= MarauderDiceAbility;
        }

        public void MarauderDiceAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new MarauderAbilityAction());
        }

        private class MarauderAbilityAction : ActionsList.GenericAction
        {
            public MarauderAbilityAction()
            {
                Name = DiceModificationName = "Marauder ship ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = 1,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Attack && Combat.ShotInfo.InArcByType(Arcs.ArcTypes.RearAux) && Combat.ChosenWeapon is PrimaryWeaponClass)
                {
                    result = true;
                }
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack && (Combat.ChosenWeapon as Upgrade.GenericSecondaryWeapon) != null)
                {
                    if (Combat.DiceRollAttack.Blanks > 0)
                    {
                        result = 90;
                    }
                    else if (Combat.DiceRollAttack.Focuses > 0 && Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0)
                    {
                        result = 90;
                    }
                    else if (Combat.DiceRollAttack.Focuses > 0)
                    {
                        result = 30;
                    }
                }

                return result;
            }
        }
    }
}
