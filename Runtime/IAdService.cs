using com.ktgame.core;
using com.ktgame.ads.core;

namespace com.ktgame.services.ads
{
	public interface IAdService : IService, IInitializable
	{
		IAdAdapter Ad { get; }
		IAdAdapter AdBackFill { get; }
		void SetPause(bool pause);
	}
}
