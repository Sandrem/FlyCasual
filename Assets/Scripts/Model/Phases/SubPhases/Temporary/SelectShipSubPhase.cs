using System.Collections;
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
            Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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
            Roster.HighlightShipsFiltered(playerNo, -1);
        }

        public override void Next()
        {
            Roster.AllShipsHighlightOff();
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Next();
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;

            if (isFriendlyAllowed)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Selection.ThisShip, ship);
                int range = distanceInfo.Range;

                if ((range >= minRange) && (range <= maxRange))
                {
                    TargetShip = ship;
                    Game.UI.HideNextButton();
                    MovementTemplates.ShowRange(Selection.ThisShip, ship);
                    finishAction.Invoke();
                    Phases.FinishSubPhase(this.GetType());
                    callBack();
                }
                else
                {
                    Game.UI.ShowError("Ship is outside of range");
                    RevertSubPhase();
                }
            }
            else
            {
                Game.UI.ShowError("Friendly ship cannot be selected");
                RevertSubPhase();
            }
            return result;
        }

        public override bool AnotherShipCanBeSelected(Ship.GenericShip anotherShip)
        {
            bool result = false;

            if (isEnemyAllowed)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Selection.ThisShip, anotherShip);
                int range = distanceInfo.Range;

                if ( (range >= minRange) && (range <= maxRange))
                {
                    Game.UI.HideNextButton();
                    TargetShip = anotherShip;
                    MovementTemplates.ShowRange(Selection.ThisShip, anotherShip);
                    finishAction.Invoke();
                    Phases.FinishSubPhase(this.GetType());
                    callBack();
                }
                else
                {
                    Game.UI.ShowError("Ship is outside of range");
                    RevertSubPhase();
                }
            }
            else
            {
                Game.UI.ShowError("Enemy ship cannot be selected");
                RevertSubPhase();
            }
            return result;
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
