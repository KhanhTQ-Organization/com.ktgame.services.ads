using com.ktgame.core;

namespace com.ktgame.services.ads
{
	public class AdServiceSettings : ServiceSettingsSingleton<AdServiceSettings>
	{
		public override string PackageName => GetType().Namespace;
	}
}
