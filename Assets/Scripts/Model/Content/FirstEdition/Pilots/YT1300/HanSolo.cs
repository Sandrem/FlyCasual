using Ship;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YT1300
    {
        public class HanSolo : YT1300
        {
            public HanSolo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Han Solo",
                    9,
                    46,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.HanSoloAbility)
                );

                ShipInfo.ArcInfo.Firepower = 3;
                ShipInfo.Hull = 8;
                ShipInfo.Shields = 5;

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Missile);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class HanSoloAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += HanSoloPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= HanSoloPilotAbility;
        }

        public void HanSoloPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new HanSoloAction());
        }

        private class HanSoloAction : ActionsList.GenericAction
        {
            public HanSoloAction()
            {
                Name = DiceModificationName = "Han Solo's ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    CallBack = callBack
                };
                diceRerollManager.Start();
                SelectAllRerolableDices();
                diceRerollManager.ConfirmRerollButtonIsPressed();
            }

            private static void SelectAllRerolableDices()
            {
                Combat.CurrentDiceRoll.SelectBySides
                (
                    new List<DieSide>(){
                        DieSide.Blank,
                        DieSide.Focus,
                        DieSide.Success,
                        DieSide.Crit
                    },
                    int.MaxValue
                );
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.NotRerolled > 0)
                {
                    result = true;
                }

                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.NotRerolled > 0)
                {
                    int focusToTurnIntoHit = 0;
                    int focusToTurnIntoHitLeft = 0;
                    if (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                    {
                        focusToTurnIntoHit = int.MaxValue;
                    }
                    else if (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsOneFocusIntoSuccess) > 0)
                    {
                        focusToTurnIntoHit = 1;
                    }
                    focusToTurnIntoHitLeft = focusToTurnIntoHit;

                    int currentHits = 0;
                    foreach (var die in Combat.DiceRollAttack.DiceList.Where(n => !n.IsRerolled))
                    {
                        if (die.IsSuccess)
                        {
                            currentHits++;
                        }
                        else if (die.Side == DieSide.Focus)
                        {
                            if (focusToTurnIntoHitLeft > 0)
                            {
                                currentHits++;
                                focusToTurnIntoHitLeft--;
                            }
                        }
                    }

                    float averagePossibleHits = 0;
                    if (focusToTurnIntoHit == int.MaxValue)
                    {
                        averagePossibleHits = (6 / 8) * Combat.DiceRollAttack.NotRerolled;
                    }
                    else if (focusToTurnIntoHit == 1)
                    {
                        if (Combat.DiceRollAttack.NotRerolled > 0)
                        {
                            averagePossibleHits = (6 / 8) + (4 / 8) * Combat.DiceRollAttack.NotRerolled - 1;
                        }
                    }
                    else
                    {
                        averagePossibleHits = (4 / 8) * Combat.DiceRollAttack.NotRerolled;
                    }

                    if (averagePossibleHits > currentHits) result = 85;
                }

                return result;
            }

        }
    }
}

