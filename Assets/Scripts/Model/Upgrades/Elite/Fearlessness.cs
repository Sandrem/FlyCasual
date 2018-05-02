using Upgrade;
using Ship;
using Abilities;
using UnityEngine;

namespace UpgradesList
{
    public class Fearlessness : GenericUpgrade
    {
        public Fearlessness() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Fearlessness";
            Cost = 1;

            AvatarOffset = new Vector2(80, 0);

            UpgradeAbilities.Add(new FearlessnessAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities
{
    public class FearlessnessAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += FearlessnessAddDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= FearlessnessAddDiceModification;
        }

        private void FearlessnessAddDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.FearlessnessAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = HostShip
            };
            HostShip.AddAvailableActionEffect(newAction);
        }

    }
}

namespace ActionsList
{

    public class FearlessnessAction : GenericAction
    {

        public FearlessnessAction()
        {
            Name = EffectName = "Fearlessness";
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) return false;

            if (!Combat.ShotInfo.InArc) return false;

            Board.ShipShotDistanceInformation reverseShotInfo = new Board.ShipShotDistanceInformation(Combat.Defender, Combat.Attacker);
            if (!reverseShotInfo.InArc || reverseShotInfo.Range != 1) return false;

            return result;
        }

        public override int GetActionEffectPriority()
        {
            return 110;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.AddDice(DieSide.Success).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();
            callBack();
        }

    }

}