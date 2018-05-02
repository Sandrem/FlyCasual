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

    public static DamageDeckCard.GenericDamageCard SelectedCriticalHitCard;

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

    public static void AcquireTargetLock(GenericShip thisShip, GenericShip targetShip, Action successCallback, Action failureCallback)
    {
        if (Letters.Count == 0) InitializeTargetLockLetters();

        ShipDistanceInformation distanceInfo = new ShipDistanceInformation(thisShip, targetShip);
        if (distanceInfo.Range >= thisShip.TargetLockMinRange && distanceInfo.Range <= thisShip.TargetLockMaxRange)
        {
            GenericToken existingBlueToken = thisShip.Tokens.GetToken(typeof(BlueTargetLockToken), '*');
            if (existingBlueToken != null)
            {
                thisShip.Tokens.RemoveToken(
                    existingBlueToken,
                    delegate { FinishAcquireTargetLock(thisShip, targetShip, successCallback); }
                );
            }
            else
            {
                FinishAcquireTargetLock(thisShip, targetShip, successCallback);
            }
        }
        else
        {
            Messages.ShowErrorToHuman("Target is out of range of Target Lock");
            failureCallback();
        }
    }
    
    private static void FinishAcquireTargetLock(GenericShip thisShip, GenericShip targetShip, Action callback)
    {
        BlueTargetLockToken tokenBlue = new BlueTargetLockToken(thisShip);
        RedTargetLockToken tokenRed = new RedTargetLockToken(targetShip);

        char letter = GetFreeTargetLockLetter();

        tokenBlue.Letter = letter;
        tokenBlue.OtherTokenOwner = targetShip;

        tokenRed.Letter = letter;
        tokenRed.OtherTokenOwner = thisShip;

        TakeTargetLockLetter(letter);

        targetShip.Tokens.AssignToken(
            tokenRed,
            delegate
            {
                thisShip.Tokens.AssignToken(tokenBlue, delegate
                {
                    thisShip.CallOnTargetLockIsAcquiredEvent(targetShip, callback);
                });
            }
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
        return thisShip.Tokens.GetTargetLockLetterPair(targetShip);
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
            GenericToken tokenToAssign = (GenericToken)Activator.CreateInstance(tokenToReassign.GetType(), new [] { toShip });
            fromShip.Tokens.RemoveToken(
                tokenToReassign.GetType(),
                delegate {
                    toShip.Tokens.AssignToken(tokenToAssign, callBack);
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
        GenericTargetLockToken assignedTargetLockToken = oldOwner.Tokens.GetTargetLockToken(letter);

        if (assignedTargetLockToken != null)
        {
            if (assignedTargetLockToken.GetType() == typeof(BlueTargetLockToken))
            {
                char existingTlOnSameTarget = GetTargetLocksLetterPair(newOwner, assignedTargetLockToken.OtherTokenOwner);
                if (existingTlOnSameTarget != ' ')
                {
                    newOwner.Tokens.RemoveToken(
                        typeof(BlueTargetLockToken),
                        delegate { FinishReassignToken(letter, oldOwner, newOwner, callback); },
                        existingTlOnSameTarget
                    );
                }
                else
                {
                    FinishReassignToken(letter, oldOwner, newOwner, callback);
                }
            }
            else
            {
                char existingTlOnSameTarget = GetTargetLocksLetterPair(assignedTargetLockToken.OtherTokenOwner, newOwner);
                if (existingTlOnSameTarget != ' ')
                {
                    newOwner.Tokens.RemoveToken(
                        typeof(RedTargetLockToken),
                        delegate { FinishReassignToken(letter, oldOwner, newOwner, callback); },
                        existingTlOnSameTarget
                    );
                }
                else
                {
                    FinishReassignToken(letter, oldOwner, newOwner, callback);
                }
            }
        }
        else
        {
            Messages.ShowError("ERROR: No such token to reassign!");
            callback();
        }
    }

    private static void FinishReassignToken(char letter, GenericShip oldOwner, GenericShip newOwner, Action callback)
    {
        GenericTargetLockToken assignedTargetLockToken = oldOwner.Tokens.GetTargetLockToken(letter);
        Type oppositeType = (assignedTargetLockToken is BlueTargetLockToken) ? typeof(RedTargetLockToken) : typeof(BlueTargetLockToken);

        oldOwner.Tokens.RemoveCondition(assignedTargetLockToken);

        var otherToken = assignedTargetLockToken.OtherTokenOwner.Tokens.GetToken(oppositeType, letter) as GenericTargetLockToken;

        otherToken.OtherTokenOwner = newOwner;
        newOwner.Tokens.AssignToken(assignedTargetLockToken, callback, letter);
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
            tokenToRemove.Host.Tokens.RemoveToken(tokenToRemove, callback);
        }
        else
        {
            GenericToken tokenToRemove = tokensList.First();
            tokensList.Remove(tokenToRemove);
            tokenToRemove.Host.Tokens.RemoveToken(
                tokenToRemove,
                delegate { RemoveTokens(tokensList, callback); }
            );
        }
    }

}
