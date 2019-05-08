using Upgrade;
using System.Collections.Generic;
using Tokens;
using ActionsList;
using Ship;
using UnityEngine;

namespace UpgradesList.FirstEdition
{
    public class Rage : GenericUpgrade
    {
        public Rage() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Rage",
                UpgradeType.Talent,
                cost: 1,
                abilityType: typeof(Abilities.FirstEdition.RageAbility)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(53, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class RageAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += RageAddAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= RageAddAction;
        }

        private void RageAddAction(GenericShip host)
        {
            GenericAction action = new RageAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip
            };
            host.AddAvailableAction(action);
        }
    }
}


namespace ActionsList
{
    public class RageAction : GenericAction
    {

        public RageAction()
        {
            Name = DiceModificationName = "Rage";
        }

        public override void ActionTake()
        {
            HostShip = Selection.ThisShip;
            //Adding Rage reroll effect
            HostShip.OnGenerateDiceModifications += AddRageCondition;
            Phases.Events.OnEndPhaseStart_NoTriggers += RemoveRageCondition;

            //Rage Condition for reroll dices on each attach during this round
            Messages.ShowInfo("Rage has been activated");
            HostShip.Tokens.AssignCondition(typeof(Conditions.RageCondition));

            //Assigns one focus and two stress tokens
            Messages.ShowInfo("Rage has assigned a focus token to " + HostShip.PilotInfo.PilotName);
            HostShip.Tokens.AssignToken(typeof(FocusToken), delegate { assignStressTokensRecursively(2); });
        }


        //Assigns recursively stress tokens delegating its callback responsability on each call until last one
        private void assignStressTokensRecursively(int tokens)
        {

            if (tokens > 0)
            {
                tokens--;
                string message = (tokens == 0) ? "Rage has assigned " + HostShip.PilotName + " the second stress token." : "Rage has assigned" + HostShip.PilotName + " the first stress token.";
                Messages.ShowInfo(message);
                HostShip.Tokens.AssignToken(typeof(StressToken), delegate { assignStressTokensRecursively(tokens); });
            }
            else
            {
                Phases.CurrentSubPhase.CallBack();
            }
        }


        private void AddRageCondition(GenericShip ship)
        {
            ship.AddAvailableDiceModification(this);
        }


        private void RemoveRageCondition()
        {
            HostShip.Tokens.RemoveCondition(typeof(Conditions.RageCondition));
            HostShip.OnGenerateDiceModifications -= AddRageCondition;

            Phases.Events.OnEndPhaseStart_NoTriggers -= RemoveRageCondition;
        }


        public override bool IsDiceModificationAvailable()
        {
            return (Combat.AttackStep == CombatStep.Attack);
        }


        public override int GetDiceModificationPriority()
        {
            //TODO: More Complex
            return 20;
        }

        //Reroll is the effect of Rage
        public override void ActionEffect(System.Action callBack)
        {
            IsReroll = true; //These effect is a reroll effect

            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                NumberOfDiceCanBeRerolled = 3,
                CallBack = callBack
            };
            diceRerollManager.Start();
        }

    }

}


namespace Conditions
{

    public class RageCondition : GenericToken
    {
        public RageCondition(GenericShip host) : base(host)
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = new UpgradesList.FirstEdition.Rage().ImageUrl;
        }
    }

}
