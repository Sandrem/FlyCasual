using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;
using Ship;

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

        protected bool CanMeasureRangeBeforeSelection = true;

        protected System.Action finishAction;

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

        public void PrepareByParameters(System.Action selectTargetAction, List<TargetTypes> targetTypes, Vector2 rangeLimits, bool showSkipButton = false)
        {
            targetsAllowed.AddRange(targetTypes);
            minRange = (int) rangeLimits.x;
            maxRange = (int) rangeLimits.y;

            finishAction = selectTargetAction;

            if (showSkipButton) UI.ShowSkipButton();
        }

        public override void Initialize()
        {
            Roster.HighlightShipsFiltered(FilterShipsToSelect);
        }

        private bool FilterShipsToSelect(GenericShip ship)
        {
            bool result = false;

            // Allow by multiple TargetTypes

            if (targetsAllowed.Contains(TargetTypes.Enemy) && ship.Owner.PlayerNo != RequiredPlayer) result = true;

            if (targetsAllowed.Contains(TargetTypes.This) && ship.ShipId == Selection.ThisShip.ShipId) result = true;

            if (targetsAllowed.Contains(TargetTypes.OtherFriendly) && ship.Owner.PlayerNo == RequiredPlayer && ship.ShipId != Selection.ThisShip.ShipId) result = true;

            // Disallow by extra requirements

            if (CanMeasureRangeBeforeSelection)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Selection.ThisShip, ship);
                if (distanceInfo.Range < minRange) return false;
                if (distanceInfo.Range > maxRange) return false;
            }

            return result;
        }

        public override void Next()
        {
            Roster.AllShipsHighlightOff();
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship, int mouseKeyIsPressed)
        {
            bool result = false;

            if (Roster.GetPlayer(RequiredPlayer).GetType() == typeof(Players.HumanPlayer))
            {
                if (targetsAllowed.Contains(TargetTypes.OtherFriendly))
                {
                    if (ship == Selection.ThisShip)
                    {
                        TryToSelectThisShip();
                    }
                    else
                    {
                        if (mouseKeyIsPressed == 1)
                        {
                            TrySelectShipByRange(ship);
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
                    Messages.ShowErrorToHuman("Friendly ship cannot be selected");
                    CancelShipSelection();
                }
            }
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip, int mouseKeyIsPressed)
        {
            bool result = false;

            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(Players.NetworkOpponentPlayer))
            {
                if (targetsAllowed.Contains(TargetTypes.Enemy))
                {
                    if (mouseKeyIsPressed == 1)
                    {
                        TrySelectShipByRange(anotherShip);
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
                    Messages.ShowErrorToHuman("Enemy ship cannot be selected");
                    CancelShipSelection();
                }
            }
            return result;
        }

        private void TryToSelectThisShip()
        {
            if (targetsAllowed.Contains(TargetTypes.This))
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

        private void TrySelectShipByRange(Ship.GenericShip ship)
        {
            Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Selection.ThisShip, ship);
            int range = distanceInfo.Range;

            if ((range >= minRange) && (range <= maxRange))
            {
                TargetShip = ship;
                UI.HideNextButton();
                MovementTemplates.ShowRange(Selection.ThisShip, ship);
                TargetShipIsSelected();
            }
            else
            {
                Messages.ShowErrorToHuman("Ship is outside of range");
                CancelShipSelection();
            }
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
