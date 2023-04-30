using ActionsList;
using Arcs;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;
using UpgradesList.SecondEdition;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class HaorChallPrototypeSoC : VultureClassDroidFighter
    {
        public HaorChallPrototypeSoC()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Haor Chall Prototype",
                "Siege of Coruscant",
                Faction.Separatists,
                1,
                2,
                0,
                limited: 2,
                abilityType: typeof(Abilities.SecondEdition.HaorChallPrototypeSoCAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                skinName: "Gray",
                isStandardLayout: true
            );

            MustHaveUpgrades.Add(typeof(IonMissiles));
            MustHaveUpgrades.Add(typeof(ContingencyProtocol));
            MustHaveUpgrades.Add(typeof(StrutLockOverride));

            ImageUrl = "https://static.wikia.nocookie.net/xwing-miniatures-second-edition/images/5/53/Haorchallprototype-siegeofcoruscant.png";

            PilotNameCanonical = "haorchallprototype-siegeofcoruscant";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After an Enemy ship in your bullseye arc at range declares another friendly ship as the defender, 
    //you may perform a calculate or lock action.

    public class HaorChallPrototypeSoCAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackStartAsAttackerGlobal -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (Combat.Defender.Owner == HostShip.Owner
                && Combat.Defender != HostShip
                && HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, ArcType.Bullseye))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackStart, AskPerformAction);
            }
        }

        private void AskPerformAction(object sender, EventArgs e)
        {
            GenericShip previousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);

            List<GenericAction> actions = new List<GenericAction>() { new CalculateAction(), new TargetLockAction() };
            HostShip.AskPerformFreeAction(
                actions,
                delegate {
                    Selection.ChangeActiveShip(previousActiveShip);
                    Triggers.FinishTrigger();
                },
                HostShip.PilotInfo.PilotName,
                "After an Enemy ship in your bullseye arc declares another friendly ship as the defender, you may perform a Calculate or Lock action"
            );
        }
    }
}
