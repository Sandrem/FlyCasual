﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;

namespace Ship
{
    public class ShipStateInfo
    {
        GenericShip HostShip;

        private int initiative;
        public int Initiative
        {
            get
            {
                int result = initiative;
                if (PilotSkillModifiers.Count > 0)
                {
                    for (int i = PilotSkillModifiers.Count - 1; i >= 0; i--)
                    {
                        PilotSkillModifiers[i].ModifyPilotSkill(ref result);
                    }
                }

                result = Mathf.Clamp(result, 0, 12);
                return result;
            }
            set
            {
                value = Mathf.Clamp(value, 0, 12);
                initiative = value;
            }
        }

        public List<IModifyPilotSkill> PilotSkillModifiers;

        public void AddPilotSkillModifier(IModifyPilotSkill modifier)
        {
            PilotSkillModifiers.Insert(0, modifier);
            Roster.UpdateShipStats(HostShip);
        }

        public void RemovePilotSkillModifier(IModifyPilotSkill modifier)
        {
            PilotSkillModifiers.Remove(modifier);
            Roster.UpdateShipStats(HostShip);
        }

        public int Firepower { get; set; }

        private int agility;
        public int Agility
        {
            get
            {
                int result = agility;
                result = Mathf.Max(result, 0);
                return result;
            }

            set
            {
                agility = value;
            }
        }

        private int hullMax;
        public int HullMax
        {
            get
            {
                int result = hullMax;
                HostShip.CallAfterGetMaxHull(ref result);
                return Mathf.Max(result, 1);
            }

            set
            {
                hullMax = Mathf.Max(value, 1);
            }
        }

        private int shieldsMax;
        public int ShieldsMax
        {
            get
            {
                int result = shieldsMax;
                return Mathf.Max(result, 0);
            }
            set
            {
                shieldsMax = Mathf.Max(value, 0);
            }
        }

        public int HullCurrent
        {
            get { return Mathf.Max(0, HullMax - HostShip.Damage.CountAssignedDamage()); }
        }

        public int ShieldsCurrent { get; set; }

        private int maxEnergy;
        public int MaxEnergy
        {
            get
            {
                int result = maxEnergy;
                return Mathf.Max(result, 0);
            }
            set
            {
                maxEnergy = Mathf.Max(value, 0);
            }
        }

        public int Energy
        {
            get
            {
                return HostShip.Tokens.CountTokensByType(typeof(EnergyToken));
            }
        }

        public int MaxForce { get; set; }

        public int Force
        {
            get
            {
                return HostShip.Tokens.CountTokensByType<ForceToken>();
            }

            set
            {
                UpdateTokens(value, typeof(ForceToken));
            }
        }

        private void UpdateTokens(int value, Type tokenType)
        {
            int currentTokens = HostShip.Tokens.CountTokensByType(tokenType);
            if (currentTokens > value)
            {
                for (int i = 0; i < currentTokens - value; i++)
                {
                    if (Phases.CurrentPhase is MainPhases.SetupPhase)
                    {
                        //skip triggers during Setup phase
                        HostShip.Tokens.RemoveCondition(tokenType);
                    }
                    else
                    {
                        HostShip.Tokens.RemoveToken(tokenType, delegate { });
                    }
                }
            }
            else if (value > currentTokens)
            {
                for (int i = 0; i < value - currentTokens; i++)
                {
                    if (Phases.CurrentPhase is MainPhases.SetupPhase)
                    {
                        //skip triggers during Setup phase
                        HostShip.Tokens.AssignCondition(tokenType);
                    }
                    else
                    {
                        HostShip.Tokens.AssignToken(tokenType, delegate { });
                    }
                }
            }
        }

        // TODO: Change/Remove so that this functionality isn't duplicated between GenericShip and GenericUpgrade

        public int MaxCharges { get; set; }

        private int charges;
        public int Charges
        {
            get { return charges; }
            set
            {
                UpdateTokens(value, typeof(ChargeToken));
                charges = value;
            }
        }

        public bool RegensCharges = false;

        public ShipStateInfo(GenericShip host)
        {
            HostShip = host;
        }

        public bool IsIonized
        {
            get { return RulesList.IonizationRule.IsIonized(this.HostShip); }
        }


    }
}
