using System;
using UnityEngine;

namespace Scenes
{
    public static class SceneName
    {
        public const string BOOT = "Boot";
        public const string MAIN_MENU = "MainMenu";
        public const string GAMEPLAY = "Gameplay";

        public static bool IsCorrectName(string name)
        {
            return name == BOOT || 
                   name == MAIN_MENU || 
                   name == GAMEPLAY;
        }
    }
}
