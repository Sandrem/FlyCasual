using ActionsList;
using Arcs;
using BoardTools;
using Ship;
using System.Collections;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.Mg100StarFortress
    {
        public class Vennie : Mg100StarFortress
        {
            public Vennie() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Vennie",
                    2,
                    67,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.VennieAbility) //,
                    //seImageNumber: 19
                );

                ModelInfo.SkinName = "Crimson";

                ImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/7/7d/Swz18_vennie_a3.png";
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
            ship.AddAvailableDiceModification(new VennieDiceModification());
        }

        private class VennieDiceModification : GenericAction
        {
            public VennieDiceModification()
            {
                Name = DiceModificationName = "Vennie's ability";
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.AddDice(DieSide.Focus).ShowWithoutRoll();
                Combat.CurrentDiceRoll.OrganizeDicePositions();
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