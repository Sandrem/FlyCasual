using ActionsList;
using Arcs;
using Ship;
using System;
using System.Collections.Generic;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class HaorChallPrototype : VultureClassDroidFighter
    {
        public HaorChallPrototype()
        {
            PilotInfo = new PilotCardInfo(
                "Haor Chall Prototype",
                1,
                23,
                limited: 2,
                abilityType: typeof(Abilities.SecondEdition.HaorChallPrototypeAbility),
                pilotTitle: "Xi Char Offering"
            );

            ModelInfo.SkinName = "Gray";

            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

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
            Messages.ShowInfo(HostName + " can perform free action");

            GenericShip previousActiveShip = Selection.ThisShip;
            Selection.ChangeActiveShip(HostShip);

            List<GenericAction> actions = new List<GenericAction>() { new CalculateAction(), new TargetLockAction() };
            HostShip.AskPerformFreeAction(actions, delegate {
                Selection.ChangeActiveShip(previousActiveShip);
                Triggers.FinishTrigger();
            });
        }
    }
}
