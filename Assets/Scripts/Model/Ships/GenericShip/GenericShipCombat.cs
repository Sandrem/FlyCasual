using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public interface IShipWeapon
    {
        GenericShip Host { get; set; }
        string Name { get; set; }

        int MinRange { get; set; }
        int MaxRange { get; set; }
        int AttackValue { get; set; }

        bool CanShootOutsideArc { get; set; }

        bool IsShotAvailable(GenericShip targetShip);
        void PayAttackCost(Action callBack);
    }

    public class PrimaryWeaponClass : IShipWeapon
    {
        public GenericShip Host { get; set; }
        public string Name { get; set; }

        public int MinRange { get; set; }
        public int MaxRange { get; set; }

        private int attackValue;
        public int AttackValue
        {
            get
            {
                int result = attackValue;
                Host.CallAfterGotNumberOfPrimaryWeaponAttackDices(ref result);
                return result;
            }
            set { attackValue = value; }
        }

        public bool CanShootOutsideArc
        {
            set {}
            get { return Host.ArcInfo.CanShootOutsideArc; }
        }

        public PrimaryWeaponClass(GenericShip host)
        {
            Host = host;
            Name = "Primary Weapon";

            MinRange = 1;
            MaxRange = 3;

            AttackValue = Host.Firepower;
        }

        public bool IsShotAvailable(GenericShip targetShip)
        {
            bool result = true;

            int range;

            if (Combat.ChosenWeapon.GetType() == GetType())
            {
                range = Combat.ShotInfo.Range;
                if (!CanShootOutsideArc)
                {
                    //TODO: Change to munitions arc
                    if (!Combat.ShotInfo.InPrimaryArc) return false;
                }
            }
            else
            {
                if (!CanShootOutsideArc)
                {
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, targetShip, this);
                    range = shotInfo.Range;

                    //TODO: Change to munitions arc
                    if (!shotInfo.InPrimaryArc) return false;
                }
                else
                {
                    Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Host, targetShip);
                    range = distanceInfo.Range;
                }
            }

            if (range < MinRange) return false;
            if (range > MaxRange) return false;

            return result;
        }

        public void PayAttackCost(Action callBack)
        {
            callBack();
        }
    }

    public partial class GenericShip
    {
        public PrimaryWeaponClass PrimaryWeapon;

        private List<CriticalHitCard.GenericCriticalHit> AssignedCritCards = new List<CriticalHitCard.GenericCriticalHit>();
        private List<CriticalHitCard.GenericCriticalHit> AssignedDamageCards = new List<CriticalHitCard.GenericCriticalHit>();
        public DiceRoll AssignedDamageDiceroll = new DiceRoll(DiceKind.Attack, 0, DiceRollCheckType.Virtual);

        // EVENTS

        public event EventHandlerShip OnActivationPhaseStart;
        public event EventHandlerShip OnActionSubPhaseStart;
        public event EventHandlerShip OnCombatPhaseStart;

        public event EventHandlerBool OnTryPerformAttack;
        public static event EventHandlerBool OnTryPerformAttackGlobal;

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

        public event EventHandlerShip OnDestroyed;

        public event EventHandlerShip AfterAttackWindow;

        public event EventHandlerShip AfterCombatEnd;

        public event EventHandlerBombDropTemplates OnGetAvailableBombDropTemplates;

        // TRIGGERS

        public void CallOnActivationPhaseStart()
        {
            if (OnActivationPhaseStart != null) OnActivationPhaseStart(this);
        }

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

            if (OnTryPerformAttackGlobal != null) OnTryPerformAttackGlobal(ref result);

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

            result = Combat.ChosenWeapon.AttackValue;

            if (AfterGotNumberOfAttackDices != null) AfterGotNumberOfAttackDices(ref result);

            if (result < 0) result = 0;
            return result;
        }

        public void CallAfterGotNumberOfPrimaryWeaponAttackDices(ref int result)
        {
            if (AfterGotNumberOfPrimaryWeaponAttackDices != null) AfterGotNumberOfPrimaryWeaponAttackDices(ref result);
        }

        public int GetNumberOfDefenceDices(GenericShip attackerShip)
        {
            int result = Agility;

            if (Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass))
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

                Triggers.RegisterTrigger(new Trigger
                {
                    Name = "Information about faceup damage card",
                    TriggerOwner = this.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFaceupCritCardReadyToBeDealtUI,
                    EventHandler = delegate { InformCrit.LoadAndShow(); }
                });

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
            Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardReadyToBeDealtUI, SufferHullDamagePart3);
        }

        private void SufferHullDamagePart3()
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
            UI.AddTestLogEntry("Ship with ID " + ShipId + " is destroyed");

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

            if (OnDestroyed != null) OnDestroyed(this);
        }

        public List<CriticalHitCard.GenericCriticalHit> GetAssignedCritCards()
        {
            return AssignedCritCards;
        }

        // ATTACK TYPES

        public int GetAnotherAttackTypesCount()
        {
            int result = 0;

            foreach (var upgrade in UpgradeBar.GetInstalledUpgrades())
            {
                IShipWeapon secondaryWeapon = upgrade as IShipWeapon;
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
            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(this, anotherShip, PrimaryWeapon);
            result = InPrimaryWeaponFireZone(shotInfo.Range, shotInfo.InPrimaryArc);
            return result;
        }

        public bool InPrimaryWeaponFireZone(int range, bool inArc)
        {
            bool result = true;
            if (range > 3) return false;
            if (!inArc) return false;
            return result;
        }

        public List<Bombs.BombDropTemplates> GetAvailableBombDropTemplates()
        {
            List<Bombs.BombDropTemplates> availableTemplates = new List<Bombs.BombDropTemplates>() { Bombs.BombDropTemplates.Straight1 };

            if (OnGetAvailableBombDropTemplates != null) OnGetAvailableBombDropTemplates(availableTemplates);

            return availableTemplates;
        }

    }

}
