using Upgrade;
using Ship;
using System;
using System.Linq;
using BoardTools;
using SubPhases;

namespace UpgradesList.SecondEdition
{
    public class StarbirdSlash : GenericUpgrade
    {
        public StarbirdSlash() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Starbird Slash",
                UpgradeType.Talent,
                cost: 1,
                restriction: new TagRestriction(Content.Tags.AWing),
                abilityType: typeof(Abilities.SecondEdition.StarbirdSlashAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/65/61/656136b2-2981-4e18-80fb-771fb2810669/swz68_starbird-slash.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class StarbirdSlashAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ship.ShipsMovedThrough.Any(n => n.Owner.PlayerNo != HostShip.Owner.PlayerNo))
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToSelectEnemyShip);
            }
        }

        private void AskToSelectEnemyShip(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AssignStrainTokenToEnemy,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: "Starbird Slash",
                description: "You may shoose an enemy ship you moved through - it gains Strain token",
                imageSource: HostUpgrade
            );
        }

        private void AssignStrainTokenToEnemy()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo($"Starbird Slash: {TargetShip.PilotInfo.PilotName} gained Strain token");
            TargetShip.Tokens.AssignToken(typeof(Tokens.StrainToken), CheckOwnStrain);
        }

        private void CheckOwnStrain()
        {
            ShotInfo shotInfo = new ShotInfo(TargetShip, HostShip, TargetShip.PrimaryWeapons);
            if (shotInfo.InArc)
            {
                Messages.ShowInfo($"Starbird Slash: {HostShip.PilotInfo.PilotName} gained Strain token too");
                HostShip.Tokens.AssignToken(typeof(Tokens.StrainToken), Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            return HostShip.ShipsMovedThrough.Contains(ship)
                && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }
    }
}