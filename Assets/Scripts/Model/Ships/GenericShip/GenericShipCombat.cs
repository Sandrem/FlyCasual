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
                int result = Host.Firepower;
                if (attackValue == int.MaxValue) Debug.Log(attackValue);
                Host.CallAfterGotNumberOfPrimaryWeaponAttackDice(ref result);
                return result;
            }
            set { attackValue = value; }
        }

        public bool CanShootOutsideArc
        {
            set { }
            get { return Host.ArcInfo.OutOfArcShotPermissions.CanShootPrimaryWeapon; }
        }

        public PrimaryWeaponClass(GenericShip host)
        {
            Host = host;
            Name = "Primary Weapon";

            MinRange = 1;
            MaxRange = 3;
        }

        public bool IsShotAvailable(GenericShip targetShip)
        {
            bool result = true;

            int range;

            Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Host, targetShip, this);
            range = shotInfo.Range;
            if (!CanShootOutsideArc)
            {
                if (!shotInfo.InShotAngle) return false;

                if (!shotInfo.CanShootPrimaryWeapon) return false;
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

        public bool IsCannotAttackSecondTime { get; set; }
        public bool CanAttackBumpedTargetAlways { get; set; }
        public bool PreventDestruction { get; set; }

        // EVENTS

        public event EventHandlerShip OnActivationPhaseStart;
        public event EventHandlerShip OnActionSubPhaseStart;
        public event EventHandlerShip OnCombatPhaseStart;
        public event EventHandlerShip OnCombatPhaseEnd;

        public event EventHandlerBoolStringList OnTryPerformAttack;
        public static event EventHandlerBoolStringList OnTryPerformAttackGlobal;

        public event EventHandlerTokensList OnGenerateAvailableAttackPaymentList;

        public event EventHandler OnAttackStartAsAttacker;
        public static event EventHandler OnAttackStartAsAttackerGlobal;
        public event EventHandler OnAttackStartAsDefender;

        public event EventHandler OnShotStartAsAttacker;
        public event EventHandler OnShotStartAsDefender;

        public event EventHandlerShip OnCheckCancelCritsFirst;

        public event EventHandler OnDefence;

        public event EventHandler OnAtLeastOneCritWasCancelledByDefender;

        public event EventHandler OnShotHitAsAttacker;
        public event EventHandler OnShotHitAsDefender;
        public static event EventHandler OnShotHitAsDefenderGlobal;

        public static event EventHandler OnTryDamagePreventionGlobal;

        public event EventHandler OnAttackHitAsAttacker;
        public event EventHandler OnAttackHitAsDefender;
        public static event EventHandler OnAttackHitAsDefenderGlobal;
        public event EventHandler OnAttackMissedAsAttacker;
        public event EventHandler OnAttackMissedAsDefender;

        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponAttackDice;
        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponDefenceDice;
        public event EventHandlerInt AfterGotNumberOfAttackDice;
        public event EventHandlerInt AfterGotNumberOfDefenceDice;

        public event EventHandlerShip AfterAssignedDamageIsChanged;

        public event EventHandlerBool OnCheckFaceupCrit;
        public event EventHandlerShipCritArgs OnFaceupCritCardReadyToBeDealt;
        public static event EventHandlerShipCritArgs OnFaceupCritCardReadyToBeDealtGlobal;
        public event EventHandlerShipCritArgs OnAssignCrit;

        public event EventHandlerShip OnDamageCardIsDealt;

        public event EventHandlerShip OnReadyToBeDestroyed;
        public event EventHandlerShip OnDestroyed;

        public event EventHandlerShip AfterAttackWindow;
        public event EventHandlerShip OnCheckSecondAttack;

        public event EventHandlerShip OnAttackFinish;

        public event EventHandlerBombDropTemplates OnGetAvailableBombDropTemplates;

        public event EventHandlerDiceroll OnImmediatelyAfterRolling;

        public event EventHandlerBool OnWeaponsDisabledCheck;

        public event EventHandler2Ships OnCanAttackBumpedTarget;
        public static event EventHandler2Ships OnCanAttackBumpedTargetGlobal;

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

        public void CallOnCombatPhaseEnd()
        {
            if (OnCombatPhaseEnd != null) OnCombatPhaseEnd(this);
        }

        public bool CallCanPerformAttack(bool result = true, List<string> stringList = null, bool isSilent = false)
        {
            if (stringList == null) stringList = new List<string>();

            if (OnTryPerformAttack != null) OnTryPerformAttack(ref result, stringList);

            if (OnTryPerformAttackGlobal != null) OnTryPerformAttackGlobal(ref result, stringList);

            if (!isSilent && stringList.Count > 0)
            {
                foreach (var errorMessage in stringList)
                {
                    Messages.ShowErrorToHuman(errorMessage);
                }
            }

            return result;
        }

        public void CallAttackStart()
        {
            if (Combat.Attacker.ShipId == this.ShipId)
            {
                CallAfterAttackWindow();
                IsAttackPerformed = true;

                if (OnAttackStartAsAttacker != null) OnAttackStartAsAttacker();
                if (OnAttackStartAsAttackerGlobal != null) OnAttackStartAsAttackerGlobal();
            }
            else if (Combat.Defender.ShipId == this.ShipId)
            {
                if (OnAttackStartAsDefender != null) OnAttackStartAsDefender();
            }
        }

        public void CallShotStart()
        {
            ClearAlreadyExecutedOppositeActionEffects();
            ClearAlreadyExecutedActionEffects();

            if (Combat.Attacker.ShipId == this.ShipId)
            {
                if (OnShotStartAsAttacker != null) OnShotStartAsAttacker();
            }
            else if (Combat.Defender.ShipId == this.ShipId)
            {
                if (OnShotStartAsDefender != null) OnShotStartAsDefender();
            }
        }

        public void CallCheckCancelCritsFirst()
        {
            if (OnCheckCancelCritsFirst != null) OnCheckCancelCritsFirst(this);
        }

        public void CallDefenceStart()
        {
            ClearAlreadyExecutedOppositeActionEffects();
            ClearAlreadyExecutedActionEffects();
            if (OnDefence != null) OnDefence();
        }

        public void CallShotHitAsAttacker()
        {
            if (OnShotHitAsAttacker != null) OnShotHitAsAttacker();
        }

        public void CallShotHitAsDefender()
        {
            if (OnShotHitAsDefenderGlobal != null) OnShotHitAsDefenderGlobal();

            if (OnShotHitAsDefender != null) OnShotHitAsDefender();
        }

        public void CallTryDamagePrevention(Action callBack)
        {
            if (OnTryDamagePreventionGlobal != null) OnTryDamagePreventionGlobal();

            Triggers.ResolveTriggers(TriggerTypes.OnTryDamagePrevention, callBack);
        }

        public void CallOnAttackHitAsAttacker()
        {
            if (OnAttackHitAsAttacker != null) OnAttackHitAsAttacker();
        }

        public void CallOnAttackHitAsDefender()
        {
            if (OnAttackHitAsDefenderGlobal != null) OnAttackHitAsDefenderGlobal();

            if (OnAttackHitAsDefender != null) OnAttackHitAsDefender();
        }

        public void CallOnAttackMissedAsAttacker()
        {
            if (OnAttackMissedAsAttacker != null) OnAttackMissedAsAttacker();
        }

        public void CallOnAttackMissedAsDefender()
        {
            if (OnAttackMissedAsDefender != null) OnAttackMissedAsDefender();
        }

        public void CallAfterAttackWindow()
        {
            if (AfterAttackWindow != null) AfterAttackWindow(this);
        }

        public void CallCheckSecondAttack(Action callBack)
        {
            if (OnCheckSecondAttack != null) OnCheckSecondAttack(this);

            Triggers.ResolveTriggers(TriggerTypes.OnCheckSecondAttack, callBack);
        }

        public void CallAttackFinish()
        {
            if (OnAttackFinish != null) OnAttackFinish(this);
        }

        public void CallOnImmediatelyAfterRolling(DiceRoll diceroll, Action callBack)
        {
            if (OnImmediatelyAfterRolling != null) OnImmediatelyAfterRolling(diceroll);

            Triggers.ResolveTriggers(TriggerTypes.OnImmediatelyAfterRolling, callBack);
        }

        public void CallOnAtLeastOneCritWasCancelledByDefender()
        {
            if (OnAtLeastOneCritWasCancelledByDefender != null) OnAtLeastOneCritWasCancelledByDefender();
        }

        public void CallOnGenerateAvailableAttackPaymentList(List<Tokens.GenericToken> tokens)
        {
            if (OnGenerateAvailableAttackPaymentList != null) OnGenerateAvailableAttackPaymentList(tokens);
        }

        public void CallOnDamageCardIsDealt(Action callBack)
        {
            if (OnDamageCardIsDealt != null) OnDamageCardIsDealt(this);

            Triggers.ResolveTriggers(TriggerTypes.OnDamageCardIsDealt, callBack);
        }

        // DICE

        public int GetNumberOfAttackDice(GenericShip targetShip)
        {
            int result = 0;

            result = Combat.ChosenWeapon.AttackValue;

            if (AfterGotNumberOfAttackDice != null) AfterGotNumberOfAttackDice(ref result);

            if (result < 0) result = 0;
            return result;
        }

        public void CallAfterGotNumberOfPrimaryWeaponAttackDice(ref int result)
        {
            if (AfterGotNumberOfPrimaryWeaponAttackDice != null) AfterGotNumberOfPrimaryWeaponAttackDice(ref result);
        }

        public int GetNumberOfDefenceDice(GenericShip attackerShip)
        {
            int result = Agility;

            if (AfterGotNumberOfDefenceDice != null) AfterGotNumberOfDefenceDice(ref result);

            if (Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass))
            {
                if (AfterGotNumberOfPrimaryWeaponDefenceDice != null) AfterGotNumberOfPrimaryWeaponDefenceDice(ref result);
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
                CriticalHitsDeck.GetCritCard(isCritical, delegate { SufferChosenCriticalHitCard(e); });
            }
            else
            {
                CriticalHitsDeck.GetCritCard(isCritical, delegate { CallOnDamageCardIsDealt(DealRegularDamageCard); });
            }
        }

        public void SufferChosenCriticalHitCard(EventArgs e)
        {
            if (DebugManager.DebugDamage) Debug.Log("+++ Crit: " + Combat.CurrentCriticalHitCard.Name);

            if (OnFaceupCritCardReadyToBeDealt != null) OnFaceupCritCardReadyToBeDealt(this, Combat.CurrentCriticalHitCard);

            if (OnFaceupCritCardReadyToBeDealtGlobal != null) OnFaceupCritCardReadyToBeDealtGlobal(this, Combat.CurrentCriticalHitCard, e);

            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Information about faceup damage card",
                TriggerOwner = this.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnFaceupCritCardReadyToBeDealtUI,
                EventHandler = delegate { InformCrit.LoadAndShow(); }
            });

            Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardReadyToBeDealt, SufferFaceupDamageCard);
        }

        private void DealRegularDamageCard()
        {
            if (Combat.CurrentCriticalHitCard != null)
            {
                AssignedDamageCards.Add(Combat.CurrentCriticalHitCard);
                DecreaseHullValue(Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SufferFaceupDamageCard()
        {
            Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardReadyToBeDealtUI, SufferFaceupDamageCardPart2);
        }

        private void SufferFaceupDamageCardPart2()
        {
            if (OnAssignCrit != null) OnAssignCrit(this, Combat.CurrentCriticalHitCard);

            if (Combat.CurrentCriticalHitCard != null)
            {
                CallOnDamageCardIsDealt(DealFaceupDamageCard);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void DealFaceupDamageCard()
        {
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

        public void LoseShield()
        {
            Shields--;
            CallAfterAssignedDamageIsChanged();
        }

        public virtual void IsHullDestroyedCheck(Action callBack)
        {
            if (Hull == 0 && !IsDestroyed)
            {
                if (OnReadyToBeDestroyed != null) OnReadyToBeDestroyed(this);

                if (!PreventDestruction)
                {
                    DestroyShip(callBack);
                }
                else
                {
                    callBack();
                }
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
            foreach (var pilotAbility in PilotAbilities)
            {
                pilotAbility.DeactivateAbility();
            }

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

        public bool AreWeaponsDisabled()
        {
            bool result = HasToken(typeof(Tokens.WeaponsDisabledToken));

            if (result == true)
            {
                if (OnWeaponsDisabledCheck != null) OnWeaponsDisabledCheck(ref result);
            }

            return result;
        }

        public bool CanAttackBumpedTarget(GenericShip defender)
        {
            bool result = false;

            if (CanAttackBumpedTargetAlways) result = true;

            if (OnCanAttackBumpedTarget != null) OnCanAttackBumpedTarget(ref result, this, defender);

            if (OnCanAttackBumpedTargetGlobal != null) OnCanAttackBumpedTargetGlobal(ref result, this, defender);

            return result;
        }

    }

}