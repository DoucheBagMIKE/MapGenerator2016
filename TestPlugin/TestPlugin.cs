using UnityEngine;
using PluginContracts;

namespace TestPlugin
{
    public class TestPlugin : IPlugin
    {
        public string Name
        {
            get
            {
                return "TestPlugin";
            }
        }

        public void Do()
        {
            Debug.Log("Plugin Method Called!!!");
        }
    }
}
