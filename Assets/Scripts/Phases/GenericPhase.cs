using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPhase
{
    protected GameManagerScript Game;

    public string Name;

    public virtual void StartPhase() { }
    public virtual void NextPhase() { }

    //TODO: move
    protected Player AnotherPlayer(Player player)
    {
        return (player == Player.Player1) ? Player.Player2 : Player.Player1;
    }

}
