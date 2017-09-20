using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {

        public delegate void EventHandler();
        public delegate void EventHandlerInt(ref int data);
        public delegate void EventHandlerBool(ref bool data);
        public delegate void EventHandlerAction(ActionsList.GenericAction action);
        public delegate void EventHandlerActionBool(ActionsList.GenericAction action, ref bool data);
        public delegate void EventHandlerShip(GenericShip ship);
        public delegate void EventHandlerShipType(GenericShip ship, System.Type type);
        public delegate void EventHandlerShipMovement(GenericShip ship, ref Movement.MovementStruct movement);
        public delegate void EventHandlerShipCritArgs(GenericShip ship, ref CriticalHitCard.GenericCriticalHit crit, EventArgs e = null);
        public delegate void EventHandlerTokenBool(Tokens.GenericToken token, ref bool data);
        public delegate void EventHandlerBombDropTemplates(List<Bombs.BombDropTemplates> availableTemplates);
        public delegate void EventHandlerDiceroll(DiceRoll diceroll);

        public event EventHandlerShip AfterStatsAreChanged;
        public event EventHandlerInt AfterGetAgility;
        public event EventHandlerInt AfterGetMaxHull;
        public event EventHandlerInt AfterGetMaxShields;

    }

}
