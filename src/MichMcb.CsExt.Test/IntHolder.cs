namespace MichMcb.CsExt.Test
{
	using System.Threading.Tasks;

	public sealed class IntHolder
	{
		public IntHolder(int delay)
		{
			Delay = delay;
		}
		public int Delay { get; }
		public int Value { get; set; }
		public static async Task Run1(object? sender, IntHolder handler)
		{
			handler.Value++;
			await Task.Delay(500);
		}
		public static async Task Run2(object? sender, IntHolder handler)
		{
			handler.Value += 2;
			await Task.Delay(500);
		}
	}
}
