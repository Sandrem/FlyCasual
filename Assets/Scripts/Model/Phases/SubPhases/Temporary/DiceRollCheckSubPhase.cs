using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubPhases
{

    public class DiceRollCheckSubPhase : GenericSubPhase
    {
        protected DiceKind diceType;
        protected int diceCount;

        protected DiceRoll CurrentDiceRoll;
        protected DelegateDiceroll checkResults;

        protected UnityEngine.Events.UnityAction finishAction;


        public override void Start()
        {
            base.Start();

            IsTemporary = true;
            finishAction = FinishAction;
            checkResults = CheckResults;

            Prepare();
            Initialize();

            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            GameObject.Find("UI").transform.Find("CheckDiceResultsPanel").gameObject.SetActive(true);

            DiceRoll DiceRollCheck;
            DiceRollCheck = new DiceRoll(diceType, diceCount, DiceRollCheckType.Check);
            DiceRollCheck.Roll(SyncDiceResults);
        }

        private void SyncDiceResults(DiceRoll diceroll)
        {
            if (!Network.IsNetworkGame)
            {
                checkResults(diceroll);
            }
            else
            {
                Network.SyncDiceResults();
            }
        }

        public void PrepareConfirmation()
        {
            Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).ConfirmDiceCheck();
        }

        public void ShowConfirmButton()
        {
            if (!Network.IsNetworkGame)
            {
                ShowDiceRollCheckConfirmButton();
            }
            else
            {
                Network.ConfirmDiceRollCheckResults();
            }
        }

        public void ShowDiceRollCheckConfirmButton()
        {
            Button closeButton = GameObject.Find("UI").transform.Find("CheckDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").GetComponent<Button>();
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(PressConfirmButton);

            closeButton.gameObject.SetActive(true);
        }

        public void CalculateDice()
        {
            CheckResults(DiceRoll.CurrentDiceRoll);
        }

        protected virtual void CheckResults(DiceRoll diceRoll)
        {
            CurrentDiceRoll = diceRoll;
            PrepareConfirmation();
        }

        protected virtual void FinishAction()
        {
            HideDiceResultMenu();
            Phases.FinishSubPhase(this.GetType());
        }

        public void HideDiceResultMenu()
        {
            GameObject.Find("UI").transform.Find("CheckDiceResultsPanel").gameObject.SetActive(false);
            HideConfirmDiceButton();
            CurrentDiceRoll.RemoveDiceModels();
        }

        public void HideConfirmDiceButton()
        {
            GameObject.Find("UI").transform.Find("CheckDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").gameObject.SetActive(false);
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

        private void PressConfirmButton()
        {
            HideConfirmDiceButton();
            if (!Network.IsNetworkGame)
            {
                Confirm();
            }
            else
            {
                Network.FinishTask();
            }
        }

        public void Confirm()
        {
            finishAction.Invoke();
        }

    }

}
