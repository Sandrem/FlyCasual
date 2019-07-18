using Abilities.SecondEdition;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
  namespace SecondEdition.NabooRoyalN1Starfighter
  {
    public class PadmeAmidala : NabooRoyalN1Starfighter
    {
      public PadmeAmidala() : base()
      {
        PilotInfo = new PilotCardInfo(
            "Padmé Amidala",
            4,
            45,
            isLimited: true,
            abilityText: "While an enemy ship in your [Front Arc] defends or performs an attack, that ship can modify only 1 [Focus] result (other results can still be modified).",
            abilityType: typeof(PadmeAmidalaAbility),
            extraUpgradeIcon: UpgradeType.Talent
        );

        ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/80/40/8040bcab-ebc0-487e-8dff-bd69da7311dd/swz40_padme-amidala.png";
      }
    }
  }
}

namespace Abilities.SecondEdition
{
  public class PadmeAmidalaAbility : GenericAbility
  {
    public override void ActivateAbility()
    {
      HostShip.AfterGotNumberOfPrimaryWeaponAttackDice += CheckAttackAbility;
      HostShip.AfterGotNumberOfDefenceDice += CheckDefensebility;
    }

    public override void DeactivateAbility()
    {
      HostShip.AfterGotNumberOfPrimaryWeaponAttackDice -= CheckAttackAbility;
      HostShip.AfterGotNumberOfDefenceDice -= CheckDefensebility;
    }

    private void CheckPadmeAbility()
    {
      // Rules.TargetIsLegalForShot.IsLegal(Selection.ThisShip, Selection.AnotherShip, ChosenWeapon, isSilent)
      //   ShotInfo shotInfo = new ShotInfo(thisShip, anotherShip, checkedWeapon)
      if (Combat.Defender.Owner != HostShip.Owner
          && GetFiringRangeAndShow(HostShip, Combat.Defender))
      {
        Messages.ShowInfo("Padmé Amidala: Defender can only modify 1 focus result");
        // apply defensive focus mod limitation
      }
      if (Combat.Attacker.Owner != HostShip.Owner
          && GetFiringRangeAndShow(HostShip, Combat.Attacker))
      {
        Messages.ShowInfo("Padmé Amidala: Attacker can only modify 1 focus result");
        // apply offensive focus mod limitation
      }
    }

    // private void CheckDefensebility(ref int count)
    // {
    //     if (HostShip.RevealedManeuver == null || Combat.Attacker.RevealedManeuver == null) return;

    //     if (HostShip.RevealedManeuver.Speed > Combat.Attacker.RevealedManeuver.Speed)
    //     {
    //         Messages.ShowInfo("Ric Olie: +1 defense die");
    //         count++;
    //     }
    // }
  }
}
