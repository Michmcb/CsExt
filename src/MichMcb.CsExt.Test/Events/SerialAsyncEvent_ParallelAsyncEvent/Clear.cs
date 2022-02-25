namespace MichMcb.CsExt.Test.Events.SerialAsyncEvent_ParallelAsyncEvent
{
	using MichMcb.CsExt.Events;
	using Xunit;

	public static class Clear
	{
		[Fact]
		public static void WorksOk()
		{
			foreach (IAsyncEvent<IntHolder> asyncEvent in new IAsyncEvent<IntHolder>[] { new SerialAsyncEvent<IntHolder>(), new ParallelAsyncEvent<IntHolder>() })
			{
				IntHolder t = new(0);
				asyncEvent.AddHandler(IntHolder.Run1);
				asyncEvent.AddHandler(IntHolder.Run2);

				asyncEvent.Clear();

				Assert.Empty(asyncEvent.Handlers);
			}
		}
	}
}
