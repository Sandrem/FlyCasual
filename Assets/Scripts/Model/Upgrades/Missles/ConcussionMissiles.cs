using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;
using RuleSets;

namespace UpgradesList
{

    public class ConcussionMissiles : GenericSecondaryWeapon, ISecondEditionUpgrade
    {
        public ConcussionMissiles() : base()
        {
            Types.Add(UpgradeType.Missile);

            Name = "Concussion Missiles";
            Cost = 4;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;

            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new ConcussionMissilesAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 6;
            AttackValue = 3;
            SpendsTargetLockOnTargetToShoot = false;
            IsDiscardedForShot = false;

            UsesCharges = true;
            MaxCharges = 3;

            UpgradeAbilities.RemoveAll(a => a is ConcussionMissilesAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.ConcussionMissilesAbilitySE());

            SEImageNumber = 38;
        }
    }
}

namespace Abilities
{
    public class ConcussionMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddConcussionMissilesDiceModification;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnGenerateDiceModifications -= AddConcussionMissilesDiceModification;
        }

        private void AddConcussionMissilesDiceModification(GenericShip host)
        {
            ConcussionMissilesAction action = new ConcussionMissilesAction()
            {
                Host = host,
                ImageUrl = HostUpgrade.ImageUrl,
                Source = HostUpgrade
            };

            host.AddAvailableDiceModification(action);
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ConcussionMissilesAbilitySE : GenericAbility
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
                // Defending ship shouldn't flip the crit.
                if (ship == Combat.Defender)
                    continue;

                if(ship.Damage.HasFacedownCards)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Concussion Missile exposes damage card",
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

namespace ActionsList
{ 

    public class ConcussionMissilesAction : GenericAction
    {

        public ConcussionMissilesAction()
        {
            Name = DiceModificationName = "Concussion Missiles";
        }

        public void AddDiceModification()
        {
            Host.OnGenerateDiceModifications += ConcussionMissilesAddDiceModification;
        }

        private void ConcussionMissilesAddDiceModification(GenericShip ship)
        {
            ship.AddAvailableDiceModification(this);
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;

            if (Combat.ChosenWeapon != Source) result = false;

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackBlanks = Combat.DiceRollAttack.Blanks;
                if (attackBlanks > 0)
                {
                    if ((attackBlanks == 1) && (Combat.Attacker.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) == 0))
                    {
                        result = 100;
                    }
                    else
                    {
                        result = 55;
                    }
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            callBack();
        }

    }

}
