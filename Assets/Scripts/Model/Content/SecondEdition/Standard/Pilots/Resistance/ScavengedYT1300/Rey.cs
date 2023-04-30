using Content;
using Ship;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Rey",
                    "Resourceful Scavenger",
                    Faction.Resistance,
                    5,
                    8,
                    25,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.ReyPilotAbility),
                    force: 2,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Title,
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Freighter,
                        Tags.YT1300
                    }
                );
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
            ship.AddAvailableDiceModificationOwn(new ReyAction(ship) { ImageUrl = HostShip.ImageUrl });
        }

        private class ReyAction : ActionsList.GenericAction
        {
            public ReyAction(GenericShip ship)
            {
                Name = DiceModificationName = "Rey";

                TokensSpend.Add(typeof(ForceToken));
                HostShip = ship;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
                HostShip.Tokens.SpendToken(typeof(ForceToken), callBack);
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                switch (Combat.AttackStep)
                {
                    case CombatStep.Attack:
                        if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Front) && HostShip.State.Force > 0 && Combat.CurrentDiceRoll.Blanks > 0) result = true;
                        break;
                    case CombatStep.Defence:
                        if (HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Front) && HostShip.State.Force > 0 && Combat.CurrentDiceRoll.Blanks > 0) result = true;
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