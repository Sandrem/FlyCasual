using Ship;
using Upgrade;
using UnityEngine;
using System;
using Tokens;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class KananJarrus : GenericUpgrade
    {
        public KananJarrus() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kanan Jarrus",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.FirstEdition.KananJarrusCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Rebel, new Vector2(50, 2));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class KananJarrusCrewAbility : GenericAbility
    {
        private GenericShip ShipToRemoveStress;

        public override void ActivateAbility()
        {
            GenericShip.OnMovementFinishGlobal += CheckAbility;
            Phases.Events.OnRoundEnd += ResetKananJarrusAbilityFlag;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnMovementFinishGlobal -= CheckAbility;
            Phases.Events.OnRoundEnd -= ResetKananJarrusAbilityFlag;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (!IsAbilityUsed && ship.Owner.PlayerNo == HostShip.Owner.PlayerNo && ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Normal)
            {
                BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, ship);
                if (distanceInfo.Range < 3)
                {
                    ShipToRemoveStress = ship;
                    RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskKananJarrusAbility);
                }
            }
        }

        private void AskKananJarrusAbility(object sender, System.EventArgs e)
        {
            if (ShipToRemoveStress.Tokens.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    RemoveStress,
                    descriptionLong: "Do you want to remove 1 Stress Token from that ship?",
                    imageHolder: HostUpgrade
                );
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