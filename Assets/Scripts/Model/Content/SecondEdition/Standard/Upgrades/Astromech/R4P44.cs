using Upgrade;
using Ship;
using Movement;
using System;
using BoardTools;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class R4P44 : GenericUpgrade
    {
        public R4P44() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R4-P44",
                UpgradeType.Astromech,
                cost: 2, 
                abilityType: typeof(Abilities.SecondEdition.R4P44Ability),
                restriction: new FactionRestriction(Faction.Republic)
            );
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/71/af/71afc8f6-022b-48f7-9694-83e6322782b2/swz33_r4-p44-upgrade.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a red maneuver, if there is an enemy ship in your bullseye arc, gain 1 calculate token.
    public class R4P44Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += RegisterMovementTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= RegisterMovementTrigger;
        }


        private void RegisterMovementTrigger(GenericShip ship)
        {
            if (HostShip.GetLastManeuverColor() == MovementComplexity.Complex && Board.GetShipsInBullseyeArc(HostShip, Team.Type.Enemy).Any())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, GainCalculateToken);
            }
        }

        private void GainCalculateToken(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostName + ": " + HostShip + " gains 1 calculate token");
            HostShip.Tokens.AssignToken(typeof(Tokens.CalculateToken), Triggers.FinishTrigger);
        }
    }
}