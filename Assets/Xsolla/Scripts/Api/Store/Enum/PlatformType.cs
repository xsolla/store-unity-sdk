using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Store
{
    public enum PlatformType
    {
		Unknown,
#if UNITY_ANDROID || UNITY_EDITOR
		GooglePlay,
#endif
#if UNITY_IOS || UNITY_EDITOR
        AppStore,
#endif
#if UNITY_PS4 || UNITY_EDITOR
		PlaystationNetwork,
#endif
#if UNITY_XBOXONE || UNITY_EDITOR
        XboxLive,
#endif
#if UNITY_WII || UNITY_EDITOR
        NintendoShop,
#endif
        Standalone,
		Other,
		Xsolla
    }

    public static class PlatformExtensions
	{
        public static string GetString(this PlatformType platform)
		{
			switch(platform) {
                case PlatformType.Unknown: return string.Empty;
                case PlatformType.Xsolla: return Constants.Platform.XSOLLA;
                case PlatformType.AppStore: return Constants.Platform.APP_STORE;
                case PlatformType.GooglePlay: return Constants.Platform.GOOGLE_PLAY;
                case PlatformType.NintendoShop: return Constants.Platform.NINTENDO_SHOP;
                case PlatformType.PlaystationNetwork: return Constants.Platform.PLAYSTATION_NETWORK;
                case PlatformType.XboxLive: return Constants.Platform.XBOX_LIVE;
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
