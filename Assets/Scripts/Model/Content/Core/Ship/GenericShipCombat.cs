using Abilities;
using ActionsList;
using Arcs;
using BoardTools;
using Movement;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    public enum WeaponTypes
    {
        PrimaryWeapon,
        Torpedo,
        Missile,
        Cannon,
        Turret,
        Illicit,
        Talent,
        Force
    }

    public partial class GenericShip
    {
        public List<PrimaryWeaponClass> PrimaryWeapons = new List<PrimaryWeaponClass>();

        public Damage Damage { get; protected set; }

        public DiceRoll AssignedDamageDiceroll = new DiceRoll(DiceKind.Attack, 0, DiceRollCheckType.Virtual);

        public bool IsCannotAttackSecondTime { get; set; }
        public bool CanAttackBumpedTargetAlways { get; set; }
        public bool IgnoressBombDetonationEffect { get; set; }
        public bool AttackIsAlwaysConsideredHit { get; set; }

        // EVENTS

        public event EventHandlerShip OnSystemsPhaseStart;

        public event EventHandlerShip OnActivationPhaseStart;
        public event EventHandlerShip OnActionSubPhaseStart;
        public event EventHandlerShip OnRoundEnd;

        public event EventHandlerBoolStringList OnTryPerformAttack;
        public static event EventHandlerBoolStringList OnTryPerformAttackGlobal;

        public event EventHandlerTokensList OnGenerateAvailableAttackPaymentList;

        public event EventHandlerShipWeaponTypeBool OnModifyWeaponAttackRequirement;
        public static event EventHandlerShipWeaponTypeBool OnModifyWeaponAttackRequirementGlobal;

        public event EventHandler OnAttackStartAsAttacker;
        public static event EventHandler OnAttackStartAsAttackerGlobal;
        public event EventHandler OnAttackStartAsDefender;
        public static event EventHandler OnAttackStartAsDefenderGlobal;

        public event EventHandler OnShotStartAsAttacker;
        public event EventHandler OnShotStartAsDefender;
        public static event EventHandler OnDiceAboutToBeRolled;

        public event EventHandlerShip OnCheckCancelCritsFirst;

        public event EventHandler OnDefenceStartAsAttacker;
        public event EventHandler OnDefenceStartAsDefender;

        public event EventHandler OnAtLeastOneCritWasCancelledByDefender;

        public event EventHandler OnShotHitAsAttacker;
        public event EventHandler OnShotHitAsDefender;
        public static event EventHandler OnShotHitAsDefenderGlobal;

        public static event EventHandlerShipDamage OnTryDamagePreventionGlobal;
        public event EventHandlerShipDamage OnTryDamagePrevention;

        public event EventHandler OnAttackHitAsAttacker;
        public event EventHandler OnAttackHitAsDefender;
        public static event EventHandler OnAttackHitAsDefenderGlobal;
        public event EventHandler OnAttackMissedAsAttacker;
        public event EventHandler OnAttackMissedAsDefender;
        public static event EventHandler OnAttackMissedAsAttackerGlobal;
        public event EventHandler OnShieldLost;

        public event EventHandlerShip OnCombatCheckExtraAttack;

        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponAttackDice;
        public event EventHandlerInt AfterGotNumberOfPrimaryWeaponDefenceDice;
        public event EventHandlerInt AfterGotNumberOfAttackDice;
        public event EventHandlerInt AfterGotNumberOfAttackDiceCap;
        public event EventHandlerInt AfterGotNumberOfDefenceDice;
        public static event EventHandlerInt AfterGotNumberOfDefenceDiceGlobal;
        public event EventHandlerInt AfterGotNumberOfDefenceDiceCap;
        public event EventHandlerInt AfterNumberOfDefenceDiceConfirmed;

        public event EventHandlerShip AfterAssignedDamageIsChanged;

        public event EventHandlerBool OnCheckFaceupCrit;
        public event EventHandlerShipCritArgs OnFaceupCritCardReadyToBeDealt;
        public static event EventHandlerShipCritArgs OnFaceupCritCardReadyToBeDealtGlobal;
        public event EventHandlerShipCritArgs OnAssignCrit;

        public event EventHandlerShipBool OnDamageWasSuccessfullyDealt;
        public event EventHandlerShip OnDamageCardIsDealt;
        public static event EventHandlerShip OnDamageCardIsDealtGlobal;
        public static event EventHandlerShipDamage OnDamageInstanceResolvedGlobal;
        public static event EventHandlerShipBomb OnAfterSufferBombEffect;

        public event EventHandlerShipRefBool OnCheckPreventDestruction;
        public event EventHandlerShipBool OnShipIsDestroyed;
        public static event EventHandlerShipBool OnShipIsDestroyedGlobal;
        public event EventHandlerShip OnShipIsRemoved;
        public static event EventHandlerShip OnShipIsRemovedGlobal;

        public event EventHandler AfterAttackWindow;

        public event EventHandlerShip OnAttackFinish;
        public event EventHandlerShip OnAttackFinishAsAttacker;
        public event EventHandlerShip OnAttackFinishAsDefender;
        public static event EventHandlerShip OnAttackFinishGlobal;

        public event EventHandlerBombDropTemplates OnGetAvailableBombDropTemplates;
        public event EventHandlerBombDropTemplates OnGetAvailableBombDropTemplatesModifications;
        public event EventHandlerBombDropTemplates OnGetAvailableBombLaunchTemplates;
        public event EventHandlerBombDropTemplates OnGetAvailableBombLaunchTemplatesModifications;
        public event EventHandlerBarrelRollTemplates OnGetAvailableBarrelRollTemplates;
        public event EventHandlerDecloakTemplates OnGetAvailableDecloakTemplates;
        public event EventHandlerBoostTemplates OnGetAvailableBoostTemplates;

        public event EventHandlerDiceroll OnImmediatelyAfterRolling;
        public event EventHandlerDiceroll OnImmediatelyAfterReRolling;

        public event EventHandlerBool OnWeaponsDisabledCheck;

        public event EventHandlerBool2Ships OnCanAttackBumpedTarget;
        public static event EventHandlerBool2Ships OnCanAttackBumpedTargetGlobal;

        public static event EventHandlerShipRefBool OnCanAttackWhileLandedOnObstacleGlobal;

        public event EventHandlerShip OnCombatActivation;
        public static event EventHandlerShip OnCombatActivationGlobal;
        public event EventHandlerShip OnCombatDeactivation;

        public event EventHandlerShip OnCheckSufferBombDetonation;

        public event EventHandlerObjArgsBool OnSufferCriticalDamage;
        public event EventHandlerObjArgsBool OnSufferDamageDecidingSeverity;

        public event EventHandlerBool OnTryConfirmDiceResults;

        public event EventHandlerShip OnCombatCompareResults;
        public event EventHandler OnAfterNeutralizeResults;

        public event EventHandler AfterAttackDiceModification;
        public event EventHandlerModifyDice OnTryDiceResultModification;
        public event EventHandlerTrySelectDie OnTrySelectDie;

        public event EventHandler OnBombWillBeDropped;
        public event EventHandler OnBombWasDropped;
        public event EventHandler OnBombWasLaunched;

        public event EventHandelerWeaponRange OnUpdateWeaponRange;
        public static event EventHandelerWeaponRange OnUpdateWeaponRangeGlobal;

        public event EventHandlerShipRefInt OnShotObstructedByMe;

        // TRIGGERS

        public void CallOnActivationPhaseStart()
        {
            if (OnActivationPhaseStart != null) OnActivationPhaseStart(this);
        }

        public void CallOnSystemsPhaseStart()
        {
            if (OnSystemsPhaseStart != null) OnSystemsPhaseStart(this);
        }

        public void CallOnRoundEnd()
        {
            if (OnRoundEnd != null) OnRoundEnd(this);
        }

        public void CallOnActionSubPhaseStart()
        {
            if (OnActionSubPhaseStart != null) OnActionSubPhaseStart(this);
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

        public void CallAfterAttackDiceModification()
        {
            if (AfterAttackDiceModification != null) AfterAttackDiceModification();
        }

        public void CallAttackStart()
        {
            if (Combat.Attacker.ShipId == this.ShipId)
            {
                IsAttackPerformed = true;

                if (OnAttackStartAsAttacker != null) OnAttackStartAsAttacker();
                if (OnAttackStartAsAttackerGlobal != null) OnAttackStartAsAttackerGlobal();
            }
            else if (Combat.Defender.ShipId == this.ShipId)
            {
                if (OnAttackStartAsDefender != null) OnAttackStartAsDefender();
                if (OnAttackStartAsDefenderGlobal != null) OnAttackStartAsDefenderGlobal();
            }
        }

        public void CallDiceAboutToBeRolled(Action callback)
        {
            if (OnDiceAboutToBeRolled != null) OnDiceAboutToBeRolled();

            Triggers.ResolveTriggers(TriggerTypes.OnDiceAboutToBeRolled, callback);
        }

        public void CallShotStart()
        {
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

        public void CallDefenceStartAsAttacker()
        {
            if (OnDefenceStartAsAttacker != null) OnDefenceStartAsAttacker();
        }

        public void CallDefenceStartAsDefender()
        {
            if (OnDefenceStartAsDefender != null) OnDefenceStartAsDefender();
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

        public void CallTryDamagePrevention(DamageSourceEventArgs e, Action callback)
        {
            if (OnTryDamagePreventionGlobal != null) OnTryDamagePreventionGlobal(this, e);
            if (OnTryDamagePrevention != null) OnTryDamagePrevention(this, e);

            Triggers.ResolveTriggers(TriggerTypes.OnTryDamagePrevention, callback);
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
            if (OnAttackMissedAsAttackerGlobal != null) OnAttackMissedAsAttackerGlobal();
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

        public void CallAttackFinishGlobal()
        {
            if (OnAttackFinishGlobal != null) OnAttackFinishGlobal(this);
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

        public void CallOnImmediatelyAfterReRolling(DiceRoll diceroll, Action callBack)
        {
            if (OnImmediatelyAfterReRolling != null) OnImmediatelyAfterReRolling(diceroll);

            Triggers.ResolveTriggers(TriggerTypes.OnImmediatelyAfterReRolling, callBack);
        }

        public void CallOnAtLeastOneCritWasCancelledByDefender()
        {
            if (OnAtLeastOneCritWasCancelledByDefender != null) OnAtLeastOneCritWasCancelledByDefender();
        }

        public Type GetWeaponAttackRequirement(GenericSpecialWeapon weapon, bool isSilent)
        {
            Type tokenTypeAttackRequirement = weapon.WeaponInfo.RequiresToken;

            GenericShip.OnModifyWeaponAttackRequirementGlobal?.Invoke(this, weapon, ref tokenTypeAttackRequirement, isSilent);
            OnModifyWeaponAttackRequirement?.Invoke(this, weapon, ref tokenTypeAttackRequirement, isSilent);

            return tokenTypeAttackRequirement;            
        }

        public void CallOnGenerateAvailableAttackPaymentList(List<GenericToken> tokens)
        {
            OnGenerateAvailableAttackPaymentList?.Invoke(tokens);
        }

        public void CallOnDamageCardIsDealt(Action callBack)
        {
            if (OnDamageCardIsDealt != null) OnDamageCardIsDealt(this);
            if (OnDamageCardIsDealtGlobal != null) OnDamageCardIsDealtGlobal(this);

            Triggers.ResolveTriggers(TriggerTypes.OnDamageCardIsDealt, callBack);
        }

        public void CallOnDamageInstanceResolved(DamageSourceEventArgs dsource, Action callback)
        {
            if (OnDamageInstanceResolvedGlobal != null) OnDamageInstanceResolvedGlobal(this, dsource);

            Triggers.ResolveTriggers(TriggerTypes.OnDamageInstanceResolved, callback);
        }

        public void CallOnShieldIsLost(Action callback)
        {
            if (OnShieldLost != null) OnShieldLost();

            Triggers.ResolveTriggers(TriggerTypes.OnShieldIsLost, callback);
        }

        public void CallCombatCheckExtraAttack(Action callback)
        {
            if (OnCombatCheckExtraAttack != null) OnCombatCheckExtraAttack(this);

            Triggers.ResolveTriggers(TriggerTypes.OnCombatCheckExtraAttack, callback);
        }

        public void CallCombatActivation(Action callback)
        {
            //Messages.ShowInfo("Ship is activated! " + this.ShipId);

            if (OnCombatActivation != null) OnCombatActivation(this);
            if (OnCombatActivationGlobal != null) OnCombatActivationGlobal(this);

            Triggers.ResolveTriggers(TriggerTypes.OnCombatActivation, callback);
        }

        public void CallCombatDeactivation(Action callback)
        {
            //Messages.ShowInfo("Ship is deactivated! " + this.ShipId);

            if (OnCombatDeactivation != null) OnCombatDeactivation(this);

            Triggers.ResolveTriggers(TriggerTypes.OnCombatDeactivation, callback);
        }

        // DICE

        public int GetNumberOfAttackDice(GenericShip targetShip)
        {
            int result = 0;

            result = Combat.ChosenWeapon.WeaponInfo.AttackValue;

            if (AfterGotNumberOfAttackDice != null) AfterGotNumberOfAttackDice(ref result);
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                if (AfterGotNumberOfPrimaryWeaponAttackDice != null) AfterGotNumberOfPrimaryWeaponAttackDice(ref result);
            }


            if (AfterGotNumberOfAttackDiceCap != null) AfterGotNumberOfAttackDiceCap(ref result);

            if (result < 0) result = 0;
            return result;
        }

        public int GetNumberOfDefenceDice(GenericShip attackerShip)
        {
            int result = State.Agility;

            if (AfterGotNumberOfDefenceDice != null) AfterGotNumberOfDefenceDice(ref result);
            if (AfterGotNumberOfDefenceDiceGlobal != null) AfterGotNumberOfDefenceDiceGlobal(ref result);

            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                if (AfterGotNumberOfPrimaryWeaponDefenceDice != null) AfterGotNumberOfPrimaryWeaponDefenceDice(ref result);
            }

            if (AfterGotNumberOfDefenceDiceCap != null) AfterGotNumberOfDefenceDiceCap(ref result);

            if (result < 0) result = 0;

            int temporary = result;
            CallAfterNumberOfDefenceDiceConfirmed(temporary);

            return result;
        }

        public void CallAfterNumberOfDefenceDiceConfirmed(int numDefenceDice)
        {
            if (AfterNumberOfDefenceDiceConfirmed != null) AfterNumberOfDefenceDiceConfirmed(ref numDefenceDice);
        }

        public bool TryDiceResultModification(Die die, GenericAbility.DiceModificationType modType, DieSide newResult, ref bool isAllowed)
        {
            if (OnTryDiceResultModification != null)
            {
                OnTryDiceResultModification(die, modType, newResult, ref isAllowed);
            }
            return isAllowed;
        }

        public bool TrySelectDie(Die die, ref bool isAllowed)
        {
            if (OnTrySelectDie != null) OnTrySelectDie(die, ref isAllowed);
            return isAllowed;
        }

        // REGEN

        public bool TryRegenShields()
        {
            bool result = false;
            if (State.ShieldsCurrent < State.ShieldsMax)
            {
                result = true;
                State.ShieldsCurrent++;
                AnimateShields();
                AfterAssignedDamageIsChanged(this);
            };
            return result;
        }

        // DAMAGE

        public void SufferDamage(object sender, EventArgs e)
        {
            if (DebugManager.DebugDamage) Debug.Log("+++ Source: " + (e as DamageSourceEventArgs).Source);
            if (DebugManager.DebugDamage) Debug.Log("+++ DamageType: " + (e as DamageSourceEventArgs).DamageType);

            bool isCritical = (AssignedDamageDiceroll.Successes > 0 && AssignedDamageDiceroll.RegularSuccesses == 0);

            if (OnSufferDamageDecidingSeverity != null) OnSufferDamageDecidingSeverity(sender, e, ref isCritical);

            if (isCritical)
            {
                bool skipSufferDamage = false;
                if (OnSufferCriticalDamage != null) OnSufferCriticalDamage(sender, e, ref skipSufferDamage);

                if (!skipSufferDamage)
                {
                    SufferDamageByType(sender, e, isCritical);
                }
            }
            else
            {
                SufferDamageByType(sender, e, isCritical);
            }
        }

        private void SufferDamageByType(object sender, EventArgs e, bool isCritical)
        {            
            if (State.ShieldsCurrent > 0)
            {
                SufferShieldDamage(isCritical);
            }
            else
            {
                SufferHullDamage(CheckFaceupCrit(isCritical), e);
            }
        }

        public void SufferHullDamage(bool isFaceup, EventArgs e)
        {
            if (DebugManager.DebugAllDamageIsCrits) isFaceup = true;

            DamageDecks.DrawDamageCard(Owner.PlayerNo, isFaceup, ProcessDrawnDamageCard, e);
        }

        public void ProcessDrawnDamageCard(EventArgs e)
        {
            AssignedDamageDiceroll.CancelHits(1);
            AssignedDamageDiceroll.RemoveAllFailures();

            if (Combat.CurrentCriticalHitCard.IsFaceup)
            {
                if (OnFaceupCritCardReadyToBeDealt != null) OnFaceupCritCardReadyToBeDealt(this, Combat.CurrentCriticalHitCard);

                if (OnFaceupCritCardReadyToBeDealtGlobal != null) OnFaceupCritCardReadyToBeDealtGlobal(this, Combat.CurrentCriticalHitCard, e);

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
                CallOnDamageCardIsDealt(delegate { Damage.DealDrawnCard(Triggers.FinishTrigger); });
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
                CallOnDamageCardIsDealt(delegate { Damage.DealDrawnCard(Triggers.FinishTrigger); });
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        public void CallHullValueIsDecreased(Action callBack)
        {
            CallAfterAssignedDamageIsChanged();

            CallOnDamageWasSuccessfullyDealt(
                Combat.CurrentCriticalHitCard.IsFaceup,
                delegate { IsHullDestroyedCheck(callBack); }
            );
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

        public void SufferShieldDamage(bool isCritical)
        {
            AssignedDamageDiceroll.CancelHits(1);
            AssignedDamageDiceroll.RemoveAllFailures();

            State.ShieldsCurrent--;
            CallAfterAssignedDamageIsChanged();

            CallOnShieldIsLost(
                delegate { CallOnDamageWasSuccessfullyDealt(isCritical, Triggers.FinishTrigger); }
            );
        }

        private void CallOnDamageWasSuccessfullyDealt(bool isCritical, Action callback)
        {
            if (OnDamageWasSuccessfullyDealt != null) OnDamageWasSuccessfullyDealt(this, isCritical);

            Triggers.ResolveTriggers(TriggerTypes.OnDamageWasSuccessfullyDealt, callback);
        }

        public void LoseShield()
        {
            State.ShieldsCurrent--;
            CallAfterAssignedDamageIsChanged();
        }

        public virtual void IsHullDestroyedCheck(Action callBack)
        {
            if (State.HullCurrent == 0 && !IsDestroyed)
            {
                bool preventDestruction = false;

                OnCheckPreventDestruction?.Invoke(this, ref preventDestruction);

                if (!preventDestruction)
                {
                    IsDestroyed = true;

                    PlayDestroyedAnimSound(
                        delegate { CallShipDestruction(
                            delegate { PlanShipRemoval(callBack); },
                            isFled: false);
                        }
                    );
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

        private void CallShipDestruction(Action callback, bool isFled)
        {
            OnShipIsDestroyed?.Invoke(this, isFled);
            OnShipIsDestroyedGlobal?.Invoke(this, isFled);

            Triggers.ResolveTriggers(TriggerTypes.OnShipIsDestroyed, callback);
        }

        private void PlanShipRemoval(Action callback)
        {
            if (IsDestructionDuringCombat())
            {
                Phases.Events.OnEngagementInitiativeChanged += RegisterShipRemovalSimultaneous;
                callback();
            }
            else
            {
                RemoveDestroyedShip(callback);
            }
        }

        private bool IsDestructionDuringCombat()
        {
            return (Phases.CurrentPhase is MainPhases.CombatPhase);
        }

        public void DestroyShipForced(Action callback, bool isFled = false)
        {
            IsDestroyed = true;

            PlayDestroyedAnimSound(
                delegate { CallShipDestruction(
                    delegate { RemoveDestroyedShip(callback); },
                    isFled: isFled
                ); }
            );            
        }

        private void RegisterShipRemovalSimultaneous()
        {
            Phases.Events.OnEngagementInitiativeChanged -= RegisterShipRemovalSimultaneous;

            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Destruction of ship #" + this.ShipId,
                TriggerType = TriggerTypes.OnEngagementInitiativeChanged,
                TriggerOwner = this.Owner.PlayerNo,
                EventHandler = delegate { RemoveDestroyedShip(Triggers.FinishTrigger); }
            });
        }

        private void RemoveDestroyedShip(Action callback)
        {
            OnShipIsRemoved?.Invoke(this);
            OnShipIsRemovedGlobal?.Invoke(this);

            Triggers.ResolveTriggers(TriggerTypes.OnShipIsRemoved, callback);
        }

        public void DeactivateAllAbilities()
        {
            foreach (var shipAbility in ShipAbilities)
            {
                shipAbility.DeactivateAbility();
            }

            foreach (var pilotAbility in PilotAbilities)
            {
                pilotAbility.DeactivateAbility();
            }

            foreach (var upgrade in UpgradeBar.GetUpgradesOnlyFaceup())
            {
                foreach (var upgradeAbility in upgrade.UpgradeAbilities)
                {
                    upgradeAbility.DeactivateAbility();
                }
            }
        }

        public List<ManeuverTemplate> GetAvailableBombDropTemplates(GenericUpgrade upgrade)
        {
            List<ManeuverTemplate> availableTemplates = new List<ManeuverTemplate>();
            availableTemplates.AddRange(upgrade.GetDefaultDropTemplates());

            OnGetAvailableBombDropTemplates?.Invoke(availableTemplates, upgrade);

            OnGetAvailableBombDropTemplatesModifications?.Invoke(availableTemplates, upgrade);

            return availableTemplates;
        }

        public List<ManeuverTemplate> GetAvailableDeviceLaunchTemplates(GenericUpgrade upgrade)
        {
            List<ManeuverTemplate> availableTemplates = new List<ManeuverTemplate>();
            availableTemplates.AddRange(upgrade.GetDefaultLaunchTemplates());

            OnGetAvailableBombLaunchTemplates?.Invoke(availableTemplates, upgrade);

            OnGetAvailableBombLaunchTemplatesModifications?.Invoke(availableTemplates, upgrade);

            return availableTemplates;
        }

        public List<ManeuverTemplate> GetAvailableBarrelRollTemplates()
        {
            List<ManeuverTemplate> availableTemplates = new List<ManeuverTemplate>(ShipBase.BarrelRollTemplatesAvailable);

            OnGetAvailableBarrelRollTemplates?.Invoke(availableTemplates);

            return availableTemplates;
        }

        public List<ManeuverTemplate> GetAvailableDecloakBarrelRollTemplates()
        {
            List<ManeuverTemplate> availableTemplates = new List<ManeuverTemplate>(ShipBase.DecloakBarrelRollTemplatesAvailable);

            OnGetAvailableDecloakTemplates?.Invoke(availableTemplates);

            return availableTemplates;
        }

        public List<BoostMove> GetAvailableBoostTemplates(GenericAction action)
        {
            var availableMoves = new List<BoostMove>
            {
                new BoostMove(ActionsHolder.BoostTemplates.Straight1),
                new BoostMove(ActionsHolder.BoostTemplates.LeftBank1),
                new BoostMove(ActionsHolder.BoostTemplates.RightBank1),
            };

            OnGetAvailableBoostTemplates?.Invoke(availableMoves, action);

            return availableMoves;
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

        public bool CanAttackWhileLandedOnObstacle()
        {
            bool result = false;

            if (OnCanAttackWhileLandedOnObstacleGlobal != null) OnCanAttackWhileLandedOnObstacleGlobal(this, ref result);

            return result;
        }

        public List<IShipWeapon> GetAllWeapons()
        {
            List<IShipWeapon> allWeapons = new List<IShipWeapon>();

            foreach (PrimaryWeaponClass primaryWeapon in PrimaryWeapons)
            {
                allWeapons.Add(primaryWeapon as IShipWeapon);
            }

            foreach (GenericUpgrade upgrade in UpgradeBar.GetSpecialWeaponsActive())
            {
                allWeapons.Add(upgrade as IShipWeapon);
            }

            return allWeapons;
        }

        public void CallCheckSufferBombDetonation(Action callback)
        {
            IgnoressBombDetonationEffect = false;

            if (OnCheckSufferBombDetonation != null) OnCheckSufferBombDetonation(this);
            Triggers.ResolveTriggers(TriggerTypes.OnCheckSufferBombDetonation, callback);
        }

        public bool CallTryConfirmDiceResults()
        {
            bool result = true;

            if (OnTryConfirmDiceResults != null) OnTryConfirmDiceResults(ref result);

            return result;
        }

        public void CallCombatCompareResults()
        {
            if (OnCombatCompareResults != null) OnCombatCompareResults(this);
        }

        public void CallAfterNeutralizeResults(Action callback)
        {
            if (OnAfterNeutralizeResults != null) OnAfterNeutralizeResults();

            Triggers.ResolveTriggers(TriggerTypes.OnAfterNeutralizeResults, callback);
        }

        public void StartBonusAttack(Action callback, Func<GenericShip, IShipWeapon, bool, bool> bonusAttackFilter = null)
        {
            if(IsCannotAttackSecondTime)
            {
                // We should never reach this but just in case.
                Messages.ShowError(PilotInfo.PilotName + ": You have already performed a bonus attack!");
                return;
            }

            IsCannotAttackSecondTime = true;

            Combat.StartSelectAttackTarget(
				this,
				delegate
                {
                    //if bonus attack was skipped, allow bonus attacks again
                    if (IsAttackSkipped) IsCannotAttackSecondTime = false;
                    callback();
                },
				bonusAttackFilter,
                PilotInfo.PilotName,
				"You may perform a bonus attack",
				this
			);
        }

        public void CallDeviceWillBeDropped(Action callback)
        {
            if (OnBombWillBeDropped != null) OnBombWillBeDropped();

            Triggers.ResolveTriggers(TriggerTypes.OnBombWillBeDropped, callback);
        }

        public void CallDeviceWasDropped(Action callback)
        {
            if (OnBombWasDropped != null) OnBombWasDropped();

            Triggers.ResolveTriggers(TriggerTypes.OnBombWasDropped, callback);
        }

        public void CallBombWasLaunched(Action callback)
        {
            if (OnBombWasLaunched != null) OnBombWasLaunched();

            Triggers.ResolveTriggers(TriggerTypes.OnBombWasLaunched, callback);
        }

        public void CallUpdateWeaponRange(IShipWeapon weapon, ref int minRange, ref int maxRange, GenericShip target=null)
        {
            if (OnUpdateWeaponRange != null) OnUpdateWeaponRange(weapon, ref minRange, ref maxRange, target);

            if (OnUpdateWeaponRangeGlobal != null) OnUpdateWeaponRangeGlobal(weapon, ref minRange, ref maxRange, target);
        }

        public void ShowAttackAnimationAndSound()
        {
            GenericSpecialWeapon chosenSecondaryWeapon = Combat.ChosenWeapon as GenericSpecialWeapon;
            if (chosenSecondaryWeapon == null || chosenSecondaryWeapon.HasType(UpgradeType.Cannon) || chosenSecondaryWeapon.HasType(UpgradeType.Illicit))
            { // Primary Weapons, Cannons, and Illicits (HotShotBlaster)
                Sounds.PlayShots(SoundInfo.ShotsName, SoundInfo.ShotsCount);
                AnimatePrimaryWeapon();
            }
            else if (chosenSecondaryWeapon.HasType(UpgradeType.Torpedo) || chosenSecondaryWeapon.HasType(UpgradeType.Missile))
            { // Torpedos and Missiles
                Sounds.PlayShots("Proton-Torpedoes", 1);
                AnimateMunitionsShot();
            }
            else if (chosenSecondaryWeapon.HasType(UpgradeType.Turret))
            { // Turrets
                Sounds.PlayShots(SoundInfo.ShotsName, SoundInfo.ShotsCount);
                AnimateTurretWeapon();
            }
        }

        public void CallAfterSufferBombEffect(GenericBomb bomb, Action callback)
        {
            OnAfterSufferBombEffect?.Invoke(this, bomb);

            Triggers.ResolveTriggers(TriggerTypes.OnAfterSufferBombEffect, callback);
        }

        public void CallShotObstructedByMe(GenericShip attacker, ref int count)
        {
            OnShotObstructedByMe?.Invoke(attacker, ref count);
        }
    }

}
