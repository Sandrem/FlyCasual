using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;
using RuleSets;
using Tokens;
using BoardTools;

namespace Ship
{
    namespace VT49Decimator
    {
        public class RearAdmiralChiraneau : VT49Decimator, ISecondEditionPilot 
        {
            public RearAdmiralChiraneau() : base()
            {
                PilotName = "Rear Admiral Chiraneau";
                PilotSkill = 8;
                Cost = 46;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new RearAdmiralChiraneauAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 5;
                Cost = 88;

                PilotAbilities.RemoveAll(ability => ability is Abilities.RearAdmiralChiraneauAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.RearAdmiralCharaneauAbilitySE());
            }
        }
    }
}

namespace ActionsList
{

    public class RearAdmiralChiraneauAction : GenericAction
    {
        public RearAdmiralChiraneauAction()
        {
            Name = DiceModificationName = "Rear Admiral Chiraneau's ability";
            IsTurnsOneFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(DieSide.Focus, DieSide.Crit);
            callBack();
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if ((Combat.AttackStep == CombatStep.Attack) && (Combat.ShotInfo.Range < 3)) result = true;
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            if (Combat.DiceRollAttack.Focuses > 0)
                return 100;

            return 0;
        }
    }

    public class RearAdmiralChiraneauSEAction : RearAdmiralChiraneauAction
    {
        public override bool IsDiceModificationAvailable()
        {
            if (Combat.AttackStep != CombatStep.Attack)
                return false;

            GenericReinforceToken rtoken = Combat.Attacker.Tokens.GetToken<GenericReinforceToken>();
            if (rtoken == null)
                return false;
        
            if (!Board.IsShipInFacing(Combat.Attacker, Combat.Defender, rtoken.Facing))
                return false;

            if (Combat.DiceRollAttack.Focuses == 0)
                return false;

            return true;
        }
    }
}

namespace Abilities
{
    public class RearAdmiralChiraneauAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddRearAdmiralChiraneauPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddRearAdmiralChiraneauPilotAbility;
        }

        protected virtual void AddRearAdmiralChiraneauPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ActionsList.RearAdmiralChiraneauAction());
        }
    }
}

namespace Abilities.SecondEdition
{
    public class RearAdmiralCharaneauAbilitySE : RearAdmiralChiraneauAbility
    {
        protected override void AddRearAdmiralChiraneauPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ActionsList.RearAdmiralChiraneauSEAction() { Host = HostShip });
        }
    }
}