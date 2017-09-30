﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SubPhases
{

    public class SelectShipSubPhase : GenericSubPhase
    {
        protected bool isEnemyAllowed;
        protected bool isFriendlyAllowed;
        protected bool isThisAllowed;
        protected int minRange = 1;
        protected int maxRange = 3;

        protected UnityEngine.Events.UnityAction finishAction;

        protected Ship.GenericShip TargetShip;

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

        public override void Initialize()
        {
            Players.PlayerNo playerNo = Players.PlayerNo.Player1;
            if (isFriendlyAllowed) playerNo = Phases.CurrentPhasePlayer;
            if (isEnemyAllowed) playerNo = Roster.AnotherPlayer(Phases.CurrentPhasePlayer);
            Roster.HighlightShipsFiltered(playerNo, -1, GenerateListOfExceptions());
        }

        private List<Ship.GenericShip> GenerateListOfExceptions()
        {
            List<Ship.GenericShip> exceptShips = new List<Ship.GenericShip>();

            if (Selection.ThisShip != null)
            {
                if (isThisAllowed) exceptShips.Add(Selection.ThisShip);

                foreach (var ship in Roster.AllShips)
                {
                    if ((isFriendlyAllowed && ship.Value.Owner.PlayerNo == Selection.ThisShip.Owner.PlayerNo) || (isEnemyAllowed && ship.Value.Owner.PlayerNo != Selection.ThisShip.Owner.PlayerNo))
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

            if (isFriendlyAllowed)
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
                RevertSubPhase();
            }
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;

            if (isEnemyAllowed)
            {
                TrySelectShipByRange(anotherShip);
            }
            else
            {
                Messages.ShowErrorToHuman("Enemy ship cannot be selected");
                RevertSubPhase();
            }
            return result;
        }

        private void TryToSelectThisShip()
        {
            if (isThisAllowed)
            {
                TargetShip = Selection.ThisShip;
                UI.HideNextButton();
                finishAction.Invoke();
            }
            else
            {
                Messages.ShowErrorToHuman("Another ship should be selected");
                RevertSubPhase();
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
                finishAction.Invoke(); 
            }
            else
            {
                Messages.ShowErrorToHuman("Ship is outside of range");
                RevertSubPhase();
            }
        }

        protected virtual void RevertSubPhase()
        {
            Phases.CurrentSubPhase = PreviousSubPhase;
            Roster.AllShipsHighlightOff();
            Phases.CurrentSubPhase.Resume();
            UpdateHelpInfo();
        }

    }

}
