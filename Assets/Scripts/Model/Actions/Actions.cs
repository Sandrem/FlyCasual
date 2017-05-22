using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Actions {

    private static GameManagerScript Game;

	private static readonly float DISTANCE_1 = 3.28f / 3f;
    private static Dictionary<char, bool> Letters = new Dictionary<char, bool>();

    //EVENTS
    public delegate void EventHandler2Ships(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender);
    public static event EventHandler2Ships OnCheckCanPerformAttack;

    static Actions()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

    public static bool AssignTargetLockToPair(Ship.GenericShip thisShip, Ship.GenericShip targetShip)
    {
        bool result = false;

        if (Letters.Count == 0) InitializeTargetLockLetters();

        MovementTemplates.ShowRange(thisShip, targetShip);

        if (GetRange(thisShip, targetShip) < 4)
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

            Selection.ThisShip.AssignToken(tokenBlue);
            targetShip.AssignToken(tokenRed);

            result = true;
        }
        else
        {
            Game.UI.ShowError("Target is out of range of Target Lock");
        }

        return result;
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

    public static bool InArcCheck(Ship.GenericShip thisShip, Ship.GenericShip anotherShip) {
        //TODO: Show Shortest Distance
        //TODO: Adapt DistancRules to show how close to outOfArc;

        Vector3 vectorFacing = thisShip.GetFrontFacing();

        bool inArc = false;

        foreach (var objThis in thisShip.GetStandFrontEdgePoins())
        {
            foreach (var objAnother in anotherShip.GetStandEdgePoints())
            {

                Vector3 vectorToTarget = objAnother.Value - objThis.Value;
                float angle = Vector3.Angle(vectorToTarget, vectorFacing);
                //Debug.Log ("Angle between " + objThis.Key + " and " + objAnother.Key + " is: " + angle.ToString ());
                MovementTemplates.ShowFiringArcRange(thisShip, anotherShip);
                if (angle <= 40)
                {
                    inArc = true;
                    //TODO: Comment shortcut to check all variants
                    //return inArc;
                }
            }
        }

        return inArc;
    }

    public static float GetVector(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        float result = 0;

        float angle = 0;
        Vector3 vectorFacing = thisShip.GetFrontFacing();
        Vector3 vectorToTarget = anotherShip.GetPosition() - thisShip.GetPosition();
        angle = Vector3.Angle(vectorToTarget, vectorFacing);

        int direction = 0;
        direction = (thisShip.TransformPoint(anotherShip.GetPosition()).x > 0) ? 1 : -1 ;

        result = angle * direction;

        return result;
    }

    public static bool IsClosing(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        bool result = false;

        int range = GetRange(thisShip, anotherShip);
        if (range <= 1) return true;
        if (range >= 3) return false;

        float distanceToFront = Vector3.Distance(thisShip.GetPosition(), anotherShip.GetCentralFrontPoint());
        float distanceToBack = Vector3.Distance(thisShip.GetPosition(), anotherShip.GetCentralBackPoint());
        result = (distanceToFront < distanceToBack) ? true : false;
        return result;
    }

    public static int GetRange(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        float distance = Vector3.Distance (thisShip.GetClosestEdgesTo (anotherShip) ["this"], thisShip.GetClosestEdgesTo (anotherShip) ["another"]);
		int range = Mathf.CeilToInt(distance / DISTANCE_1);
		return range;
	}

	public static void OnlyCheckShot() {
		CheckShot ();
	}

	public static bool CheckShot() {
        bool result = true;
        OnCheckCanPerformAttack(ref result, Selection.ThisShip, Selection.AnotherShip);
		return result;
	}

	public static void PerformAttack() {
        Game.UI.HideContextMenu();
        if (CheckShot()) Combat.PerformAttack(Selection.ThisShip, Selection.AnotherShip);
	}

}
