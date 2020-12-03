namespace MichMcb.CsExt.Workflow
{
	using System.Threading.Tasks;

	public interface IAsyncWorkAcceptor<TObj>
	{
		Task AcceptAsync(TObj obj);
	}
	public interface IAsyncWorkAcceptor<TObj, TResult>
	{
		Task<TResult> AcceptAsync(TObj obj);
	}
	public interface IAsyncWorkAcceptor<TObj1, TObj2, TResult>
	{
		Task<TResult> AcceptAsync(TObj1 obj1, TObj2 obj2);
	}
	public interface IAsyncWorkAcceptor<TObj1, TObj2, TObj3, TResult>
	{
		Task<TResult> AcceptAsync(TObj1 obj1, TObj2 obj2, TObj3 obj3);
	}
	public interface IAsyncWorkAcceptor<TObj1, TObj2, TObj3, TObj4, TResult>
	{
		Task<TResult> AcceptAsync(TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4);
	}
	public interface IAsyncWorkAcceptor<TObj1, TObj2, TObj3, TObj4, TObj5, TResult>
	{
		Task<TResult> AcceptAsync(TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4, TObj5 obj5);
	}
}
