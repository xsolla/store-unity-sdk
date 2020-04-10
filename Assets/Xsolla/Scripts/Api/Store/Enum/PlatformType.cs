using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
    public enum PlatformType
    {
		None,
        Xsolla,
        Standalone,
        Other
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE // UNITY_STANDALONE - for tests
        , GooglePlay
#endif
#if UNITY_IOS || UNITY_EDITOR || UNITY_STANDALONE
        , AppStore
#endif
#if UNITY_PS4 || UNITY_EDITOR || UNITY_STANDALONE
        , PlaystationNetwork
#endif
#if UNITY_XBOXONE || UNITY_EDITOR || UNITY_STANDALONE
        , XboxLive
#endif
#if UNITY_WII || UNITY_EDITOR || UNITY_STANDALONE
        , NintendoShop
#endif
    }

    public static class PlatformExtensions
	{
        public static string GetString(this PlatformType platform)
		{
			switch(platform) {
                case PlatformType.None: return string.Empty;
                case PlatformType.Xsolla: return Constants.Platform.XSOLLA;
#if UNITY_IOS || UNITY_EDITOR || UNITY_STANDALONE
                case PlatformType.AppStore: return Constants.Platform.APP_STORE;
#endif
#if UNITY_ANDROID || UNITY_EDITOR || UNITY_STANDALONE
                case PlatformType.GooglePlay: return Constants.Platform.GOOGLE_PLAY;
#endif
#if UNITY_WII || UNITY_EDITOR || UNITY_STANDALONE
                case PlatformType.NintendoShop: return Constants.Platform.NINTENDO_SHOP;
#endif
#if UNITY_PS4 || UNITY_EDITOR || UNITY_STANDALONE
                case PlatformType.PlaystationNetwork: return Constants.Platform.PLAYSTATION_NETWORK;
#endif
#if UNITY_XBOXONE || UNITY_EDITOR || UNITY_STANDALONE
                case PlatformType.XboxLive: return Constants.Platform.XBOX_LIVE;
#endif
                case PlatformType.Standalone: {
#if UNITY_ANDROID
						return Constants.Platform.ANDROID_STANDALONE;
#endif
#if UNITY_IOS
						return Constants.Platform.IOS_STANDALONE;
#endif
#if UNITY_STANDALONE
						return Constants.Platform.PC_STANDALONE;
#else
						return Constants.Platform.PC_OTHER;
#endif
                    }
                case PlatformType.Other: {
#if UNITY_ANDROID
						return Constants.Platform.ANDROID_OTHER;
#endif
#if UNITY_IOS
						return Constants.Platform.IOS_OTHER;
#endif
#if UNITY_STANDALONE
                        return Constants.Platform.PC_OTHER;
#else
						return Constants.Platform.PC_OTHER;
#endif
                    }
                default: return Constants.Platform.PC_OTHER;
            }
		}
    }
}
