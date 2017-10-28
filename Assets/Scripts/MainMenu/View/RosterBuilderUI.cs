using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RosterBuilderUI : MonoBehaviour {

    public void AddShip(int playerNo)
    {
        RosterBuilder.TryAddShip(Tools.IntToPlayer(playerNo));
    }

    public void OnPlayerFactionChanged()
    {
        RosterBuilder.PlayerFactonChange();
    }

}
