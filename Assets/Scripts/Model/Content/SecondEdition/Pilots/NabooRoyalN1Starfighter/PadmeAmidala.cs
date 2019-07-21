using Abilities.SecondEdition;
using System.Collections;
using System.Collections.Generic;
using Upgrade;
using Ship;
using Conditions;
using Tokens;

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
      GenericShip.OnAttackStartAsAttackerGlobal += CheckPadmeAbilityAttacker;
      GenericShip.OnAttackStartAsDefenderGlobal += CheckPadmeAbilityDefender;
    }

    public override void DeactivateAbility()
    {
      GenericShip.OnAttackStartAsAttackerGlobal -= CheckPadmeAbilityAttacker;
      GenericShip.OnAttackStartAsDefenderGlobal -= CheckPadmeAbilityDefender;
      
    }
    public void CheckPadmeAbilityDefender()
    {
      CheckPadmeAbility(false);
    }
    
    public void CheckPadmeAbilityAttacker()
    {
      CheckPadmeAbility(true);
    }
    public void CheckPadmeAbility(bool isAttacker)
    {
      if (!isAttacker &&
          Combat.Defender.Owner != HostShip.Owner && 
          HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Front))
      {
        PadmeAmidalaCondition condition = new PadmeAmidalaCondition(Combat.Defender, HostShip);
        Combat.Defender.Tokens.AssignCondition(condition);
      }
      if (isAttacker &&
          Combat.Attacker.Owner != HostShip.Owner &&
          HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Front))
      {
        PadmeAmidalaCondition condition = new PadmeAmidalaCondition(Combat.Attacker, HostShip);
        Combat.Attacker.Tokens.AssignCondition(condition);
      }
    }
  }
}

namespace Conditions
{
    public class PadmeAmidalaCondition : GenericToken
    {
        bool FocusHasBeenModified = false;

        public PadmeAmidalaCondition(GenericShip host, GenericShip source) : base(host)
        {
            Name = ImageName = "Debuff Token";
            TooltipType = source.GetType();
            Temporary = false;
        }

        public override void WhenAssigned()
        {
            Messages.ShowInfo("Padmé Amidala: " + Host.PilotInfo.PilotName + " can only modify 1 focus result for this attack.");
            FocusHasBeenModified = false;
            
            Host.OnTryDiceResultModification += CheckIfCanChangeDie;
            Host.OnTrySelectDie += CheckIfCanSelectDie;

            Host.OnAttackFinishAsDefender += RemovePadmeAmidalaCondition;
            Host.OnAttackFinishAsAttacker += RemovePadmeAmidalaCondition;
        }

        public void CheckIfCanChangeDie(
          Die die, Abilities.GenericAbility.DiceModificationType modType, DieSide newResult, ref bool isAllowed
        )
        // Add focus modification limitation code here.
        {
          // set FocusHasBeenModified in some check in here
          if (FocusHasBeenModified == true && die.Side == DieSide.Focus)
          {
            isAllowed = false;
            Messages.ShowInfo("Padmé Amidala: Die modification is prevented");
          }
          else if (die.Side == DieSide.Focus)
          {
            FocusHasBeenModified = true;
          }
        }

        public void CheckIfCanSelectDie(
          Die die, ref bool isAllowed
        )
        {
          if (FocusHasBeenModified == true && die.Side == DieSide.Focus)
          {
            isAllowed = false;
            Messages.ShowErrorToHuman("Padmé Amidala: Unable to select focus results");
          }
          else if (die.Side == DieSide.Focus)
          {
            FocusHasBeenModified = true;
          }
        }
        public void RemovePadmeAmidalaCondition(GenericShip ship)
        {
            Host.Tokens.RemoveCondition(this);
        }

        public override void WhenRemoved()
        {
            Messages.ShowInfo("Padmé Amidala: " + Host.PilotInfo.PilotName + "'s ability to modify focus results restored");

            // remove focus modification limitation code here.
            Host.OnTryDiceResultModification -= CheckIfCanChangeDie;
            Host.OnTrySelectDie -= CheckIfCanSelectDie;

            Host.OnAttackFinishAsDefender -= RemovePadmeAmidalaCondition;
            Host.OnAttackFinishAsAttacker -= RemovePadmeAmidalaCondition;
        }
    }
}