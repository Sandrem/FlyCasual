using System.Collections;
using System.Collections.Generic;
using ActionsList;
using Actions;
using Upgrade;
using Ship;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class UpsilonClassCommandShuttle : FirstEdition.UpsilonClassShuttle.UpsilonClassShuttle
        {
            public UpsilonClassCommandShuttle() : base()
            {
                ShipInfo.ShipName = "Upsilon-class Command Shuttle";

                ShipInfo.DefaultShipFaction = Faction.FirstOrder;
                ShipInfo.FactionsAll = new List<Faction>() { Faction.FirstOrder };

                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(ReinforceForeAction)));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(JamAction)));

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Cannon);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Crew);

                IconicPilots[Faction.FirstOrder] = typeof(LieutenantDormitz);

                ShipAbilities.Add(new Abilities.SecondEdition.LinkedBattery());

                // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/d/d4/Maneuver_lambda_shuttle.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LinkedBattery : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAddDice;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAddDice;
        }

        private void CheckAddDice(ref int count)
        {
            if (Combat.ChosenWeapon is GenericSpecialWeapon && (Combat.ChosenWeapon as GenericSpecialWeapon).HasType(UpgradeType.Cannon)) count++;
        }
    }
}