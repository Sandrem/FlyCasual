using ActionsList;
using BoardTools;
using Movement;
using Obstacles;
using Ship;
using SquadBuilderNS;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class SparePartsCanisters : GenericUpgrade
    {
        public SparePartsCanisters() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Spare Parts Canisters",
                UpgradeType.Modification,
                cost: 4,
                restriction: new UpgradeBarRestriction(UpgradeType.Astromech),
                abilityType: typeof(Abilities.SecondEdition.SparePartsCanistersAbility),
                charges: 1
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/79d9f2b2bc32bd78ab67dc82eece696a.png";
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            if (HostShip.UpgradeBar.GetUpgradesAll().Any(n => n.HasType(UpgradeType.Astromech)))
            {
                return true;
            }
            else
            {
                Messages.ShowError("Spare Parts Canisters: Astromech must be equipped");
                return false;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class SparePartsCanistersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddRecoverChargeAction;
            HostShip.OnGenerateActions += AddDropSparePartsAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddRecoverChargeAction;
            HostShip.OnGenerateActions -= AddDropSparePartsAction;
        }

        private void AddRecoverChargeAction(GenericShip ship)
        {
            SparePartsCanistersRecoverCharge action = new SparePartsCanistersRecoverCharge()
            {
                Source = HostUpgrade,
                HostShip = ship,
                ImageUrl = HostUpgrade.ImageUrl,
                DoAction = DoRecoverCharge
            };
            ship.AddAvailableAction(action);
        }

        private void DoRecoverCharge()
        {
            Action Callback = Phases.CurrentSubPhase.CallBack;
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();

            GenericUpgrade astromech = HostShip.UpgradeBar.GetUpgradesAll().FirstOrDefault(n => n.HasType(UpgradeType.Astromech));
            if (astromech != null && astromech.State.Charges < astromech.State.MaxCharges & !astromech.UpgradeInfo.CannotBeRecharged)
            {
                Messages.ShowInfo("Spare Parts Canisters: Charge of " + astromech.UpgradeInfo.Name + " is restored");
                astromech.State.RestoreCharge();
            }

            Callback();
        }

        private void AddDropSparePartsAction(GenericShip ship)
        {
            SparePartsCanistersDropCharge action = new SparePartsCanistersDropCharge()
            {
                Source = HostUpgrade,
                HostShip = ship,
                ImageUrl = HostUpgrade.ImageUrl,
                DoAction = DropSparePartsToken
            };
            ship.AddAvailableAction(action);
        }

        private void DropSparePartsToken()
        {
            Action Callback = Phases.CurrentSubPhase.CallBack;
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();

            Debris spareParts = new Debris("Spare Parts", "spareparts");
            spareParts.Spawn("Spare Parts " + HostShip.ShipId, Board.GetBoard());
            ObstaclesManager.AddObstacle(spareParts);

            spareParts.ObstacleGO.transform.position = HostShip.GetBack();
            spareParts.ObstacleGO.transform.eulerAngles = HostShip.GetAngles() + new Vector3(0, 180, 0);
            spareParts.IsPlaced = true;

            GameManagerScript.Wait(
                1,
                delegate
                {
                    Messages.ShowInfo("Spare Parts are dropped");
                    BreakAllLocksRecursive(Callback);
                }
            );
        }

        private void BreakAllLocksRecursive(Action callback)
        {
            RedTargetLockToken redTlToken = HostShip.Tokens.GetToken<RedTargetLockToken>(letter: '*');
            
            if (redTlToken != null)
            {
                Messages.ShowInfo("Lock \"" + redTlToken.Letter + "\" is broken");
                HostShip.Tokens.RemoveToken(
                    redTlToken,
                    delegate { BreakAllLocksRecursive(callback); }
                );
            }
            else
            {
                callback();
            }
        }
    }
}

namespace ActionsList
{

    public class SparePartsCanistersRecoverCharge : GenericAction
    {
        public SparePartsCanistersRecoverCharge()
        {
            Name = "Spare Parts Canisters: Recover";
        }

        public override bool IsActionAvailable()
        {
            GenericUpgrade astromech = HostShip.UpgradeBar.GetUpgradesAll().FirstOrDefault(n => n.HasType(UpgradeType.Astromech));
            if (astromech == null || astromech.State.Charges == astromech.State.MaxCharges || astromech.UpgradeInfo.CannotBeRecharged)
            {
                return false;
            }
            else
            {
                return Source.State.Charges > 0;
            }
        }

        public override int GetActionPriority()
        {
            return 0;
        }
    }

    public class SparePartsCanistersDropCharge : GenericAction
    {
        public SparePartsCanistersDropCharge()
        {
            Name = "Spare Parts Canisters: Drop";
        }

        public override bool IsActionAvailable()
        {
            return Source.State.Charges > 0;
        }

        public override int GetActionPriority()
        {
            return 0;
        }
    }

}