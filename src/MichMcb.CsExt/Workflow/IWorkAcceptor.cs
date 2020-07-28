namespace MichMcb.CsExt.Workflow
{
	public interface IWorkAcceptor<TObj>
	{
		void Accept(TObj obj);
	}
	public interface IWorkAcceptor<TObj, TResult>
	{
		TResult Accept(TObj obj);
	}
	public interface IWorkAcceptor<TObj1, TObj2, TResult>
	{
		TResult Accept(TObj1 obj1, TObj2 obj2);
	}
	public interface IWorkAcceptor<TObj1, TObj2, TObj3, TResult>
	{
		TResult Accept(TObj1 obj1, TObj2 obj2, TObj3 obj3);
	}
	public interface IWorkAcceptor<TObj1, TObj2, TObj3, TObj4, TResult>
	{
		TResult Accept(TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4);
	}
	public interface IWorkAcceptor<TObj1, TObj2, TObj3, TObj4, TObj5, TResult>
	{
		TResult Accept(TObj1 obj1, TObj2 obj2, TObj3 obj3, TObj4 obj4, TObj5 obj5);
	}
}
