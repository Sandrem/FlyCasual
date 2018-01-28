using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;
using ActionsList;
using Ship;
using System.ComponentModel;
using Tokens;
using System.Linq;

public static partial class Actions
{

    private static Dictionary<char, bool> Letters;

    public static CriticalHitCard.GenericCriticalHit SelectedCriticalHitCard;

    public static GenericAction CurrentAction;

    public enum BarrelRollTemplates
    {
        Straight1,
        Bank1,
        Straight2
    }

    public enum BoostTemplates
    {
        [Description("Straight 1")]
        Straight1,
        [Description("Bank 1 Right")]
        RightBank1,
        [Description("Bank 1 Left")]
        LeftBank1,
        [Description("Turn 1 Right")]
        RightTurn1,
        [Description("Turn 1 Left")]
        LeftTurn1
    }

    public enum BarrelRollTemplateVariants
    {
        Straight1Left,
        Straight1Right,
        Bank1LeftForward,
        Bank1RightForward,
        Bank1LeftBackwards,
        Bank1RightBackwards,
        Straight2Left,
        Straight2Right
    }

    public static void Initialize()
    {
        Letters = new Dictionary<char, bool>();
    }

    public static void AssignTargetLockToPair(GenericShip thisShip, GenericShip targetShip, Action successCallback, Action failureCallback)
    {
        if (Letters.Count == 0) InitializeTargetLockLetters();

        ShipDistanceInformation distanceInfo = new ShipDistanceInformation(thisShip, targetShip);
        if (distanceInfo.Range >= thisShip.TargetLockMinRange && distanceInfo.Range <= thisShip.TargetLockMaxRange)
        {
            GenericToken existingBlueToken = thisShip.GetToken(typeof(BlueTargetLockToken), '*');
            if (existingBlueToken != null)
            {
                thisShip.RemoveToken(
                    existingBlueToken,
                    delegate { FinishAssignTargetLockPair(thisShip, targetShip, successCallback); }
                );
            }
            else
            {
                FinishAssignTargetLockPair(thisShip, targetShip, successCallback);
            }
        }
        else
        {
            Messages.ShowErrorToHuman("Target is out of range of Target Lock");
            failureCallback();
        }
    }

    private static void FinishAssignTargetLockPair(GenericShip thisShip, GenericShip targetShip, Action callback)
    {
        BlueTargetLockToken tokenBlue = new BlueTargetLockToken();
        RedTargetLockToken tokenRed = new RedTargetLockToken();

        char letter = GetFreeTargetLockLetter();

        tokenBlue.Letter = letter;
        tokenBlue.OtherTokenOwner = targetShip;

        tokenRed.Letter = letter;
        tokenRed.OtherTokenOwner = thisShip;

        TakeTargetLockLetter(letter);

        targetShip.AssignToken(
            tokenRed,
            delegate { thisShip.AssignToken(tokenBlue, callback); }
        );
    }

    private static void InitializeTargetLockLetters()
    {
        Letters.Add('A', true);
        Letters.Add('B', true);
        Letters.Add('C', true);
        Letters.Add('D', true);
        Letters.Add('E', true);
        Letters.Add('F', true);
        Letters.Add('G', true);
        Letters.Add('H', true);

        Letters.Add('I', true);
        Letters.Add('J', true);
        Letters.Add('K', true);
        Letters.Add('L', true);
        Letters.Add('M', true);
        Letters.Add('N', true);
        Letters.Add('O', true);
        Letters.Add('P', true);
    }

    private static char GetFreeTargetLockLetter()
    {
        char result = ' ';
        foreach (var letter in Letters)
        {
            if (letter.Value) return letter.Key;
        }
        return result;
    }

    public static char GetTargetLocksLetterPair(GenericShip thisShip, GenericShip targetShip)
    {
        return thisShip.GetTargetLockLetterPair(targetShip);
    }

    private static void TakeTargetLockLetter(char takenLetter)
    {
        Letters[takenLetter] = false;
    }

    public static void ReleaseTargetLockLetter(char takenLetter)
    {
        Letters[takenLetter] = true;
    }

    public static float GetVector(GenericShip thisShip, GenericShip anotherShip)
    {
        float result = 0;

        float angle = 0;
        Vector3 vectorFacing = thisShip.GetFrontFacing();
        Vector3 vectorToTarget = anotherShip.GetCenter() - thisShip.GetCenter();
        angle = Mathf.Abs(Vector3.Angle(vectorToTarget, vectorFacing));

        int direction = 0;
        direction = (thisShip.Model.transform.InverseTransformPoint(anotherShip.GetCenter()).x > 0) ? 1 : -1;

        result = angle * direction;

        return result;
    }

    public static bool IsClosing(GenericShip thisShip, GenericShip anotherShip)
    {
        bool result = false;

        ShipDistanceInformation distanceInfo = new ShipDistanceInformation(thisShip, anotherShip);
        int range = distanceInfo.Range;
        if (range <= 1) return true;
        if (range >= 3) return false;

        float distanceToFront = Vector3.Distance(thisShip.GetPosition(), anotherShip.ShipBase.GetCentralFrontPoint());
        float distanceToBack = Vector3.Distance(thisShip.GetPosition(), anotherShip.ShipBase.GetCentralBackPoint());
        result = (distanceToFront < distanceToBack) ? true : false;
        return result;
    }

    public static int GetFiringRangeAndShow(GenericShip thisShip, GenericShip anotherShip)
    {
        ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(thisShip, anotherShip, thisShip.PrimaryWeapon);
        bool inArc = MovementTemplates.ShowFiringArcRange(shotInfo);
        if (!inArc) Messages.ShowInfoToHuman("Out of primary weapon arc");
        return shotInfo.Range;
    }

