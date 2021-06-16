namespace MichMcb.CsExt.Workflow
{
	using System;
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IWorkAcceptor<TObj>
	{
		void Accept(TObj obj);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IWorkAcceptor<TObj, TResult>
	{
		TResult Accept(TObj obj);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IWorkAcceptor<TObj1, TObj2, TResult>
	{
		TResult Accept(TObj1 obj1, TObj2 obj2);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IWorkAcceptor<TObj1, TObj2, TObj3, TResult>
	{
		TResult Accept(TObj1 obj1, TObj2 obj2, TObj3 obj3);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IWorkAcceptor<TObj1, TObj2, TObj3, TObj4, TResult>
	{
		TResult Accept(TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4);
	}
	[Obsolete("Since there's no concrete implementations of this there's not much point having it around")]
	public interface IWorkAcceptor<TObj1, TObj2, TObj3, TObj4, TObj5, TResult>
	{
		TResult Accept(TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4, TObj5 obj5);
	}
}
