using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class Sigma6BoY : TIEInterceptor
        {
            public Sigma6BoY() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Sigma 6",
                    "Battle of Yavin",
                    Faction.Imperial,
                    4,
                    4,
                    0,
                    charges: 2,
                    isLimited: true,
                    abilityType: typeof(Sigma6BoYAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    isStandardLayout: true
                );

                AutoThrustersAbility oldAbility = (AutoThrustersAbility) ShipAbilities.First(n => n.GetType() == typeof(AutoThrustersAbility));
                //oldAbility.DeactivateAbility();
                ShipAbilities.Remove(oldAbility);
                ShipAbilities.Add(new SensitiveControlsBoYRealAbility());

                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.Daredevil));
                MustHaveUpgrades.Add(typeof(UpgradesList.SecondEdition.AfterBurners));

                ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/3/3f/Sigma6-battleofyavin.png";

                PilotNameCanonical = "sigma6-battleofyavin";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Sigma6BoYAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinishSuccessfully -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Board.IsOffTheBoard(ship)) return;

            if (ship.AssignedManeuver.Speed >= 3
                && HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskPerformSlamAction);
            }
        }

        private void AskPerformSlamAction(object sender, System.EventArgs e)
        {
            if (HostShip.State.Charges > 0)
            {
                HostShip.BeforeActionIsPerformed += PayChargeCost;

                HostShip.AskPerformFreeAction
                (
                    new SlamAction(canBePerformedAsFreeAction: true) { HostShip = HostShip },
                    CleanUp,
                    HostShip.PilotInfo.PilotName,
                    "You may spend 1 charge to perform an SLAM action"
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PayChargeCost(GenericAction action, ref bool isFreeAction)
        {
            HostShip.SpendCharge();
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= PayChargeCost;
            Triggers.FinishTrigger();
        }
    }
}