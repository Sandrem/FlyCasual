using Ship;
using Upgrade;
using System;
using SubPhases;
using UpgradesList;
using Tokens;
using Abilities;

namespace UpgradesList
{
    public class KananJarrus : GenericUpgrade
    {
        public bool IsAbilityUsed;

        public KananJarrus() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Kanan Jarrus";
            Cost = 3;

            isUnique = true;

            UpgradeAbilities.Add(new KananJarrusCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
    }
}

namespace Abilities
{
    public class KananJarrusCrewAbility : GenericAbility
    {
        private GenericShip ShipToRemoveStress;

        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishGlobal += CheckAbility;
            Phases.OnRoundEnd += ResetKananJarrusAbilityFlag;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishGlobal -= CheckAbility;
            Phases.OnRoundEnd -= ResetKananJarrusAbilityFlag;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (!IsAbilityUsed && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo && ship.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.White)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(HostShip, ship);
                if (distanceInfo.Range < 3)
                {
                    ShipToRemoveStress = ship;
                    RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, AskKananJarrusAbility);
                }
            }
        }

        private class KananJarrusAbilityArgs: EventArgs
        {
            public KananJarrus KananJarrusUpgradeCard;
            public GenericShip ShipToRemoveStress;
        }

        private void AskKananJarrusAbility(object sender, System.EventArgs e)
        {
            if (ShipToRemoveStress.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(AlwaysUseByDefault, RemoveStress, null, null, true);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void RemoveStress(object sender, EventArgs e)
        {
            ShipToRemoveStress.RemoveToken(typeof(StressToken));
            IsAbilityUsed = true;

            DecisionSubPhase.ConfirmDecision();
        }

        private void ResetKananJarrusAbilityFlag()
        {
            IsAbilityUsed = false;
        }

    }
}
