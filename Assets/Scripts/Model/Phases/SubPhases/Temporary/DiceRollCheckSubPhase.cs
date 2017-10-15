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
            DiceRollCheck.Roll(checkResults);
        }

        public void PrepareConfirm()
        {
            Roster.GetPlayer(Selection.ActiveShip.Owner.PlayerNo).ConfirmDiceCheck();
        }

        public void ShowConfirmButton()
        {
            Button closeButton = GameObject.Find("UI").transform.Find("CheckDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").GetComponent<Button>();
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(finishAction);

            closeButton.gameObject.SetActive(true);
        }

        protected virtual void CheckResults(DiceRoll diceRoll)
        {
            CurrentDiceRoll = diceRoll;
            PrepareConfirm();
        }

        protected virtual void FinishAction()
        {
            HideDiceResultMenu();
            Phases.FinishSubPhase(this.GetType());
        }

        public void HideDiceResultMenu()
        {
            GameObject.Find("UI").transform.Find("CheckDiceResultsPanel").gameObject.SetActive(false);
            HideDiceModificationButtons();
            CurrentDiceRoll.RemoveDiceModels();
        }

        public void HideDiceModificationButtons()
        {
            foreach (Transform button in GameObject.Find("UI").transform.Find("CheckDiceResultsPanel").Find("DiceModificationsPanel"))
            {
                if (button.name.StartsWith("Button"))
                {
                    MonoBehaviour.Destroy(button.gameObject);
                }
            }
            GameObject.Find("UI").transform.Find("CheckDiceResultsPanel").Find("DiceModificationsPanel").Find("Confirm").gameObject.SetActive(false);
        }

        public override void Next()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;
            return result;
        }

        public void Confirm()
        {
            finishAction.Invoke();
        }

    }

}
