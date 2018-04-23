using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;
using Ship;
using System;
using System.Linq;
using Players;
using UnityEngine.UI;

namespace SubPhases
{
    public enum TargetTypes
    {
        This,
        OtherFriendly,
        Enemy
    }

    public class SelectShipSubPhase : GenericSubPhase
    {
        protected List<TargetTypes> targetsAllowed = new List<TargetTypes>();
        protected int minRange = 1;
        protected int maxRange = 3;

        public bool CanMeasureRangeBeforeSelection = true;

        protected Action finishAction;
        public Func<GenericShip, bool> FilterTargets;
        public Func<GenericShip, int> GetAiPriority;

        public bool IsInitializationFinished;

        public GenericShip TargetShip;

        public string AbilityName;
        public string Description;
        public string ImageUrl;

        public override void Start()
        {
            IsTemporary = true;

            Prepare();
            Initialize();

            CanBePaused = true;

            UpdateHelpInfo();

            base.Start();
        }

        public override void Prepare()
        {

        }

        public void PrepareByParameters(Action selectTargetAction, Func<GenericShip, bool> filterTargets, Func<GenericShip, int> getAiPriority, PlayerNo subphaseOwnerPlayerNo, bool showSkipButton, string abilityName, string description, string imageUrl = null)
        {
            FilterTargets = filterTargets;
            GetAiPriority = getAiPriority;
            finishAction = selectTargetAction;
            RequiredPlayer = subphaseOwnerPlayerNo;
            if (showSkipButton) UI.ShowSkipButton();
            AbilityName = abilityName;
            Description = description;
            ImageUrl = imageUrl;
        }

        public override void Initialize()
        {
            // If not skipped
            if (Phases.CurrentSubPhase == this) Roster.GetPlayer(RequiredPlayer).SelectShipForAbility();
        }

        public void HighlightShipsToSelect()
        {
            ShowSubphaseDescription();
            Roster.HighlightShipsFiltered(FilterTargets);
            IsInitializationFinished = true;
        }

        private void ShowSubphaseDescription()
        {
            if (AbilityName != null && Roster.GetPlayer(RequiredPlayer).GetType() == typeof(HumanPlayer))
            {
                GameObject subphaseDescriptionGO = GameObject.Find("UI").transform.Find("CurrentSubphaseDescription").gameObject;
                subphaseDescriptionGO.transform.Find("CardImage").GetComponent<SmallCardArt>().Initialize(ImageUrl);
                subphaseDescriptionGO.transform.Find("AbilityName").GetComponent<Text>().text = AbilityName;
                subphaseDescriptionGO.transform.Find("Description").GetComponent<Text>().text = Description;
                subphaseDescriptionGO.SetActive(true);
            }
        }

        protected static void HideSubphaseDescription()
        {
            GameObject subphaseDescriptionGO = GameObject.Find("UI").transform.Find("CurrentSubphaseDescription").gameObject;
            subphaseDescriptionGO.SetActive(false);
        }

        public void AiSelectPrioritizedTarget()
        {
            List<GenericShip> filteredShips = Roster.AllShips.Values.Where(n => FilterTargets(n)).ToList();
            if (filteredShips == null || filteredShips.Count == 0)
            {
                SkipButton();
            }
            else
            {
                GenericShip prioritizedTarget = null;
                int maxPriority = 0;

                foreach (var ship in filteredShips)
                {
                    int calculatedPriority = GetAiPriority(ship);
                    if (calculatedPriority > maxPriority)
                    {
                        maxPriority = calculatedPriority;
                        prioritizedTarget = ship;
                    }
                }

                if (prioritizedTarget != null)
                {
                    AiSelectShipAsTarget(prioritizedTarget);
                }
                else
                {
                    SkipButton();
                }
            }
        }

        public override void Next()
        {
            Roster.AllShipsHighlightOff();
            HideSubphaseDescription();

            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;

            if (!IsInitializationFinished) return result;

            if (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(HumanPlayer))
            {
                if (FilterTargets(ship))
                {
                    if (ship == Selection.ThisShip)
                    {
                        TryToSelectThisShip();
                    }
                    else
                    {
                        if (mouseKeyIsPressed == 1)
                        {
                            SelectShip(ship);
                        }
                        else if (mouseKeyIsPressed == 2)
                        {
                            if (CanMeasureRangeBeforeSelection)
                            {
                                Actions.GetRangeAndShow(Selection.ThisShip, ship);
                            }
                            else
                            {
                                Messages.ShowError("Cannot measure range before selection");
                            }
                        }
                    }
                }
                else
                {
                    Messages.ShowErrorToHuman("This friendly ship cannot be selected");
                    CancelShipSelection();
                }
            }
            return result;
        }

        public override bool AnotherShipCanBeSelected(GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;

            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(NetworkOpponentPlayer))
            {
                if (FilterTargets(anotherShip))
                {
                    if (mouseKeyIsPressed == 1)
                    {
                        SelectShip(anotherShip);
                    }
                    else if (mouseKeyIsPressed == 2)
                    {
                        if (CanMeasureRangeBeforeSelection)
                        {
                            Actions.GetRangeAndShow(Selection.ThisShip, anotherShip);
                        }
                        else
                        {
                            Messages.ShowError("Cannot measure range before selection");
                        }
                    }
                }
                else
                {
                    Messages.ShowErrorToHuman("This enemy ship cannot be selected");
                    CancelShipSelection();
                }
            }
            return result;
        }

        private void AiSelectShipAsTarget(GenericShip ship)
        {
            SelectShip(ship);
        }

        private void TryToSelectThisShip()
        {
            if (FilterTargets(Selection.ThisShip))
            {
                TargetShip = Selection.ThisShip;
                UI.HideNextButton();
                TargetShipIsSelected();
            }
            else
            {
                Messages.ShowErrorToHuman("Another ship should be selected");
                CancelShipSelection();
            }
        }

        private void SelectShip(GenericShip ship)
        {
            TargetShip = ship;
            UI.HideNextButton();
            MovementTemplates.ShowRange(Selection.ThisShip, ship);
            TargetShipIsSelected();
        }

        private void CancelShipSelection()
        {
            GameMode.CurrentGameMode.RevertSubPhase();
        }

        public void CallRevertSubPhase()
        {
            RevertSubPhase();
        }

        public virtual void RevertSubPhase()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Roster.AllShipsHighlightOff();
            Phases.CurrentSubPhase.Resume();
            UpdateHelpInfo();
        }

        private void TargetShipIsSelected()
        {
            if (!Network.IsNetworkGame)
            {
                InvokeFinish();
            }
            else
            {
                Network.SelectTargetShip(TargetShip.ShipId);
            }
            
        }

        public void InvokeFinish()
        {
            finishAction.Invoke();
        }

        public static void FinishSelectionNoCallback()
        {
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Phases.CurrentSubPhase.Resume();
        }

        public static void FinishSelection()
        {
            Action callback = Phases.CurrentSubPhase.CallBack;
            FinishSelectionNoCallback();
            callback();
        }

        public override void Pause()
        {
            base.Pause();

            HideSubphaseDescription();
        }

        public override void Resume()
        {
            base.Resume();

            ShowSubphaseDescription();
        }

    }

}
