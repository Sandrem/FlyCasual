using Abilities;
using Upgrade;

namespace UpgradesList
{

    public class AutoblasterTurret : GenericSecondaryWeapon
    {
        public AutoblasterTurret() : base()
        {
            Types.Add(UpgradeType.Turret);

            Name = "Autoblaster Turret";
            Cost = 2;

            MinRange = 1;
            MaxRange = 1;
            AttackValue = 2;

            CanShootOutsideArc = true;
            
            //From Cannon Autoblaster
            UpgradeAbilities.Add(new AutoblasterAbility());
        }
    }
}