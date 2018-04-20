using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using GameModes;

namespace SubPhases
{
    public class Decision
    {
        public string Name { get; private set; }
        public EventHandler Effect { get; private set; }
        public string Tooltip { get; private set; }
        public int Count { get; private set; }

        public bool HasTooltip
        {
            get { return Tooltip != null; }
        }

        public Decision(string name, EventHandler effect, string tooltip = null, int count = -1)
        {
            Name = name;
            Effect = effect;
            Tooltip = tooltip;
            Count = count;
        }

        public void AddTooltip(string tooltip)
        {
            Tooltip = tooltip;
        }

        public void SetCount(int count)
        {
            Count = count;
        }

        public void ExecuteDecision(object sender = null, EventArgs e = null)
        {
            Effect.Invoke(sender, e);
        }
    }

    public enum DecisionViewTypes
    {
        TextButtons,
        ImageButtons
    }

    public class DecisionSubPhase : GenericSubPhase
    {
        private GameObject decisionPanel;
        private GameObject buttonsHolder;
        public string InfoText;
        private List<Decision> decisions = new List<Decision>();
        public string DefaultDecisionName;
        public Players.GenericPlayer DecisionOwner;
        public bool ShowSkipButton;
        public DecisionViewTypes DecisionViewType = DecisionViewTypes.TextButtons;
        public Action OnSkipButtonIsPressed;
        public bool WasDecisionButtonPressed;

        private const float defaultWindowHeight = 75;
        private const float buttonHeight = 45;

        public override void Start()
        {
            base.Start();

            IsTemporary = true;

            decisionPanel = GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject;
            buttonsHolder = decisionPanel.transform.Find("Center/DecisionsPanel").gameObject;

            GameMode.CurrentGameMode.StartSyncDecisionPreparation();
        }

        public virtual void PrepareDecision(Action callBack)
        {
            callBack();
        }

        public void StartIsFinished()
        {
            Initialize();

            UpdateHelpInfo();
        }

        public string AddDecision(string name, EventHandler call, string tooltip = null, int count = -1)
        {
            int counter = 2;
            string newName = name;
            while (decisions.Exists(n => n.Name == newName))
            {
                newName = name + " #" + counter++;
            }
            decisions.Add(new Decision(newName, call, tooltip, count));

            return newName;
        }

        public string AddTooltip(string name, string imageUrl)
        {
            int counter = 2;
            string newName = name;
            while (decisions.Find(n => n.Name == newName && n.HasTooltip) !=null)
            {
                newName = name + " #" + counter++;
            }
            decisions.Find(n => n.Name == newName).AddTooltip(imageUrl);

            return newName;
        }

        public List<Decision> GetDecisions()
        {
            return decisions;
        }

        public override void Initialize()
        {
            if (decisions.Count != 0)
            {
                decisionPanel.transform.Find("InformationPanel").GetComponentInChildren<Text>().text = InfoText;

                switch (DecisionViewType)
                {
                    case DecisionViewTypes.TextButtons:
                        decisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            395,
                            defaultWindowHeight + ((decisions.Count + 1) / 2) * buttonHeight
                        );
                        buttonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            395,
                            defaultWindowHeight + ((decisions.Count + 1) / 2) * buttonHeight
                        );
                        break;
                    case DecisionViewTypes.ImageButtons:
                        decisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            Mathf.Max(395, decisions.Count * 194 + (decisions.Count + 1) * 10),
                            defaultWindowHeight + 300 + 10
                        );
                        buttonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            decisions.Count * 194 + (decisions.Count + 1) * 10,
                            defaultWindowHeight + 300 + 10
                        );
                        break;
                    default:
                        break;
                }

                buttonsHolder.transform.localPosition = new Vector2(-buttonsHolder.GetComponent<RectTransform>().sizeDelta.x / 2, -70);

                int i = 0;
                foreach (var decision in decisions)
                {
                    GameObject prefab = null;

                    switch (DecisionViewType)
                    {
                        case DecisionViewTypes.TextButtons:
                            prefab = (GameObject)Resources.Load("Prefabs/DecisionButton", typeof(GameObject));
                            break;
                        case DecisionViewTypes.ImageButtons:
                            prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/SmallCardPanel", typeof(GameObject));
                            break;
                        default:
                            break;
                    }

                    GameObject button = MonoBehaviour.Instantiate(prefab, buttonsHolder.transform);

                    switch (DecisionViewType)
                    {
                        case DecisionViewTypes.TextButtons:
                            button.transform.localPosition = new Vector3((i % 2 == 0) ? 5 : 200, -buttonHeight * (i / 2), 0);

                            button.GetComponentInChildren<Text>().text = decision.Name;

                            if (decision.HasTooltip)
                            {
                                Tooltips.AddTooltip(button, decision.Tooltip);
                            }

                            EventTrigger trigger = button.AddComponent<EventTrigger>();
                            EventTrigger.Entry entry = new EventTrigger.Entry();
                            entry.eventID = EventTriggerType.PointerClick;
                            entry.callback.AddListener(
                                (data) => { DecisionButtonWasPressed(decision, button); }
                            );
                            trigger.triggers.Add(entry);

                            break;
                        case DecisionViewTypes.ImageButtons:
                            button.transform.localPosition = new Vector3(10*(i+1) + i*194, 0, 0);

                            SmallCardPanel script = button.GetComponent<SmallCardPanel>();
                            script.Initialize(
                                decision.Name,
                                decision.Tooltip,
                                delegate { GameMode.CurrentGameMode.TakeDecision(decision, button); },
                                decision.Count
                            );

                            break;
                        default:
                            break;
                    }

                    button.name = "Button" + i;
                    i++;
                }

                if (DecisionOwner == null) DecisionOwner = Roster.GetPlayer(Phases.CurrentPhasePlayer);

                if (ShowSkipButton) UI.ShowSkipButton(); else UI.HideSkipButton();

                GameMode.CurrentGameMode.FinishSyncDecisionPreparation();
            }
        }

        private void DecisionButtonWasPressed(Decision decision, GameObject button)
        {
            if (!WasDecisionButtonPressed)
            {
                WasDecisionButtonPressed = true;
                GameMode.CurrentGameMode.TakeDecision(decision, button);
            }
        }

        public override void Pause()
        {
            HideDecisionWindowUI();
        }

        public override void Resume()
        {
            HideDecisionWindowUI();

            base.Resume();

            Phases.CurrentSubPhase = this;
            UpdateHelpInfo();

            GameMode.CurrentGameMode.StartSyncDecisionPreparation();
        }

        public override void Next()
        {
            HideDecisionWindowUI();
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        protected void HideDecisionWindowUI()
        {
            decisions = new List<Decision>();

            if (decisionPanel != null) decisionPanel.gameObject.SetActive(false);

            if (buttonsHolder != null)
            {
                foreach (Transform button in buttonsHolder.transform)
                {
                    MonoBehaviour.Destroy(button.gameObject);
                }
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

        public void ExecuteDecision(string decisionName)
        {
            decisions.Find(n => n.Name == decisionName).ExecuteDecision();
        }

        public override void DoDefault()
        {
            ExecuteDecision(DefaultDecisionName);
        }

        public static void ConfirmDecision()
        {
            Action callBack = Phases.CurrentSubPhase.CallBack;
            ConfirmDecisionNoCallback();
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
            if (OnSkipButtonIsPressed != null) OnSkipButtonIsPressed();
            ConfirmDecision();
        }

        public void ShowDecisionWindowUI()
        {
            WasDecisionButtonPressed = false;
            GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject.SetActive(true);
        }

    }

}
