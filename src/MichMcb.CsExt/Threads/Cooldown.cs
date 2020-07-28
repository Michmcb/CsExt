using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace MichMcb.Threads
{
	public sealed class Cooldown
	{
		private int currentWaitTime;
		private readonly Stopwatch stopwatch;
		public Cooldown()
		{
			stopwatch = new Stopwatch();
		}
		public Cooldown(int initialCooldown)
		{
			currentWaitTime = initialCooldown;
			stopwatch = Stopwatch.StartNew();
		}
		public void Proc(int cooldown)
		{
			int waitingTime;
			lock (stopwatch)
			{
				// The difference between how long we have to wait and how long we've actually waited
				waitingTime = currentWaitTime - (int)stopwatch.ElapsedMilliseconds;
				stopwatch.Restart();
			}
			if (waitingTime <= 0)
			{
				// If we've waited enough, then all is well; set the waiting time to the cooldown issued and restart the Stopwatch
				currentWaitTime = cooldown;
			}
			else
			{
				// If we haven't waited enough, then we need to issue a delay to the thread that requested this, and then the new waiting time compounds
				// Specifically, the new wait time should be the 
				// e.g. If we've only waited 2/5 seconds but we want another 5 second proc, then the current caller needs to wait 3 seconds (waitingTime), and the next Proc will have to wait for 3 + 5 seconds.
				// The waiting time compounds for every thing that wants to use this. We add on the cooldown plus the waiting time we're doling out
				currentWaitTime = cooldown + waitingTime;
				Thread.Sleep(waitingTime);
			}
		}
		public Task ProcAsync(int cooldown)
		{
			int waitingTime;
			lock (stopwatch)
			{
				// The difference between how long we have to wait and how long we've actually waited
				waitingTime = currentWaitTime - (int)stopwatch.ElapsedMilliseconds;
				stopwatch.Restart();
			}
			if (waitingTime <= 0)
			{
				// If we've waited enough, then all is well; set the waiting time to the cooldown issued and restart the Stopwatch
				currentWaitTime = cooldown;
				return Task.CompletedTask;
			}
			else
			{
				// If we haven't waited enough, then we need to issue a delay to the thread that requested this, and then the new waiting time compounds
				// Specifically, the new wait time should be the 
				// e.g. If we've only waited 2/5 seconds but we want another 5 second proc, then the current caller needs to wait 3 seconds (waitingTime), and the next Proc will have to wait for 3 + 5 seconds.
				// The waiting time compounds for every thing that wants to use this. We add on the cooldown plus the waiting time we're doling out
				currentWaitTime = cooldown + waitingTime;
				return Task.Delay(waitingTime);
			}
		}
	}
}
