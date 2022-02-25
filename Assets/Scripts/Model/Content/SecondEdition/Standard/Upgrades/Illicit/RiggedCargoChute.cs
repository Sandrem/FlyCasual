using ActionsList;
using BoardTools;
using Movement;
using Obstacles;
using Ship;
using SubPhases;
using System;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class RiggedCargoChute : GenericUpgrade
    {
        public RiggedCargoChute() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Rigged Cargo Chute",
                UpgradeType.Illicit,
                cost: 4,
                charges: 1,
                restriction: new BaseSizeRestriction(BaseSize.Medium, BaseSize.Large), 
                seImageNumber: 62,
                abilityType: typeof(Abilities.SecondEdition.RiggedCargoChuteAbility)
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class RiggedCargoChuteAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddRiggedCargoChuteAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddRiggedCargoChuteAction;
        }

        private void AddRiggedCargoChuteAction(GenericShip host)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                GenericAction action = new RiggedCargoChuteAction()
                {
                    ImageUrl = HostUpgrade.ImageUrl,
                    HostShip = HostShip,
                    DoAction = DropCargoToken
                };
                host.AddAvailableAction(action);
            }
        }

        private void DropCargoToken()
        {
            Action Callback = Phases.CurrentSubPhase.CallBack;
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();

            ManeuverTemplate dropTemplate = new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed1, isBombTemplate: true);
            dropTemplate.ApplyTemplate(HostShip, HostShip.GetBack(), Direction.Bottom);

            Debris looseCargo = new Debris("Loose Cargo", "loosecargo");
            looseCargo.Spawn("Loose Cargo " + HostShip.ShipId, Board.GetBoard());
            ObstaclesManager.AddObstacle(looseCargo);

            looseCargo.ObstacleGO.transform.position = dropTemplate.GetFinalPosition();
            looseCargo.ObstacleGO.transform.eulerAngles = dropTemplate.GetFinalAngles();
            looseCargo.IsPlaced = true;

            GameManagerScript.Wait(
                1,
                delegate
                {
                    dropTemplate.DestroyTemplate();
                    Callback();
                }
            );
            
        }
    }
}

namespace ActionsList
{
    public class RiggedCargoChuteAction : GenericAction
    {
        public RiggedCargoChuteAction()
        {
            Name = "Rigged Cargo Chute";
        }
    }
}

