using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class LukeSkywalker : T65XWing
        {
            public LukeSkywalker() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Luke Skywalker",
                    "Red Five",
                    Faction.Rebel,
                    5,
                    6,
                    22,
                    isLimited: true,
                    abilityType: typeof(LukeSkywalkerAbility),
                    force: 2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.LightSide,
                        Tags.XWing
                    },
                    seImageNumber: 2,
                    skinName: "Luke Skywalker"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LukeSkywalkerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnDefenceStartAsDefender += RecoverForce;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDefenceStartAsDefender -= RecoverForce;
        }

        private void RecoverForce()
        {
            if (HostShip.State.Force < HostShip.State.MaxForce)
            {
                HostShip.State.RestoreForce();
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " recovered 1 Force");
            }
        }
    }
}