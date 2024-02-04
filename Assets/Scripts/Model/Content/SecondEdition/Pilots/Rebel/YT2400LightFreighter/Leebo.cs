using Content;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.YT2400LightFreighter
    {
        public class Leebo : YT2400LightFreighter
        {
            public Leebo() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Leebo",
                    "Dry-Witted Droid",
                    Faction.Rebel,
                    3,
                    8,
                    16,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.LeeboAbility),
                    tags: new List<Tags>
                    {
                        Tags.Droid,
                        Tags.Freighter
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 78,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
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

        private void CheckCalculateSpent(GenericShip ship, GenericToken token)
        {
            if (Combat.AttackStep == CombatStep.None)
                return;

            if (HostShip != Combat.Attacker && HostShip != Combat.Defender)
                return;

            if (!(token is CalculateToken))
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
                    EventHandler = AssignCalculateToken
                });
            }
        }
        private void AssignCalculateToken(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Calculate token is assigned to " + HostShip.PilotInfo.PilotName);
            HostShip.Tokens.AssignToken(new Tokens.CalculateToken(HostShip), Triggers.FinishTrigger);
        }
    }
}
