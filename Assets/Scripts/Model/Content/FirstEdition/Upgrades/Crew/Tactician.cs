using Ship;
using Upgrade;
using System.Collections.Generic;
using UnityEngine;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class Tactician : GenericUpgrade
    {
        public Tactician() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tactician",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.TacticianAbility)
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(41, 1));
        }        
    }
}

namespace Abilities.FirstEdition
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
                Messages.ShowInfo(Combat.Defender.PilotInfo.PilotName + " gains a stress token from Tactician");
                Triggers.FinishTrigger();
            });
        }

    }
}
