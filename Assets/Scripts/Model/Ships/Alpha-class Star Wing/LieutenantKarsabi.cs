using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tokens;
using SubPhases;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class LieutenantKarsabi : AlphaClassStarWing
        {
            public LieutenantKarsabi() : base()
            {
                PilotName = "Lieutenant Karsabi";
                PilotSkill = 5;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.LieutenantKarsabiAbility());
            }
        }
    }
}

namespace Abilities
{
    public class LieutenantKarsabiAbility : GenericAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnTokenIsAssigned += RegisterLieutenantKarsabiAbility;
        }

        private void RegisterLieutenantKarsabiAbility(Ship.GenericShip ship, System.Type tokenType)
        {
            if (tokenType == typeof(WeaponsDisabledToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, CheckStress);
            }
        }

        private void CheckStress(object sender, System.EventArgs e)
        {
            if (!HostShip.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(AlwaysUseByDefault, UseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            if (HostShip.HasToken(typeof(WeaponsDisabledToken))) HostShip.RemoveToken(typeof(WeaponsDisabledToken));
            HostShip.AssignToken(new StressToken(), DecisionSubPhase.ConfirmDecision);
        }
    }
}

