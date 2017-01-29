using UnityEngine;
using PluginContracts;
using System;

namespace PluginTest
{
    public class PluginTest : IPlugin
    {
        public string Name
        {
            get
            {
                return "PluginTest";
            }
        }

        public void Do()
        {
            Debug.Log("Plugin Code Called!");
            GameObject go = new GameObject("Test");
        }
    }
}
