using UnityEngine;
using System.Collections.Generic;
using SimplePlugin;
using PluginContracts;

public class GameManager : MonoBehaviour
{
    void Start ()
    {
        ICollection<IPlugin> plugins = GenericPluginLoader<IPlugin>.LoadPlugins(Application.dataPath + "/Plugins");
        foreach (var item in plugins)
        {
            item.Do();
        }

    }
}
