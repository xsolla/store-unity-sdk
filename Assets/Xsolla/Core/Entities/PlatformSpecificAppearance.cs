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
		/// Width of widget frame. Default is "740px"
		/// </summary>
		public string widgetWidth;

		/// <summary>
		/// Height of widget frame. Default is "760px"
		/// </summary>
		public string widgetHeight;

		/// <summary>
		/// Property controls the vertical stacking order, default is 1000
		/// </summary>
		public int? zIndex;

		/// <summary>
		/// Opacity of the overlay (from 0 to 1), default is '0.6'
		/// </summary>
		public float? overlayOpacity;

		/// <summary>
		/// Background of the overlay, default is '#000000'
		/// </summary>
		public string overlayBackgroundColor;

		/// <summary>
		/// Widget frame cannot be closed, default false
		/// </summary>
		public bool? isModal;

		/// <summary>
		/// Toggle if clicking the overlay should close lightbox, default true
		/// </summary>
		public bool? closeByClickOverlay;

		/// <summary>
		/// Toggle if pressing of ESC key should close lightbox, default true
		/// </summary>
		public bool? closeByKeyboardEscape;

		/// <summary>
		/// Background of the frame, default is '#ffffff'
		/// </summary>
		public string contentBackgroundColor;

		/// <summary>
		/// Margin around frame, default '10px',
		/// </summary>
		public string contentMargin;

		/// <summary>
		/// Type of animated loading spinner, can be 'xsolla', 'round', 'none' or 'custom', default is "round"
		/// </summary>
		public string spinnerType;

		/// <summary>
		/// Color of the spinner, default is '#cccccc'
		/// </summary>
		public string spinnerColor;

		/// <summary>
		/// URL of custom spinner, default is null
		/// </summary>
		public string spinnerUrl;

		/// <summary>
		///  Rotation period of custom spinner, default 0
		/// </summary>
		public float? spinnerRotationPeriod;
	}
}