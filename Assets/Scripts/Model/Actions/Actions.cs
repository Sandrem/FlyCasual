using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;

public static partial class Actions {

    private static GameManagerScript Game;

    private static Dictionary<char, bool> Letters = new Dictionary<char, bool>();

    public static CriticalHitCard.GenericCriticalHit SelectedCriticalHitCard;

    static Actions()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public static void AssignTargetLockToPair(Ship.GenericShip thisShip, Ship.GenericShip targetShip, Action successCallback, Action failureCallback)
    {
        if (Letters.Count == 0) InitializeTargetLockLetters();

        ShipDistanceInformation distanceInfo = new ShipDistanceInformation(thisShip, targetShip);
        if (distanceInfo.Range < 4)
        {
            Tokens.GenericToken existingBlueToken = thisShip.GetToken(typeof(Tokens.BlueTargetLockToken), '*');
            if (existingBlueToken != null)
            {
                if ((existingBlueToken as Tokens.BlueTargetLockToken).LockedShip != null)
                {
                    (existingBlueToken as Tokens.BlueTargetLockToken).LockedShip.RemoveToken(typeof(Tokens.RedTargetLockToken), (existingBlueToken as Tokens.BlueTargetLockToken).Letter);
                }
                thisShip.RemoveToken(typeof(Tokens.BlueTargetLockToken), (existingBlueToken as Tokens.BlueTargetLockToken).Letter);
            }

            Tokens.BlueTargetLockToken tokenBlue = new Tokens.BlueTargetLockToken();
            Tokens.RedTargetLockToken tokenRed = new Tokens.RedTargetLockToken();

            char letter = GetFreeTargetLockLetter();
            tokenBlue.Letter = letter;
            tokenBlue.LockedShip = targetShip;
            tokenRed.Letter = letter;
            TakeTargetLockLetter(letter);

            targetShip.AssignToken(tokenRed, delegate { } );
            Selection.ThisShip.AssignToken(tokenBlue, successCallback);
        }
        else
        {
            Messages.ShowErrorToHuman("Target is out of range of Target Lock");
            failureCallback();
        }
    }

    private static void InitializeTargetLockLetters()
    {
        Letters.Add('A', true);
        Letters.Add('B', true);
        Letters.Add('C', true);
        Letters.Add('D', true);
        Letters.Add('E', true);
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
        Letters.Add('Q', true);
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

    public static char GetTargetLocksLetterPair(Ship.GenericShip thisShip, Ship.GenericShip targetShip)
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

    public static float GetVector(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
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

    public static bool IsClosing(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        bool result = false;

        ShipDistanceInformation distanceInfo = new ShipDistanceInformation(thisShip, anotherShip);
        int range = distanceInfo.Range;
        if (range <= 1) return true;
        if (range >= 3) return false;

        float distanceToFront = Vector3.Distance(thisShip.GetPosition(), anotherShip.GetCentralFrontPoint());
        float distanceToBack = Vector3.Distance(thisShip.GetPosition(), anotherShip.GetCentralBackPoint());
        result = (distanceToFront < distanceToBack) ? true : false;
        return result;
    }

    public static int GetFiringRangeAndShow(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(thisShip, anotherShip);
        MovementTemplates.ShowFiringArcRange(shotInfo);
        return shotInfo.Range;
    }

    public static bool HasTarget(Ship.GenericShip thisShip)
    {
        foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships)
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(thisShip, anotherShip.Value);
            if ((shotInfo.Range < 4) && (shotInfo.InArc))
            {
                return true;
            }
        }

        return false;
    }

    public static int CountEnemiesTargeting(Ship.GenericShip thisShip)
    {
        int result = 0;

        foreach (var anotherShip in Roster.GetPlayer(Roster.AnotherPlayer(thisShip.Owner.PlayerNo)).Ships)
        {
            ShipShotDistanceInformation shotInfo = new ShipShotDistanceInformation(anotherShip.Value, thisShip);
            if ((shotInfo.Range < 4) && (shotInfo.InArc))
            {
                result++;
            }
        }

        return result;
    }

    public static bool HasTargetLockOn(Ship.GenericShip attacker, Ship.GenericShip defender)
    {
        bool result = false;
        char letter = ' ';
        letter = Actions.GetTargetLocksLetterPair(attacker, defender);
        if (letter != ' ') result = true;
        return result;
    }

}
