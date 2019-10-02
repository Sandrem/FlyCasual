using Ship;
using Upgrade;
using UnityEngine;
using System;
using Tokens;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class KananJarrus : GenericUpgrade
    {
        public KananJarrus() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Kanan Jarrus",
                UpgradeType.Crew,
                cost: 12,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                abilityType: typeof(Abilities.SecondEdition.KananJarrusCrewAbility),
                addForce: 1,
                seImageNumber: 86
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class KananJarrusCrewAbility : GenericAbility
    {
        private GenericShip ShipToRemoveStress;

        public override void ActivateAbility()
        {
            GenericShip.OnPositionFinishGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnPositionFinishGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.IsBumped || BoardTools.Board.IsOffTheBoard(ship) || !HostShip.Tokens.HasToken<ForceToken>())
            {
                return;
            }
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo && ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Normal)
            {
                BoardTools.DistanceInfo distanceInfo = new BoardTools.DistanceInfo(HostShip, ship);
                if (distanceInfo.Range < 3)
                {
                    ShipToRemoveStress = ship;
                    RegisterAbilityTrigger(TriggerTypes.OnPositionFinish, AskKananJarrusAbility);
                }
            }
        }

        private void AskKananJarrusAbility(object sender, System.EventArgs e)
        {
            if (ShipToRemoveStress.Tokens.HasToken(typeof(StressToken)) && HostShip.Tokens.HasToken<ForceToken>())
            {
                AskToUseAbility(
                    HostUpgrade.UpgradeInfo.Name,
                    AlwaysUseByDefault,
                    RemoveStress,
                    descriptionLong: "Do you want to spend 1 Force to remove 1 Stress Token from that ship?",
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
            HostShip.Tokens.SpendToken(typeof(ForceToken), () => 
                ShipToRemoveStress.Tokens.RemoveToken(
                    typeof(StressToken),
                    DecisionSubPhase.ConfirmDecision
                ));            
        }                
    }
}