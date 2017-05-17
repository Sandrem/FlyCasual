using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Players
{
    public enum PlayerNo
    {
        Player1,
        Player2
    }

    public enum PlayerType
    {
        Human,
        Ai
    }

    public partial class GenericPlayer
    {
        public PlayerType Type;
        public string Name;
        public PlayerNo PlayerNo;
        public int Id;
    }

}
