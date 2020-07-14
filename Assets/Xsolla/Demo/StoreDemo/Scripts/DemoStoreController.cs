public class DemoStoreController : StoreController
{
	protected override IDemoImplementation GetImplementation()
	{
		return DemoImplementation.Instance;
	}
}
