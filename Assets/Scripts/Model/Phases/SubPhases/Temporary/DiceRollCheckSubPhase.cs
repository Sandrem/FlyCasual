using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SubPhases
{

    public class DiceRollCheckSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.ConfirmDiceCheck, GameCommandTypes.SyncDiceRerollSelected, GameCommandTypes.SyncDiceResults }; } }

        public DiceKind DiceKind;
        public int DiceCount;

        public DiceRoll CurrentDiceRoll;
        protected DelegateDiceroll checkResults;

        public Action AfterRoll;


        public override void Start()
        {
            base.Start();

            IsTemporary = true;
            if (AfterRoll == null) AfterRoll = FinishAction;
            checkResults = CheckResults;

            Prepare();
            Initialize();

            UpdateHelpInfo();
        }

        public override void Initialize()
        {
            GameObject.Find("UI").transform.Find("CheckDiceResultsPanel").gameObject.SetActive(true);

            DiceRoll DiceRollCheck;
            DiceRollCheck = new DiceRoll(DiceKind, DiceCount, DiceRollCheckType.Check);
            DiceRollCheck.Roll(SyncDiceResults);
        }

        private void SyncDiceResults(DiceRoll diceroll)
        {
            checkResults(diceroll);
        }

        public void PrepareConfirmation()
        {
            Phases.CurrentSubPhase.IsReadyForCommands = true;

            Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).ConfirmDiceCheck();
        }

        public void ShowConfirmButton()
        {
            ShowDiceRollCheckConfirmButton();
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

            Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).DiceCheckConfirm();

            /*if (!Network.IsNetworkGame)
            {
                Confirm();
            }
            else
            {
                Network.FinishTask();
            }*/
        }

        public void Confirm()
        {
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            AfterRoll.Invoke();
        }

    }

}
