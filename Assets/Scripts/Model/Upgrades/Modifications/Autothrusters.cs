using Ship;
using Upgrade;
using ActionsList;
using System.Linq;
using UnityEngine;
using SquadBuilderNS;
using Abilities;

namespace UpgradesList
{
    public class Autothrusters : GenericUpgrade
    {
        public Autothrusters() : base()
        {
            Types.Add(UpgradeType.Modification);
            Name = "Autothrusters";
            Cost = 2;

            UpgradeAbilities.Add(new AutothrustersAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.ActionBar.HasAction(typeof(BoostAction));
        }

        public override bool IsAllowedForSquadBuilderPostCheck(SquadList squadList)
        {
            bool result = false;

            result = IsAllowedForShip(Host);
            if (!result) Messages.ShowError("Autothrusters can be installed only if ship has Boost action icon");

            return result;
        }
    }
}

namespace Abilities
{
    public class AutothrustersAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += TryAddAutothrustersDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= TryAddAutothrustersDiceModification;
        }

        private void TryAddAutothrustersDiceModification(GenericShip host)
        {
            GenericAction newAction = new AutothrustersDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{
    public class AutothrustersDiceModification : GenericAction
    {
        public AutothrustersDiceModification()
        {
            Name = DiceModificationName = "Autothrusters";
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.CurrentDiceRoll.Blanks > 0) result = 100;

            return result;
        }

        public override bool IsDiceModificationAvailable()
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
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);

            callBack();
        }
    }

}