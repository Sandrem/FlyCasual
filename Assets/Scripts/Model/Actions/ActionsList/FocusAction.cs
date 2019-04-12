﻿using BoardTools;
using Editions;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace ActionsList
{

    public class FocusAction: GenericAction
    {

        public FocusAction() {
            Name = DiceModificationName = "Focus";

            TokensSpend.Add(typeof(Tokens.FocusToken));
            IsTurnsAllFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ApplyFocus();
            Selection.ActiveShip.Tokens.SpendToken(typeof(Tokens.FocusToken), callBack);
        }

        public override bool IsDiceModificationAvailable()
        {
            return Edition.Current is Editions.FirstEdition || Combat.CurrentDiceRoll.Focuses != 0;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.DiceRollDefence.Successes;
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    int defenceFocuses = Combat.DiceRollDefence.Focuses;
                    if (defenceFocuses > 0)
                    {
                        result = (defenceFocuses > 1) ? 50 : 40;
                    }
                }
            }

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0)
                {
                    result = (attackFocuses > 1) ? 50 : 40;
                }
            }

            return result;
        }

        public override void ActionTake()
        {
            Selection.ThisShip.Tokens.AssignToken(typeof(FocusToken), Phases.CurrentSubPhase.CallBack);
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
                        if (currentUpgrade.UpgradeInfo.WeaponInfo.RequiresToken == typeof(FocusToken))
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
