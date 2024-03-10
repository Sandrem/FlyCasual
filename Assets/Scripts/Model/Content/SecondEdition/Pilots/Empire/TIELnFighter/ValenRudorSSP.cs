using Abilities.SecondEdition;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class ValenRudorSSP : TIELnFighter
        {
            public ValenRudorSSP() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Valen Rudor",
                    "Braggadocious Baron",
                    Faction.Imperial,
                    3,
                    3,
                    0,
                    isLimited: true,
                    abilityType: typeof(ValenRudorSSPAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                MustHaveUpgrades.Add(typeof(Disciplined));
                MustHaveUpgrades.Add(typeof(PrecisionIonEngines));

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/valenrudor-swz105.png";

                PilotNameCanonical = "valenrudor-swz105";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ValenRudorSSPAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += RegisterValenRudorSSPAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= RegisterValenRudorSSPAbility;
        }

        private void RegisterValenRudorSSPAbility(GenericShip ship)
        {
            var distanceInfo = new DistanceInfo(HostShip, Combat.Defender);
            if (distanceInfo.Range <= 1 && Combat.Defender.Owner == HostShip.Owner)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, PerformFreeAction);
            }
        }

        private void PerformFreeAction(object sender, System.EventArgs e)
        {
            var previousSelectedShip = Selection.ThisShip;
            Selection.ThisShip = HostShip;

            HostShip.AskPerformFreeAction(
                HostShip.GetAvailableActions(),
                delegate
                {
                    Selection.ThisShip = previousSelectedShip;
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "After a friendly ship at range 0-1 defends, you may perform an action",
                HostShip
            );
        }
    }
}
