using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ship;
using UnityEngine;

namespace CommandsList
{
    public class ShotCamera : GenericCommand
    {
        public ShotCamera()
        {
            Keyword = "shotcamera";
            Description =   "Moves camera to show ship's shot:\n" +
                            "shotcamera id:<shipId> target:<shipId>";

            Console.AddAvailableCommand(this);
        }

        public override void Execute(Dictionary<string, string> parameters)
        {
            int shipId = -1;
            if (parameters.ContainsKey("id")) int.TryParse(parameters["id"], out shipId);

            int targetId = -1;
            if (parameters.ContainsKey("target")) int.TryParse(parameters["target"], out targetId);

            bool isRestore = false;
            if (parameters.ContainsKey("restore")) isRestore = true;

            if (shipId != -1 && targetId != -1)
            {
                SetShotCamera(shipId, targetId);
            }
            else if (isRestore)
            {
                RestoreCamera();
            }
            else
            {
                ShowHelp();
            }
        }

        private void SetShotCamera(int shipId, int targetId)
        {
            GenericShip attacker = Roster.AllShips.FirstOrDefault(n => n.Key == "ShipId:" + shipId).Value;
            GenericShip defender = Roster.AllShips.FirstOrDefault(n => n.Key == "ShipId:" + targetId).Value;

            if (attacker != null && defender != null)
            {
                ShowShotCamera(attacker, defender);
                ShowMessage();
            }
            else
            {
                ShowHelp();
            }
        }

        public static void ShowShotCamera(GenericShip attacker, GenericShip defender)
        {
            Vector3 position = attacker.GetCenter();
            Transform directionTransform = defender.GetModelTransform();
            Vector3 direction = defender.GetCenter();

            float shiftBackSize = (attacker.ShipBase.Size == BaseSize.Large) ? 1f : 0.75f;
            float shiftRightSize = (attacker.ShipBase.Size == BaseSize.Large) ? 1f : 0.75f;
            float shiftUpSize = (attacker.ShipBase.Size == BaseSize.Large) ? 1.5f : 1f;

            Vector3 vectorToTarget = (direction - position).normalized;
            Vector3 vectorLeft = new Vector3(vectorToTarget.z, 0, -vectorToTarget.x);

            Vector3 shift = vectorToTarget * -1 * shiftBackSize
                + vectorLeft * shiftRightSize
                + new Vector3(0, shiftUpSize, 0);
            CameraScript.AnimateChangePosition(position + shift, directionTransform);
        }

        private void RestoreCamera()
        {
            CameraScript.RestoreCamera();
        }

        private void ShowMessage()
        {
            Console.Write("ShotCamera command is resolved", LogTypes.Everything, true);
        }
    }
}
