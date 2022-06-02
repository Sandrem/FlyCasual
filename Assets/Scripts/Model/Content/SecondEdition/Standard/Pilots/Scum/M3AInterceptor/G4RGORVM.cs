using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class G4RGORVM : M3AInterceptor
        {
            public G4RGORVM() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "G4R-GOR V/M",
                    "Tilted Droid",
                    Faction.Scum,
                    0,
                    3,
                    6,
                    tags: new List<Tags>
                    {
                        Tags.Droid
                    },
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.G4RGORVMAbility)
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/4c/2c/4c2c309f-f9b0-4a93-a3a5-28b43fe981c3/swz66_g4r-g0r_vm.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class G4RGORVMAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, DealDamage);
        }

        private void DealDamage(object sender, EventArgs e)
        {
            List<GenericShip> sufferedShips = new List<GenericShip>();

            foreach (var ship in Roster.AllShips.Values)
            {
                if (ship.ShipId == HostShip.ShipId) continue;

                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range < 1) sufferedShips.Add(ship);
            }

            if (sufferedShips.Any())
            {
                Messages.ShowInfo(HostName + " deals 1 Crit to " + sufferedShips.Count + " ships");
                DealDamageToShips(sufferedShips, 1, true, Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}