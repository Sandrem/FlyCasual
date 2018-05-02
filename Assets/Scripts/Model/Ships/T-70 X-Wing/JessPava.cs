using System;
using Ship;
using Movement;
using System.Linq;

namespace Ship
{
    namespace T70XWing
    {
        public class JessPava : T70XWing
        {
            public JessPava() : base()
            {
                PilotName = "Jess Pava";
                PilotSkill = 3;
                Cost = 25;

                IsUnique = true;                

                PilotAbilities.Add(new Abilities.JessPavaAbility());
            }
        }
    }
}

namespace Abilities
{
    public class JessPavaAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddJessPavaActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddJessPavaActionEffect;
        }

        private void AddJessPavaActionEffect(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.JessPavaActionEffect();
            newAction.Host = host;
            newAction.ImageUrl = host.ImageUrl;
            host.AddAvailableActionEffect(newAction);
        }        
    }
}

namespace ActionsList
{

    public class JessPavaActionEffect : GenericAction
    {
        public JessPavaActionEffect()
        {
            Name = EffectName = "Jess Pava";

            // Used for abilities like Dark Curse's that can prevent rerolls
            IsReroll = true;
        }

        private int getDices() {
            int dices = Roster.AllShips.Values.Where(ship => FilterTargets(ship)).Count();
            return dices;
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) ||
                (Combat.AttackStep == CombatStep.Defence))
            {                
                if (getDices() > 0) result = true;
            }
            return result;
        }

        public override int GetActionEffectPriority()
        {
            return 90;
        }

        private bool FilterTargets(GenericShip ship)
        {
            //Filter other friendly ships range 1
            Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Host, ship);
            return  ship.Owner.PlayerNo == Host.Owner.PlayerNo &&
                    ship != Host && 
                    distanceInfo.Range == 1;
        }

        public override void ActionEffect(System.Action callBack)
        {
            int dices = getDices();
            if (dices > 0)
            {
                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = dices,
                    CallBack = callBack
                };
                diceRerollManager.Start();
            }
        }

    }

}