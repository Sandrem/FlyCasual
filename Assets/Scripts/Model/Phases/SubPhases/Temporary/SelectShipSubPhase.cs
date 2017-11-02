using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameModes;

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

        protected System.Action finishAction;

        public Ship.GenericShip TargetShip;

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
            Players.PlayerNo playerNo = Players.PlayerNo.Player1;
            if (targetsAllowed.Contains(TargetTypes.OtherFriendly)) playerNo = Phases.CurrentPhasePlayer;
            if (targetsAllowed.Contains(TargetTypes.Enemy)) playerNo = Roster.AnotherPlayer(Phases.CurrentPhasePlayer);
            Roster.HighlightShipsFiltered(playerNo, -1, GenerateListOfExceptions());
        }

        private List<Ship.GenericShip> GenerateListOfExceptions()
        {
            List<Ship.GenericShip> exceptShips = new List<Ship.GenericShip>();

            if (Selection.ThisShip != null)
            {
                if (targetsAllowed.Contains(TargetTypes.This)) exceptShips.Add(Selection.ThisShip);

                foreach (var ship in Roster.AllShips)
                {
                    if ((targetsAllowed.Contains(TargetTypes.OtherFriendly) && ship.Value.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo) || (targetsAllowed.Contains(TargetTypes.Enemy) && ship.Value.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo))
                    {
                        Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Selection.ThisShip, ship.Value);
                        if ((distanceInfo.Range < minRange) || (distanceInfo.Range > maxRange))
                        {
                            exceptShips.Add(ship.Value);
                        }
                    }
                }
            }

            return exceptShips;
        }

        public override void Next()
        {
            Roster.AllShipsHighlightOff();
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
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
                        TrySelectShipByRange(ship);
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

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;

            if (Roster.GetPlayer(RequiredPlayer).GetType() != typeof(Players.NetworkOpponentPlayer))
            {
                if (targetsAllowed.Contains(TargetTypes.Enemy))
                {
                    TrySelectShipByRange(anotherShip);
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

        protected virtual void RevertSubPhase()
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

        public static void FinishSelection()
        {
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Phases.CurrentSubPhase.Resume();
            Triggers.FinishTrigger();
        }

    }

}
