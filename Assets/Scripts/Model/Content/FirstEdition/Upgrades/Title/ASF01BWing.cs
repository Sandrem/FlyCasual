using Ship;
using Upgrade;
using System.Collections.Generic;
using Mods.ModsList;

namespace UpgradesList.FirstEdition
{
    public class ASF01BWing : GenericUpgrade
    {
        public ASF01BWing() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "A/SF-01 B-wing",
                UpgradeType.Title,
                cost: 0,          
                restriction: new ShipRestriction(typeof(Ship.FirstEdition.BWing.BWing))
                //abilityType: typeof(Abilities.FirstEdition.AdaptiveAileronsAbility)
            );

            FromMod = typeof(RecoveringBWing);
            ImageUrl = "https://azrapse.es/asf01bwing.png";
        }

        public override void PreAttachToShip(GenericShip host)
        {
            base.PreAttachToShip(host);

            // TODOREVERT
            // Host.MaxEnergy += 3;
        }

        public override void PreDettachFromShip()
        {
            base.PreDettachFromShip();

            // TODOREVERT
            // Host.MaxEnergy -= 3;
        }
    }
}

/*namespace Abilities
{
    public class ASF01BWingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += GetEnergy;
            HostShip.OnTokenIsAssigned += SpendEnergyDecision;
        }

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.AddGrantedAction(new ActionsList.RecoverAction(), HostUpgrade);
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= GetEnergy;
            HostShip.OnTokenIsAssigned -= SpendEnergyDecision;
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ActionBar.RemoveGrantedAction(typeof(ActionsList.RecoverAction), HostUpgrade);
        }

        public void GetEnergy(GenericShip ship)
        {
            var color = ship.AssignedManeuver.ColorComplexity;
            var energyGain = 0;
            switch (color)
            {
                case Movement.MovementComplexity.Easy:
                    energyGain = 1;
                    break;
            }

            if (energyGain > 0 && ship.Energy < ship.MaxEnergy)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, (s, e) => ship.Tokens.AssignToken(typeof(EnergyToken), Triggers.FinishTrigger));
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
}*/
