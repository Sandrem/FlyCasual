﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SubPhases
{

    public class DecisionSubPhase : GenericSubPhase
    {
        private GameObject decisionPanel;
        public string InfoText;
        private Dictionary<string, EventHandler> decisions = new Dictionary<string, EventHandler>();
        protected Dictionary<string, string> tooltips = new Dictionary<string, string>();
        public string DefaultDecision;
        protected Players.GenericPlayer DecisionOwner;
        public bool ShowSkipButton;

        private const float defaultWindowHeight = 75;
        private const float buttonHeight = 45;

        public override void Start()
        {
            IsTemporary = true;

            decisionPanel = GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject;

            PrepareDecision(StartIsFinished);
        }

        public virtual void PrepareDecision(Action callBack)
        {
            callBack();
        }

        private void StartIsFinished()
        {
            Initialize();

            UpdateHelpInfo();
        }

        public string AddDecision(string name, EventHandler call)
        {
            int counter = 2;
            string newName = name;
            while (decisions.ContainsKey(newName))
            {
                newName = name + " #" + counter++;
            }
            decisions.Add(newName, call);

            return newName;
        }

        public string AddTooltip(string name, string imageUrl)
        {
            int counter = 2;
            string newName = name;
            while (tooltips.ContainsKey(newName))
            {
                newName = name + " #" + counter++;
            }
            tooltips.Add(newName, imageUrl);

            return newName;
        }

        public Dictionary<string, EventHandler> GetDecisions()
        {
            return decisions;
        }

        public override void Initialize()
        {
            if (decisions.Count != 0)
            {
                decisionPanel.transform.Find("InformationPanel").GetComponentInChildren<Text>().text = InfoText;

                int i = 0;
                foreach (var item in decisions)
                {
                    GameObject prefab = (GameObject)Resources.Load("Prefabs/DecisionButton", typeof(GameObject));
                    GameObject buttonsHolder = decisionPanel.transform.Find("DecisionsPanel").gameObject;
                    GameObject button = MonoBehaviour.Instantiate(prefab, buttonsHolder.transform);
                    button.transform.localPosition = new Vector3((i % 2 == 0) ? 5 : 200, -buttonHeight * (i / 2), 0);
                    button.name = "Button" + i;

                    button.GetComponentInChildren<Text>().text = item.Key;

                    if (tooltips.ContainsKey(item.Key))
                    {
                        Tooltips.AddTooltip(button, tooltips[item.Key]);
                    }

                    EventTrigger trigger = button.AddComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.eventID = EventTriggerType.PointerClick;
                    entry.callback.AddListener(
                        (data) => { GameModes.GameMode.CurrentGameMode.TakeDecision(item, button); }
                    );
                    trigger.triggers.Add(entry);

                    i++;
                }
                decisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(decisionPanel.GetComponent<RectTransform>().sizeDelta.x, defaultWindowHeight + ((i + 1) / 2) * buttonHeight);

                if (DecisionOwner == null) DecisionOwner = Roster.GetPlayer(Phases.CurrentPhasePlayer);

                if (ShowSkipButton) UI.ShowSkipButton();

                DecisionOwner.TakeDecision();
            }
        }

        public override void Pause()
        {
            HidePanel();
        }

        public override void Resume()
        {
            Phases.CurrentSubPhase = this;
            UpdateHelpInfo();
            Initialize();
        }

        public override void Next()
        {
            HidePanel();
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        private void HidePanel()
        {
            decisionPanel.gameObject.SetActive(false);
            foreach (Transform button in decisionPanel.transform.Find("DecisionsPanel"))
            {
                MonoBehaviour.Destroy(button.gameObject);
            }
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

        public override void DoDefault()
        {
            decisions[DefaultDecision].Invoke(null, null);
        }

        public void ExecuteDecision(string decisionName)
        {
            decisions[decisionName].Invoke(null, null);
        }

        public static void ConfirmDecision()
        {
            Tooltips.EndTooltip();
            UI.HideSkipButton();

            Action callBack = Phases.CurrentSubPhase.CallBack;
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Phases.CurrentSubPhase.Resume();
            callBack();
        }

        public static void ConfirmDecisionNoCallback()
        {
            Tooltips.EndTooltip();
            UI.HideSkipButton();

            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Phases.CurrentSubPhase.Resume();
        }

        public override void SkipButton()
        {
            ConfirmDecision();
        }

    }

}
