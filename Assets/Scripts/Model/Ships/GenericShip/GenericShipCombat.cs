using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{

    public partial class GenericShip
    {
        public Arcs.GenericArc Arc = new Arcs.GenericArc();

        private List<CriticalHitCard.GenericCriticalHit> AssignedCritCards = new List<CriticalHitCard.GenericCriticalHit>();
        private List<CriticalHitCard.GenericCriticalHit> AssignedDamageCards = new List<CriticalHitCard.GenericCriticalHit>();
        public DiceRoll AssignedDamageDiceroll = new DiceRoll(DiceKind.Attack, 0, DiceRollCheckType.Combat);

        // EVENTS

        public event EventHandlerShip OnActionSubPhaseStart;
        public event EventHandlerShip OnCombatPhaseStart;

        public event EventHandlerBool OnTryPerformAttack;

        public event EventHandler OnAttack;
        public event EventHandler OnDefence;

        public event EventHandler OnAttackHitAsAttacker;
        public event EventHandler OnAttackHitAsDefender;

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
        
        public bool CallCanPerformAttack(bool result = true)
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

        public void CallOnAttackHitAsAttacker()
        {
            if (OnAttackHitAsAttacker != null) OnAttackHitAsAttacker();
        }

        public void CallOnAttackHitAsDefender()
        {
            if (OnAttackHitAsDefender != null) OnAttackHitAsDefender();
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

        public bool TryDiscardFaceDownDamageCard()
        {
            bool result = false;

            if (AssignedDamageCards.Count != 0)
            {
                result = true;
                int random = UnityEngine.Random.Range(0, AssignedDamageCards.Count);
                AssignedDamageCards.RemoveAt(random);
                Hull++;
                AfterAssignedDamageIsChanged(this);
            }
            return result;
        }

        public void FlipFacedownFaceupDamageCard(CriticalHitCard.GenericCriticalHit critCard)
        {
            critCard.DiscardEffect(this);
            AssignedCritCards.Remove(critCard);
            if (Owner.GetType() == typeof(Players.HumanPlayer)) Messages.ShowInfoToHuman("Critical damage card \"" + critCard.Name + "\" is flipped facedown");
        }

        // DAMAGE

        public void SufferDamage(object sender, EventArgs e)
        {
            if (DebugManager.DebugDamage) Debug.Log("+++ Source: " + (e as DamageSourceEventArgs).Source);
            if (DebugManager.DebugDamage) Debug.Log("+++ DamageType: " + (e as DamageSourceEventArgs).DamageType);

            if (Shields > 0)
            {
                SufferShieldDamage();
            }
            else
            {
                bool isCritical = (AssignedDamageDiceroll.RegularSuccesses == 0);
                SufferHullDamage(CheckFaceupCrit(isCritical), e);
            }
        }

        public void SufferHullDamage(bool isCritical, EventArgs e)
        {
            AssignedDamageDiceroll.CancelHits(1);

            if (DebugManager.DebugAllDamageIsCrits) isCritical = true;

            if (isCritical)
            {
                Combat.CurrentCriticalHitCard = CriticalHitsDeck.GetCritCard();

                if (DebugManager.DebugDamage) Debug.Log("+++ Crit: " + Combat.CurrentCriticalHitCard.Name);

                if (OnFaceupCritCardReadyToBeDealt != null) OnFaceupCritCardReadyToBeDealt(this, ref Combat.CurrentCriticalHitCard);

                if (OnFaceupCritCardReadyToBeDealtGlobal != null) OnFaceupCritCardReadyToBeDealtGlobal(this, ref Combat.CurrentCriticalHitCard, e);

                Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardReadyToBeDealt, SufferHullDamagePart2);
            }
            else
            {
                AssignedDamageCards.Add(CriticalHitsDeck.GetCritCard());
                DecreaseHullValue(Triggers.FinishTrigger);
            }
        }

        private void SufferHullDamagePart2()
        {
            if (OnAssignCrit != null) OnAssignCrit(this, ref Combat.CurrentCriticalHitCard);

            if (Combat.CurrentCriticalHitCard != null)
            {
                AssignedCritCards.Add(Combat.CurrentCriticalHitCard);
                DecreaseHullValue(delegate { Combat.CurrentCriticalHitCard.AssignCrit(this); });
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public void DecreaseHullValue(Action callBack)
        {
            Hull--;
            Hull = Mathf.Max(Hull, 0);

            CallAfterAssignedDamageIsChanged();

            IsHullDestroyedCheck(callBack);
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
            AssignedDamageDiceroll.CancelHits(1);

            Shields--;
            CallAfterAssignedDamageIsChanged();
            Triggers.FinishTrigger();
        }

        public void IsHullDestroyedCheck(Action callBack)
        {
            if (Hull == 0 && !IsDestroyed)
            {
                DestroyShip(callBack);
            }
            else
            {
                callBack();
            }
        }

        public void DestroyShip(Action callBack, bool forced = false)
        {
            Game.UI.AddTestLogEntry(PilotName + "\'s ship is destroyed");

            IsDestroyed = true;
            PlayDestroyedAnimSound(delegate { CheckShipModelDestruction(forced); callBack(); });
        }

        private void CheckShipModelDestruction(bool forced = false)
        {
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
            Phases.OnCombatSubPhaseRequiredPilotSkillIsChanged -= PerformShipDestruction;
            Roster.DestroyShip(this.GetTag());

            if (OnDestroyed != null) OnDestroyed();
        }

        public List<CriticalHitCard.GenericCriticalHit> GetAssignedCritCards()
        {
            return AssignedCritCards;
        }

        // ATTACK TYPES

        public int GetAttackTypes()
        {
            int result = 0;

            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Selection.ThisShip, Selection.AnotherShip);
            if (InPrimaryWeaponFireZone(shotInfo.Range, shotInfo.InArc)) result++;

            foreach (var upgrade in InstalledUpgrades)
            {
                Upgrade.GenericSecondaryWeapon secondaryWeapon = upgrade.Value as Upgrade.GenericSecondaryWeapon;
                if (secondaryWeapon != null)
                {
                    if (secondaryWeapon.IsShotAvailable(Selection.AnotherShip)) result++;
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
