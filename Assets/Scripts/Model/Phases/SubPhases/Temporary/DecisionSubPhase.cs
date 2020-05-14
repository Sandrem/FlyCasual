using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;
using GameModes;
using GameCommands;
using Actions;

namespace SubPhases
{
    public class Decision
    {
        public string Name { get; private set; }
        public EventHandler Effect { get; private set; }
        public string Tooltip { get; private set; }
        public int Count { get; private set; }
        public ActionColor Color { get; private set; }
        public bool IsCentered { get; private set; }

        public bool HasTooltip
        {
            get { return Tooltip != null; }
        }

        public Decision(string name, EventHandler effect, string tooltip = null, int count = -1, ActionColor color = ActionColor.White, bool isCentered = false)
        {
            Name = name;
            Effect = effect;
            Tooltip = tooltip;
            Count = count;
            Color = color;
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
        public override List<GameCommandTypes> AllowedGameCommandTypes {
            get {
                return new List<GameCommandTypes>() {
                    GameCommandTypes.Decision,
                    GameCommandTypes.PressSkip,
                    GameCommandTypes.PressNext
                };
            }
        }

        private GameObject DecisionPanel;
        private GameObject ButtonsHolder;
        protected List<Decision> decisions = new List<Decision>();
        public string DefaultDecisionName;
        public Players.GenericPlayer DecisionOwner;
        public bool ShowSkipButton;
        public DecisionViewTypes DecisionViewType = DecisionViewTypes.TextButtons;
        public Action OnSkipButtonIsPressed;
        public Action OnSkipButtonIsPressedOverwrite;
        public Action OnNextButtonIsPressed;
        public bool WasDecisionButtonPressed;
        public bool IsForced;
        public bool DecisionWasPreparedAndShown;
        public Vector2 ImagesDamageCardSize = new Vector2(194, 300);

        public override void Start()
        {
            base.Start();

            IsTemporary = true;

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

        public string AddDecision(string name, EventHandler call, string tooltip = null, int count = -1, ActionColor color = ActionColor.White, bool isCentered = false)
        {
            int counter = 2;
            string newName = name;
            while (decisions.Exists(n => n.Name == newName))
            {
                newName = name + " #" + counter++;
            }
            decisions.Add(new Decision(newName, call, tooltip, count, color, isCentered));

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
            decisionName = decisionName.Replace("\"", "\\\"");
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
            DecisionPanel = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/DecisionsPanel"), GameObject.Find("UI/DecisionPanelHolder").transform);
            DecisionPanel.name = "DecisionsPanel";
            ButtonsHolder = DecisionPanel.transform.Find("Center/DecisionsPanel").gameObject;

            if (DescriptionShort != null) ShowDecisionDescription(DescriptionShort, DescriptionLong, ImageSource);

            float defaultWindowHeight = (DescriptionLong != null) ? 190f : 70f;
            float buttonHeight = 70f;

            if (decisions.Count != 0)
            {
                if (DescriptionLong == null) DecisionPanel.transform.Find("DescriptionHolder/Description").GetComponentInChildren<Text>().text = DescriptionShort;

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

                    GameObject button = MonoBehaviour.Instantiate(prefab, ButtonsHolder.transform);
                    SmallCardPanel script = null;

                    switch (DecisionViewType)
                    {
                        case DecisionViewTypes.TextButtons:
                            if (!decision.IsCentered)
                            {
                                float offsetX = (currentColumn == 1) ? 7.5f : 350;

                                button.transform.localPosition = new Vector3(offsetX, -(buttonHeight + 5)* rowsUsed, 0);
                                button.GetComponent<RectTransform>().sizeDelta = new Vector2(335, 67.5f);

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
                                button.transform.localPosition = new Vector3(182.5f, -(buttonHeight + 5) * rowsUsed, 0);
                                button.GetComponent<RectTransform>().sizeDelta = new Vector2(335, 67.5f);

                                rowsUsed++;
                                currentColumn = 1;
                            }

                            button.GetComponentInChildren<Text>().text = decision.Name;

                            switch (decision.Color)
                            {
                                case ActionColor.White:
                                    button.GetComponentInChildren<Text>().color = Color.white;
                                    break;
                                case ActionColor.Red:
                                    button.GetComponentInChildren<Text>().color = Color.red;
                                    break;
                                case ActionColor.Purple:
                                    button.GetComponentInChildren<Text>().color = new Color(128, 0, 128);
                                    break;
                                default:
                                    break;
                            }

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

                            if (currentColumn == 6)
                            {
                                currentColumn = 1;
                                rowsUsed++;
                            }

                            button.transform.localPosition = new Vector3(15 * (currentColumn) + (currentColumn-1) * ImagesDamageCardSize.x, rowsUsed * -ImagesDamageCardSize.y - 15 * rowsUsed, 0);

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

                            currentColumn++;

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
                        DecisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            700f,
                            defaultWindowHeight + rowsUsed * (buttonHeight + 5)
                        );
                        ButtonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            700f,
                            defaultWindowHeight + rowsUsed * (buttonHeight + 5)
                        );
                        break;
                    case DecisionViewTypes.ImagesUpgrade:
                        DecisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            Mathf.Max(700f, decisions.Count * Editions.Edition.Current.UpgradeCardSize.x + (decisions.Count + 1) * 15),
                            defaultWindowHeight + Editions.Edition.Current.UpgradeCardSize.y + 15
                        );
                        ButtonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            decisions.Count * Editions.Edition.Current.UpgradeCardSize.x + (decisions.Count + 1) * 15,
                            defaultWindowHeight + Editions.Edition.Current.UpgradeCardSize.y + 15
                        );
                        break;
                    case DecisionViewTypes.ImagesDamageCard:
                        DecisionPanel.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            Mathf.Max(700f, Mathf.Min(decisions.Count, 5) * ImagesDamageCardSize.x + (Mathf.Min(decisions.Count, 5) + 1) * 15),
                            defaultWindowHeight + ImagesDamageCardSize.y * (rowsUsed + 1) + 10 * (rowsUsed + 1)
                        );
                        ButtonsHolder.GetComponent<RectTransform>().sizeDelta = new Vector3(
                            Mathf.Min(decisions.Count, 5) * ImagesDamageCardSize.x + (Mathf.Min(decisions.Count, 5) + 1) * 15,
                            defaultWindowHeight + ImagesDamageCardSize.y * (rowsUsed + 1) + 15 * (rowsUsed + 1)
                        );
                        break;
                    default:
                        break;
                }

