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
        public bool IsRed { get; private set; }

        public bool HasTooltip
        {
            get { return Tooltip != null; }
        }

        public Decision(string name, EventHandler effect, string tooltip = null, int count = -1, bool isRed = false)
        {
            Name = name;
            Effect = effect;
            Tooltip = tooltip;
            Count = count;
            IsRed = isRed;
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
        ImagesUpgrade,
        ImagesDamageCard
    }

    public class DecisionSubPhase : GenericSubPhase
    {
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.Decision, GameCommandTypes.PressSkip }; } }

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
        public bool IsForced;
        public bool DecisionWasPreparedAndShown;
        public Vector2 ImagesDamageCardSize = new Vector2(194, 300);

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

        public virtual void StartIsFinished()
        {
            Initialize();
            UpdateHelpInfo();

            if (!DecisionWasPreparedAndShown)
            {
                DecisionWasPreparedAndShown = true;

                IsReadyForCommands = true;
                GameMode.CurrentGameMode.FinishSyncDecisionPreparation();
            }
        }

        public string AddDecision(string name, EventHandler call, string tooltip = null, int count = -1, bool isRed = false)
        {
            int counter = 2;
            string newName = name;
            while (decisions.Exists(n => n.Name == newName))
            {
                newName = name + " #" + counter++;
            }
            decisions.Add(new Decision(newName, call, tooltip, count, isRed));

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

        public static void SendDecisionCommand(string decisionName)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("name", decisionName);
            GameController.SendCommand(
                GameCommandTypes.Decision,
                Phases.CurrentSubPhase.GetType(),
                parameters.ToString()
            );
        }

        public static void ExecuteDecision(string decisionName)
        {
            Phases.CurrentSubPhase.IsReadyForCommands = false;

            Decision decision = (Phases.CurrentSubPhase as DecisionSubPhase).GetDecisions().FirstOrDefault(n => n.Name == decisionName);

            if (decision == null)
            {
                Console.Write("Cannot find decision name: " + decisionName, LogTypes.Errors, true, "red");

                string alldecisions = null;
                foreach (var singleDecision in (Phases.CurrentSubPhase as DecisionSubPhase).GetDecisions())
                {
                    alldecisions += singleDecision.Name + " ";
                }
                Console.Write("Available decisions: " + alldecisions, LogTypes.Errors, true, "red");
            }

            decision.ExecuteDecision();
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
                    case DecisionViewTypes.ImagesUpgrade:
                        decisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            Mathf.Max(395, decisions.Count * RuleSets.RuleSet.Instance.UpgradeCardSize.x + (decisions.Count + 1) * 10),
                            defaultWindowHeight + RuleSets.RuleSet.Instance.UpgradeCardSize.y + 10
                        );
                        buttonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            decisions.Count * RuleSets.RuleSet.Instance.UpgradeCardSize.x + (decisions.Count + 1) * 10,
                            defaultWindowHeight + RuleSets.RuleSet.Instance.UpgradeCardSize.y + 10
                        );
                        break;
                    case DecisionViewTypes.ImagesDamageCard:
                        decisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            Mathf.Max(395, decisions.Count * ImagesDamageCardSize.x + (decisions.Count + 1) * 10),
                            defaultWindowHeight + ImagesDamageCardSize.y + 10
                        );
                        buttonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            decisions.Count * ImagesDamageCardSize.x + (decisions.Count + 1) * 10,
                            defaultWindowHeight + ImagesDamageCardSize.y + 10
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
                        case DecisionViewTypes.ImagesUpgrade:
                        case DecisionViewTypes.ImagesDamageCard:
                            prefab = (GameObject)Resources.Load("Prefabs/SquadBuilder/SmallCardPanel", typeof(GameObject));
                            break;
                        default:
                            break;
                    }

                    GameObject button = MonoBehaviour.Instantiate(prefab, buttonsHolder.transform);
                    SmallCardPanel script = null;

                    switch (DecisionViewType)
                    {
                        case DecisionViewTypes.TextButtons:
                            button.transform.localPosition = new Vector3((i % 2 == 0) ? 5 : 200, -buttonHeight * (i / 2), 0);

                            button.GetComponentInChildren<Text>().text = decision.Name;
                            button.GetComponentInChildren<Text>().color = (decision.IsRed) ? Color.red : Color.white;

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
                        case DecisionViewTypes.ImagesUpgrade:
                            button.transform.localPosition = new Vector3(10*(i+1) + i* RuleSets.RuleSet.Instance.UpgradeCardSize.x, 0, 0);

                            script = button.GetComponent<SmallCardPanel>();
                            script.Initialize(
                                decision.Name,
                                decision.Tooltip,
                                delegate { GameMode.CurrentGameMode.TakeDecision(decision, button); },
                                DecisionViewTypes.ImagesUpgrade,
                                decision.Count
                            );

                            break;
                        case DecisionViewTypes.ImagesDamageCard:
                            button.transform.localPosition = new Vector3(10 * (i + 1) + i * ImagesDamageCardSize.x, 0, 0);

                            script = button.GetComponent<SmallCardPanel>();
                            script.Initialize(
                                decision.Name,
                                decision.Tooltip,
                                delegate { GameMode.CurrentGameMode.TakeDecision(decision, button); },
                                DecisionViewTypes.ImagesDamageCard,
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

        public override void DoDefault()
        {
            DecisionSubPhase.SendDecisionCommand(DefaultDecisionName);
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
