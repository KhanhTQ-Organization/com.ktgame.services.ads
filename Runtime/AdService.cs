using com.ktgame.ads.core;
using com.ktgame.core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace com.ktgame.services.ads
{
    [Service(typeof(IAdService))]
    public class AdService : MonoBehaviour, IAdService
    {
        public int Priority => 0;
        
        public bool Initialized { get; set; }
        public IAdAdapter Ad { private set; get; }
        
        public UniTask OnInitialize(IArchitecture architecture)
        {
            return UniTask.CompletedTask;
        }
        
        public void SetPause(bool pause)
        {
            Ad?.SetPause(pause);
        }
    }
}
