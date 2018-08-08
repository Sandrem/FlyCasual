using Ship;
using BoardTools;
using Abilities;
using ActionsList;
using Arcs;
using RuleSets;

namespace Ship
{
    namespace LancerClassPursuitCraft
    {
        public class SabineWren : LancerClassPursuitCraft, ISecondEditionPilot
        {
            public SabineWren() : base()
            {
                PilotName = "Sabine Wren";
                PilotSkill = 5;
                Cost = 35;

                IsUnique = true;

                PilotAbilities.Add(new SabineWrenLancerPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 68;
            }
        }
    }
}

namespace Abilities
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
