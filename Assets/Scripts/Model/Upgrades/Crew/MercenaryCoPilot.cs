using Upgrade;

namespace UpgradesList
{
    public class MercenaryCopilot : GenericUpgrade
    {
        public MercenaryCopilot() : base()
        {
            Type = UpgradeType.Crew;
            Name = ShortName = "Mercenary Copilot";
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionEffectsList += MercenaryCopilotActionEffect;
        }

        private void MercenaryCopilotActionEffect(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.MercenaryCopilotAction()
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
    public class MercenaryCopilotAction : ActionsList.GenericAction
    {

        public MercenaryCopilotAction()
        {
            Name = EffectName = "Mercenary Copilot";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                Board.ShipShotDistanceInformation shotInformation = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                if (shotInformation.Range == 3)
                {
                    result = true;
                }
            }
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 20;
            }

            return result;
        }

    }
}