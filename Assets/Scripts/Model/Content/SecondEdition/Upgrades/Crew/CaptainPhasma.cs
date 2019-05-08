using Ship;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using Tokens;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class CaptainPhasma : GenericUpgrade
    {
        public CaptainPhasma() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Captain Phasma",
                UpgradeType.Crew,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.FirstOrder),
                abilityType: typeof(Abilities.SecondEdition.CaptainPhasmaAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/784566afc8c540d5e392689865207059.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainPhasmaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseEnd_Triggers += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseEnd_Triggers -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseEnd, AssignStressToEnemyShips);
        }

        private void AssignStressToEnemyShips(object sender, System.EventArgs e)
        {
            List<GenericShip> shipsToAssignStress = new List<GenericShip>();
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values.Where(n => !n.IsStressed))
            {
                DistanceInfo distanceInfo = new DistanceInfo(HostShip, enemyShip);
                if (distanceInfo.Range <= 1) shipsToAssignStress.Add(enemyShip);
            }

            AssignStressRecursive(shipsToAssignStress);
        }

        private void AssignStressRecursive(List<GenericShip> shipsToAssignStress)
        {
            if (shipsToAssignStress.Count > 0)
            {
                GenericShip shipToAssignStress = shipsToAssignStress.First();
                shipsToAssignStress.Remove(shipToAssignStress);

                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + "assigns a stress token to " + shipToAssignStress.PilotInfo.PilotName);
                shipToAssignStress.Tokens.AssignToken(
                    typeof(StressToken),
                    delegate { AssignStressRecursive(shipsToAssignStress); }
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}