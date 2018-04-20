using Upgrade;
using Ship;
using ActionsList;
using UnityEngine;

namespace UpgradesList
{
    public class Finn : GenericUpgrade
    {
        public Finn() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Finn";
            Cost = 5;

            AvatarOffset = new Vector2(53, 0);

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += FinnActionEffect;
        }

        private void FinnActionEffect(GenericShip host)
        {
            GenericAction newAction = new FinnDiceModification()
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
    public class FinnDiceModification : GenericAction
    {

        public FinnDiceModification()
        {
            Name = EffectName = "Finn's ability";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.AddDice(DieSide.Blank).ShowWithoutRoll();
            Combat.CurrentDiceRoll.OrganizeDicePositions();

            callBack();
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            switch (Combat.AttackStep)
            {
                case CombatStep.Attack:
                    if ((Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass)) && (Combat.ShotInfo.InArc)) result = true;
                    break;
                case CombatStep.Defence:
                    Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
                    if (shotInfo.InArc) result = true;
                    break;
                default:
                    break;
            }

            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 110;

            return result;
        }

    }
}