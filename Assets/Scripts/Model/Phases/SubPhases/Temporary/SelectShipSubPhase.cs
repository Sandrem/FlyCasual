using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;
using Ship;
using System;
using System.Linq;
using Players;

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

        public GenericShip TargetShip;

        public override void Start()
        {
            IsTemporary = true;

            Prepare();
            Initialize();

            CanBePaused = true;

            UpdateHelpInfo();
        }

        public override void Prepare()
        {

        }

        public void PrepareByParametersOld(Action selectTargetAction, List<TargetTypes> targetTypes, Vector2 rangeLimits, bool showSkipButton = false)
        {
            targetsAllowed.AddRange(targetTypes);
            minRange = (int) rangeLimits.x;
            maxRange = (int) rangeLimits.y;

            finishAction = selectTargetAction;

            if (showSkipButton) UI.ShowSkipButton();
        }

        public void PrepareByParametersNew(Action selectTargetAction, Func<GenericShip, bool> filterTargets, Func<GenericShip, int> getAiPriority, PlayerNo subphaseOwnerPlayerNo, bool showSkipButton = true)
        {
            FilterTargets = filterTargets;
            GetAiPriority = getAiPriority;
            finishAction = selectTargetAction;
            RequiredPlayer = subphaseOwnerPlayerNo;
            if (showSkipButton) UI.ShowSkipButton();
        }

        public override void Initialize()
        {
            Roster.GetPlayer(RequiredPlayer).SelectShipForAbility();
        }

        public void HighlightShipsToSelect()
        {
            Roster.HighlightShipsFiltered(FilterTargets);
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
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;

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
            FinishSelectionNoCallback();
            Triggers.FinishTrigger();
        }

    }

}
