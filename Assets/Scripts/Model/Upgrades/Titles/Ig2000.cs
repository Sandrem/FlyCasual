using Ship;
using Ship.Aggressor;
using Upgrade;
using Abilities;
using System.Linq;
using System;
using UnityEngine;
using System.Collections.Generic;
using RuleSets;

namespace UpgradesList
{
    public class Ig2000 : GenericUpgrade, ISecondEditionUpgrade
    {
        public Ig2000() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "IG-2000";
            Cost = 0;

            UpgradeAbilities.Add(new Ig2000Ability());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 2;

            SEImageNumber = 149;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Aggressor;
        }
    }
}

namespace Abilities
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
                    if (ship.Value.UpgradeBar.GetUpgradesOnlyFaceup().Count(n => n is UpgradesList.Ig2000) > 0)
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
                    if (ship.Value.UpgradeBar.GetUpgradesOnlyFaceup().Count(n => n is UpgradesList.Ig2000) > 0)
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
