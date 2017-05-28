using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Upgrade
{

    public class ProtonTorpedoes : GenericSecondaryWeapon
    {
        public ProtonTorpedoes(Ship.GenericShip host) : base(host)
        {
            Type = UpgradeSlot.Torpedoes;

            Name = "Proton Torpedoes";
            ShortName = "Proton Torp.";
            ImageUrl = "https://vignette2.wikia.nocookie.net/xwing-miniatures/images/e/eb/Proton-torpedoes.png";

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            //TODO: set host in constructor
            ProtonTorpedoesAction action = new ProtonTorpedoesAction();
            action.Host = host;
            action.ImageUrl = ImageUrl;
            action.AddDiceModification();

            host.AddAvailableAction(action);
        }

        public override bool IsShotAvailable(Ship.GenericShip anotherShip)
        {
            bool result = true;

            if (isDiscarded) return false;

            int distance = Actions.GetFiringRange(Host, anotherShip);
            if (distance < MinRange) return false;
            if (distance > MaxRange) return false;

            if (!Actions.InArcCheck(Host, anotherShip)) return false;

            if (!Actions.HasTargetLockOn(Host, anotherShip)) return false;

            return result;
        }

        public override void PayAttackCost()
        {
            char letter = Actions.GetTargetLocksLetterPair(Combat.Attacker, Combat.Defender);
            Combat.Attacker.SpendToken(typeof(Tokens.BlueTargetLockToken), letter);
            Combat.Defender.RemoveToken(typeof(Tokens.RedTargetLockToken), letter);

            Discard();
        }

    }

    public class ProtonTorpedoesAction : ActionsList.GenericAction
    {
        public Ship.GenericShip Host;

        public ProtonTorpedoesAction()
        {
            Name = EffectName = "Proton Torpedoes";

            //AddDiceModification (host requied first);
        }

        public void AddDiceModification()
        {
            Host.AfterGenerateDiceModifications += ProtonTorpedoesAddDiceModification;
        }

        private void ProtonTorpedoesAddDiceModification(ref List<ActionsList.GenericAction> list)
        {
            list.Add(this);
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;

            if (Combat.SecondaryWeapon == null)
            {
                result = false;
            }
            else
            {
                if (Combat.SecondaryWeapon.Name != Name) result = false;
            }

            return result;
        }

        public override void ActionEffect()
        {
            Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Crit);
        }

    }

}
