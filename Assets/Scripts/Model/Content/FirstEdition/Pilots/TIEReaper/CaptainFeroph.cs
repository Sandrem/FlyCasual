using Tokens;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEReaper
    {
        public class CaptainFeroph : TIEReaper
        {
            public CaptainFeroph() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Feroph",
                    4,
                    24,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.CaptainFerophAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class CaptainFerophAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnImmediatelyAfterRolling -= CheckAbility;
        }

        private void CheckAbility(DiceRoll diceroll)
        {
            if (diceroll.Type == DiceKind.Defence && diceroll.CheckType == DiceRollCheckType.Combat && Combat.Attacker.Tokens.HasToken<JamToken>())
            {
                Messages.ShowInfo("Captain Feroph: Evade result is added");
                diceroll.AddDice(DieSide.Success).ShowWithoutRoll();
                diceroll.OrganizeDicePositions();
            }
        }
    }
}
