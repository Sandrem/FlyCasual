using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YT1300
    {
        public class Rey : YT1300
        {
            public Rey() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rey",
                    8,
                    45,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ReyPilotAbility),
                    extraUpgradeIcon: UpgradeType.Missile
                );

                ShipInfo.ArcInfo.Arcs.ForEach(a => a.Firepower = 3);
                ShipInfo.Hull = 8;
                ShipInfo.Shields = 5;

                ShipInfo.SubFaction = Faction.Resistance;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class ReyPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddDiceModification;
        }

        public void AddDiceModification(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ReyAction());
        }

        private class ReyAction : ActionsList.GenericAction
        {
            public ReyAction()
            {
                Name = DiceModificationName = "Rey's ability";
                IsReroll = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    SidesCanBeRerolled = new List<DieSide>() { DieSide.Blank },
                    NumberOfDiceCanBeRerolled = 2,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                switch (Combat.AttackStep)
                {
                    case CombatStep.Attack:
                        if (Combat.ShotInfo.InArc) result = true;
                        break;
                    case CombatStep.Defence:
                        ShotInfo shotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
                        if (shotInfo.InArc) result = true;
                        break;
                    default:
                        break;
                }

                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.CurrentDiceRoll.BlanksNotRerolled > 0) result = 95;

                return result;
            }

        }
    }
}