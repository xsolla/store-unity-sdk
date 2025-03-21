namespace Xsolla.Core
{
	//TEXTREVIEW
	public class PlatformSpecificAppearance
	{
		/// <summary>
		/// Defines the type of Activity to use for payment UI appearance for the Android platform.
		/// </summary>
		public AndroidActivityType? AndroidActivityType;

		/// <summary>
		/// Defines the Payment UI appearance for the Web platform.
		/// </summary>
		public WebGlAppearance WebGlAppearance;
	}

	//TEXTREVIEW
	public class WebGlAppearance
	{
		/// <summary>
		/// Width of the widget. The default value is '740px'.
		/// </summary>
		public string widgetWidth;

		/// <summary>
		/// Height of the widget. The default value is '760px'.
		/// </summary>
		public string widgetHeight;

		/// <summary>
		/// Controls the vertical stacking order (z-index). The default value is 1000.
		/// </summary>
		public int? zIndex;

		/// <summary>
		/// Opacity of the overlay (from 0 to 1). The default value is 0.6.
		/// </summary>
		public float? overlayOpacity;

		/// <summary>
		/// Background color of the overlay in hex format. The default value is '#000000'.
		/// </summary>
		public string overlayBackgroundColor;

		/// <summary>
		/// If true, the widget frame cannot be closed. The default value is false.
		/// </summary>
		public bool? isModal;

		/// <summary>
		/// If true, clicking the overlay will close the widget. The default value is true.
		/// </summary>
		public bool? closeByClickOverlay;

		/// <summary>
		/// If true, pressing the 'ESC' key will close the widget. The default value is true.
		/// </summary>
		public bool? closeByKeyboardEscape;

		/// <summary>
		/// Background color of the widget in hex format. The default value is '#ffffff'.
		/// </summary>
		public string contentBackgroundColor;

		/// <summary>
		/// Margin around the widget frame. The default value is '10px'.
		/// </summary>
		public string contentMargin;

		/// <summary>
		/// Type of the animated loading spinner. Can be 'xsolla', 'round', 'none', or 'custom'. The default value is 'round'.
		/// </summary>
		public string spinnerType;

		/// <summary>
		/// Color of the spinner in hex format. The default value is '#cccccc'.
		/// </summary>
		public string spinnerColor;

		/// <summary>
		/// URL of the custom spinner. The default value is null.
		/// </summary>
		public string spinnerUrl;

		/// <summary>
		/// Rotation period of the custom spinner in seconds. The default value is 0.
		/// </summary>
		public float? spinnerRotationPeriod;
	}
}