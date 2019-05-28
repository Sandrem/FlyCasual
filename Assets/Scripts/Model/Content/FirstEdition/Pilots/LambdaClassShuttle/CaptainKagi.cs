using Ship;
using System.Collections.Generic;

namespace Ship
{
    namespace FirstEdition.LambdaClassShuttle
    {
        public class CaptainKagi : LambdaClassShuttle
        {
            public CaptainKagi() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Kagi",
                    8,
                    27,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.CaptainKagiAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class CaptainKagiAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsDisallowed += CanPerformTargetLock;
        }

        public override void DeactivateAbility()
        {
            RulesList.TargetLocksRule.OnCheckTargetLockIsDisallowed -= CanPerformTargetLock;
        }

        public void CanPerformTargetLock(ref bool result, GenericShip attacker, ITargetLockable defender)
        {
            bool abilityIsActive = false;
            if (defender is GenericShip)
            {
                if ((defender as GenericShip).ShipId != HostShip.ShipId)
                {
                    if ((defender as GenericShip).Owner.PlayerNo == HostShip.Owner.PlayerNo)
                    {
                        BoardTools.DistanceInfo positionInfo = new BoardTools.DistanceInfo(attacker, HostShip);
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
}