    public static int GetRangeAndShow(GenericShip thisShip, GenericShip anotherShip)
    {
        ShipDistanceInformation distanceInfo = new ShipDistanceInformation(thisShip, anotherShip);
        MovementTemplates.ShowRangeRuler(distanceInfo);

        int range = distanceInfo.Range;
        if (range < 4)
        {
            Messages.ShowInfo("Range " + range);
        }
        else
        {
            Messages.ShowError("Out of range");
        }
        
        return distanceInfo.Range;
    }

    public static bool HasTarget(GenericShip thisShip)
    {
        foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships)
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(thisShip, anotherShip.Value, thisShip.PrimaryWeapon);
            if ((shotInfo.Range < 4) && (shotInfo.InShotAngle))
            {
                return true;
            }
        }

        return false;
    }

    public static int CountEnemiesTargeting(GenericShip thisShip, int direction = 0)
    {
        int result = 0;

        foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships)
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(anotherShip.Value, thisShip, anotherShip.Value.PrimaryWeapon);
            if ((shotInfo.Range < 4) && (shotInfo.InShotAngle))
            {
                if (direction == 0)
                {
                    result++;
                }
                else
                {
                    ShipShotDistanceInformation reverseShotInfo = new ShipShotDistanceInformation(thisShip, anotherShip.Value, thisShip.PrimaryWeapon);
                    if (direction == 1)
                    {
                        if (reverseShotInfo.InArc) result++;
                    }
                    else if (direction == -1)
                    {
                        if (!reverseShotInfo.InArc) result++;
                    }
                }
                
            }
        }

        return result;
    }

    public static bool HasTargetLockOn(GenericShip attacker, GenericShip defender)
    {
        bool result = false;
        char letter = ' ';
        letter = Actions.GetTargetLocksLetterPair(attacker, defender);
        if (letter != ' ') result = true;
        return result;
    }

    // TAKE ACTION TRIGGERS

    public static void TakeAction(GenericAction action)
    {
        var ship = Selection.ThisShip;
        Tooltips.EndTooltip();
        UI.HideSkipButton();
        ship.AddAlreadyExecutedAction(action);
        CurrentAction = action;
        action.ActionTake();
    }

    public static void FinishAction(Action callback)
    {
        ActionIsTaken(callback);
    }

    private static void ActionIsTaken(Action callback)
    {
        Selection.ThisShip.CallActionIsTaken(Actions.CurrentAction, delegate { EndActionDecisionSubhase(callback); });
    }

    private static void EndActionDecisionSubhase(Action callback)
    {
        Selection.ThisShip.CallOnActionDecisionSubphaseEnd(callback);
    }

    public static void ReassignToken(GenericToken tokenToReassign, GenericShip fromShip, GenericShip toShip, Action callBack)
    {
        List<Type> simpleTokens = new List<Type>() { typeof(FocusToken), typeof(EvadeToken) };

        if (simpleTokens.Contains(tokenToReassign.GetType()))
        {
            GenericToken tokenToAssign = (GenericToken)Activator.CreateInstance(tokenToReassign.GetType());
            fromShip.RemoveToken(
                tokenToReassign.GetType(),
                delegate {
                    toShip.AssignToken(tokenToAssign, callBack);
                });
        }
        else
        {
            ReassignTargetLockToken(
                (tokenToReassign as GenericTargetLockToken).Letter,
                fromShip,
                toShip,
                callBack
            );
        }

    }

    public static void ReassignTargetLockToken(char letter, GenericShip oldOwner, GenericShip newOwner, Action callback)
    {
        GenericTargetLockToken assignedTargetLockToken = oldOwner.GetTargetLockToken(letter);

        if (assignedTargetLockToken != null)
        {
            Type oppositeType = null;
            if (assignedTargetLockToken.GetType() == typeof(BlueTargetLockToken))
            {
                oppositeType = typeof(RedTargetLockToken);
                char existingTlOnSameTarget = GetTargetLocksLetterPair(newOwner, assignedTargetLockToken.OtherTokenOwner);
                if (existingTlOnSameTarget != ' ')
                {
                    /*TL*/ newOwner.RemoveToken(typeof(BlueTargetLockToken), existingTlOnSameTarget);
                }
            }
            else
            {
                oppositeType = typeof(BlueTargetLockToken);
                char existingTlOnSameTarget = GetTargetLocksLetterPair(assignedTargetLockToken.OtherTokenOwner, newOwner);
                if (existingTlOnSameTarget != ' ')
                {
                    /*TL*/ newOwner.RemoveToken(typeof(RedTargetLockToken), existingTlOnSameTarget);
                }
            }

            oldOwner.RemoveCondition(assignedTargetLockToken);

            var otherToken = assignedTargetLockToken.OtherTokenOwner.GetToken(oppositeType, letter) as GenericTargetLockToken;

            otherToken.OtherTokenOwner = newOwner;
            newOwner.AssignToken(assignedTargetLockToken, callback, letter);
        }
        else
        {
            Messages.ShowError("ERROR: No such token to reassign!");
            callback();
        }
    }

    public static void RemoveTokens(List<GenericToken> tokensList, Action callback)
    {
        if (tokensList.Count == 0)
        {
            callback();
        }
        else if (tokensList.Count == 1)
        {
            GenericToken tokenToRemove = tokensList.First();
            tokenToRemove.Host.RemoveToken(tokenToRemove, callback);
        }
        else
        {
            GenericToken tokenToRemove = tokensList.First();
            tokensList.Remove(tokenToRemove);
            tokenToRemove.Host.RemoveToken(
                tokenToRemove,
                delegate { RemoveTokens(tokensList, callback); }
            );
        }
    }

}
