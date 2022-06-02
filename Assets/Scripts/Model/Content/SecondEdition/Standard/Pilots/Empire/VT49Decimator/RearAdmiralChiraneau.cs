using BoardTools;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.VT49Decimator
    {
        public class RearAdmiralChiraneau : VT49Decimator
        {
            public RearAdmiralChiraneau() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Rear Admiral Chiraneau",
                    "Advisor to Admiral Piett",
                    Faction.Imperial,
                    5,
                    8,
                    26,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.RearAdmiralChiraneauAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Title
                    },
                    seImageNumber: 147
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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

        private void AddRearAdmiralChiraneauPilotAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new ActionsList.SecondEdition.RearAdmiralChiraneauAction());
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class RearAdmiralChiraneauAction : GenericAction
    {
        public override string ImageUrl => new Ship.SecondEdition.VT49Decimator.RearAdmiralChiraneau().ImageUrl;

        public RearAdmiralChiraneauAction()
        {
            Name = DiceModificationName = "Rear Admiral Chiraneau";

            IsTurnsOneFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.DiceRollAttack.ChangeOne(DieSide.Focus, DieSide.Crit);
            callBack();
        }

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

        public override int GetDiceModificationPriority()
        {
            if (Combat.DiceRollAttack.Focuses > 0)
                return 100;

            return 0;
        }
    }
}