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
            HostShip.OnDefenceStartAsAttacker += RegisterAutoblasterEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsAttacker -= RegisterAutoblasterEffect;
        }

        private void RegisterAutoblasterEffect()
        {
            if (Combat.ChosenWeapon.GetType() == HostUpgrade.GetType())
            {
                Combat.DiceRollAttack.CancelCritsFirst = true;

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