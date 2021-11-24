namespace Xsolla.Demo
{
    public partial class DemoMarker
    {
		private static DemoMarker _marker;
		public static DemoMarker Marker
		{
			get
			{
				if (_marker == null)
					_marker = new DemoMarker();

				return _marker;
			}
		}

		public static bool IsStorePartAvailable
		{
			get
			{
				return (Marker is IStoreDemoMarker);
			}
		}
		public static bool IsInventoryPartAvailable
		{
			get
			{
				return (Marker is IInventoryDemoMarker);
			}
		}
		public static bool IsLoginPartAvailable
		{
			get
			{
				return (Marker is ILoginDemoMarker);
			}
		}

		public static bool IsStoreDemo
		{
			get
			{
				return IsStorePartAvailable && IsInventoryPartAvailable && IsLoginPartAvailable;
			}
		}
		public static bool IsInventoryDemo
		{
			get
			{
				return !IsStorePartAvailable && IsInventoryPartAvailable && IsLoginPartAvailable;
			}
		}
		public static bool IsLoginDemo
		{
			get
			{
				return !IsStorePartAvailable && !IsInventoryPartAvailable && IsLoginPartAvailable;
			}
		}
	}
}
