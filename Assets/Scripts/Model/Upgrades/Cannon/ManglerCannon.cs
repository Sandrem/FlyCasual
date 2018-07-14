﻿using System;
using System.Collections;
using System.Collections.Generic;
using Abilities;
using ActionsList;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ManglerCannon : GenericSecondaryWeapon
    {
		public ManglerCannon() : base()
        {
            Types.Add(UpgradeType.Cannon);

            Name = "Mangler Cannon";
            Cost = 4;

            MinRange = 1;
            MaxRange = 3;
            AttackValue = 3;

            UpgradeAbilities.Add(new ManglerCannonAbility());
        }        
    }
}

namespace Abilities
{
    public class ManglerCannonAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += ManglerCannonAddDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= ManglerCannonAddDiceModification;
        }

        private void ManglerCannonAddDiceModification(Ship.GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ManglerCannonAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip,
                Source = HostUpgrade
            });
        }
    }
}

namespace ActionsList
{ 
    public class ManglerCannonAction : GenericAction
    {

		public ManglerCannonAction()
        {
            Name = DiceModificationName = "Mangler Cannon";
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && Combat.ChosenWeapon == Source;            
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
				int attackSuccesses = Combat.DiceRollAttack.RegularSuccesses;
                if (attackSuccesses > 0) result = 100;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
			Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }
    }    
}
