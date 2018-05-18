using Ship;
using Upgrade;
using System;
using SubPhases;
using UpgradesList;
using Tokens;
using Abilities;

namespace UpgradesList
{
    public class InspiringRecruit : GenericUpgrade
    {
        public bool IsAbilityUsed;

        public InspiringRecruit() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Inspiring Recruit";
            Cost = 1;
            
            UpgradeAbilities.Add(new InspiringRecruitAbility());
        }
    }
}

namespace Abilities
{
    public class InspiringRecruitAbility : GenericAbility
    {
        private GenericShip ShipToRemoveStress;

        public override void ActivateAbility()
        {
            GenericShip.OnTokenIsRemovedGlobal += CheckAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTokenIsRemovedGlobal -= CheckAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckAbility(GenericShip ship, Type tokenType)
        {
            if (IsAbilityUsed) return;
            if (tokenType != typeof(StressToken)) return;
            if (ship.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, ship);
            if (distanceInfo.Range < 3)
            {
                ShipToRemoveStress = ship;
                RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, AskInspiringRecruitAbility);
            }
        }

        private void AskInspiringRecruitAbility(object sender, System.EventArgs e)
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

    }
}
