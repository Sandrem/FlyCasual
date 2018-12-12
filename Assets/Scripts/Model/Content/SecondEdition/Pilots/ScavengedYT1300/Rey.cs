using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.ScavengedYT1300
    {
        public class Rey : ScavengedYT1300
        {
            public Rey() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Rey",
                    5,
                    80,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ReyPilotAbility),
                    force: 2,
                    extraUpgradeIcon: UpgradeType.Force //,
                                                        //seImageNumber: 69
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f9/27/f9276fd7-52fd-4b05-8599-05039fa479e6/swz19_a2_reys-pilot.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
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
            ship.AddAvailableDiceModification(new ReyAction(ship));
        }

        private class ReyAction : ActionsList.GenericAction
        {
            public ReyAction(GenericShip ship)
            {
                Name = DiceModificationName = "Rey's Ability";
                TokensSpend.Add(typeof(ForceToken));
                HostShip = ship;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
                Combat.CurrentDiceRoll.OrganizeDicePositions();

                HostShip.Tokens.SpendToken(
                    typeof(ForceToken),
                    callBack
                );
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                switch (Combat.AttackStep)
                {
                    case CombatStep.Attack:
                        if (Combat.ShotInfo.InArc && HostShip.State.Force > 0 && Combat.CurrentDiceRoll.Blanks > 0) result = true;
                        break;
                    case CombatStep.Defence:
                        ShotInfo shotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
                        if (shotInfo.InArc && HostShip.State.Force > 0 && Combat.CurrentDiceRoll.Blanks > 0) result = true;
                        break;
                    default:
                        break;
                }

                return result;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;
                DiceRoll diceValues = Combat.CurrentDiceRoll;
                if (diceValues.Blanks > 0)
                {
                    if (diceValues.Focuses == 0 && diceValues.Blanks == 1)
                    {
                        result = 81;
                    }
                    else
                    {
                        result = 40;
                    }
                }

                return result;
            }

        }
    }
}