using Abilities;
using Upgrade;
using Ship;

namespace UpgradesList
{
	public class Autoblaster : GenericSecondaryWeapon
	{
		public Autoblaster() : base()
		{
            Types.Add(UpgradeType.Cannon);

			Name = "Autoblaster";
			Cost = 5;

			MinRange = 1;
			MaxRange = 1;
			AttackValue = 3;

            //From Autoblaster turret
            UpgradeAbilities.Add(new AutoblasterAbility());
        }		

	}
}

namespace Abilities
{
    public class AutoblasterAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDefence += RegisterAutoblasterEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefence -= RegisterAutoblasterEffect;
        }

        private void RegisterAutoblasterEffect()
        {
            Combat.DiceRollAttack.CancelCritsFirst = true;
            if ((Combat.ChosenWeapon is UpgradesList.Autoblaster) ||
                (Combat.ChosenWeapon is UpgradesList.AutoblasterTurret))
            {
                foreach (Die die in Combat.DiceRollAttack.DiceList)
                {
                    if (die.Side == DieSide.Success)
                    {
                        die.IsUncancelable = true;
                    }
                }
            }
        }
    }
}