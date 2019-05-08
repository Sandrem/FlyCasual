using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SubPhases
{
    public class MultiSelectionSubphase : GenericSubPhase
    {
        public Func<GenericShip, bool> Filter { get; set; }
        public int MaxToSelect { get; set; }
        public Action WhenDone { get; set; }

        public string AbilityName;
        public string Description;
        public IImageHolder ImageSource;

        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.SelectShip, GameCommandTypes.PressNext }; } }

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
                ShowSubphaseDescription(AbilityName, Description, ImageSource);
                Roster.HighlightShipsFiltered(Filter);

                IsReadyForCommands = true;
                UI.ShowNextButton();
            }
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            if (mouseKeyIsPressed == 1)
            {
                if (CanBeToggled(ship))
                {
                    Selection.ToggleMultiSelection(ship);
                }
                else
                {
                    Messages.ShowError("Only " + MaxToSelect + " ships can be selected");
                }
            }

            return false;
        }

        public override bool AnotherShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
        {
            if (mouseKeyIsPressed == 1)
            {
                if (CanBeToggled(ship))
                {
                    Selection.ToggleMultiSelection(ship);
                }
            }

            return false;
        }

        private bool CanBeToggled(GenericShip ship)
        {
            if (!Filter(ship))
            {
                Messages.ShowError("This ship cannot be selected");
                return false;
            }

            if (Selection.MultiSelectedShips.Count < MaxToSelect || Selection.MultiSelectedShips.Contains(ship))
            {
                return true;
            }
            else
            {
                Messages.ShowError("Only " + MaxToSelect + " ships can be selected");
                return false;
            }
        }

        public override void NextButton()
        {
            Next();
        }

        public override void Next()
        {
            WhenDone();
            FinishSubPhase();
        }

        private void FinishSubPhase()
        {
            Selection.ClearMultiSelection();

            Roster.AllShipsHighlightOff();
            HideSubphaseDescription();
            UI.HideNextButton();

            Action callback = Phases.CurrentSubPhase.CallBack;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase.Resume();
            callback();
        }
    }
}
