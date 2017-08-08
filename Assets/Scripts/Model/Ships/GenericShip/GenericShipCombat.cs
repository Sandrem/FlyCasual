using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
 
        private List<CriticalHitCard.GenericCriticalHit> AssignedCritCards = new List<CriticalHitCard.GenericCriticalHit>();
        private List<CriticalHitCard.GenericCriticalHit> AssignedDamageCards = new List<CriticalHitCard.GenericCriticalHit>();
        public DiceRoll AssignedDamageDiceroll = new DiceRoll(DiceKind.Attack, 0);

        // EVENTS

        public event EventHandlerShip OnActionSubPhaseStart;
        public event EventHandlerShip OnCombatPhaseStart;

        public event EventHandlerBool OnTryPerformAttack;

        public event EventHandler OnAttack;
        public event EventHandler OnDefence;

        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponAttackDices;
        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponDefenceDices;
        public event EventHandlerInt AfterGotNumberOfAttackDices;

        public event EventHandlerShip AfterAssignedDamageIsChanged;

        public event EventHandlerBool OnCheckFaceupCrit;
        public event EventHandlerShipCritArgs OnFaceupCritCardReadyToBeDealt;
        public static event EventHandlerShipCritArgs OnFaceupCritCardReadyToBeDealtGlobal;
        public event EventHandlerShipCritArgs OnAssignCrit;
        

        public event EventHandler OnDestroyed;

        public event EventHandlerShip AfterAttackWindow;

        public event EventHandlerShip AfterCombatEnd;

        // TRIGGERS

        public void CallOnActionSubPhaseStart()
        {
            if (OnActionSubPhaseStart != null) OnActionSubPhaseStart(this);
        }

        public void CallOnCombatPhaseStart()
        {
            if (OnCombatPhaseStart != null) OnCombatPhaseStart(this);
        }
        
        public bool CallTryPerformAttack(bool result = true)
        {
            if (OnTryPerformAttack != null) OnTryPerformAttack(ref result);

            return result;
        }

        public void CallAttackStart()
        {
            ClearAlreadyExecutedActionEffects();
            if (Combat.Attacker.ShipId == this.ShipId) IsAttackPerformed = true;
            if (OnAttack != null) OnAttack();
        }

        public void CallDefenceStart()
        {
            ClearAlreadyExecutedActionEffects();
            if (OnDefence != null) OnDefence();
        }

        public void CallAfterAttackWindow()
        {
            if (AfterAttackWindow != null) AfterAttackWindow(this);
        }

        public void CallCombatEnd()
        {
            if (AfterCombatEnd != null) AfterCombatEnd(this);
        }

        // DICES

        public int GetNumberOfAttackDices(GenericShip targetShip)
        {
            int result = 0;

            if (Combat.SecondaryWeapon == null)
            {
                result = Firepower;
                if (AfterGotNumberOfPrimaryWeaponAttackDices != null) AfterGotNumberOfPrimaryWeaponAttackDices(ref result);
            }
            else
            {
                result = Combat.SecondaryWeapon.GetAttackValue();
            }

            if (AfterGotNumberOfAttackDices != null) AfterGotNumberOfAttackDices(ref result);

            if (result < 0) result = 0;
            return result;
        }

        public int GetNumberOfDefenceDices(GenericShip attackerShip)
        {
            int result = Agility;

            if (Combat.SecondaryWeapon == null)
            {
                if (AfterGotNumberOfPrimaryWeaponDefenceDices != null) AfterGotNumberOfPrimaryWeaponDefenceDices(ref result);
            }
            return result;
        }

        // REGEN

        public bool TryRegenShields()
        {
            bool result = false;
            if (Shields < MaxShields)
            {
                result = true;
                Shields++;
                AfterAssignedDamageIsChanged(this);
            };
            return result;
        }

        public bool TryRegenHull()
        {
            bool result = false;
            if (Hull < MaxHull)
            {
                result = true;
                Hull++;
                AfterAssignedDamageIsChanged(this);
            };
            return result;
        }

        // DAMAGE

        public void SufferDamage(object sender, EventArgs e)
        {
            if (Shields > 0)
            {
                SufferShieldDamage();
            }
            else
            {
                bool isCritical = (AssignedDamageDiceroll.RegularSuccesses == 0);
                SufferHullDamage(CheckFaceupCrit(isCritical));
            }
        }

        public void SufferHullDamage(bool isCritical)
        {
            if (DebugManager.DebugAllDamageIsCrits) isCritical = true;

            if (isCritical)
            {
                Combat.CurrentCriticalHitCard = CriticalHitsDeck.GetCritCard();

                //if (DebugManager.DebugDamage) Debug.Log("+++ Crit: " + Combat.CurrentCriticalHitCard.Name);
                //if (DebugManager.DebugDamage) Debug.Log("+++ Source: " + (e as DamageSourceEventArgs).Source);
                //if (DebugManager.DebugDamage) Debug.Log("+++ DamageType: " + (e as DamageSourceEventArgs).DamageType);

                if (OnFaceupCritCardReadyToBeDealt != null) OnFaceupCritCardReadyToBeDealt(this, ref Combat.CurrentCriticalHitCard);

                if (OnFaceupCritCardReadyToBeDealtGlobal != null) OnFaceupCritCardReadyToBeDealtGlobal(this, ref Combat.CurrentCriticalHitCard);

                if (OnAssignCrit != null) OnAssignCrit(this, ref Combat.CurrentCriticalHitCard);

                if (Combat.CurrentCriticalHitCard != null)
                {
                    AssignedCritCards.Add(Combat.CurrentCriticalHitCard);
                    Combat.CurrentCriticalHitCard.AssignCrit(this);
                }
            }
            else
            {
                AssignedDamageCards.Add(CriticalHitsDeck.GetCritCard());
                Triggers.FinishTrigger();
            }

            AssignedDamageDiceroll.CancelHits(1);

            DecreaseHullValue();
        }

        public void DecreaseHullValue()
        {
            Hull--;
            Hull = Mathf.Max(Hull, 0);

            CallAfterAssignedDamageIsChanged();

            IsHullDestroyedCheck();
        }

        public void CallAfterAssignedDamageIsChanged()
        {
            if (AfterAssignedDamageIsChanged != null) AfterAssignedDamageIsChanged(this);
        }

        private bool CheckFaceupCrit(bool result)
        {
            if (OnCheckFaceupCrit != null) OnCheckFaceupCrit(ref result);
            return result;
        }

        public void SufferShieldDamage()
        {
            Shields--;
            AssignedDamageDiceroll.CancelHits(1);
            CallAfterAssignedDamageIsChanged();
            Triggers.FinishTrigger();
        }

        public void IsHullDestroyedCheck()
        {
            if (Hull == 0)
            {
                DestroyShip();
            }
        }

        public void DestroyShip(bool forced = false)
        {
            Game.UI.AddTestLogEntry(PilotName + "\'s ship is destroyed");

            if ((Phases.CurrentSubPhase.RequiredPilotSkill == PilotSkill) && (!IsAttackPerformed) && (!forced) && (Phases.CurrentPhase.GetType() == typeof(MainPhases.CombatPhase)))
            {
                Phases.OnCombatSubPhaseRequiredPilotSkillIsChanged += PerformShipDestruction;
            }
            else
            {
                PerformShipDestruction();
            }
        }

        private void PerformShipDestruction()
        {
            if (!IsDestroyed)
            {
                Roster.DestroyShip(this.GetTag());

                if (OnDestroyed != null) OnDestroyed();
                IsDestroyed = true;
            }
        }

        public List<CriticalHitCard.GenericCriticalHit> GetAssignedCritCards()
        {
            return AssignedCritCards;
        }

        // ATTACK TYPES

        //Todo: Rework

        public int GetAttackTypes(int range, bool inArc)
        {
            int result = 0;

            if (InPrimaryWeaponFireZone(range, inArc)) result++;

            foreach (var upgrade in InstalledUpgrades)
            {
                if (upgrade.Value.Type == Upgrade.UpgradeSlot.Torpedoes)
                {
                    if ((upgrade.Value as Upgrade.GenericSecondaryWeapon).IsShotAvailable(Selection.AnotherShip)) result++;
                }
            }

            return result;
        }

        public bool InPrimaryWeaponFireZone(GenericShip anotherShip)
        {
            bool result = true;
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(this, anotherShip);
            result = InPrimaryWeaponFireZone(shotInfo.Range, shotInfo.InArc);
            return result;
        }

        public bool InPrimaryWeaponFireZone(int range, bool inArc)
        {
            bool result = true;
            if (range > 3) return false;
            if (!inArc) return false;
            return result;
        }

    }

}
