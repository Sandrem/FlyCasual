using Arcs;
using BoardTools;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Hwk290LightFreighter
    {
        public class KananJarrus : Hwk290LightFreighter
        {
            public KananJarrus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Kanan Jarrus",
                    3,
                    42,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KananJarrusHwk290Ability),
                    extraUpgradeIcons: new List<UpgradeType>(){ UpgradeType.ForcePower, UpgradeType.Illicit },
                    force: 1,
                    factionOverride: Faction.Scum,
                    forceAlignmentOverride: ForceAlignment.Light
                );

                ImageUrl = "https://i.imgur.com/yDkh3To.png";

                ModelInfo.SkinName = "Black";

                PilotNameCanonical = "kananjarrus-hwk290lightfreighter";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KananJarrusHwk290Ability : Abilities.FirstEdition.KananJarrusPilotAbility
    {
        protected override void CheckPilotAbility()
        {
            bool hasForceTokens = HostShip.State.Force > 0;

            if (hasForceTokens && (IsDefenderMe() || IsDefenderInMyMobileArc()))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskDecreaseAttack);
            }
        }

        private bool IsDefenderMe()
        {
            return Combat.Defender.ShipId == HostShip.ShipId;
        }

        private bool IsDefenderInMyMobileArc()
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapons);
            return shotInfo.InArcByType(ArcType.SingleTurret);
        }

        protected override void AskDecreaseAttack(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                AlwaysUseByDefault,
                DecreaseAttack,
                descriptionLong: "Do you want to spend a force token? (If you do, the attacker rolls 1 fewer attack die)",
                imageHolder: HostShip
            );
        }

        protected override void DecreaseAttack(object sender, EventArgs e)
        {
            RegisterDecreaseNumberOfAttackDice();
            HostShip.State.SpendForce(1, DecisionSubPhase.ConfirmDecision);
        }
    }
}
