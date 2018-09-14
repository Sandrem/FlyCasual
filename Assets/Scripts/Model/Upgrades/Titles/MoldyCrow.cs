using Ship;
using Ship.HWK290;
using Upgrade;
using Abilities;
using RuleSets;
using Abilities.SecondEdition;
using Arcs;

namespace UpgradesList
{
    public class MoldyCrow : GenericUpgrade, ISecondEditionUpgrade
    {
        public MoldyCrow() : base()
        {
            Types.Add(UpgradeType.Title);
            Name = "Moldy Crow";
            Cost = 3;

            isUnique = true;

            UpgradeAbilities.Add(new MoldyCrowAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 12;
            UpgradeAbilities.Clear();
            UpgradeAbilities.Add(new MoldyCrowAbilitySe());

            SEImageNumber = 104;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship is HWK290;
        }
    }
}

namespace Abilities
{
    public class MoldyCrowAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.BeforeRemovingTokenInEndPhase += KeepFocusTokens;
        }

        public override void DeactivateAbility()
        {
            HostShip.BeforeRemovingTokenInEndPhase -= KeepFocusTokens;
        }

        private void KeepFocusTokens(Tokens.GenericToken token, ref bool remove)
        {
            if (token is Tokens.FocusToken) remove = false;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MoldyCrowAbilitySe : GenericAbility
    {
        private int tokenCount = 0;

        public override void ActivateAbility()
        {
            HostShip.ChangeFirepowerBy(1);

            HostShip.ShipBaseArcsType = Arcs.BaseArcsType.ArcMobile; //This seems to work, but still prompt the user twice at the beginning
            HostShip.InitializeShipBaseArc();
            HostShip.SetShipInsertImage();

            HostShip.AfterGotNumberOfAttackDice += CheckWeakArc;
            HostShip.BeforeRemovingTokenInEndPhase += KeepTwoFocusTokens;
            Phases.Events.OnEndPhaseStart_NoTriggers += OnEndPhaseStart_NoTriggers;
        }

        private void OnEndPhaseStart_NoTriggers()
        {
            tokenCount = 0;
        }

        public override void DeactivateAbility()
        {
            HostShip.ChangeFirepowerBy(-1);
            HostShip.ArcInfo.Arcs.RemoveAll(arc => arc is ArcPrimary);
            HostShip.AfterGotNumberOfAttackDice -= CheckWeakArc;
            HostShip.BeforeRemovingTokenInEndPhase -= KeepTwoFocusTokens;
            Phases.Events.OnEndPhaseStart_NoTriggers -= OnEndPhaseStart_NoTriggers;
        }

        private void KeepTwoFocusTokens(Tokens.GenericToken token, ref bool remove)
        {
            if (tokenCount < 2) //We can only keep up to two focus tokens in Second Edition
            {
                if (token is Tokens.FocusToken) remove = false;
                tokenCount++;
            }
            else
            {
                if (token is Tokens.FocusToken) remove = true;
            }
        }

        private void CheckWeakArc(ref int count)
        {
            if (!Combat.ShotInfo.InArcByType(Arcs.ArcTypes.Primary)) count--;
        }
    }
}
