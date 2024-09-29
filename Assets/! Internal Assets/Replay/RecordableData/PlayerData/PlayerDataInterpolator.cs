using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;

internal class PlayerDataInterpolator : IInterpolator<PlayerData>
{
    public PlayerData Lerp(PlayerData start, PlayerData end, double t)
    {
        return PlayerData.Lerp(start, end, t);
    }
}