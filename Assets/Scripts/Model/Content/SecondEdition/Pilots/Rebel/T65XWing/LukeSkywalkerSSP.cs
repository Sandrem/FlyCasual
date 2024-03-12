using Abilities.SecondEdition;
using Content;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.T65XWing
    {
        public class LukeSkywalkerSSP : T65XWing
        {
            public LukeSkywalkerSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Luke Skywalker",
                    "Red Five",
                    Faction.Rebel,
                    5,
                    6,
                    0,
                    isLimited: true,
                    abilityType: typeof(LukeSkywalkerSSPAbility),
                    force: 2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech
                    },
                    tags: new List<Tags>
                    {
                        Tags.LightSide,
                        Tags.XWing
                    },
                    skinName: "Luke Skywalker",
                    isStandardLayout: true
                );

                MustHaveUpgrades.Add(typeof(InstinctiveAim));
                MustHaveUpgrades.Add(typeof(ProtonTorpedoes));
                MustHaveUpgrades.Add(typeof(R2D2));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/lukeskywalker-swz106.png";

                PilotNameCanonical = "lukeskywalker-swz106";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LukeSkywalkerSSPAbility : GenericAbility
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