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
        public delegate void EventHandlerActionBool(ActionsList.GenericAction action, ref bool data);
        public delegate void EventHandlerShip(GenericShip ship);
        public delegate void EventHandlerShipMovement(GenericShip ship, ref Movement movement);
        public delegate void EventHandlerShipCrit(GenericShip ship, ref CriticalHitCard.GenericCriticalHit crit);

        public event EventHandler OnDestroyed;
        public event EventHandler OnDefence;
        public event EventHandler OnAttack;
        public event EventHandlerShip OnMovementStart;
        public event EventHandlerShip OnMovementFinish;
        public event EventHandlerShip OnPositionFinish;
        public event EventHandlerShip OnMovementFinishWithColliding;
        public event EventHandlerShip OnMovementFinishWithoutColliding;
        public event EventHandlerShip AfterAssignedDamageIsChanged;
        public event EventHandlerShip AfterStatsAreChanged;
        public event EventHandlerShip AfterAttackWindow;
        public event EventHandlerShip OnCombatPhaseStart;
        public event EventHandlerShip OnLandedOnObstacle;
        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponAttackDices;
        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponDefenceDices;
        public event EventHandlerInt AfterGetPilotSkill;
        public event EventHandlerInt AfterGetAgility;
        public event EventHandlerBool OnTrySpendFocus;
        public event EventHandlerBool OnTryReroll;
        public event EventHandlerBool OnTryPerformAttack;
        public event EventHandlerBool OnCheckFaceupCrit;
        public event EventHandlerShipMovement AfterGetManeuverColor;
        public event EventHandlerShipMovement AfterGetManeuverAvailablity;
        public event EventHandlerShipCrit OnAssignCrit;

        //TRIGGERS

        public void AttackStart()
        {
            if (Combat.Attacker.ShipId == this.ShipId) IsAttackPerformed = true;
            if (OnAttack != null) OnAttack();
        }

        public void DefenceStart()
        {
            if (OnDefence != null) OnDefence();
        }

        public void StartMoving()
        {
            if (OnMovementStart != null) OnMovementStart(this);
        }

        public void FinishMoving()
        {
            if (OnMovementFinish != null) OnMovementFinish(this);
        }

        public void FinishPosition()
        {
            if (OnPositionFinish != null) OnPositionFinish(this);
        }

        public void FinishMovementWithColliding()
        {
            if (OnMovementFinishWithColliding != null) OnMovementFinishWithColliding(this);
        }

        public void FinishMovingWithoutColliding()
        {
            if (OnMovementFinishWithoutColliding != null) OnMovementFinishWithoutColliding(this);
        }

        public void CallAfterAttackWindow()
        {
            if (AfterAttackWindow != null) AfterAttackWindow(this);
        }

        public bool CallTryPerformAttack(bool result = true)
        {
            if (OnTryPerformAttack != null) OnTryPerformAttack(ref result);

            return result;
        }

        public void CallOnCombatPhaseStart()
        {
            if (OnCombatPhaseStart != null) OnCombatPhaseStart(this);
        }
    }

}
