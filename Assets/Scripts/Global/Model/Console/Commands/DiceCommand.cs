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
            { "blank",      DieSide.Blank   },
            { "empty",      DieSide.Blank   },
            { "focus",      DieSide.Focus   },
            { "eye",        DieSide.Focus   },
            { "success",    DieSide.Success },
            { "hit",        DieSide.Success },
            { "evade",      DieSide.Success },
            { "crit",       DieSide.Crit    }
        };

        public DiceCommand()
        {
            Keyword = "dice";
            Description =   "Modify dice results in current dice pool\n" +
                            "dice modify old:<side> new:<side> [count:<number>]\n" +
                            "dice add type:<side> [count:<number>]\n" +
                            "where side: blank, empty, focus, eye, success, hit, crit, evade";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            if (parameters.ContainsKey("modify"))
            {
                TryModifyDice(parameters);
            }
            else if (parameters.ContainsKey("add"))
            {
                TryAddDice(parameters);
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
            if (parameters.ContainsKey("count")) int.TryParse(parameters["count"], out count);

            if (oldDieSide != DieSide.Unknown && newDieSide != DieSide.Unknown && count > 0)
            {
                ModifyDice(oldDieSide, newDieSide, count);
            }
            else
            {
                ShowHelp();
            }
        }

        private void TryAddDice(Dictionary<string, string> parameters)
        {
            DieSide dieSideType = DieSide.Unknown;
            if (parameters.ContainsKey("type") && stringToDieSide.ContainsKey(parameters["type"])) dieSideType = stringToDieSide[parameters["type"]];

            int count = 1;
            if (parameters.ContainsKey("count")) int.TryParse(parameters["count"], out count);

            if (dieSideType != DieSide.Unknown && count > 0)
            {
                AddDice(dieSideType, count);
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

        private void AddDice(DieSide newDieSide, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Die newDie = DiceRoll.CurrentDiceRoll.AddDice(newDieSide);
                newDie.ShowWithoutRoll();
            }
            DiceRoll.CurrentDiceRoll.OrganizeDicePositions();
        }
    }
}
