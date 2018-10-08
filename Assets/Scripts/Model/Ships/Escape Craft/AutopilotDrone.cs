using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using RuleSets;
using BoardTools;

namespace Ship
{
    namespace EscapeCraft
    {
        public class AutopilotDrone : EscapeCraft, ISecondEditionPilot
        {
            public AutopilotDrone() : base()
            {
                PilotName = "Autopilot Drone";
                PilotSkill = 1;
                Cost = 12;

                IsUnique = true;

                UsesCharges = true;
                MaxCharges = 3;

                PrintedUpgradeIcons.Remove(Upgrade.UpgradeType.Crew);
                UpgradeBar.RemoveSlot(Upgrade.UpgradeType.Modification);

                ShipAbilities.Add(new Abilities.SecondEdition.AutopilotDroneAbility());

                SEImageNumber = 229;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AutopilotDroneAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += RegisterLoseCharge;
            Phases.Events.OnActivationPhaseEnd_Triggers += CheckDestruction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= RegisterLoseCharge;
            Phases.Events.OnActivationPhaseEnd_Triggers -= CheckDestruction;
        }

        private void RegisterLoseCharge(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, LoseCharge);
        }

        private void LoseCharge(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Autopilot Drone: Charge is lost");
            HostShip.RemoveCharge(Triggers.FinishTrigger);
        }

        private void CheckDestruction()
        {
            if (HostShip.Charges == 0) RegisterAbilityTrigger(TriggerTypes.OnActivationPhaseEnd, DestroyThisShip);
        }

        private void DestroyThisShip(object sender, System.EventArgs e)
        {
            List<GenericShip> sufferedShips = new List<GenericShip>();

            foreach (var ship in Roster.AllShips.Values)
            {
                if (ship.ShipId == HostShip.ShipId) continue;

                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range < 2) sufferedShips.Add(ship);
            }

            Messages.ShowInfo("Autopilot Drone is destroyed");
            HostShip.DestroyShipForced(delegate { DealCritDamageToShips(sufferedShips); });
        }

        private class ShipEventArgs: System.EventArgs
        {
            public GenericShip Ship;
        }

        private void DealCritDamageToShips(List<GenericShip> ships)
        {
            foreach (var ship in ships)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAbilityDirect, DealDamageToShip, new ShipEventArgs() { Ship = ship });
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }

        private void DealDamageToShip(object sender, System.EventArgs e)
        {
            GenericShip ship = (e as ShipEventArgs).Ship;

            Messages.ShowInfo(ship.PilotName + " is dealt Critical Hit by destruction of Autopilot Drone");

            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
            {
                DamageType = DamageTypes.CardAbility,
                Source = HostShip
            };
            
            ship.Damage.TryResolveDamage(0, damageArgs, Triggers.FinishTrigger, critDamage: 1);
        }
    }
}