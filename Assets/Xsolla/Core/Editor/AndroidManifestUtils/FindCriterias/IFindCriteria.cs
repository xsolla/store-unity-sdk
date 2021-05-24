namespace Xsolla.Core
{
	public interface IFindCriteria<in T>
	{
		bool MatchesCriteria(T obj);
	}
}