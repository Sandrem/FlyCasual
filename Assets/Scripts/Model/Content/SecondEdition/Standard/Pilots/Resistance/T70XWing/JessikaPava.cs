using ActionsList;
using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.T70XWing
    {
        public class JessikaPava : T70XWing
        {
            public JessikaPava() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Jessika Pava",
                    "The Great Destroyer",
                    Faction.Resistance,
                    3,
                    5,
                    11,
                    isLimited: true,
                    charges: 1,
                    regensCharges: 1,
                    abilityType: typeof(Abilities.SecondEdition.JessikaPavaAbility),
                    tags: new List<Tags>
                    {
                        Tags.XWing
                    }
                );

                ImageUrl = "https://squadbuilder.fantasyflightgames.com/card_images/en/bc26d8864f421f1362473aa4982108ba.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JessikaPavaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddJessPavaActionEffects;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddJessPavaActionEffects;
        }

        private void AddJessPavaActionEffects(GenericShip host)
        {
            GenericAction actionPilot = new ActionsList.SecondEdition.JessPavaActionEffect()
            {
                HostShip = host,
                ImageUrl = host.ImageUrl
            };
            host.AddAvailableDiceModificationOwn(actionPilot);
        }
    }
}

namespace ActionsList.SecondEdition
{

    public class JessPavaActionEffect : ActionsList.FirstEdition.JessPavaActionEffect
    {
        public override bool IsDiceModificationAvailable()
        {
            bool baseResult = base.IsDiceModificationAvailable();

            return baseResult && (HostShip.State.Charges > 0 || GetAstromechWithNonRecurrungCharges() != null);
        }

        private GenericUpgrade GetAstromechWithNonRecurrungCharges()
        {
            GenericUpgrade astromechUpgrade = HostShip.UpgradeBar.GetInstalledUpgrade(UpgradeType.Astromech);
            if (astromechUpgrade != null)
            {
                if (astromechUpgrade.State.Charges > 0 && astromechUpgrade.UpgradeInfo.RegensChargesCount == 0) return astromechUpgrade;
            }

            return null;
        }

        protected override bool InRange(DistanceInfo distanceInfo)
        {
            return distanceInfo.Range <= 1;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (HostShip.State.Charges > 0)
            {
                HostShip.SpendCharge();
            }
            else
            {
                GenericUpgrade astromechUpgrade = GetAstromechWithNonRecurrungCharges();
                if (astromechUpgrade != null)
                {
                    Sounds.PlayShipSound("Astromech-Beeping-and-whistling");
                    astromechUpgrade.State.SpendCharge();
                }
                else
                {
                    Messages.ShowError("Error: This ship has no available charges to spend");
                }
            }

            base.ActionEffect(callBack);
        }

    }

}

namespace Abilities.FirstEdition
{
    public class JessPavaAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddJessPavaActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddJessPavaActionEffect;
        }

        private void AddJessPavaActionEffect(GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.FirstEdition.JessPavaActionEffect();
            newAction.HostShip = host;
            newAction.ImageUrl = host.ImageUrl;
            host.AddAvailableDiceModificationOwn(newAction);
        }
    }
}

namespace ActionsList.FirstEdition
{

    public class JessPavaActionEffect : GenericAction
    {
        public JessPavaActionEffect()
        {
            Name = DiceModificationName = "Jess Pava";

            // Used for abilities like Dark Curse's that can prevent rerolls
            IsReroll = true;
        }

        private int getDices()
        {
            int dices = Roster.AllShips.Values.Where(ship => FilterTargets(ship)).Count();
            return dices;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) ||
                (Combat.AttackStep == CombatStep.Defence))
            {
                if (getDices() > 0) result = true;
            }
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            return 90;
        }

        private bool FilterTargets(GenericShip ship)
        {
            //Filter other friendly ships range 1
            DistanceInfo distanceInfo = new DistanceInfo(HostShip, ship);
            return ship.Owner.PlayerNo == HostShip.Owner.PlayerNo &&
                    ship != HostShip &&
                    InRange(distanceInfo);
        }

        protected virtual bool InRange(DistanceInfo distanceInfo)
        {
            return distanceInfo.Range == 1;
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