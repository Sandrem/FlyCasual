using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Movement
{
    public class Formation
    {
        public string Name { get; protected set; }
        public List<GenericShip> Ships { get; protected set; }
        public string CurrentManeuver { get; protected set; }
        public GenericShip CurrentTarget { get; protected set; }

        private const float FORMATION_SPACING_CM = 2f;

        public static Formation CreateFormation(List<GenericShip> ships, string name = null)
        {
            return new Formation(ships, name);
        }

        protected Formation(List<GenericShip> ships, string name)
        {
            Name = name;
            Ships = ships.OrderBy(ship => ship.PilotSkill).ToList(); //order by ascending pilot skill, so the ships that activates first is in the front of the formation
            ships.ForEach(ship => ship.Formation = this);
            Phases.Events.OnRoundEnd += ResetManeuver;
        }

        private void ResetManeuver()
        {
            CurrentManeuver = null;
            CurrentTarget = null;
        }

        public void Break()
        {
            Ships.ForEach(ship => ship.Formation = null);
            Ships.Clear();
            Phases.Events.OnRoundEnd -= ResetManeuver;
        }

        public void Break(GenericShip ship)
        {
            if (Ships.Count > 2)
            {
                Ships.Remove(ship);
                ship.Formation = null;
            }
            else Break();
        }

        public void SetManeuverAndTarget(string maneuver, GenericShip target)
        {
            if (CurrentManeuver != null || CurrentTarget != null)
                throw new InvalidOperationException("Maneuver/target has already been set for this round!");

            CurrentManeuver = maneuver;
            CurrentTarget = target;
        }

        public void SetFormationPreSetup(int count, int shipCount)
        {
            int shipsPerRow = Ships.Count <= 2 ? Ships.Count : (int)Math.Ceiling(Ships.Count / 2f);

            for (int i = 0; i < Ships.Count; i++) 
            {
                var ship = Ships[Ships.Count - 1 - i]; //place the last ship in a formation first, as per the HotAC rules
                int row = i < shipsPerRow ? 0 : 1;

                float distance = Board.CalculateDistance(shipCount);
                float side = (ship.Owner.PlayerNo == Players.PlayerNo.Player1) ? -1 : 1;

                var x = -Board.SIZE_X / 2 + count * distance - side * -1 * (((i % shipsPerRow) - (shipsPerRow - 1) / 2f) * (ship.ShipBase.SHIPSTAND_SIZE_CM + FORMATION_SPACING_CM));
                var y = side * ((Board.SIZE_Y / 2 + Board.DISTANCE_1) + (ship.ShipBase.SHIPSTAND_SIZE_CM + FORMATION_SPACING_CM) * (1 - row));

                SetStartingPosition(ship, x, y);                
            }
        }

        private void SetStartingPosition(GenericShip ship, float x, float y)
        {
            var position = Board.BoardIntoWorld(new UnityEngine.Vector3(x, 0, y));
            ship.SetPosition(position);
        }
    }
}
