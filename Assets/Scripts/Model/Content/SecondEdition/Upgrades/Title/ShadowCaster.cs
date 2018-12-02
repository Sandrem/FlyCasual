using Arcs;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ShadowCaster : GenericUpgrade
    {
        public ShadowCaster() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Shadow Caster",
                UpgradeType.Title,
                cost: 6,
                isLimited: true,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.LancerClassPursuitCraft.LancerClassPursuitCraft)),
                abilityType: typeof(Abilities.SecondEdition.ShadowCasterAbility),
                seImageNumber: 152
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class ShadowCasterAbility : Abilities.FirstEdition.ShadowCasterAbility
    {
        protected override void CheckShadowCasterAbility()
        {
            if (IsAbilityUsed) return;

            if (!(Combat.ShotInfo.InArcByType(ArcType.Mobile) && Combat.ShotInfo.InArcByType(ArcType.Primary))) return;

            RegisterAbilityTrigger(TriggerTypes.OnAttackHit, AssignTractorBeamToken);
        }
    }
}