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
            //TODO: Engine Upgrade must add icon to available actions

            bool result = false;

            if (ship.PrintedActions.Any(n => n.GetType() == typeof(BoostAction))) result = true;
            else if (ship.UpgradeBar.HasUpgradeInstalled(typeof(EngineUpgrade))) result = true;

            return result;
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
            HostShip.AfterGenerateAvailableActionEffectsList += TryAddAutothrustersDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= TryAddAutothrustersDiceModification;
        }

        private void TryAddAutothrustersDiceModification(GenericShip host)
        {
            GenericAction newAction = new AutothrustersDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
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

            if (Combat.CurrentDiceRoll.Blanks > 0) result = 100;

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
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);

            callBack();
        }
    }

}