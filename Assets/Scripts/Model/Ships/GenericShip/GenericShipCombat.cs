using DamageDeckCard;
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

        public AssignedDamageCards Damage { get; private set; }

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
        public event EventHandler OnShieldLost;

        public event EventHandler OnCombatCheckExtraAttack;

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
        public event EventHandlerShip OnShipIsDestroyed;

        public event EventHandler AfterAttackWindow;

        public event EventHandlerShip OnAttackFinish;
        public event EventHandlerShip OnAttackFinishAsAttacker;
        public event EventHandlerShip OnAttackFinishAsDefender;

        public event EventHandlerBombDropTemplates OnGetAvailableBombDropTemplates;
        public event EventHandlerBarrelRollTemplates OnGetAvailableBarrelRollTemplates;
        public event EventHandlerBoostTemplates OnGetAvailableBoostTemplates;

        public event EventHandlerDiceroll OnImmediatelyAfterRolling;

        public event EventHandlerBool OnWeaponsDisabledCheck;

        public event EventHandler2Ships OnCanAttackBumpedTarget;
        public static event EventHandler2Ships OnCanAttackBumpedTargetGlobal;

        public event EventHandlerShip OnCombatActivation;
        public static event EventHandlerShip OnCombatActivationGlobal;

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
            if (AfterAttackWindow != null) AfterAttackWindow();
        }

        public void CallAttackFinish()
        {
            if (OnAttackFinish != null) OnAttackFinish(this);
        }

        public void CallAttackFinishAsAttacker()
        {
            if (OnAttackFinishAsAttacker != null) OnAttackFinishAsAttacker(this);
        }

        public void CallAttackFinishAsDefender()
        {
            if (OnAttackFinishAsDefender != null) OnAttackFinishAsDefender(this);
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

        public void CallOnShieldIsLost(Action callback)
        {
            if (OnShieldLost != null) OnShieldLost();

            Triggers.ResolveTriggers(TriggerTypes.OnShieldIsLost, callback);
        }

        public void CallCombatCheckExtraAttack(Action callback)
        {
            if (OnCombatCheckExtraAttack != null) OnCombatCheckExtraAttack();

            Triggers.ResolveTriggers(TriggerTypes.OnCombatCheckExtraAttack, callback);
        }

        public void CallCombatActivation(Action callback)
        {
            Messages.ShowInfo("Ship is activated! " + this.ShipId);

            if (OnCombatActivation != null) OnCombatActivation(this);
            if (OnCombatActivationGlobal != null) OnCombatActivationGlobal(this);

            Triggers.ResolveTriggers(TriggerTypes.OnCombatActivation, callback);
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

            if (result < 0) result = 0;

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

        public void SufferHullDamage(bool isFaceup, EventArgs e)
        {
            AssignedDamageDiceroll.CancelHits(1);

            if (DebugManager.DebugAllDamageIsCrits) isFaceup = true;

            DamageDecks.DrawDamageCard(Owner.PlayerNo, isFaceup, ProcessDrawnDamageCard, e);
        }

        private void ProcessDrawnDamageCard(GenericDamageCard damageCard, EventArgs e)
        {
            Combat.CurrentCriticalHitCard = damageCard;

            if (Combat.CurrentCriticalHitCard.IsFaceup)
            {
                if (OnFaceupCritCardReadyToBeDealt != null) OnFaceupCritCardReadyToBeDealt(this, Combat.CurrentCriticalHitCard);

                if (OnFaceupCritCardReadyToBeDealtGlobal != null) OnFaceupCritCardReadyToBeDealtGlobal(this, Combat.CurrentCriticalHitCard);

                Triggers.RegisterTrigger(new Trigger
                {
                    Name = "Information about faceup damage card",
                    TriggerOwner = this.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnFaceupCritCardReadyToBeDealtUI,
                    EventHandler = InformCrit.LoadAndShow
                });

                Triggers.ResolveTriggers(TriggerTypes.OnFaceupCritCardReadyToBeDealt, SufferFaceupDamageCard);
            }
            else
            {
                CallOnDamageCardIsDealt(Damage.DealDrawnCard);
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
                CallOnDamageCardIsDealt(Damage.DealDrawnCard);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public void CallHullValueIsDecreased(Action callBack)
        {
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

            CallOnShieldIsLost(Triggers.FinishTrigger);
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

            PlayDestroyedAnimSound(delegate { CheckShipModelDestruction(callBack, forced); });
        }

        private void CheckShipModelDestruction(Action callback, bool forced = false)
        {
            if ((Phases.CurrentSubPhase.RequiredPilotSkill == PilotSkill) && (!IsAttackPerformed) && (!forced) && (Phases.CurrentPhase.GetType() == typeof(MainPhases.CombatPhase)))
            {
                Phases.OnCombatSubPhaseRequiredPilotSkillIsChanged += RegisterShipDestruction;
            }
            else
            {
                PerformShipDestruction(callback);
            }
        }

        private void RegisterShipDestruction()
        {
            Phases.OnCombatSubPhaseRequiredPilotSkillIsChanged -= RegisterShipDestruction;

            // TODO: Register trigger to destroy ship on PS change
        }

        private void PerformShipDestruction(Action callback)
        {
            Roster.DestroyShip(this.GetTag());
            foreach (var pilotAbility in PilotAbilities)
            {
                pilotAbility.DeactivateAbility();
                foreach (var upgrade in UpgradeBar.GetInstalledUpgrades())
                {
                    foreach (var upgradeAbility in upgrade.UpgradeAbilities)
                    {
                        upgradeAbility.DeactivateAbility();
                    }
                }
            }

            if (OnShipIsDestroyed != null) OnShipIsDestroyed(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipIsDestroyed, callback);
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

        public List<Actions.BarrelRollTemplates> GetAvailableBarrelRollTemplates()
        {
            List<Actions.BarrelRollTemplates> availableTemplates = new List<Actions.BarrelRollTemplates>() { Actions.BarrelRollTemplates.Straight1 };

            if (OnGetAvailableBarrelRollTemplates != null) OnGetAvailableBarrelRollTemplates(availableTemplates);

            return availableTemplates;
        }

        public List<Actions.BoostTemplates> GetAvailableBoostTemplates()
        {
            List<Actions.BoostTemplates> availableTemplates = new List<Actions.BoostTemplates>
            {
                Actions.BoostTemplates.LeftBank1,
                Actions.BoostTemplates.Straight1,
                Actions.BoostTemplates.RightBank1,
            };

            if (OnGetAvailableBoostTemplates != null)
            {
                OnGetAvailableBoostTemplates(availableTemplates);
            }
            return availableTemplates;
        }

        public bool AreWeaponsDisabled()
        {
            bool result = Tokens.HasToken(typeof(Tokens.WeaponsDisabledToken));

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

        public List<IShipWeapon> GetAllWeapons()
        {
            List<IShipWeapon> allWeapons = new List<IShipWeapon>
            {
                PrimaryWeapon
            };

            foreach (var upgrade in UpgradeBar.GetInstalledUpgrades())
            {
                IShipWeapon secondaryWeapon = upgrade as IShipWeapon;
                if (secondaryWeapon != null) allWeapons.Add(secondaryWeapon);
            }

            return allWeapons;
        }

    }

}