using System;
using Ship;

namespace Ship
{
    namespace LambdaShuttle
    {
        public class CaptainKagi : LambdaShuttle
        {
            public CaptainKagi() : base()
            {
                PilotName = "Captain Kagi";
                PilotSkill = 8;
                Cost = 27;
                IsUnique = true;

                PilotAbilities.Add(new Abilities.CaptainKagiAbility());
            }
        }
    }
}


namespace Abilities
{
    public class CaptainKagiAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed += CanPerformTargetLock;
        }

        public override void DeactivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsAllowed -= CanPerformTargetLock;
        }

        public void CanPerformTargetLock(ref bool result, GenericShip attacker, GenericShip defender)
        {
            bool abilityIsActive = false;
            if (defender.ShipId != HostShip.ShipId)
            {
                if (defender.Owner.PlayerNo == HostShip.Owner.PlayerNo)
                {
                    BoardTools.ShipDistanceInfo positionInfo = new BoardTools.ShipDistanceInfo(attacker, HostShip);
                    if (positionInfo.Range >= attacker.TargetLockMinRange && positionInfo.Range <= attacker.TargetLockMaxRange)
                    {
                        abilityIsActive = true;
                    }
                }
            }

            if (abilityIsActive)
            {
                if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                {
                    Messages.ShowErrorToHuman("Captain Kagi: You cannot target lock that ship");
                }
                result = false;
            }
        }

    }
}
