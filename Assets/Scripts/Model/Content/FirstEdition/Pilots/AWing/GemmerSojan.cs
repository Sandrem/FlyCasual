﻿using BoardTools;
using Ship;
using Tokens;

namespace Ship
{
    namespace FirstEdition.AWing
    {
        public class GemmerSojan : AWing
        {
            public GemmerSojan() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gemmer Sojan",
                    5,
                    22,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.GemmerSojanAbility)
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class GemmerSojanAbility : GenericAbility
    {
        private bool AbilityIsActive;

        public override void ActivateAbility()
        {
            AbilityIsActive = false;
            GenericShip.OnPositionFinishGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnPositionFinishGlobal -= CheckAbility;
            TryDeactivateAbilityBonus();
        }

        private void CheckAbility(GenericShip shipChangedPosition)
        {
            //Only if host or enemy changed position;
            if (shipChangedPosition == HostShip || shipChangedPosition.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                foreach (GenericShip enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships.Values)
                {
                    DistanceInfo distInfo = new DistanceInfo(HostShip, enemyShip);
                    if (distInfo.Range == 1)
                    {
                        TryActivateAbilityBonus();
                        return;
                    }
                }

                //If no range 1 enemy ships
                TryDeactivateAbilityBonus();
            }
        }

        private void TryActivateAbilityBonus()
        {
            if (!AbilityIsActive)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " is within range 1 of at least one enemy ship and gains +1 Agility.");
                HostShip.ChangeAgilityBy(+1);
                HostShip.Tokens.AssignCondition(typeof(Conditions.GemmerSojanCondition));
                AbilityIsActive = true;
            }
        }

        private void TryDeactivateAbilityBonus()
        {
            if (AbilityIsActive)
            {
                Messages.ShowError(HostShip.PilotInfo.PilotName + ": -1 Agility");
                HostShip.ChangeAgilityBy(-1);
                HostShip.Tokens.RemoveCondition(typeof(Conditions.GemmerSojanCondition));
                AbilityIsActive = false;
            }
        }
    }
}

namespace Conditions
{
    public class GemmerSojanCondition : GenericToken
    {
        public GemmerSojanCondition(GenericShip host) : base(host)
        {
            Name = "Buff Token";
            Temporary = false;
            Tooltip = host.ImageUrl;
        }
    }
}