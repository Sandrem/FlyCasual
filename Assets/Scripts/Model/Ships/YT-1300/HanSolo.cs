﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Ship;
using System;
using RuleSets;
using BoardTools;

namespace Ship
{
    namespace YT1300
    {
        public class HanSolo : YT1300, ISecondEditionPilot
        {
            public HanSolo() : base()
            {
                PilotName = "Han Solo";
                PilotSkill = 9;
                Cost = 46;

                IsUnique = true;

                Firepower = 3;
                MaxHull = 8;
                MaxShields = 5;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Missile);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.HanSoloAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 6;
                Cost = 92;

                PilotAbilities.RemoveAll(a => a is Abilities.HanSoloAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.HanSoloRebelPilotAbilitySE());

                SEImageNumber = 69;
            }
        }
    }
}

namespace Abilities
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

namespace Abilities.SecondEdition
{
    public class HanSoloRebelPilotAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostShip.PilotName,
                IsDiceModificationAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                int.MaxValue,
                timing: DiceModificationTimingType.AfterRolled,
                isTrueReroll: false
            );
        }

        private bool IsDiceModificationAvailable()
        {
            foreach (var obstacle in Obstacles.ObstaclesManager.GetPlacedObstacles())
            {
                ShipObstacleDistance obstacleDistance = new ShipObstacleDistance(HostShip, obstacle);
                if (obstacleDistance.Range < 2) return true;
            }
            
            return false;
        }

        private int GetAiPriority()
        {
            return 95;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            HostShip.Tokens.AssignToken(typeof(Tokens.StressToken), () => callback(true));
        }
    }
}