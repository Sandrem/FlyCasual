using Ship;
using Upgrade;
using System;
using SubPhases;
using UpgradesList;
using Tokens;
using Abilities;
using UnityEngine;

namespace UpgradesList
{
    public class KananJarrus : GenericUpgrade
    {
        public bool IsAbilityUsed;

        public KananJarrus() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Kanan Jarrus";
            Cost = 3;

            isUnique = true;

            AvatarOffset = new Vector2(50, 2);

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

        private void AskKananJarrusAbility(object sender, System.EventArgs e)
        {
            if (ShipToRemoveStress.Tokens.HasToken(typeof(StressToken)))
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
            IsAbilityUsed = true;

            ShipToRemoveStress.Tokens.RemoveToken(
                typeof(StressToken),
                DecisionSubPhase.ConfirmDecision
            );
        }

        private void ResetKananJarrusAbilityFlag()
        {
            IsAbilityUsed = false;
        }

    }
}
