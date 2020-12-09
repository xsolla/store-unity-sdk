namespace Xsolla.Demo
{
	public interface ILoginAuthorization : IStoreStringAction
	{
		void TryAuth(params object[] args);
	}
}
