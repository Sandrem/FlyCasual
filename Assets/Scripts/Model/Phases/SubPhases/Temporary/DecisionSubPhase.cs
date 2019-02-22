﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using GameModes;
using GameCommands;

namespace SubPhases
{
    public class Decision
    {
        public string Name { get; private set; }
        public EventHandler Effect { get; private set; }
        public string Tooltip { get; private set; }
        public int Count { get; private set; }
        public bool IsRed { get; private set; }
        public bool IsCentered { get; private set; }

        public bool HasTooltip
        {
            get { return Tooltip != null; }
        }

        public Decision(string name, EventHandler effect, string tooltip = null, int count = -1, bool isRed = false, bool isCentered = false)
        {
            Name = name;
            Effect = effect;
            Tooltip = tooltip;
            Count = count;
            IsRed = isRed;
            IsCentered = isCentered;
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
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.Decision, GameCommandTypes.PressSkip, GameCommandTypes.AssignManeuver }; } }

        private GameObject decisionPanel;
        private GameObject buttonsHolder;
        public string InfoText;
        protected List<Decision> decisions = new List<Decision>();
        public string DefaultDecisionName;
        public Players.GenericPlayer DecisionOwner;
        public bool ShowSkipButton;
        public DecisionViewTypes DecisionViewType = DecisionViewTypes.TextButtons;
        public Action OnSkipButtonIsPressed;
        public Action OnSkipButtonIsPressedOverwrite;
        public bool WasDecisionButtonPressed;
        public bool IsForced;
        public bool DecisionWasPreparedAndShown;
        public Vector2 ImagesDamageCardSize = new Vector2(194, 300);

        private const float defaultWindowHeight = 75*1.5f;
        private const float buttonHeight = 45*1.5f;

        public override void Start()
        {
            base.Start();

            IsTemporary = true;

            decisionPanel = GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject;
            buttonsHolder = decisionPanel.transform.Find("Center/DecisionsPanel").gameObject;

            PrepareDecision(StartIsFinished);
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
                DecisionOwner.TakeDecision();
            }
        }

        public string AddDecision(string name, EventHandler call, string tooltip = null, int count = -1, bool isRed = false, bool isCentered = false)
        {
            int counter = 2;
            string newName = name;
            while (decisions.Exists(n => n.Name == newName))
            {
                newName = name + " #" + counter++;
            }
            decisions.Add(new Decision(newName, call, tooltip, count, isRed, isCentered));

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

        public static GameCommand GenerateDecisionCommand(string decisionName)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("name", decisionName);
            return GameController.GenerateGameCommand(
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

                Decision altDecision = (Phases.CurrentSubPhase as DecisionSubPhase).GetDecisions().FirstOrDefault(n => n.Name.Contains(decisionName));

                if (altDecision != null)
                {
                    decision = altDecision;
                    Console.Write("Similar decision is taken: " + altDecision.Name);
                }
            }

            decision.ExecuteDecision();
        }

        public override void Initialize()
        {
            if (decisions.Count != 0)
            {
                decisionPanel.transform.Find("InformationPanel").GetComponentInChildren<Text>().text = InfoText;

                int i = 0;
                int rowsUsed = 0;
                int currentColumn = 1;

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
                            if (!decision.IsCentered)
                            {
                                float offsetX = (currentColumn == 1) ? 7.5f : 300;
                                
                                button.transform.localPosition = new Vector3(offsetX, -buttonHeight * rowsUsed, 0);

                                if (currentColumn == 1)
                                {
                                    currentColumn++;
                                }
                                else
                                {
                                    currentColumn = 1;
                                    rowsUsed++;
                                }
                            }
                            else
                            {
                                button.transform.localPosition = new Vector3(105 * 1.5f, -buttonHeight * rowsUsed, 0);

                                rowsUsed++;
                                currentColumn = 1;
                            }

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
                            button.transform.localPosition = new Vector3(15*(i+1) + i* Editions.Edition.Current.UpgradeCardSize.x, 0, 0);

                            script = button.GetComponent<SmallCardPanel>();
                            script.Initialize(
                                decision.Name,
                                decision.Tooltip,
                                delegate {
                                    GameCommand command = GenerateDecisionCommand(decision.Name);
                                    GameMode.CurrentGameMode.ExecuteCommand(command);
                                },
                                DecisionViewTypes.ImagesUpgrade,
                                decision.Count
                            );

                            break;
                        case DecisionViewTypes.ImagesDamageCard:
                            button.transform.localPosition = new Vector3(15 * (i + 1) + i * ImagesDamageCardSize.x, 0, 0);

                            script = button.GetComponent<SmallCardPanel>();
                            script.Initialize(
                                decision.Name,
                                decision.Tooltip,
                                delegate {
                                    GameCommand command = GenerateDecisionCommand(decision.Name);
                                    GameMode.CurrentGameMode.ExecuteCommand(command);
                                },
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

                switch (DecisionViewType)
                {
                    case DecisionViewTypes.TextButtons:
                        if (currentColumn == 2) rowsUsed++;
                        decisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            395*1.5f,
                            defaultWindowHeight + rowsUsed * buttonHeight
                        );
                        buttonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            395*1.5f,
                            defaultWindowHeight + rowsUsed * buttonHeight
                        );
                        break;
                    case DecisionViewTypes.ImagesUpgrade:
                        decisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            Mathf.Max(395 * 1.5f, decisions.Count * Editions.Edition.Current.UpgradeCardSize.x + (decisions.Count + 1) * 15),
                            defaultWindowHeight + Editions.Edition.Current.UpgradeCardSize.y + 15
                        );
                        buttonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            decisions.Count * Editions.Edition.Current.UpgradeCardSize.x + (decisions.Count + 1) * 15,
                            defaultWindowHeight + Editions.Edition.Current.UpgradeCardSize.y + 15
                        );
                        break;
                    case DecisionViewTypes.ImagesDamageCard:
                        decisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            Mathf.Max(395 * 1.5f, decisions.Count * ImagesDamageCardSize.x + (decisions.Count + 1) * 15),
                            defaultWindowHeight + ImagesDamageCardSize.y + 10
                        );
                        buttonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            decisions.Count * ImagesDamageCardSize.x + (decisions.Count + 1) * 15,
                            defaultWindowHeight + ImagesDamageCardSize.y + 15
                        );
                        break;
                    default:
                        break;
                }

                buttonsHolder.transform.localPosition = new Vector2(-buttonsHolder.GetComponent<RectTransform>().sizeDelta.x / 2, -105);

                if (DecisionOwner == null) DecisionOwner = Roster.GetPlayer(Phases.CurrentPhasePlayer);

                if (ShowSkipButton) UI.ShowSkipButton(); else UI.HideSkipButton();
            }
        }

        private void DecisionButtonWasPressed(Decision decision, GameObject button)
        {
            if (!WasDecisionButtonPressed)
            {
                WasDecisionButtonPressed = true;

                GameCommand command = GenerateDecisionCommand(decision.Name);
                GameMode.CurrentGameMode.ExecuteCommand(command);
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

            PrepareDecision(StartIsFinished);
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
            GameCommand command = DecisionSubPhase.GenerateDecisionCommand(DefaultDecisionName);
            GameMode.CurrentGameMode.ExecuteCommand(command);
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
            if (OnSkipButtonIsPressedOverwrite != null)
            {
                OnSkipButtonIsPressedOverwrite();
            }
            else
            {
                if (OnSkipButtonIsPressed != null) OnSkipButtonIsPressed();
                ConfirmDecision();
            }
        }

        public void ShowDecisionWindowUI()
        {
            WasDecisionButtonPressed = false;
            GameObject.Find("UI").transform.Find("DecisionsPanel").gameObject.SetActive(true);
        }

    }

}
