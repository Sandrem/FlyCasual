using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ManglerCannon : GenericSecondaryWeapon
    {
		public ManglerCannon() : base()
        {
            Type = UpgradeType.Cannon;

            Name = "Mangler Cannon";
            ShortName = "Mangler";
            Cost = 4;

            MinRange = 1;
            MaxRange = 3;
            AttackValue = 3;

        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            AddDiceModification();
        }

        private void AddDiceModification()
        {
            ActionsList.ManglerCannonAction action = new ActionsList.ManglerCannonAction()
            {
                Host = Host,
                ImageUrl = ImageUrl,
                Source = this
            };
            action.AddDiceModification();

            Host.AddAvailableAction(action);
        }

    }

}

namespace ActionsList
{ 

    public class ManglerCannonAction : GenericAction
    {

		public ManglerCannonAction()
        {
            Name = EffectName = "Mangler Cannon";

        }

        public void AddDiceModification()
        {
            Host.AfterGenerateAvailableActionEffectsList += ManglerCannonAddDiceModification;
        }

        private void ManglerCannonAddDiceModification(Ship.GenericShip ship)
        {
            ship.AddAvailableActionEffect(this);
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;

            if (Combat.ChosenWeapon != Source) result = false;

            return result;
        }

        public override int GetActionEffectPriority()
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
			Combat.CurentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }

    }

}
