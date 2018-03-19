using Ship;
using Upgrade;
using System.Collections.Generic;
using Abilities;

namespace UpgradesList
{
    public class LightScykInterceptor : GenericUpgradeSlotUpgrade
    {        
        public LightScykInterceptor() : base()
        { 
            Types.Add(UpgradeType.Title);
            Name = "\"Light Scyk\" Interceptor";
            Cost = -2;

            UpgradeAbilities.Add(new AllDamageIsFaceUpAbility());
            UpgradeAbilities.Add(new TreatAllBanksAsGreenAbility());
            ForbiddenSlots = new List<UpgradeType> { UpgradeType.Modification };
        }        

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is Ship.M3AScyk.M3AScyk;
        }        
    }
}

namespace Abilities
{
    public class AllDamageIsFaceUpAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckFaceupCrit += HostShip_OnCheckFaceupCrit;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckFaceupCrit -= HostShip_OnCheckFaceupCrit;
        }

        private void HostShip_OnCheckFaceupCrit(ref bool faceUp)
        {
            faceUp = true;
        }
    }    
}
