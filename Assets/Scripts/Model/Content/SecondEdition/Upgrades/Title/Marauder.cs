using Arcs;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Marauder : GenericUpgrade
    {
        public Marauder() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Marauder",
                UpgradeType.Title,
                cost: 3,
                isLimited: true,
                addSlot: new UpgradeSlot(UpgradeType.Gunner),
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.FiresprayClassPatrolCraft.FiresprayClassPatrolCraft)),
                abilityType: typeof(Abilities.SecondEdition.MarauderAbility),
                seImageNumber: 150
            );
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
                if (Combat.AttackStep == CombatStep.Attack && Combat.ShotInfo.InArcByType(ArcType.RearAux) && Combat.ChosenWeapon is PrimaryWeaponClass)
                {
                    result = true;
                }
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack && (Combat.ChosenWeapon as Upgrade.GenericSpecialWeapon) != null)
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