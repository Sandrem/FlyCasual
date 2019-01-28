using Actions;
using ActionsList;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YT2400LightFreighter
    {
        public class Leebo : YT2400LightFreighter
        {
            public Leebo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Leebo",
                    3,
                    88,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LeeboAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 78
                );

                ShipInfo.ActionIcons.SwitchToDroidActions();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LeeboAbility : GenericAbility
    {
        bool spentCalculate = false;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += CheckAssignCalculate;
            HostShip.OnTokenIsSpent += CheckCalculateSpent;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= CheckAssignCalculate;
            HostShip.OnTokenIsSpent -= CheckCalculateSpent;
        }

        private void CheckCalculateSpent(GenericShip ship, System.Type type)
        {
            if (Combat.AttackStep == CombatStep.None)
                return;

            if (HostShip != Combat.Attacker && HostShip != Combat.Defender)
                return;

            if (type != typeof(Tokens.CalculateToken))
                return;

            spentCalculate = true;
        }

        private void CheckAssignCalculate(GenericShip ship)
        {
            if (spentCalculate)
            {
                spentCalculate = false;
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Assign calculate to Leebo.",
                    TriggerType = TriggerTypes.OnAttackFinish,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = delegate {
                        HostShip.Tokens.AssignToken(new Tokens.CalculateToken(HostShip), Triggers.FinishTrigger);
                    }
                });
            }
        }
    }
}
