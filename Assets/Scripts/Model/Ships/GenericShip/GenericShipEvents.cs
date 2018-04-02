using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    public partial class GenericShip
    {

        public delegate void EventHandler();
        public delegate void EventHandlerInt(ref int data);
        public delegate void EventHandlerBool(ref bool data);
        public delegate void EventHandlerAction(ActionsList.GenericAction action);
        public delegate void EventHandlerActionBool(ActionsList.GenericAction action, ref bool data);
        public delegate void EventHandlerActionInt(ActionsList.GenericAction action, ref int priority);
        public delegate void EventHandlerShipActionBool(GenericShip ship, ActionsList.GenericAction action, ref bool data);
        public delegate void EventHandlerShip(GenericShip ship);
        public delegate void EventHandlerShipBool(GenericShip ship, bool flag);
        public delegate void EventHandler2Ships(ref bool result, GenericShip attacker, GenericShip defender);
        public delegate void EventHandlerShipType(GenericShip ship, System.Type type);
        public delegate void EventHandlerShipTypeBool(GenericShip ship, System.Type type, ref bool data);
        public delegate void EventHandlerShipMovement(GenericShip ship, ref Movement.MovementStruct movement);
        public delegate void EventHandlerShipCritArgs(GenericShip ship, DamageDeckCard.GenericDamageCard crit, EventArgs e = null);
        public delegate void EventHandlerTokenBool(Tokens.GenericToken token, ref bool data);
        public delegate void EventHandlerBombDropTemplates(List<Bombs.BombDropTemplates> availableTemplates);
        public delegate void EventHandlerBarrelRollTemplates(List<Actions.BarrelRollTemplates> availableTemplates);
        public delegate void EventHandlerBoostTemplates(List<Actions.BoostTemplates> availableTemplates);
        public delegate void EventHandlerDiceroll(DiceRoll diceroll);
        public delegate void EventHandlerTokensList(List<Tokens.GenericToken> tokens);
        public delegate void EventHandlerBoolStringList(ref bool result, List<string> stringList);
        public delegate void EventHandlerObjArgsBool(object sender, EventArgs e, ref bool isChanged);
        public delegate void EventHandlerUpgrade(GenericUpgrade upgrade);
        public delegate void EventHandlerDualUpgrade(GenericDualUpgrade upgrade);

        public event EventHandlerShip AfterStatsAreChanged;
        public event EventHandlerInt AfterGetMaxHull;

    }

}
