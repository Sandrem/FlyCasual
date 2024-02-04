using Upgrade;
using System.Collections.Generic;
using Ship;
using System.Linq;
using System;
using SubPhases;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class MargSablClosure : GenericUpgrade
    {
        public MargSablClosure() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Marg Sabl Closure",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.SecondEdition.MargSablClosureAbility),
                restriction: new BaseSizeRestriction(BaseSize.Small, BaseSize.Medium)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/64/ff/64ff6a96-57a4-44ef-938f-68cda3df71b6/swz81_upgrade_marg-sabl-clousre.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MargSablClosureAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckManeuver;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckManeuver;
        }

        private void CheckManeuver(GenericShip ship)
        {
            if (HostShip.AssignedManeuver.IsMovedThroughObstacle
                || HostShip.AssignedManeuver.IsDeployManeuver
            )
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, CheckEnemy);
            }
        }

        private void CheckEnemy(object sender, EventArgs e)
        {
            if (HasEnemyInRange())
            {
                SelectTargetForAbility(
                    AssignStrain,
                    FilterTargets,
                    GetAiPriority,
                    HostShip.Owner.PlayerNo,
                    name: HostUpgrade.UpgradeInfo.Name,
                    description: "Choose an enemy ship in your Front Arc at range 1-2. That ship gains 1 strain token.",
                    imageSource: HostUpgrade
                );
            }
            else
            {
                Messages.ShowInfoToHuman($"{HostUpgrade.UpgradeInfo.Name}: No enemy ships in required range / sector");
                Triggers.FinishTrigger();
            }
        }

        private void AssignStrain()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {TargetShip.PilotInfo.PilotName} gains Strain token");

            TargetShip.Tokens.AssignToken(
                typeof(Tokens.StrainToken),
                Triggers.FinishTrigger
            );
        }

        private bool FilterTargets(GenericShip ship)
        {
            if (!HostShip.SectorsInfo.IsShipInSector(ship, Arcs.ArcType.Front)) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return distInfo.Range >= 1 && distInfo.Range <= 2;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private bool HasEnemyInRange()
        {
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                if (FilterTargets(enemyShip)) return true;
            }

            return false;
        }
    }
}