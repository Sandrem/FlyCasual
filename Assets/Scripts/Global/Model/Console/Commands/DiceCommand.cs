using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommandsList
{
    public class DiceCommand : GenericCommand
    {
        private Dictionary<string, DieSide> stringToDieSide = new Dictionary<string, DieSide>()
        {
            { "blank", DieSide.Blank },
            { "focus", DieSide.Focus },
            { "success", DieSide.Success },
            { "crit", DieSide.Crit }
        };

        public DiceCommand()
        {
            Keyword = "dice";
            Description =   "Modify dice results in current dice pool\n" +
                            "dice modify old:<side> new:<side> [count:<number>]\n" +
                            "where side: blank, focus, success, crit";
            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("modify"))
            {
                TryModifyDice(parameters);
            }
            else
            {
                ShowHelp();
            }
        }

        private void TryModifyDice(Dictionary<string, string> parameters)
        {
            DieSide oldDieSide = DieSide.Unknown;
            if (parameters.ContainsKey("old") && stringToDieSide.ContainsKey(parameters["old"])) oldDieSide = stringToDieSide[parameters["old"]];

            DieSide newDieSide = DieSide.Unknown;
            if (parameters.ContainsKey("new") && stringToDieSide.ContainsKey(parameters["new"])) newDieSide = stringToDieSide[parameters["new"]];

            int count = 1;
            if (parameters.ContainsKey("count")) int.TryParse(parameters["crits"], out count);

            if (oldDieSide != DieSide.Unknown && newDieSide != DieSide.Unknown && count > 0)
            {
                ModifyDice(oldDieSide, newDieSide, count);
            }
            else
            {
                ShowHelp();
            }
        }

        private void ModifyDice(DieSide oldDieSide, DieSide newDieSide, int count)
        {
            DiceRoll.CurrentDiceRoll.Change(oldDieSide, newDieSide, count);
        }
    }
}
