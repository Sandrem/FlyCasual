using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class BiggsDarklighter : XWing
        {
            public BiggsDarklighter() : base()
            {
                IsHidden = true;

                PilotName = "Biggs Darklighter";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/9/90/Biggs-darklighter.png";
                IsUnique = true;
                PilotSkill = 5;
                Cost = 25;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                Actions.OnCheckTargetIsLegal += CanPerformAttack;
            }

            public void CanPerformAttack(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender)
            {
                /*bool abilityIsActive = false;
                if (defender.ShipId != this.ShipId)
                {
                    if (defender.Owner.PlayerNo == this.Owner.PlayerNo)
                    {
                        if (Actions.GetRange(defender, this) <= 1)
                        {
                            if (Combat.SecondaryWeapon == null)
                            {
                                if (attacker.InPrimaryWeaponFireZone(this)) abilityIsActive = true;
                            }
                            else
                            {
                                if (Combat.SecondaryWeapon.IsShotAvailable(this)) abilityIsActive = true;
                            }
                        }
                    }
                }

                if (abilityIsActive)
                {
                    if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                    {
                        Game.UI.ShowError("Biggs DarkLighter: You cannot attack target ship");
                    }
                    result = false;
                }*/
            }
        }
    }
}
