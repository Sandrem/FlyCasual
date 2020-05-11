using ActionsList;
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
        public Action<Action> WhenDone { get; set; }

        public GenericAction HostAction { get; set; }

        public override List<GameCommandTypes> AllowedGameCommandTypes { get { return new List<GameCommandTypes>() { GameCommandTypes.SelectShip, GameCommandTypes.PressNext, GameCommandTypes.CancelShipSelection }; } }

        public Func<GenericShip, int> GetAiPriority;

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
                ShowSubphaseDescription(DescriptionShort, DescriptionLong, ImageSource);
                UI.ShowNextButton();

                CameraScript.RestoreCamera();

                IsReadyForCommands = true;
                Roster.GetPlayer(RequiredPlayer).SelectShipsForAbility();
            }
        }

        public void AiSelectPrioritizedTarget()
        {
            List<GenericShip> filteredShips = Roster.AllShips.Values.Where(n => Filter(n)).ToList();
            if (filteredShips != null && filteredShips.Count != 0)
            {
                Dictionary<GenericShip, int> prioritizedTargets = new Dictionary<GenericShip, int>();

                // Calculate priority of each valid ship
                foreach (var ship in filteredShips)
                {
                    prioritizedTargets.Add(ship, GetAiPriority(ship));
                }

                // Soft by priority
                prioritizedTargets = prioritizedTargets.OrderByDescending(n => n.Value).ToDictionary(n => n.Key, m => m.Value);

                // Select N with highest priority
                for (int i = 0; i < MaxToSelect; i++)
                {
                    if (prioritizedTargets.Count > 0)
                    {
                        var shipToSelectData = prioritizedTargets.First();
                        // Select only if priority > 0
                        if (shipToSelectData.Value != 0) Selection.ToggleMultiSelection(shipToSelectData.Key);
                        prioritizedTargets.Remove(shipToSelectData.Key);
                    }
                }
            }

            NextButton();
        }

        public override bool ThisShipCanBeSelected(GenericShip ship, int mouseKeyIsPressed)
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
            Selection.HideMultiSelectionHighlight();

            WhenDone(FinishSubPhase);
        }

        private void FinishSubPhase()
        {
            Pause();

            Selection.ClearMultiSelection();

            Action callback = Phases.CurrentSubPhase.CallBack;
            Phases.CurrentSubPhase = Phases.CurrentSubPhase.PreviousSubPhase;
            Phases.CurrentSubPhase.Resume();
            callback();
        }

        public override void Pause()
        {
            Selection.HideMultiSelectionHighlight();

            Roster.AllShipsHighlightOff();
            HideSubphaseDescription();
            UI.HideNextButton();
        }
    }
}
