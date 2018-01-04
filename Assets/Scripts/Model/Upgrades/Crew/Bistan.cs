using Upgrade;
 
namespace UpgradesList
{
    public class Bistan : GenericUpgrade
    {
        public Bistan() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Bistan";
            Cost = 2;
			isUnique = true;
        }
 
        public override bool IsAllowedForShip(Ship.GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }
                              
        public override void AttachToShip(Ship.GenericShip host)
        {
           base.AttachToShip(host);
 
           host.AfterGenerateAvailableActionEffectsList += BistanActionEffect;
        }
 
        private void BistanActionEffect(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.BistanAction()
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
	// When attacking at Range 1-2, you may change one hit success to one critical hit
    public class BistanAction : ActionsList.GenericAction
    {
        public BistanAction()
        {
            Name = EffectName = "Bistan";
        }
 
        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }
 
        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack)
            {
                Board.ShipShotDistanceInformation shotInformation = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                if (shotInformation.Range <= 2)
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
