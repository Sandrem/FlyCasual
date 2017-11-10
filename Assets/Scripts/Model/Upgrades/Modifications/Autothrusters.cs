using Ship;
using Upgrade;
using ActionsList;
using System.Linq;
using UnityEngine;

namespace UpgradesList
{ 
    public class Autothrusters : GenericUpgrade
    {
        public Autothrusters() : base()
        {
            Type = UpgradeType.Modification;
            Name = "Autothrusters";
            Cost = 2;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            //TODO: Fix restrictions
            //return (ship.BuiltInActions.Count(n => n.GetType() == typeof(BoostAction)) != 0);

            return true;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += TryAddAutothrustersDiceModification;
        }

        private void TryAddAutothrustersDiceModification(GenericShip host)
        {
            GenericAction newAction = new AutothrustersDiceModification()
            {
                ImageUrl = ImageUrl,
                Host = host
            };
            host.AddAvailableActionEffect(newAction);
        }
    }
}

namespace ActionsList
{
    public class AutothrustersDiceModification : GenericAction
    {
        public AutothrustersDiceModification()
        {
            Name = EffectName = "Autothrusters";
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.CurentDiceRoll.Blanks > 0) result = 100;

            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Combat.ShotInfo.InArc)
                {
                    if (Combat.ShotInfo.Range > 2)
                    {
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);

            callBack();
        }
    }

}