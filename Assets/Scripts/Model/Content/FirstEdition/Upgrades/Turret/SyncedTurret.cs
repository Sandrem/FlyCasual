﻿using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class SyncedTurret : GenericSpecialWeapon
    {
        public SyncedTurret() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Synced Turret",
                UpgradeType.Turret,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 2,
                    requiresToken: typeof(BlueTargetLockToken),
                    canShootOutsideArc: true
                ),
                abilityType: typeof(Abilities.FirstEdition.SyncedTurretAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    // If the defender is inside your primary firing arc, you may reroll
    // a number of attack dice up to your primary weapon value. 
    public class SyncedTurretAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddSyncedTurretAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddSyncedTurretAbility;
        }

        private void AddSyncedTurretAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new SyncedTurretAction() { Source = HostUpgrade });
        }

        private class SyncedTurretAction : ActionsList.GenericAction
        {
            public SyncedTurretAction()
            {
                Name = DiceModificationName = "Synced Turret";
            }

            public override void ActionEffect(System.Action callBack)
            {
                // TODOREVERT

                // reroll dice
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    SidesCanBeRerolled = null,      // all the sides can be reroll
                    NumberOfDiceCanBeRerolled = Combat.Attacker.PrimaryWeapons.First().WeaponInfo.AttackValue,
                    CallBack = callBack// all the dices can be reroll
                };
                diceRerollManager.Start();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Attack && Combat.ShotInfo.InPrimaryArc && Combat.ChosenWeapon.GetType() == Source.GetType()) result = true;
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 90;
                return result;
            }
        }
    }
}