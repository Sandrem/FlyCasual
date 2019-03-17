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
        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.SelectShip, GameCommandTypes.PressSkip }; } }

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
        public IImageHolder ImageSource;

        public bool ShowSkipButton = true;

        public override void Start()
        {
            IsTemporary = true;

            Prepare();
            Initialize();

            CanBePaused = true;

            UpdateHelpInfo();

            base.Start();

            // If not skipped
            if (Phases.CurrentSubPhase == this)
            {
                IsReadyForCommands = true;
                Roster.GetPlayer(RequiredPlayer).SelectShipForAbility();
            }
        }

        public override void Prepare()
        {

        }

        public void PrepareByParameters(Action selectTargetAction, Func<GenericShip, bool> filterTargets, Func<GenericShip, int> getAiPriority, PlayerNo subphaseOwnerPlayerNo, bool showSkipButton, string abilityName, string description, IImageHolder imageSource = null)
        {
            FilterTargets = filterTargets;
            GetAiPriority = getAiPriority;
            finishAction = selectTargetAction;
            RequiredPlayer = subphaseOwnerPlayerNo;
            if (showSkipButton)
            {
                UI.ShowSkipButton();
            }
            else
            {
                UI.HideSkipButton();
            }
            AbilityName = abilityName;
            Description = description;
            ImageSource = imageSource;
        }

        public override void Initialize()
        {

        }

        public void HighlightShipsToSelect()
        {
            ShowSubphaseDescription(AbilityName, Description, ImageSource);
            Roster.HighlightShipsFiltered(FilterTargets);
            IsInitializationFinished = true;
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
            UI.HideSkipButton();

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
                            SendSelectShipCommand(ship);
                        }
                        else if (mouseKeyIsPressed == 2)
                        {
                            if (CanMeasureRangeBeforeSelection)
                            {
                                ActionsHolder.GetRangeAndShow(Selection.ThisShip, ship);
                            }
                            else
                            {
                                Messages.ShowError("You cannot measure range before selecting another ship.");
                            }
                        }
                    }
                }
                else
                {
                    Messages.ShowErrorToHuman("You cannot select this friendly ship.");
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
                if (mouseKeyIsPressed == 1)
                {
                    if (FilterTargets(anotherShip))
                    {
                        SendSelectShipCommand(anotherShip);
                    }
                    else
                    {
                        Messages.ShowErrorToHuman("You cannot select this enemy ship.");
                        CancelShipSelection();
                    }
                }
                else if (mouseKeyIsPressed == 2)
                {
                    if (CanMeasureRangeBeforeSelection)
                    {
                        ActionsHolder.GetRangeAndShow(Selection.ThisShip, anotherShip);
                    }
                    else
                    {
                        Messages.ShowError("You cannot measure range before selecting another ship.");
                    }
                }
            }
            return result;
        }

        private void AiSelectShipAsTarget(GenericShip ship)
        {
            SendSelectShipCommand(ship);
        }

        private void TryToSelectThisShip()
        {
            if (FilterTargets(Selection.ThisShip))
            {
                SendSelectShipCommand(Selection.ThisShip);
            }
            else
            {
                Messages.ShowErrorToHuman("Please select a different ship.");
                CancelShipSelection();
            }
        }

        public static void SendSelectShipCommand(GenericShip ship)
        {
            JSONObject parameters = new JSONObject();
            parameters.AddField("id", ship.ShipId.ToString());
            GameController.SendCommand(
                GameCommandTypes.SelectShip,
                Phases.CurrentSubPhase.GetType(),
                parameters.ToString()
            );
        }

        public static void SelectShip(int shipId)
        {
            GenericShip ship = Roster.GetShipById("ShipId:" + shipId);

            (Phases.CurrentSubPhase as SelectShipSubPhase).IsReadyForCommands = false;

            (Phases.CurrentSubPhase as SelectShipSubPhase).TargetShip = ship;

            UI.HideNextButton();
            if (ship != Selection.ThisShip) MovementTemplates.ShowRange(Selection.ThisShip, ship);

            if (!Network.IsNetworkGame)
            {
                (Phases.CurrentSubPhase as SelectShipSubPhase).InvokeFinish();
            }
            else
            {
                Network.SelectTargetShip(ship.ShipId);
            }
        }

        protected virtual void CancelShipSelection()
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
            HideSubphaseDescription();
            Phases.CurrentSubPhase.Resume();
            UpdateHelpInfo();
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

            ShowSubphaseDescription(AbilityName, Description, ImageSource);
        }

    }

}
