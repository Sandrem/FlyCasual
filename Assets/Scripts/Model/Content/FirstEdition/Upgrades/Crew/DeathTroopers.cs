using Ship;
using Upgrade;
using System.Collections.Generic;
using BoardTools;
using Tokens;

namespace UpgradesList.FirstEdition
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
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.FirstEdition.DeathTroopersAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class DeathTroopersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsDefenderGlobal += DeathTroopersEffect;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsDefenderGlobal -= DeathTroopersEffect;
        }

        private void DeathTroopersEffect()
        {
            // If the defender is another friendly ship
            if (Combat.Defender.Owner.Id == HostShip.Owner.Id && Combat.Defender.ShipId != HostShip.ShipId)
            {
                // ...and it's at range 1 of me
                var distanceToDefender = new DistanceInfo(HostShip, Combat.Defender);
                if (distanceToDefender.Range <= 1)
                {
                    // ...and I am in arc, at range 1-3
                    var attackInfo = new ShotInfo(Combat.Attacker, HostShip, Combat.ChosenWeapon);
                    if (attackInfo.InArc && attackInfo.Range <= 3)
                    {
                        // ...assign a stress token to the attacker
                        RegisterAbilityTrigger(TriggerTypes.OnAttackStart, (s, e) =>
                        {
                            Messages.ShowInfo(string.Format("{0}'s Death Troopers assign stress to {1}!", HostShip.PilotName, Combat.Attacker.PilotName));
                            Sounds.PlayShipSound("DeathTrooper");
                            Combat.Attacker.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
                        });
                    }
                }
            }
        }
    }
}
