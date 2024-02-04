using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AttackShuttle
    {
        public class EzraBridger : AttackShuttle
        {
            public EzraBridger() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Ezra Bridger",
                    "Spectre-6",
                    Faction.Rebel,
                    3,
                    4,
                    7,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.EzraBridgerPilotAbility),
                    force: 1,
                    tags: new List<Tags>
                    {
                        Tags.Spectre,
                        Tags.LightSide
                    },
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.ForcePower,
                        UpgradeType.Turret,
                        UpgradeType.Crew,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    seImageNumber: 36,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class EzraBridgerPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddEzraBridgerPilotAbilitySE;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddEzraBridgerPilotAbilitySE;
        }

        private void AddEzraBridgerPilotAbilitySE(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(new EzraBridgerActionSE());
        }

        private class EzraBridgerActionSE : ActionsList.GenericAction
        {
            public override string Name => HostShip.PilotInfo.PilotName;
            public override string DiceModificationName => HostShip.PilotInfo.PilotName;
            public override string ImageUrl => HostShip.ImageUrl;

            public EzraBridgerActionSE()
            {
                TokensSpend.Add(typeof(Tokens.ForceToken));
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
                Combat.Attacker.State.SpendForce(1, callBack);
            }

            public override bool IsDiceModificationAvailable()
            {
                if (HostShip.Tokens.HasToken(typeof(Tokens.StressToken)) && HostShip.Tokens.HasToken(typeof(Tokens.ForceToken)))
                    return true;

                return false;
            }

            public override int GetDiceModificationPriority()
            {
                if (Combat.CurrentDiceRoll.Focuses > 0)
                {
                    if (Combat.CurrentDiceRoll.Focuses > 1)
                        return 50;
                    else
                        return 45;
                }

                return 0;
            }
        }

    }
}