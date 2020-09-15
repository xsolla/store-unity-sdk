public interface ILoginAuthorization : IStoreStringAction
{
	void TryAuth(params object[] args);
}
