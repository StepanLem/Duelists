using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace Assets.__Internal_Assets.Architecture.Gameplay
{
    public class GameplayEnterParams : SceneEnterParams
    {
        public GameplayMode Mode { get; }

        public GameplayEnterParams(GameplayMode mode) : base(Scenes.GAMEPLAY)
        {
            Mode = mode;
        }
    }
}
