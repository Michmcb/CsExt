namespace MichMcb.CsExt.Test.Events.SerialAsyncEvent_ParallelAsyncEvent
{
	using MichMcb.CsExt.Events;
	using Xunit;
	public static class AddHandler_RemoveHandler
	{
		[Fact]
		public static void AddHandler()
		{
			// C# events do allow you to add the same handler multiple times, and this behaviour is consistent with that
			foreach (IAsyncEvent<IntHolder> asyncEvent in new IAsyncEvent<IntHolder>[] { new SerialAsyncEvent<IntHolder>(), new ParallelAsyncEvent<IntHolder>() })
			{
				IntHolder t = new(0);
				asyncEvent.AddHandler(IntHolder.Run1);
				asyncEvent.AddHandler(IntHolder.Run1);
				asyncEvent.AddHandler(IntHolder.Run1);

				Assert.Collection(asyncEvent.Handlers,
					x => Assert.Equal(x, IntHolder.Run1),
					x => Assert.Equal(x, IntHolder.Run1),
					x => Assert.Equal(x, IntHolder.Run1));
			}
		}
		[Fact]
		public static void RemoveHandler()
		{
			foreach (IAsyncEvent<IntHolder> asyncEvent in new IAsyncEvent<IntHolder>[] { new SerialAsyncEvent<IntHolder>(), new ParallelAsyncEvent<IntHolder>() })
			{
				IntHolder t = new(0);
				asyncEvent.AddHandler(IntHolder.Run1);
				asyncEvent.AddHandler(IntHolder.Run2);

				Assert.Collection(asyncEvent.Handlers,
					x => Assert.Equal(x, IntHolder.Run1),
					x => Assert.Equal(x, IntHolder.Run2));

				asyncEvent.RemoveHandler(IntHolder.Run1);
				Assert.Collection(asyncEvent.Handlers,
					x => Assert.Equal(x, IntHolder.Run2));

				asyncEvent.RemoveHandler(IntHolder.Run2);
				Assert.Empty(asyncEvent.Handlers);
			}
		}
	}
}
