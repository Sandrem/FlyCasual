using System.Collections;
using System.Collections.Generic;
using Upgrade;
using Ship;
using Abilities;
using Arcs;

namespace UpgradesList
{

    public class SyncedTurret : GenericSecondaryWeapon
    {
        public SyncedTurret() : base()
        {
            Types.Add(UpgradeType.Turret);

            Name = "Synced Turret";
            Cost = 4;

            MinRange = 1;
            MaxRange = 2;
            AttackValue = 3;

            CanShootOutsideArc = true;
            RequiresTargetLockOnTargetToShoot = true;

            UpgradeAbilities.Add(new SyncedTurretAbility());
        }
    }
}

namespace Abilities
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
            ship.AddAvailableDiceModification(new SyncedTurretAction() { Source = HostUpgrade } );
        }

        private class SyncedTurretAction : ActionsList.GenericAction
        {
            public SyncedTurretAction()
            {
                Name = DiceModificationName = "Synced Turret";
            }
                
            public override void ActionEffect(System.Action callBack)
            {
               // reroll dice
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    SidesCanBeRerolled = null,      // all the sides can be reroll
                    NumberOfDiceCanBeRerolled = Combat.Attacker.PrimaryWeapon.AttackValue,
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