                ButtonsHolder.transform.localPosition = new Vector2(-ButtonsHolder.GetComponent<RectTransform>().sizeDelta.x / 2, -185);

                if (DecisionOwner == null)
                {
                    DecisionOwner = Roster.GetPlayer(Phases.CurrentPhasePlayer);
                }
                RequiredPlayer = DecisionOwner.PlayerNo;
                Roster.HighlightPlayer(RequiredPlayer);

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

        public static void ResetInput()
        {
            (Phases.CurrentSubPhase as DecisionSubPhase).WasDecisionButtonPressed = false;
            (Phases.CurrentSubPhase as DecisionSubPhase).IsReadyForCommands = true;
        }

        public override void Next()
        {
            HideDecisionWindowUI();
            Phases.CurrentSubPhase = PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override void NextButton()
        {
            OnNextButtonIsPressed();
        }

        protected void HideDecisionWindowUI()
        {
            GameObject.Destroy(DecisionPanel);
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
                OnSkipButtonIsPressed?.Invoke();
                ConfirmDecision();
            }
        }

        public void ShowDecisionWindowUI()
        {
            WasDecisionButtonPressed = false;
            foreach (Transform transform in GameObject.Find("UI/DecisionPanelHolder").transform)
            {
                transform.gameObject.SetActive(true);
            }
        }

        public void ShowDecisionDescription(string title, string description, IImageHolder imageSource = null)
        {
            if (title != null)
            {
                DecisionPanel.transform.Find("AbilityName").GetComponent<Text>().text = title;

                if (DescriptionLong != null)
                {
                    DecisionPanel.transform.Find("DescriptionHolder/Description").GetComponent<Text>().text = description;
                    if (imageSource != null)
                    {
                        DecisionPanel.transform.Find("DescriptionHolder/CardImage").GetComponent<SmallCardArt>().Initialize(imageSource);
                    }
                    else
                    {
                        DecisionPanel.transform.Find("DescriptionHolder/CardImage").gameObject.SetActive(false);
                        DecisionPanel.transform.Find("DescriptionHolder/Description").GetComponent<RectTransform>().sizeDelta = new Vector2(680, 100);
                    }
                }
                else
                {
                    DecisionPanel.transform.Find("DescriptionHolder").gameObject.SetActive(false);
                    DecisionPanel.transform.Find("Center").localPosition += new Vector3(0, 120, 0);
                }
            }
        }

    }

}
