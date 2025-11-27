namespace Xsolla.Core
{
	public class PlatformSpecificAppearance
	{
		/// <summary>
		/// Specifies the type of Activity used for the payment UI appearance on Android builds.
		/// </summary>
		public AndroidActivityType? AndroidActivityType;

		/// <summary>
		/// Specifies the payment UI appearance settings for the WebGL builds.
		/// The payment UI is presented via Pay Station.
		/// </summary>
		public WebGlAppearance WebGlAppearance;
	}

	public class WebGlAppearance
	{
		/// <summary>
		/// Open payment UI in iframe on all devices.
		/// </summary>
		public bool? iframeOnly;

		/// <summary>
		/// Width of the widget. Default is `740px`.
		/// </summary>
		public string widgetWidth;

		/// <summary>
		/// Height of the widget. Default is `760px`.
		/// </summary>
		public string widgetHeight;

		/// <summary>
		/// Controls the vertical stacking order (z-index). Default is `1000`.
		/// </summary>
		public int? zIndex;

		/// <summary>
		/// Opacity of the overlay (range: 0 to 1). Default is `0.6`.
		/// </summary>
		public float? overlayOpacity;

		/// <summary>
		/// Overlay background color in hex format. Default is `#000000`.
		/// </summary>
		public string overlayBackgroundColor;

		/// <summary>
		/// If `true`, the widget frame can't be closed. Default is `false`.
		/// </summary>
		public bool? isModal;

		/// <summary>
		/// If `true`, clicking on the overlay will close the widget. Default is `true`.
		/// </summary>
		public bool? closeByClickOverlay;

		/// <summary>
		/// If `true`, pressing the `ESC` key closes the widget. Default is `true`.
		/// </summary>
		public bool? closeByKeyboardEscape;

		/// <summary>
		/// Background color of the widget in hex format. Default is '#ffffff'.
		/// </summary>
		public string contentBackgroundColor;

		/// <summary>
		/// Margin around the widget frame. Default is '10px'.
		/// </summary>
		public string contentMargin;

		/// <summary>
		/// Type of animated loading spinner. Options: 'xsolla', 'round', 'none', or 'custom'. Default is 'round'.
		/// </summary>
		public string spinnerType;

		/// <summary>
		/// Spinner color in hex format. Default is '#cccccc'.
		/// </summary>
		public string spinnerColor;

		/// <summary>
		/// URL of the custom spinner. Default is null.
		/// </summary>
		public string spinnerUrl;

		/// <summary>
		/// Rotation period of the custom spinner in seconds. Default is 0.
		/// </summary>
		public float? spinnerRotationPeriod;
	}
}
