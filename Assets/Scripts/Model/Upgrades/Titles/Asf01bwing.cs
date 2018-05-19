using Ship;
using Ship.YWing;
using Upgrade;
using Abilities;
using Mods.ModsList;
using Ship.BWing;
using Tokens;
using System;
using SubPhases;

namespace UpgradesList
{
    public class ASF01BWing : GenericUpgrade
    {
        public ASF01BWing() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "A/SF-01 B-wing";
            Cost = 0;

            ImageUrl = "https://azrapse.es/asf01bwing.png";

            FromMod = typeof(RecoveringBWing);

            UpgradeAbilities.Add(new ASF01BWingAbility());
            UpgradeAbilities.Add(new GenericActionBarAbility<ActionsList.RecoverAction>());

        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is BWing;
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            Host.MaxEnergy += 3;
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            Host.MaxEnergy -= 3;
        }
    }
}

//Special ops
namespace Abilities
{
    public class ASF01BWingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {            
            HostShip.OnMovementFinish += GetEnergy;
            HostShip.OnTokenIsAssigned += SpendEnergyDecision;
        }
                
        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= GetEnergy;
            HostShip.OnTokenIsAssigned -= SpendEnergyDecision;
        }

        public void GetEnergy(GenericShip ship)
        {            
            var color = ship.AssignedManeuver.ColorComplexity;
            var energyGain = 0;
            switch(color)
            {
                case Movement.MovementComplexity.Easy:
                    energyGain = 1;
                    break;                
            }

            if(energyGain > 0 && ship.Energy < ship.MaxEnergy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, (s, e) => ship.Tokens.AssignToken(new EnergyToken(ship), Triggers.FinishTrigger));                
            }
        }

        private void SpendEnergyDecision(GenericShip ship, Type type)
        {
            if (type == typeof(StressToken) && ship.Energy > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, SpendEnergyToDiscardStress);
            }
        }

        private void SpendEnergyToDiscardStress(object sender, EventArgs e)
        {
            AskToUseAbility(NeverUseByDefault, DoSpendEnergyToRemoveStress, null, null, false, HostShip.PilotName + ": Spend 1 energy to discard 1 stress?");
        }

        private void DoSpendEnergyToRemoveStress(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasToken<StressToken>() && HostShip.Energy > 0)
            {
                HostShip.Tokens.SpendToken(typeof(EnergyToken), () => HostShip.Tokens.RemoveToken(typeof(StressToken), DecisionSubPhase.ConfirmDecision));
            }
        }              
    }
}