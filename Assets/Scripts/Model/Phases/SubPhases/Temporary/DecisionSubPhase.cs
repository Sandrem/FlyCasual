using System;
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
        protected string infoText;
        protected Dictionary<string, EventHandler> decisions = new Dictionary<string, EventHandler>();
        protected Dictionary<string, string> tooltips = new Dictionary<string, string>();
        protected string defaultDecision;

        public override void Start()
        {
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            isTemporary = true;

            decisionPanel = Game.PrefabsList.PanelDecisions;

            Prepare();
            Initialize();

            UpdateHelpInfo();
        }

        public virtual void Prepare()
        {

        }

        public override void Initialize()
        {
            decisionPanel.transform.Find("InformationPanel").GetComponentInChildren<Text>().text = infoText;

            int i = 0;
            foreach (var item in decisions)
            {

                GameObject prefab = (GameObject)Resources.Load("Prefabs/DecisionButton", typeof(GameObject));
                GameObject buttonsHolder = decisionPanel.transform.Find("DecisionsPanel").gameObject;
                GameObject button = MonoBehaviour.Instantiate(prefab, buttonsHolder.transform);
                button.transform.localPosition = new Vector3((i % 2 == 0) ? 5 : 200, 0, 0);
                button.name = "Button" + i;

                button.GetComponentInChildren<Text>().text = item.Key;

                if (tooltips.ContainsKey(item.Key))
                {
                    Tooltips.AddTooltip(button, tooltips[item.Key]);
                }

                EventTrigger trigger = button.AddComponent<EventTrigger>();
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                entry.callback.AddListener((data) => { item.Value.Invoke(button, null); });
                trigger.triggers.Add(entry);

                i++;
            }

            Roster.GetPlayer(Phases.CurrentPhasePlayer).TakeDecision();  
        }

        public override void Next()
        {
            decisionPanel.gameObject.SetActive(false);
            foreach (Transform button in decisionPanel.transform.Find("DecisionsPanel"))
            {
                MonoBehaviour.Destroy(button.gameObject);
            }
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

        public override void DoDefault()
        {
            decisions[defaultDecision].Invoke(null, null);
        }

    }

}
