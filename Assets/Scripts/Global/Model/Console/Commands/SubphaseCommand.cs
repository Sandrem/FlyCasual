﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SubPhases;

namespace CommandsList
{
    public class SubphaseCommand : GenericCommand
    {
        public SubphaseCommand()
        {
            Keyword = "subphase";
            Description =   "Shows current subphase and list of previous subphases\n" +
                            "subphase finish - finish current subphase\n" +
                            "subphase resume - resume current subphase\n" +
                            "subphase back - return to previous subphase";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("finish"))
            {
                FinishCurrentSubphase();
            }
            else if (parameters.ContainsKey("resume"))
            {
                ResumeCurrentSubphase();
            }
            else if (parameters.ContainsKey("back"))
            {
                GoBackSubphase();
            }
            else
            {
                TryShowCurrentSubphase();
            }
        }

        private void TryShowCurrentSubphase()
        {
            if (Phases.CurrentSubPhase != null)
            {
                ShowCurrentSubphase();
            }
            else
            {
                Console.Write("Phases are not initialized yet!", LogTypes.Everything, true, "red");
            }
        }

        private void ShowCurrentSubphase()
        {
            Console.Write("\nCurrent subphase: " + Phases.CurrentSubPhase.GetType().ToString(), LogTypes.Everything, true);

            Console.Write("\nPrevious subphases: " + Phases.CurrentSubPhase.GetType().ToString());
            if (Phases.CurrentSubPhase.PreviousSubPhase != null) ShowPreviousSubphasesRecursive(Phases.CurrentSubPhase, 10);
        }

        private void ShowPreviousSubphasesRecursive(GenericSubPhase subphase, int count)
        {
            if (subphase.PreviousSubPhase != null && count != 0)
            {
                Console.Write(subphase.PreviousSubPhase.GetType().ToString());
                ShowPreviousSubphasesRecursive(subphase.PreviousSubPhase, count--);
            }
            else if (count == 0)
            {
                Console.Write("...");
            }
        }

        private void FinishCurrentSubphase()
        {
            if (Phases.CurrentSubPhase != null)
            {
                Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            }
            else
            {
                Console.Write("Phases are not initialized yet!", LogTypes.Everything, true, "red");
            }
        }

        private void ResumeCurrentSubphase()
        {
            if (Phases.CurrentSubPhase != null)
            {
                Phases.CurrentSubPhase.Resume();
            }
            else
            {
                Console.Write("Phases are not initialized yet!", LogTypes.Everything, true, "red");
            }
        }

        private void GoBackSubphase()
        {
            if (Phases.CurrentSubPhase != null)
            {
                Phases.GoBack();
            }
            else
            {
                Console.Write("Phases are not initialized yet!", LogTypes.Everything, true, "red");
            }
        }
    }
}
