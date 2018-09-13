using RuleSets;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace Ship
{
    namespace M3AScyk
    {
        public class GenesisRed : M3AScyk, ISecondEditionPilot
        {
            public GenesisRed() : base()
            {
                PilotName = "Genesis Red";
                PilotSkill = 7;
                Cost = 19;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.GenesisRedAbility());

                SkinName = "Genesis Red";
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 35;

                PilotAbilities.RemoveAll(ability => ability is Abilities.GenesisRedAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.GenesisRedAbilitySE());

                SEImageNumber = 184;
            }
        }
    }
}

namespace Abilities
{
    //After you aquire a target lock, assign focus and evade tokens to your ship until you have the same number of each token as the locked ship.
    public class GenesisRedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTargetLockIsAcquired += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTargetLockIsAcquired -= RegisterTrigger;
        }

        private void RegisterTrigger(Ship.GenericShip target)
        {
            RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, delegate { GenesisRedAbilityEffect(target); });
        }

        private void GenesisRedAbilityEffect(Ship.GenericShip target)
        {
            AssignTokensIfLessThanTarget(target, typeof(Tokens.FocusToken), () => new Tokens.FocusToken(HostShip), delegate
            {
                AssignTokensIfLessThanTarget(target, typeof(Tokens.EvadeToken), () => new Tokens.EvadeToken(HostShip), Triggers.FinishTrigger);
            });
        }

        private void AssignTokensIfLessThanTarget(Ship.GenericShip target, Type tokenType, Func<Tokens.GenericToken> createToken, Action callback)
        {
            if (HostShip.Tokens.CountTokensByType(tokenType) < target.Tokens.CountTokensByType(tokenType))
            {
                HostShip.Tokens.AssignToken(createToken(), delegate { AssignTokensIfLessThanTarget(target, tokenType, createToken, callback); });
            }
            else
            {
                callback();
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GenesisRedAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTargetLockIsAcquired += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTargetLockIsAcquired -= RegisterTrigger;
        }

        private void RegisterTrigger(Ship.GenericShip target)
        {
            RegisterAbilityTrigger(TriggerTypes.OnTargetLockIsAcquired, delegate { GenesisRedAbilityEffect(target); });
        }

        private void GenesisRedAbilityEffect(Ship.GenericShip target)
        {
            List<GenericToken> targetTokens = target.Tokens.GetAllTokens();
            int focusTokens = targetTokens.Where(token => token is FocusToken).Count();
            int evadeTokens = targetTokens.Where(token => token is EvadeToken).Count();

            // welcome to hell
            HostShip.Tokens.RemoveAllTokensByType(typeof(FocusToken), delegate
            {
                HostShip.Tokens.RemoveAllTokensByType(typeof(EvadeToken), delegate
                {
                    HostShip.Tokens.AssignTokens(
                        () => new FocusToken(HostShip),
                        focusTokens,
                        delegate
                        {
                            HostShip.Tokens.AssignTokens(
                                () => new EvadeToken(HostShip),
                                evadeTokens,
                                Triggers.FinishTrigger);
                        });
                });
            });
        }
    }
}