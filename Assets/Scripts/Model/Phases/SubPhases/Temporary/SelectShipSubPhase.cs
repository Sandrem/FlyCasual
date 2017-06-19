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
            isTemporary = true;

            Prepare();
            Initialize();

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
            Phases.CurrentSubPhase = PreviousSubPhase;
            Phases.CurrentSubPhase.Next();
            UpdateHelpInfo();
        }

        public override bool ThisShipCanBeSelected(Ship.GenericShip ship)
        {
            bool result = false;
            if (isFriendlyAllowed)
            {
                int range = Actions.GetRange(Selection.ActiveShip, ship);
                if ((range >= minRange) && (range <= maxRange))
                {
                    TargetShip = ship;
                    MovementTemplates.ShowRange(Selection.ActiveShip, TargetShip);
                    finishAction.Invoke();
                    Phases.FinishSubPhase(this.GetType());
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
                int range = Actions.GetRange(Selection.ActiveShip, anotherShip);
                if( (range >= minRange) && (range <= maxRange))
                {
                    TargetShip = anotherShip;
                    MovementTemplates.ShowRange(Selection.ActiveShip, TargetShip);
                    finishAction.Invoke();
                    Phases.FinishSubPhase(this.GetType());
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
            UpdateHelpInfo();
        }

    }

}
