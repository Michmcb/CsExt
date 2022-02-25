namespace MichMcb.CsExt.Test.Events.SerialAsyncEvent_ParallelAsyncEvent
{
	using MichMcb.CsExt.Events;
	using System.Diagnostics;
	using System.Threading.Tasks;
	using Xunit;

	public static class Invoke
	{
		[Fact]
		public static async Task OneHandler()
		{
			foreach (IAsyncEvent<IntHolder> asyncEvent in new IAsyncEvent<IntHolder>[] { new SerialAsyncEvent<IntHolder>(), new ParallelAsyncEvent<IntHolder>() })
			{
				IntHolder t = new(500);
				asyncEvent.AddHandler(IntHolder.Run1);
				await asyncEvent.Invoke(null!, t);
				Assert.Equal(1, t.Value);
			}
		}
		[Fact]
		public static async Task TwoDifferentHandlers()
		{
			foreach (IAsyncEvent<IntHolder> asyncEvent in new IAsyncEvent<IntHolder>[] { new SerialAsyncEvent<IntHolder>(), new ParallelAsyncEvent<IntHolder>() })
			{
				IntHolder t = new(500);
				asyncEvent.AddHandler(IntHolder.Run1);

				await asyncEvent.Invoke(null, t);
				Assert.Equal(1, t.Value);

				asyncEvent.AddHandler(IntHolder.Run2);
				await asyncEvent.Invoke(null!, t);
				Assert.Equal(4, t.Value);
			}
		}
		[Fact]
		public static async Task SerialExecution()
		{
			IntHolder t = new(500);
			SerialAsyncEvent<IntHolder> asyncEvent = new();
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);

			Stopwatch sw = Stopwatch.StartNew();
			await asyncEvent.Invoke(null!, t);
			sw.Stop();
			Assert.True(sw.Elapsed.TotalMilliseconds >= t.Delay * asyncEvent.Handlers.Count);
		}
		[Fact]
		public static async Task ParallelExecution()
		{
			IntHolder t = new(500);
			ParallelAsyncEvent<IntHolder> asyncEvent = new();
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);
			asyncEvent.AddHandler(IntHolder.Run1);

			Stopwatch sw = Stopwatch.StartNew();
			await asyncEvent.Invoke(null!, t);
			sw.Stop();
			Assert.True(sw.Elapsed.TotalMilliseconds >= 500);
			Assert.True(sw.Elapsed.TotalMilliseconds < t.Delay * asyncEvent.Handlers.Count);
		}
	}
}
