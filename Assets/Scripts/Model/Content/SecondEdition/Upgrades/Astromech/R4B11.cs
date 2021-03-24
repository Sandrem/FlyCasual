using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class R4B11 : GenericUpgrade
    {
        public R4B11() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "R4-B11",
                UpgradeType.Astromech,
                cost: 3, 
                abilityType: typeof(Abilities.SecondEdition.R4B11Ability),
                restriction: new FactionRestriction(Faction.Scum)
            );
            
            ImageUrl = "https://i.imgur.com/fyETLhg.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform an attack, you may remove 1 orange or red token from the defender to reroll any number of defense dice
    public class R4B11Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}