using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class Ig2000 : GenericUpgrade
    {
        public Ig2000() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "IG-2000",
                UpgradeType.Title,
                cost: 0,
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.Aggressor.Aggressor)),
                abilityType: typeof(Abilities.FirstEdition.Ig2000Ability)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class Ig2000Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnGameStart += ActivateIg2000Ability;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnGameStart -= ActivateIg2000Ability;
            DeactivateIg2000Ability();
        }

        private void ActivateIg2000Ability()
        {
            foreach (var ship in HostShip.Owner.Ships)
            {
                if (ship.Value.ShipId != HostShip.ShipId)
                {
                    if (ship.Value.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.UpgradeInfo.Name == "IG-2000"))
                    {
                        Type pilotAbilityType = ship.Value.PilotAbilities[0].GetType();
                        GenericAbility pilotAbility = (GenericAbility)System.Activator.CreateInstance(pilotAbilityType);
                        pilotAbility.Initialize(HostShip);
                        HostShip.PilotAbilities.Add(pilotAbility);
                    }
                }
            }
        }

        private void DeactivateIg2000Ability()
        {
            foreach (var ship in HostShip.Owner.Ships)
            {
                if (ship.Value.ShipId != HostShip.ShipId)
                {
                    if (ship.Value.UpgradeBar.GetUpgradesOnlyFaceup().Any(n => n.UpgradeInfo.Name == "IG-2000"))
                    {
                        List<GenericAbility> anotherPilotAbilities = new List<GenericAbility>(ship.Value.PilotAbilities);

                        foreach (var anotherPilotAbility in anotherPilotAbilities)
                        {
                            if (anotherPilotAbility.GetType() == HostShip.PilotAbilities[0].GetType())
                            {
                                anotherPilotAbility.DeactivateAbility();
                                ship.Value.PilotAbilities.Remove(anotherPilotAbility);
                                break;
                            }
                        }
                    }
                }
            }
        }

    }
}