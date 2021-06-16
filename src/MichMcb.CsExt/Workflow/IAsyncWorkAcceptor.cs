namespace MichMcb.CsExt.Workflow
{
	using System;
	using System.Threading.Tasks;

	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IAsyncWorkAcceptor<TObj>
	{
		Task AcceptAsync(TObj obj);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IAsyncWorkAcceptor<TObj, TResult>
	{
		Task<TResult> AcceptAsync(TObj obj);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IAsyncWorkAcceptor<TObj1, TObj2, TResult>
	{
		Task<TResult> AcceptAsync(TObj1 obj1, TObj2 obj2);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IAsyncWorkAcceptor<TObj1, TObj2, TObj3, TResult>
	{
		Task<TResult> AcceptAsync(TObj1 obj1, TObj2 obj2, TObj3 obj3);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IAsyncWorkAcceptor<TObj1, TObj2, TObj3, TObj4, TResult>
	{
		Task<TResult> AcceptAsync(TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IAsyncWorkAcceptor<TObj1, TObj2, TObj3, TObj4, TObj5, TResult>
	{
		Task<TResult> AcceptAsync(TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4, TObj5 obj5);
	}
}
