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
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Alpha-class%20Star%20Wing/lieutenant-karsabi.png";
                PilotSkill = 5;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilitiesList.Add(new PilotAbilities.LieutenantKarsabiAbility());
            }
        }
    }
}

namespace PilotAbilities
{
    public class LieutenantKarsabiAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.OnTokenIsAssigned += RegisterLieutenantKarsabiAbility;
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
            if (!Host.HasToken(typeof(StressToken)))
            {
                AskToUseAbility(IsUseByDefault, UseAbility);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool IsUseByDefault()
        {
            return true;
        }

        private void UseAbility(object sender, System.EventArgs e)
        {
            if (Host.HasToken(typeof(WeaponsDisabledToken))) Host.RemoveToken(typeof(WeaponsDisabledToken));
            Host.AssignToken(new StressToken(), DecisionSubPhase.ConfirmDecision);
        }
    }
}

