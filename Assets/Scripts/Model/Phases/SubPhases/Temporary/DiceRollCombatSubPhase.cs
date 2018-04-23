using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubPhases
{

    public class DiceRollCombatSubPhase : GenericSubPhase
    {
        protected DiceKind diceType;
        protected int diceCount;

        protected DiceRoll CurentDiceRoll;
        protected DelegateDiceroll checkResults;

        public override void Start()
        {
            base.Start();

            IsTemporary = true;
            CallBack = FinishAction;

            Prepare();
            Initialize();

            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(true);

            if (Combat.AttackStep == CombatStep.Attack)
            {
                ShowAttackAnimationAndSound();
                Combat.Attacker.CallDiceAboutToBeRolled(RollDice);
            }
            else
            {
                Combat.Defender.CallDiceAboutToBeRolled(RollDice);
            }
        }

        private void RollDice()
        {
            DiceRoll DiceRollCombat;
            DiceRollCombat = new DiceRoll(diceType, diceCount, DiceRollCheckType.Combat);
            DiceRollCombat.Roll(SyncDiceResults);
        }

        private void ShowAttackAnimationAndSound()
        {
            Upgrade.GenericSecondaryWeapon chosenSecondaryWeapon = Combat.ChosenWeapon as Upgrade.GenericSecondaryWeapon;
            if (chosenSecondaryWeapon == null || chosenSecondaryWeapon.hasType(Upgrade.UpgradeType.Cannon) || chosenSecondaryWeapon.hasType(Upgrade.UpgradeType.Illicit))
            { // Primary Weapons, Cannons, and Illicits (HotShotBlaster)
                Sounds.PlayShots(Selection.ActiveShip.SoundShotsPath, Selection.ActiveShip.ShotsCount);
                Selection.ThisShip.AnimatePrimaryWeapon();
            }
            else if (chosenSecondaryWeapon.hasType(Upgrade.UpgradeType.Torpedo) || chosenSecondaryWeapon.hasType(Upgrade.UpgradeType.Missile))
            { // Torpedos and Missiles
                Sounds.PlayShots("Proton-Torpedoes", 1);
                Selection.ThisShip.AnimateMunitionsShot();
            }
            else if (chosenSecondaryWeapon.hasType(Upgrade.UpgradeType.Turret))
            { // Turrets
                Sounds.PlayShots(Selection.ActiveShip.SoundShotsPath, Selection.ActiveShip.ShotsCount);
                Selection.ThisShip.AnimateTurretWeapon();
            }
        }

        private void SyncDiceResults(DiceRoll diceroll)
        {
            if (!Network.IsNetworkGame)
            {
                ImmediatelyAfterRolling(diceroll);
            }
            else
            {
                Network.SyncDiceResults();
            }
        }

        public void CalculateDice()
        {
            ImmediatelyAfterRolling(DiceRoll.CurrentDiceRoll);
        }

        private void ImmediatelyAfterRolling(DiceRoll diceroll)
        {
            Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Attacker : Combat.Defender;
            Selection.ActiveShip.CallOnImmediatelyAfterRolling(diceroll, delegate { FinallyCheckResults(diceroll); });
        }

        private void FinallyCheckResults(DiceRoll diceroll)
        {
            checkResults(diceroll);
        }

        public void PrepareToggleConfirmButton(bool isActive)
        {
            Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).ToggleCombatDiceResults(isActive);
        }

        protected virtual void CheckResults(DiceRoll diceRoll)
        {
            CurentDiceRoll = diceRoll;
            Selection.ActiveShip = (Combat.AttackStep == CombatStep.Attack) ? Combat.Defender : Combat.Attacker;
            Selection.ActiveShip.Owner.UseOppositeDiceModifications();
        }

        protected virtual void FinishAction()
        {
            HideDiceResultMenu();
            Phases.FinishSubPhase(this.GetType());
        }

        public void HideDiceResultMenu()
        {
            GameObject.Find("UI/CombatDiceResultsPanel").gameObject.SetActive(false);
            HideDiceModificationButtons();
            CurentDiceRoll.RemoveDiceModels();
        }

        public void HideDiceModificationButtons()
        {
            foreach (Transform button in GameObject.Find("UI/CombatDiceResultsPanel").transform.Find("DiceModificationsPanel"))
            {
                if (button.name.StartsWith("Button"))
                {
                    MonoBehaviour.Destroy(button.gameObject);
                }
            }
            PrepareToggleConfirmButton(false);
        }

        public override void Pause()
        {
            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(false);
        }

        public override void Resume()
        {
            base.Resume();

            GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").gameObject.SetActive(true);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;
            return result;
        }

        public void ToggleConfirmButton(bool isActive)
        {
            Button closeButton = GameObject.Find("UI").transform.Find("CombatDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").GetComponent<Button>();
            if (isActive)
            {
                closeButton.onClick.RemoveAllListeners();
                closeButton.onClick.AddListener(delegate { CallBack(); });
            }
            closeButton.gameObject.SetActive(isActive);
        }

    }

}
