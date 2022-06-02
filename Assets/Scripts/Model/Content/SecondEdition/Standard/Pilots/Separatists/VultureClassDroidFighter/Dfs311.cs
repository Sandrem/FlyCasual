using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class Dfs311 : VultureClassDroidFighter
    {
        public Dfs311()
        {
            PilotInfo = new PilotCardInfo25
            (
                "DFS-311",
                "Scouting Drone",
                Faction.Separatists,
                1,
                3,
                10,
                true,
                abilityType: typeof(Abilities.SecondEdition.Dfs311Ability),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Missile,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/07/b9/07b9dde5-0302-4c31-b54e-92ef136400b1/swz31_dfs-311.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //At the start of the Engagment Phase you may transfer 1 of your calculate tokens to another friendly ship at range 0-3.
    public class Dfs311Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if (HostShip.Tokens.HasToken<Tokens.CalculateToken>())
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
            }
        }

        private void Ability(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasToken<Tokens.CalculateToken>() && TargetsForAbilityExist(FilterAbilityTarget))
            {
                Messages.ShowInfoToHuman(HostName + ": Select a ship to transfer 1 calculate token to");

                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    GetAiAbilityPriority,
                    HostShip.Owner.PlayerNo,
                    HostShip.PilotInfo.PilotName,
                    "Select a ship to transfer 1 calculate token to",
                    HostShip
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void SelectAbilityTarget()
        {
            HostShip.Tokens.TransferToken(typeof(Tokens.CalculateToken), TargetShip, SelectShipSubPhase.FinishSelection);
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            //TODO: when should the AI use this ability?   
            return 0;
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 0, 3);
        }
    }
}
