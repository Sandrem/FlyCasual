using BoardTools;
using Players;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Movement
{
    /// <summary>
    /// Class for managing HotAC formations of AI ships
    /// 
    /// The code for breaking formations makes the following assumptions (which are not 100% clear in the HotAC rules):
    /// * Range and direction check is only done at the end of the round, and for all ships in the formation at the same time
    /// * Ships are considered facing the same direction if their facing angle is the same, rounded to nearest whole degree
    ///   (This could maybe be less strict. Some play that within 45 degrees (or even 90) is enough to stay in formation)
    /// </summary>
    public class Formation
    {
        public string Name { get; protected set; }
        public List<GenericShip> Ships { get; protected set; }
        public string CurrentManeuver { get; protected set; }
        public GenericShip CurrentTarget { get; protected set; }
        public GenericPlayer Owner { get { return Ships.First().Owner; } }

        private const float FORMATION_SPACING_CM = 2f;

        public static Formation CreateFormation(List<GenericShip> ships, string name = null)
        {
            return new Formation(ships, name);
        }

        protected Formation(List<GenericShip> ships, string name)
        {
            Name = name;
            Ships = ships.OrderBy(ship => ship.PilotSkill).ToList(); //order by ascending pilot skill, so the ships that activates first is in the front of the formation
            Ships.ForEach(AddToFormation);
            Phases.Events.OnRoundEnd += EndOfRoundCleanup;
        }

        private void AddToFormation(GenericShip ship)
        {
            ship.Formation = this;
            ship.OnSufferDamageConfirmed += Ship_OnSufferDamageConfirmed;
        }

        private void RemoveFromFormation(GenericShip ship)
        {
            ship.Formation = null;
            ship.OnSufferDamageConfirmed -= Ship_OnSufferDamageConfirmed;
        }

        private void Ship_OnSufferDamageConfirmed(GenericShip ship, bool flag, EventArgs e)
        {
            var source = ((e as DamageSourceEventArgs).Source as GenericShip);

            if (source != null && source.Owner != Owner)
            {
                BreakAll();
            }
        }

        private void EndOfRoundCleanup()
        {
            CurrentManeuver = null;
            CurrentTarget = null;

            //if any ship has collided with an enemy, the entire formation breaks
            if (Ships.Any(ship => ship.ShipsBumped.Any(b => b.Owner != ship.Owner)))
            {
                BreakAll();
            }
            else
            {
                //All ships that are not within range 1 of a another ship in formation will break:
                Ships.Where(IsOutOfRange).ToList().ForEach(Break);

                //All ships that are not facing the same direction as the majority will break:
                //1. group ships by facing and order by group size
                var groups = Ships
                    .GroupBy(ship => (int)Math.Round(ship.Model.transform.eulerAngles.y, 0))
                    .OrderByDescending(g => g.Count());

                //1. break all ships that are not in the first (largest) group
                groups.Skip(1).ToList().ForEach(g => g.ToList().ForEach(Break));
            }
        }

        private bool IsOutOfRange(GenericShip ship)
        {
            return (!Ships.Any(s => s != ship && new DistanceInfo(ship, s).Range <= 1));
        }

        public void BreakAll()
        {
            Ships.ForEach(RemoveFromFormation);
            Ships.Clear();
            Phases.Events.OnRoundEnd -= EndOfRoundCleanup;
        }

        public void Break(GenericShip ship)
        {
            if (Ships.Count > 2)
            {
                Ships.Remove(ship);
                RemoveFromFormation(ship);
            }
            else BreakAll();
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
                int row = Ships.Count <= shipsPerRow ? 1 : (i < shipsPerRow ? 0 : 1);

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
