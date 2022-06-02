using ActionsList;
using Arcs;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class HaorChallPrototype : VultureClassDroidFighter
    {
        public HaorChallPrototype()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Haor Chall Prototype",
                "Xi Char Offering",
                Faction.Separatists,
                1,
                2,
                4,
                limited: 2,
                abilityType: typeof(Abilities.SecondEdition.HaorChallPrototypeAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                skinName: "Gray"
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/04/05/0405890a-0f0a-444e-b9eb-8d92dbdf3d63/swz29_hadr-chall.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After an Enemy ship in your bullseye arc at range 0-2 declares another friendly ship as the defender, 
    //you may perform a calculate or lock action.
    public class HaorChallPrototypeAbility : GenericAbility
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
                && HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, ArcType.Bullseye)
                && new BoardTools.DistanceInfo(HostShip, Combat.Attacker).Range <= 2)
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
                "After an Enemy ship in your bullseye arc at range 0-2 declares another friendly ship as the defender, you may perform a Calculate or Lock action",
                HostShip
            );
        }
    }
}
