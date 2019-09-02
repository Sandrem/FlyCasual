using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardTools;
using ActionsList;
using Ship;
using System.ComponentModel;
using Tokens;
using System.Linq;
using SubPhases;
using Upgrade;
using Arcs;

namespace Actions
{
    public enum ActionColor
    {
        White,
        Red,
        Purple
    }

    public enum ActionFailReason
    {
        WrongRange,
        OutOfArc,
        Bumped,
        OffTheBoard,
        ObstacleHit,
        NoTemplateAvailable
    }

    public class ActionInfo
    {
        public Type ActionType { get; private set; }
        public ActionColor Color { get; private set; }
        public GenericUpgrade Source { get; private set; }

        public ActionInfo(Type actionType, ActionColor color = ActionColor.White, GenericUpgrade source = null)
        {
            ActionType = actionType;
            Color = color;
            Source = source;
        }
    }

    public class LinkedActionInfo
    {
        public Type ActionType { get; private set; }
        public Type ActionLinkedType { get; private set; }
        public ActionColor LinkedColor { get; private set; }

        public LinkedActionInfo(Type actionType, Type actionLinkedType, ActionColor linkedColor = ActionColor.Red)
        {
            ActionType = actionType;
            ActionLinkedType = actionLinkedType;
            LinkedColor = linkedColor;
        }
    }
}

public static partial class ActionsHolder
{
    private static Dictionary<char, bool> Letters;

    public static GenericDamageCard SelectedCriticalHitCard;

    public static GenericAction CurrentAction;

    public enum BarrelRollTemplates
    {
        Straight1,
        Bank1,
        Straight2
    }

    public enum DecloakTemplates
    {
        Straight2,
        Bank2
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

    public enum DecloakTemplateVariants
    {
        Straight2Forward,
        Straight2Left,
        Straight2Right,
        Bank2LeftForward,
        Bank2RightForward,
        Bank2LeftBackwards,
        Bank2RightBackwards,
        Bank2ForwardLeft,
        Bank2ForwardRight
    }

    public static List<GenericToken> TokensToRemove;

    private class EventArgsTokensList : EventArgs
    {
        public List<GenericToken> List;
    }

    public static void Initialize()
    {
        Letters = new Dictionary<char, bool>();
    }

    public static void AcquireTargetLock(GenericShip thisShip, ITargetLockable targetShip, Action successCallback, Action failureCallback, bool ignoreRange = false)
    {
        if (Letters.Count == 0) InitializeTargetLockLetters();

        if (ignoreRange || Rules.TargetLocks.TargetLockIsAllowed(thisShip, targetShip))
        {
            List<BlueTargetLockToken> existingBlueTokens = thisShip.Tokens.GetTokens<BlueTargetLockToken>('*');

            GetTokensToRemoveThenAssign(existingBlueTokens, thisShip, targetShip, successCallback);
        }
        else
        {
            Messages.ShowErrorToHuman("The target is out of range of Target Lock");
            failureCallback();
        }
    }

