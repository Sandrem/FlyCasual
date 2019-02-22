using ActionsList;
using Arcs;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Belbullab22Starfighter
{
    public class CaptainSear : Belbullab22Starfighter
    {
        public CaptainSear()
        {
            PilotInfo = new PilotCardInfo(
                "Captain Sear",
                2,
                42,
                true,
                abilityType: typeof(Abilities.SecondEdition.CaptainSearAbility),
                pilotTitle: "Kage Infiltrator"
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/76/ba/76baabac-2258-4d60-9cf9-d7b0cdf0faeb/swz29_captain-sear.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While a friendly ship at range 0-3 performs a primary attack, if the defender is in its bullseye arc, before the Neutralize Results step, 
    //the friendly ship may spend 1 calculate token to cancel 1 evade result.
    public class CaptainSearAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsCompareResultsGlobal += DiceModification;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsCompareResultsGlobal -= DiceModification;
        }

        private void DiceModification(GenericShip ship)
        {
            GenericAction newAction = new CaptainSearDiceModification()
            {
                ImageUrl = HostImageUrl,
                HostShip = ship,
                CaptainSearShip = HostShip

            };
            ship.AddAvailableDiceModification(newAction);
        }

        private class CaptainSearDiceModification : GenericAction
        {
            public GenericShip CaptainSearShip;

            public CaptainSearDiceModification()
            {
                Name = DiceModificationName = "Captain Sear";
                DiceModificationTiming = DiceModificationTimingType.CompareResults;
            }

            public override int GetDiceModificationPriority()
            {
                int result = 0;

                if (Combat.DiceRollDefence.Successes <= Combat.DiceRollAttack.Successes) result = 100;

                return result;
            }

            public override bool IsDiceModificationAvailable()
            {
                bool result = false;

                if (Combat.DiceRollDefence.Successes > 0
                    && Combat.Attacker == HostShip
                    && HostShip.Tokens.HasToken<Tokens.CalculateToken>()
                    && HostShip.Owner == CaptainSearShip.Owner
                    && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon
                    && HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye)
                    && new BoardTools.DistanceInfo(CaptainSearShip, HostShip).Range <= 3
                )
                {
                    result = true;
                }

                return result;
            }

            public override void ActionEffect(Action callBack)
            {
                HostShip.Tokens.SpendToken(typeof(Tokens.CalculateToken), () =>
                {
                    Combat.DiceRollDefence.ChangeOne(DieSide.Success, DieSide.Blank, false);
                    callBack();
                });
            }

        }
    }
}
