using ActionsList;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.HMPDroidGunship
    {
        public class OnderonOppressor : HMPDroidGunship
        {
            public OnderonOppressor() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Onderon Oppressor",
                    3,
                    40,
                    limited: 2,
                    extraUpgradeIcons: new List<UpgradeType> { UpgradeType.Crew, UpgradeType.Device },
                    abilityType: typeof(Abilities.SecondEdition.OnderonOppressorAbility)
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d7/74/d774cd1d-eadc-4cf4-bf7f-f8169f9d14a3/swz71_card_oppressor.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class OnderonOppressorAbility : CombinedAbility
    {
        public override List<Type> CombinedAbilities => new List<Type>()
        {
            typeof(OnderonOppressorBarrelRollAbility),
            typeof(OnderonOppressorSideslipAbility),
        };

        private class OnderonOppressorBarrelRollAbility : TriggeredAbility
        {
            public override TriggerForAbility Trigger => new AfterYouPerformAction
            (
                actionType: typeof(BarrelRollAction),
                hasToken: typeof(StressToken)
            );

            public override AbilityPart Action => new AssignTokenAction
            (
                typeof(CalculateToken),
                targetShip: GetThisShip,
                showMessage: GetMessage
            );

            private GenericShip GetThisShip()
            {
                return HostShip;
            }

            private string GetMessage()
            {
                return "Onderon Oppressor: Gained Calculate token";
            }
        }

        private class OnderonOppressorSideslipAbility : TriggeredAbility
        {
            public override TriggerForAbility Trigger => new AfterManeuver
            (
                onlyIfBearing: ManeuverBearing.SideslipAny,
                hasToken: typeof(StressToken)
            );

            public override AbilityPart Action => new AssignTokenAction
            (
                typeof(CalculateToken),
                targetShip: GetThisShip,
                showMessage: GetMessage
            );

            private GenericShip GetThisShip()
            {
                return HostShip;
            }

            private string GetMessage()
            {
                return "Onderon Oppressor: Gained Calculate token";
            }
        }
    }
}