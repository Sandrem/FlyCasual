using Ship;
using UnityEngine;
using Upgrade;
using Abilities;
using Tokens;

namespace UpgradesList
{
    public class Tactician : GenericUpgrade
    {
        public Tactician() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Tactician";
            Cost = 2;

            isLimited = true;

            AvatarOffset = new Vector2(41, 1);

            UpgradeAbilities.Add(new TacticianAbility());
        }
    }
}

namespace Abilities
{
    public class TacticianAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= RegisterTrigger;
        }

        private void RegisterTrigger(GenericShip ship)
        {
            if (Combat.Attacker == HostShip && Combat.ShotInfo.InArc && Combat.ShotInfo.Range == 2)
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Tactician's ability",
                    TriggerType = TriggerTypes.OnAttackFinish,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = DoTacticianAbility
                });
        }

        private void DoTacticianAbility(object sender, System.EventArgs e)
        {
            Combat.Defender.Tokens.AssignToken(typeof(StressToken), delegate
            {
                Messages.ShowInfo("Defender gained stress from Tactician");
                Triggers.FinishTrigger();
            });
        }

    }
}
