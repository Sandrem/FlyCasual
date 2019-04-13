using BoardTools;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace ActionsList
{

    public class CalculateAction : GenericAction
    {

        public CalculateAction() {
            Name = DiceModificationName = "Calculate";

            TokensSpend.Add(typeof(Tokens.CalculateToken));
            IsTurnsOneFocusIntoSuccess = true;
            CanBeUsedFewTimes = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ApplyCalculate();
            Selection.ActiveShip.Tokens.SpendToken(typeof(Tokens.CalculateToken), callBack);
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int defenceFocuses = Combat.CurrentDiceRoll.Focuses;
                int numFocusTokens = Selection.ActiveShip.Tokens.CountTokensByType(typeof(FocusToken));
                if (numFocusTokens > 0 && defenceFocuses > 1)
                {
                    // Multiple focus results on our defense roll and we have a Focus token.  Use it instead of the Calculate.
                    result = 0;
                }
                else if (defenceFocuses > 0)
                {
                    // We don't have a focus token.  Better use the Calculate.
                    result = 41;
                }

            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0)
                {
                    result = 41;
                }
            }

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.CurrentDiceRoll.Focuses != 0;
        }

        public override void ActionTake()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(CalculateToken), Phases.CurrentSubPhase.CallBack);
        }

        public override int GetActionPriority()
        {
            int result = 0;
            int maxOrdinanceRange = -1;
            int minOrdinanceRange = 99;
            int minShipTargetRange = 1;
            int curOrdinanceMax = -1;
            int curOrdinanceMin = -1;
            int numValidTargets = 0;

            if (Selection.ThisShip.ShipInfo.ShipName == "E-wing")
            {
                minShipTargetRange = 2;
            }
            if (Selection.ThisShip.UpgradeBar.HasUpgradeInstalled(typeof(UpgradesList.FirstEdition.Expertise))) return 10;

            result = (ActionsHolder.HasTarget(Selection.ThisShip)) ? 50 : 20;
            if (result == 50)
            {
                // Find the combined maximum and minimum range of all of our ordinance that currently has charges.
                foreach (GenericUpgrade currentUpgrade in Selection.ThisShip.UpgradeBar.GetUpgradesOnlyFaceup())
                {
                    if (currentUpgrade.HasType(UpgradeType.Missile) || currentUpgrade.HasType(UpgradeType.Torpedo) && currentUpgrade.State.Charges > 0)
                    {
                        if (currentUpgrade.UpgradeInfo.WeaponInfo.RequiresToken == typeof(CalculateToken))
                        {
                            curOrdinanceMax = currentUpgrade.UpgradeInfo.WeaponInfo.MaxRange;
                            curOrdinanceMin = currentUpgrade.UpgradeInfo.WeaponInfo.MinRange;

                            if (curOrdinanceMin < minOrdinanceRange && curOrdinanceMin >= minShipTargetRange)
                            {
                                minOrdinanceRange = curOrdinanceMin;
                            }
                            if (curOrdinanceMax > maxOrdinanceRange)
                            {
                                maxOrdinanceRange = curOrdinanceMax;
                            }
                        }
                    }
                }
                // If our minimum range is less than 99, we have ordinance that is loaded and have set our min and max ranges.
                // Check all enemy ships to see if they are in range of our ordinance.
                if (minOrdinanceRange < 99)
                {
                    foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(Selection.ThisShip.Owner.PlayerNo)).Ships)
                    {
                        ShotInfo shotInfo = new ShotInfo(Selection.ThisShip, anotherShip.Value, Selection.ThisShip.PrimaryWeapons);
                        if ((shotInfo.Range <= maxOrdinanceRange) && (shotInfo.Range >= minOrdinanceRange) && (shotInfo.IsShotAvailable))
                        {
                            numValidTargets++;
                        }
                    }
                }
                if (numValidTargets > 0)
                {
                    // We have ordinance and we have targets for that ordinance.  Make this equivalent to a target lock in terms of priority.
                    result += 5;
                }
            }
            return result;
        }

    }

}
