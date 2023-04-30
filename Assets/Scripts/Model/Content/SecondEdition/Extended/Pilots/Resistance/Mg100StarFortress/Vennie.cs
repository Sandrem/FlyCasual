using ActionsList;
using Arcs;
using BoardTools;
using Content;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class Vennie : Mg100StarFortress
        {
            public Vennie() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Vennie",
                    "Crimson Cutter",
                    Faction.Resistance,
                    2,
                    6,
                    14,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VennieAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Gunner,
                        UpgradeType.Device,
                        UpgradeType.Device,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Crimson";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VennieAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddVennieAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddVennieAbility;
        }

        private void AddVennieAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new VennieDiceModification() 
            {
                ImageUrl = HostShip.ImageUrl
            });
        }

        private class VennieDiceModification : GenericAction
        {
            public VennieDiceModification()
            {
                Name = DiceModificationName = "Vennie";
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.AddDiceAndShow(DieSide.Focus);
                callBack();
            }

            public override bool IsDiceModificationAvailable()
            {
                if (Combat.AttackStep == CombatStep.Defence)
                {
                    foreach (GenericShip friendlyShip in Combat.Defender.Owner.Ships.Values)
                    {
                        ShotInfo shotInfo = new ShotInfo(friendlyShip, Combat.Attacker, friendlyShip.PrimaryWeapons);
                        if (shotInfo.InArcByType(ArcType.SingleTurret)) return true;
                    }
                }
                return false;
            }

            public override int GetDiceModificationPriority()
            {
                return 110;
            }
        }

    }
}