using Ship;
using Ship.ProtectorateStarfighter;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class ConcordDawnProtector : GenericUpgrade
    {
        public ConcordDawnProtector() : base()
        {
            Type = UpgradeType.Title;
            Name = "Concord Dawn Protector";
            Cost = 1;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is ProtectorateStarfighter;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += TryAddConcordDawnProtectorDiceModification;
        }

        private void TryAddConcordDawnProtectorDiceModification(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ConcordDawnProtectorDiceModification()
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
    public class ConcordDawnProtectorDiceModification : GenericAction
    {
        public ConcordDawnProtectorDiceModification()
        {
            Name = EffectName = "Concord Dawn Protector";
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            result = 110;

            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && Combat.ShotInfo.InArc)
            {
                Board.ShipDistanceInformation shipDistance = new Board.ShipDistanceInformation(Combat.Attacker, Combat.Defender);
                if (shipDistance.Range == 1)
                {
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
                    if (shotInfo.InArc)
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurentDiceRoll.AddDice(DieSide.Success).ShowWithoutRoll();
            Combat.CurentDiceRoll.OrganizeDicePositions();

            callBack();
        }
    }

}
