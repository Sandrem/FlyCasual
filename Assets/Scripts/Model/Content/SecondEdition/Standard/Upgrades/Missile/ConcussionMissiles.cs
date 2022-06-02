using Ship;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ConcussionMissiles : GenericSpecialWeapon
    {
        public ConcussionMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Concussion Missiles",
                UpgradeType.Missile,
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    charges: 3
                ),
                abilityType: typeof(Abilities.SecondEdition.ConcussionMissilesAbility),
                seImageNumber: 38
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ConcussionMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnShotHitAsAttacker += RegisterConcussionHit;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnShotHitAsAttacker -= RegisterConcussionHit;
        }

        private void RegisterConcussionHit()
        {
            if (Combat.ChosenWeapon == HostUpgrade)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Concussion Missile Hit",
                    TriggerType = TriggerTypes.OnShotHit,
                    TriggerOwner = Combat.Attacker.Owner.PlayerNo,
                    EventHandler = delegate {
                        ConcussionMissileHit();
                    }
                });
            }
        }

        private void ConcussionMissileHit()
        {
            var shipsHitByBlast = BoardTools.Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1), Team.Type.Any);

            foreach (GenericShip ship in shipsHitByBlast)
            {
                if (ship.Damage.HasFacedownCards)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Concussion Missile exposes a damage card",
                        TriggerType = TriggerTypes.OnAbilityDirect,
                        TriggerOwner = Combat.Defender.Owner.PlayerNo,
                        EventHandler = delegate {
                            ship.Damage.ExposeRandomFacedownCard(Triggers.FinishTrigger);
                        }
                    });
                }
            }

            Triggers.ResolveTriggers(TriggerTypes.OnAbilityDirect, Triggers.FinishTrigger);
        }
    }
}
