//namespace MichMcb.CsExt.Events
//{
//	using System.Collections.Generic;
//	using System.Threading.Tasks;

//	public sealed class AsyncEventHandlerList<TObj>
//	{
//		public delegate Task AsyncEventHandler(object sender, TObj e);
//		private readonly HashSet<AsyncEventHandler> handlers;
//		public AsyncEventHandlerList()
//		{
//			handlers = new();
//		}
//		public void AddHandler(AsyncEventHandler handler)
//		{
//			handlers.Add(handler);
//		}
//		public bool RemoveHandler(AsyncEventHandler handler)
//		{
//			return handlers.Remove(handler);
//		}
//		public void Clear()
//		{
//			handlers.Clear();
//		}
//		public async Task InvokeAsync(object sender, TObj e)
//		{
//			Task[] tasks = new Task[handlers.Count];
//			int i = 0;
//			foreach (AsyncEventHandler h in handlers)
//			{
//				tasks[i++] = h.Invoke(sender, e);
//			}
//			await Task.WhenAll(tasks);
//		}
//	}
//}