    private static void GetTokensToRemoveThenAssign(List<BlueTargetLockToken> existingBlueTokens, GenericShip thisShip, ITargetLockable targetShip, Action successCallback)
    {
        TokensToRemove = new List<GenericToken>();
        List<GenericToken> tokensAskToRemove = new List<GenericToken>();

        foreach (BlueTargetLockToken existingBlueToken in existingBlueTokens)
        {
            bool tokenMustBeRemoved = false;
            bool tokenMaybeWillBeRemoved = false;

            //Two TLs on the same target AND on different targets are allowed
            if (thisShip.TwoTargetLocksOnSameTargetsAreAllowed.Count > 0 && thisShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Count > 0)
            {
                //If target is different
                if (existingBlueToken.OtherTargetLockTokenOwner != targetShip)
                {
                    //If already >1 of tokens, then ask to remove
                    int alreadyAssignedSameTokens = thisShip.Tokens.GetTokens<BlueTargetLockToken>('*').Count;
                    if (alreadyAssignedSameTokens > 1 && TokensToRemove.Count < alreadyAssignedSameTokens - 1)
                    {
                        tokenMaybeWillBeRemoved = true;
                    }
                }
                else //If target is the same
                {
                    //If already >1 of tokens, then remove all except one
                    int alreadyAssignedSameTokens = thisShip.Tokens.GetTokens<BlueTargetLockToken>('*').Count(t => t.OtherTargetLockTokenOwner == targetShip);
                    if (alreadyAssignedSameTokens > 1 && TokensToRemove.Count < alreadyAssignedSameTokens - 1)
                    {
                        tokenMustBeRemoved = true;
                    }
                }
            }
            //Two TLs on the different targets are allowed
            else if (thisShip.TwoTargetLocksOnSameTargetsAreAllowed.Count == 0 && thisShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Count > 0)
            {
                //Remove if token is on the same target
                if (existingBlueToken.OtherTargetLockTokenOwner == targetShip)
                {
                    tokenMustBeRemoved = true;
                }
                else //If target is different
                {
                    //If already >1 of tokens, then ask to remove
                    int alreadyAssignedSameTokens = thisShip.Tokens.GetTokens<BlueTargetLockToken>('*').Count;
                    if (alreadyAssignedSameTokens > 1 && TokensToRemove.Count < alreadyAssignedSameTokens - 1)
                    {
                        tokenMaybeWillBeRemoved = true;
                    }
                }
            } 
            //Two TLs on the same target are allowed
            else if (thisShip.TwoTargetLocksOnSameTargetsAreAllowed.Count > 0 && thisShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Count == 0)
            {
                //Remove all if new target is another
                if (existingBlueToken.OtherTargetLockTokenOwner != targetShip)
                {
                    tokenMustBeRemoved = true;
                }
                else //If target is the same
                {
                    //If already >1 of tokens, then remove all except one
                    int alreadyAssignedSameTokens = thisShip.Tokens.GetTokens<BlueTargetLockToken>('*').Count(t => t.OtherTargetLockTokenOwner == targetShip);
                    if (alreadyAssignedSameTokens > 1 && TokensToRemove.Count < alreadyAssignedSameTokens -1)
                    {
                        tokenMustBeRemoved = true;
                    }
                }
            }
            //Always remove if only 1 token is allowed
            else if (thisShip.TwoTargetLocksOnSameTargetsAreAllowed.Count == 0 && thisShip.TwoTargetLocksOnDifferentTargetsAreAllowed.Count == 0)
            {
                tokenMustBeRemoved = true;
            }

            if (tokenMustBeRemoved)
            {
                TokensToRemove.Add(existingBlueToken);
            }
            else if (tokenMaybeWillBeRemoved)
            {
                tokensAskToRemove.Add(existingBlueToken);
            }
        }

        if (tokensAskToRemove.Count > 0)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Select token to remove",
                TriggerOwner = thisShip.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnAbilityDirect,
                EventHandler = StartSelectTokenToRemoveSubPhase,
                Sender = thisShip,
                EventArgs = new EventArgsTokensList()
                {
                    List = tokensAskToRemove
                }
            });
        }

        Triggers.ResolveTriggers(
            TriggerTypes.OnAbilityDirect,
            delegate{ AssignNewTargetLockToken(TokensToRemove, thisShip, targetShip, successCallback); }
        );
    }

    private static void StartSelectTokenToRemoveSubPhase(object sender, System.EventArgs e)
    {
        EventArgsTokensList tokensToAskList = e as EventArgsTokensList;

        var subphase = Phases.StartTemporarySubPhaseNew<TokenToRemoveSubPhase>(
            "Select token to remove",
            Triggers.FinishTrigger
        );

        subphase.DescriptionShort = "Select token to remove";
        subphase.RequiredPlayer = (sender as GenericShip).Owner.PlayerNo;
        subphase.ShowSkipButton = false;

        foreach (var token in tokensToAskList.List)
        {
            subphase.AddDecision(
                (token as BlueTargetLockToken).Letter.ToString(),
                delegate
                {
                    TokensToRemove.Add(token);
                    DecisionSubPhase.ConfirmDecision();
                }
            );
        }

        subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;

        subphase.Start();
    }

    private class TokenToRemoveSubPhase : DecisionSubPhase { };

    private static void AssignNewTargetLockToken(List<GenericToken> tokensToRemove, GenericShip thisShip, ITargetLockable targetShip, Action successCallback)
    {
        if (tokensToRemove.Count != 0)
        {
            thisShip.Tokens.RemoveTokens(
                tokensToRemove,
                delegate { FinishAcquireTargetLock(thisShip, targetShip, successCallback); }
            );
        }
        else
        {
            FinishAcquireTargetLock(thisShip, targetShip, successCallback);
        }
    }

   
    private static void FinishAcquireTargetLock(GenericShip thisShip, ITargetLockable targetShip, Action callback)
    {
        BlueTargetLockToken tokenBlue = new BlueTargetLockToken(thisShip);
        RedTargetLockToken tokenRed = new RedTargetLockToken(targetShip);

        char letter = GetFreeTargetLockLetter();

        tokenBlue.Letter = letter;
        tokenBlue.OtherTargetLockTokenOwner = targetShip;

        tokenRed.Letter = letter;
        tokenRed.OtherTargetLockTokenOwner = thisShip;

        TakeTargetLockLetter(letter);

        targetShip.AssignToken(
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

    public static List<char> GetTargetLocksLetterPairs(ITargetLockable thisShip, ITargetLockable targetShip)
    {
        return thisShip.GetTargetLockLetterPairsOn(targetShip);
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

        DistanceInfo distanceInfo = new DistanceInfo(thisShip, anotherShip);
        int range = distanceInfo.Range;
        if (range <= 1) return true;
        if (range >= 3) return false;

        float distanceToFront = Vector3.Distance(thisShip.GetPosition(), anotherShip.ShipBase.GetCentralFrontPoint());
        float distanceToBack = Vector3.Distance(thisShip.GetPosition(), anotherShip.ShipBase.GetCentralBackPoint());
        result = (distanceToFront < distanceToBack) ? true : false;
        return result;
    }

    public static ShotInfo GetFiringRangeAndShow(GenericShip thisShip, GenericShip anotherShip)
    {
        // TODOREVERT
        //IShipWeapon outOfArcWeapon = (IShipWeapon) thisShip.UpgradeBar.GetUpgradesOnlyFaceup().FirstOrDefault(n => n is IShipWeapon && (n as IShipWeapon).CanShootOutsideArc == true);

        IShipWeapon checkedWeapon = thisShip.PrimaryWeapons.First();
        //IShipWeapon checkedWeapon = outOfArcWeapon ?? thisShip.PrimaryWeapon;

        ShotInfo shotInfo = new ShotInfo(thisShip, anotherShip, checkedWeapon);
        MovementTemplates.ShowFiringArcRange(shotInfo);
        return shotInfo;
    }

    public static int GetRangeAndShow(GenericShip thisShip, GenericShip anotherShip)
    {
        DistanceInfo distanceInfo = new DistanceInfo(thisShip, anotherShip);
        MovementTemplates.ShowRangeRuler(distanceInfo.MinDistance);

        string messageText = "Range to target: " + RangeInfoToText(distanceInfo) +
            "\nRange in Front sector: " + ArcRangeInfoToText(thisShip.SectorsInfo.GetSectorInfo(anotherShip, ArcType.Front)) +
            "\nRange in Bullseye sector: " + ArcRangeInfoToText(thisShip.SectorsInfo.GetSectorInfo(anotherShip, ArcType.Bullseye));

        List<ArcType> mentionedFacings = new List<ArcType>() { ArcType.None, ArcType.Front };

        foreach (GenericArc arc in thisShip.ArcsInfo.Arcs)
        {
            ArcType arcType = ArcToSector(arc);

            if (!mentionedFacings.Contains(arcType))
            {
                messageText += "\nRange in " + arc.Facing.ToString() + " sector: " + ArcRangeInfoToText(thisShip.SectorsInfo.GetSectorInfo(anotherShip, arcType));
                mentionedFacings.Add(arcType);
            }
        }

        Messages.ShowInfo(messageText);
        
        return distanceInfo.Range;
    }

    private static ArcType ArcToSector(GenericArc arc)
    {
        switch (arc.Facing)
        {
            case ArcFacing.Front:
                return ArcType.Front;
            case ArcFacing.Left:
                return ArcType.Left;
            case ArcFacing.Right:
                return ArcType.Right;
            case ArcFacing.Rear:
                return ArcType.Rear;
            case ArcFacing.FullFront:
                return ArcType.FullFront;
            case ArcFacing.FullRear:
                return ArcType.FullRear;
            case ArcFacing.Bullseye:
                return ArcType.Bullseye;
            default:
                return ArcType.None;
        }
    }

    private static string RangeInfoToText(DistanceInfo info)
    {
        return (info.Range < 4) ? info.Range.ToString() : "Beyond range 3";
    }

    private static string ArcRangeInfoToText(ShotInfoArc info)
    {
        if (info.InArc)
        {
            if (info.Range < 4)
            {
                return info.Range.ToString();
            }
            else
            {
                return "Beyond range 3";
            }
        }
        else
        {
            return "Not in arc";
        }
    }

    public static bool HasTarget(GenericShip thisShip)
    {
        foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships)
        {
            foreach (IShipWeapon weapon in thisShip.GetAllWeapons())
            {
                ShotInfo shotInfo = new ShotInfo(thisShip, anotherShip.Value, weapon);
                if (shotInfo.IsShotAvailable)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public static int CountEnemiesTargeting(GenericShip thisShip, int direction = 0)
    {
        int result = 0;

        foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships.Values)
        {
            // Test to see if we're in the primary arc of any enemy ships.
            ShotInfo shotInfo = new ShotInfo(anotherShip, thisShip, anotherShip.PrimaryWeapons);
            if ((shotInfo.Range < 4) && (shotInfo.IsShotAvailable))
            {
                if (direction == 0)
                {
                    result++;
                }
                else
                {
                    if (direction == 1)
                    {
                        if (thisShip.SectorsInfo.IsShipInSector(anotherShip, Arcs.ArcType.FullFront)) result++;
                    }
                    else if (direction == -1)
                    {
                        if (thisShip.SectorsInfo.IsShipInSector(anotherShip, Arcs.ArcType.FullRear)) result++;
                    }
                }
                
            }
            foreach (GenericUpgrade SecondaryWeapon in anotherShip.UpgradeBar.GetSpecialWeaponsAll())
            {
                // Test to see if we're in the secondary arc of any enemy ships.
                IShipWeapon currentWeapon = SecondaryWeapon as IShipWeapon;
                shotInfo = new ShotInfo(anotherShip, thisShip, currentWeapon);
                if ((shotInfo.Range < 4) && (shotInfo.IsShotAvailable))
                {
                    if (direction == 0)
                    {
                        result++;
                    }
                    else
                    {
                        if (direction == 1)
                        {
                            if (thisShip.SectorsInfo.IsShipInSector(anotherShip, Arcs.ArcType.FullFront)) result++;
                        }
                        else if (direction == -1)
                        {
                            if (thisShip.SectorsInfo.IsShipInSector(anotherShip, Arcs.ArcType.FullRear)) result++;
                        }
                    }
                }

            }
        }

        return result;
    }

    public static bool HasTargetLockOn(GenericShip attacker, ITargetLockable defender)
    {
        List<char> letter = GetTargetLocksLetterPairs(attacker, defender);
        return letter.Count > 0;
    }

    // TAKE ACTION TRIGGERS

    public static void TakeActionStart(GenericAction action)
    {
        var ship = Selection.ThisShip;
        Tooltips.EndTooltip();
        UI.HideSkipButton();
        ship.AddAlreadyExecutedAction(action);
        CurrentAction = action;
        action.ActionTake();
    }

    public static void TakeActionFinish(Action callback)
    {
        bool isActionSkipped = (ActionsHolder.CurrentAction == null);

        if (!isActionSkipped)
        {
            ActionIsTaken(callback);
        }
        else
        {
            ActionIsSkipped();
            callback();
        }
    }

    private static void ActionIsTaken(Action callback)
    {
        Selection.ThisShip.CallActionIsTaken(ActionsHolder.CurrentAction, callback);
    }

    private static void ActionIsSkipped()
    {
        Selection.ThisShip.CallActionIsSkipped();
    }

    public static void EndActionDecisionSubhase(Action callback)
    {
        Selection.ThisShip.CallOnActionDecisionSubphaseEnd(callback);
    }

    public static void ReassignToken(GenericToken tokenToReassign, GenericShip fromShip, GenericShip toShip, Action callBack)
    {
        List<Type> lockTokens = new List<Type>() { typeof(BlueTargetLockToken), typeof(RedTargetLockToken) };

        if (lockTokens.Contains(tokenToReassign.GetType()))
        {
            ReassignTargetLockToken(
                (tokenToReassign as GenericTargetLockToken).Letter,
                fromShip,
                toShip,
                callBack
            );
        }
        else
        {
            GenericToken tokenToAssign = (GenericToken)Activator.CreateInstance(tokenToReassign.GetType(), new [] { toShip });
            fromShip.Tokens.RemoveToken(
                tokenToReassign.GetType(),
                delegate {
                    toShip.Tokens.AssignToken(tokenToAssign, callBack);
                });
        }
    }

    public static void ReassignTargetLockToken(char letter, GenericShip oldOwner, GenericShip newOwner, Action callback)
    {
        GenericTargetLockToken assignedTargetLockToken = oldOwner.Tokens.GetTargetLockToken(letter);

        if (assignedTargetLockToken != null)
        {
            if (assignedTargetLockToken.GetType() == typeof(BlueTargetLockToken))
            {
                List<char> existingTlOnSameTarget = GetTargetLocksLetterPairs(newOwner, assignedTargetLockToken.OtherTargetLockTokenOwner);
                if (existingTlOnSameTarget.Count > 0)
                {
                    newOwner.Tokens.RemoveToken(
                        typeof(BlueTargetLockToken),
                        delegate { FinishReassignToken(letter, oldOwner, newOwner, callback); },
                        existingTlOnSameTarget.First()
                    );
                }
                else
                {
                    FinishReassignToken(letter, oldOwner, newOwner, callback);
                }
            }
            else
            {
                List<char> existingTlOnSameTarget = GetTargetLocksLetterPairs(assignedTargetLockToken.OtherTargetLockTokenOwner, newOwner);
                if (existingTlOnSameTarget.Count > 0)
                {
                    newOwner.Tokens.RemoveToken(
                        typeof(RedTargetLockToken),
                        delegate { FinishReassignToken(letter, oldOwner, newOwner, callback); },
                        existingTlOnSameTarget.First()
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

        GenericTargetLockToken otherToken = assignedTargetLockToken.OtherTargetLockTokenOwner.GetAnotherToken(oppositeType, letter);

        otherToken.OtherTargetLockTokenOwner = newOwner;
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
            tokenToRemove.RemoveFromHost(callback);
        }
        else
        {
            GenericToken tokenToRemove = tokensList.First();
            tokensList.Remove(tokenToRemove);
            tokenToRemove.RemoveFromHost(
                delegate { RemoveTokens(tokensList, callback); }
            );
        }
    }

}
