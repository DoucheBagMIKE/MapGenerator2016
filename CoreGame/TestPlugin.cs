using UnityEngine;
using Contracts.Interfaces;

namespace CoreGame
{
    public class TestPlugin : IPlugin
    {
        public string Name
        {
            get
            {
                new GameObject("I Created a GameObject!");
                return "TestPlugin";
            }
        }
    }
}
