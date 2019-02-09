using Ship;
using Upgrade;
using System.Collections.Generic;
using BoardTools;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class DeathTroopers : GenericUpgrade
    {
        public DeathTroopers() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Death Troopers",
                types: new List<UpgradeType>()
                {
                    UpgradeType.Crew,
                    UpgradeType.Crew
                },
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.DeathTroopersAbility),
                seImageNumber: 113
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class DeathTroopersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnBeforeTokenIsRemovedGlobal += DeathTroopersEffect;
        }
                
        public override void DeactivateAbility()
        {
            GenericShip.OnBeforeTokenIsRemovedGlobal -= DeathTroopersEffect;
        }

        private void DeathTroopersEffect(GenericShip ship, GenericToken token, ref bool allowed)
        {
            // During activation phase enemy ships at range 0-1 cannot remove stress tokens
            if (Phases.CurrentPhase is MainPhases.ActivationPhase 
                && token is StressToken
                && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo
                && Board.IsShipBetweenRange(HostShip, ship, 0, 1))
            {
                allowed = false;
                Messages.ShowInfo(string.Format("{0}'s Death Troopers prevent {1} from removing stress token!", HostShip.PilotInfo.PilotName, ship.PilotInfo.PilotName));
                Sounds.PlayShipSound("DeathTrooper");
            }
        }
    }
}
