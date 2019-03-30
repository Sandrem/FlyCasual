using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;
using UnityEngine;

namespace CommandsList
{
    public class SetPositionCommand : GenericCommand
    {
        public SetPositionCommand()
        {
            Keyword = "setposition";
            Description =   "Sets position of ship:\n" +
                            "setposition id:<shipId> x:<number> y:<number>";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            int shipId = -1;
            if (parameters.ContainsKey("id")) int.TryParse(parameters["id"], out shipId);

            float positionX = 0;
            if (parameters.ContainsKey("x")) float.TryParse(parameters["x"], out positionX);

            int positionY = 0;
            if (parameters.ContainsKey("y")) int.TryParse(parameters["y"], out positionY);

            if (shipId != -1)
            {
                SetPosition(shipId, positionX, positionY);
            }
            else
            {
                ShowHelp();
            }
        }

        private void SetPosition(int shipId, float positionX, float positionY)
        {
            GenericShip ship = Roster.AllShips.FirstOrDefault(n => n.Key == "ShipId:" + shipId).Value;

            if (ship != null)
            {
                ship.SetPosition(new Vector3(positionX, 0, positionY));
                ShowMessage();
            }
            else
            {
                ShowHelp();
            }
        }

        private void ShowMessage()
        {
            Console.Write("SetPosition command is resolved", LogTypes.Everything, true);
        }
    }
}
