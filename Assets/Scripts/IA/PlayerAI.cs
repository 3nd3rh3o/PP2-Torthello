using System;
using UnityEngine;

namespace Torthello
{
    public interface IPlayerAI 
    { 
        public Awaitable<int> GetBestMove();
    }
}