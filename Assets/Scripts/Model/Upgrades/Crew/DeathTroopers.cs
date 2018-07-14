using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using System.Collections.Generic;
using UnityEngine;
using SquadBuilderNS;
using System;
using Abilities;
using BoardTools;

namespace UpgradesList
{
    public class DeathTroopers : GenericUpgrade
    {
        public DeathTroopers() : base()
        {
            Types.Add(UpgradeType.Crew);
            Types.Add(UpgradeType.Crew);
            Name = "Death Troopers";
            Cost = 2;
            isUnique = true;

            UpgradeAbilities.Add(new DeathTroopersAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
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
                    if(attackInfo.InArc && attackInfo.Range <= 3)
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


