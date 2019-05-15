using Actions;
using ActionsList;
using Arcs;
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
        public delegate void EventHandlerBoolDirection(ref bool data, Direction direction);
        public delegate void EventHandlerAction(GenericAction action);
        public delegate void EventHandlerActionRef(ref GenericAction action);
        public delegate void EventHandlerActionBool(GenericAction action, ref bool data);
        public delegate void EventHandlerActionInt(GenericAction action, ref int priority);
        public delegate void EventHandlerShipActionBool(GenericShip ship, GenericAction action, ref bool data);
        public delegate void EventHandlerShip(GenericShip ship);
        public delegate void EventHandlerShipDamage(GenericShip ship, DamageSourceEventArgs e);
        public delegate void EventHandlerShipBool(GenericShip ship, bool flag);
        public delegate void EventHandlerShipRefBool(GenericShip ship, ref bool flag);
        public delegate void EventHandlerBool2Ships(ref bool result, GenericShip attacker, GenericShip defender);
        public delegate void EventHandler2Ships(GenericShip attacker, GenericShip defender);
        public delegate void EventHandlerShipType(GenericShip ship, System.Type type);
        public delegate void EventHandlerShipTypeBool(GenericShip ship, System.Type type, ref bool data);
        public delegate void EventHandlerShipMovement(GenericShip ship, ref Movement.ManeuverHolder movement);
        public delegate void EventHandlerShipCritArgs(GenericShip ship, GenericDamageCard crit, EventArgs e = null);
        public delegate void EventHandlerTokenBool(Tokens.GenericToken token, ref bool data);
        public delegate void EventHandlerShipTokenBool(GenericShip ship, Tokens.GenericToken token, ref bool data);
        public delegate void EventHandlerBombDropTemplates(List<Bombs.BombDropTemplates> availableTemplates);
        public delegate void EventHandlerBarrelRollTemplates(List<ActionsHolder.BarrelRollTemplates> availableTemplates);
        public delegate void EventHandlerDecloakTemplates(List<ActionsHolder.DecloakTemplates> availableTemplates);
        public delegate void EventHandlerBoostTemplates(List<BoostMove> availableTemplates);
        public delegate void EventHandlerDiceroll(DiceRoll diceroll);
        public delegate void EventHandlerTokensList(List<Tokens.GenericToken> tokens);
        public delegate void EventHandlerBoolStringList(ref bool result, List<string> stringList);
        public delegate void EventHandlerObjArgsBool(object sender, EventArgs e, ref bool isChanged);
        public delegate void EventHandlerUpgrade(GenericUpgrade upgrade);
        public delegate void EventHandlerDualUpgrade(GenericDualUpgrade upgrade);
        public delegate void EventHandelerWeaponRange(IShipWeapon weapon, ref int minRange, ref int maxRange, GenericShip target);
        public delegate void EventHandlerArcFacingList(List<ArcFacing> facings);
        public delegate void EventHandlerFailedAction(GenericAction action, List<ActionFailReason> failReasons, ref bool isDefaultFailOverwritten);
        public delegate void EventHandlerCheckRange(GenericShip anotherShip, int minRange, int maxRange, BoardTools.RangeCheckReason reason, ref bool isInRange);
        public delegate void EventHandlerForceAlignmentBool(ForceAlignment alignment, ref bool data);
        public delegate void EventHandlerShipBomb(GenericShip ship, GenericBomb bomb);

        public event EventHandlerShip AfterStatsAreChanged;
        public event EventHandlerInt AfterGetMaxHull;
        public event EventHandlerForceAlignmentBool OnForceAlignmentEquipCheck;

    }

}
