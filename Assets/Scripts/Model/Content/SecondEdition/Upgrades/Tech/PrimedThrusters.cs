using ActionsList;
using Ship;
using System.Collections.Generic;
using Upgrade;
namespace UpgradesList.SecondEdition
{
    public class PrimedThrusters : GenericUpgrade
    {
        public PrimedThrusters() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Primed Thrusters",
                UpgradeType.Tech,
                cost: 8,
                abilityType: typeof(Abilities.SecondEdition.PrimedThrustersAbility)//,
                                                                                   //seImageNumber: 69
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/86/a1/86a1115b-eb55-491b-84e4-67e2b6124999/swz19_a1_primed-thrusters.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PrimedThrustersAbility : GenericAbility
    {
        private bool set = false;

        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += UsePrimedThruster;
            HostShip.OnTokenIsRemoved += UsePrimedThruster;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= UsePrimedThruster;
            HostShip.OnTokenIsRemoved -= UsePrimedThruster;
        }

        private void UsePrimedThruster(GenericShip ship, System.Type type)
        {
            if (type == typeof(Tokens.StressToken))
            {
                if (!set && HostShip.Tokens.CountTokensByType(typeof(Tokens.StressToken)) <= 2)
                {
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Add(typeof(BoostAction));
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Add(typeof(BarrelRollAction));
                    set = true;
                }
                else if (set && HostShip.Tokens.CountTokensByType(typeof(Tokens.StressToken)) > 2)
                {
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Remove(typeof(BoostAction));
                    HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Remove(typeof(BarrelRollAction));
                    set = false;
                }
            }
        }
    }
}