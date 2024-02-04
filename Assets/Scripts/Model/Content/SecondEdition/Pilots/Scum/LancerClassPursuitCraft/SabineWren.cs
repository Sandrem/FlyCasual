using ActionsList;
using Arcs;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class SabineWren : LancerClassPursuitCraft
        {
            public SabineWren() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sabine Wren",
                    "Artistic Saboteur",
                    Faction.Scum,
                    3,
                    6,
                    9,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SabineWrenLancerPilotAbility),
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter,
                        Tags.Mandalorian
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 220,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                PilotNameCanonical = "sabinewren-lancerclasspursuitcraft";
            }
        }
    }
}

namespace Abilities.SecondEdition
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
            ship.AddAvailableDiceModificationOwn(new SabineWrenDiceModification()
            {
                ImageUrl = HostShip.ImageUrl
            });
        }

        private class SabineWrenDiceModification : GenericAction
        {
            public SabineWrenDiceModification()
            {
                Name = DiceModificationName = "Sabine Wren";
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.AddDiceAndShow(DieSide.Focus);
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;
                if (Combat.AttackStep == CombatStep.Defence)
                {
                    ShotInfo shotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapons);
                    if (shotInfo.InArcByType(ArcType.SingleTurret)) result = true;
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
