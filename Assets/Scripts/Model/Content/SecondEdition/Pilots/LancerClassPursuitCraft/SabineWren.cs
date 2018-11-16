using ActionsList;
using Arcs;
using BoardTools;
using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class SabineWren : LancerClassPursuitCraft
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Sabine Wren",
                    3,
                    68,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.SabineWrenLancerPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 220;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class SabineWrenLancerPilotAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddSabinebility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddSabinebility;
        }

        private void AddSabinebility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new SabineWrenDiceModification());
        }

        private class SabineWrenDiceModification : GenericAction
        {
            public SabineWrenDiceModification()
            {
                Name = DiceModificationName = "Sabine Wren's ability";
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.AddDice(DieSide.Focus).ShowWithoutRoll();
                Combat.CurrentDiceRoll.OrganizeDicePositions();
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Defence)
                {
                    ShotInfo shotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
                    if (shotInfo.InArcByType(ArcTypes.Mobile)) result = true;
                }
                return result;
            }

            public override int GetDiceModificationPriority()
            {
                return 110;
            }
        }

    }
}