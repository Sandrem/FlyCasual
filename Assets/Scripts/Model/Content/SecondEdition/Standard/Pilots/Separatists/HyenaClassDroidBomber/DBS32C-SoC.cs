using System.Collections.Generic;
using Upgrade;
using System.Linq;
using System;
using Ship;
using ActionsList;
using Actions;
using Content;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.HyenaClassDroidBomber
{
    public class DBS32CSoC : HyenaClassDroidBomber
    {
        public DBS32CSoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DBS-32C",
                "Siege of Coruscant",
                Faction.Separatists,
                3,
                3,
                0,
                charges: 2,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DBS32CSoCAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Torpedo,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                isStandardLayout: true
            );

            ShipInfo.ActionIcons.RemoveActions(typeof(ReloadAction));
            ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(JamAction), ActionColor.Red));

            MustHaveUpgrades.Add(typeof(PlasmaTorpedoes));
            MustHaveUpgrades.Add(typeof(ContingencyProtocol));
            MustHaveUpgrades.Add(typeof(StrutLockOverride));

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/4/43/Dbs32c-siegeofcoruscant.png";

            PilotNameCanonical = "dbs32c-siegeofcoruscant";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DBS32CSoCAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is CalculateAction && HostShip.State.Charges > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AskToPerformJamAction);
            }
        }

        private void AskToPerformJamAction(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += CheckSpendCharge;

            HostShip.AskPerformFreeAction
            (
                new JamAction(),
                FinishAbility,
                descriptionShort: HostShip.PilotInfo.PilotName,
                descriptionLong: "You may spend 1 Charge to perform a Jam action"
            );
        }

        private void FinishAbility()
        {
            HostShip.BeforeActionIsPerformed -= CheckSpendCharge;
            Triggers.FinishTrigger();
        }

        private void CheckSpendCharge(GenericAction action, ref bool data)
        {
            if (action is JamAction) HostShip.State.Charges--;
        }
    }
}