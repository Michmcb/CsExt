﻿namespace MichMcb.CsExt.Threads
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;
	/// <summary>
	/// Allows things to happen once every period of time.
	/// Doesn't use anything disposable.
	/// </summary>
	public sealed class FixedCooldown
	{
		private TimeSpan currentWaitTime;
		private readonly Stopwatch stopwatch;
		/// <summary>
		/// Creates a new instance that is currently off cooldown, and so will not cause any delay on the first wait.
		/// </summary>
		/// <param name="cooldown">The cooldown, in milliseconds.</param>
		[Obsolete("Prefer the TimeSpan constructor instead")]
		public FixedCooldown(int cooldown)
		{
			stopwatch = new Stopwatch();
			Cooldown = TimeSpan.FromMilliseconds(cooldown);
		}
		/// <summary>
		/// Creates a new instance that is currently off cooldown, and so will not cause any delay on the first wait.
		/// </summary>
		/// <param name="cooldown">The cooldown.</param>
		public FixedCooldown(TimeSpan cooldown)
		{
			stopwatch = new Stopwatch();
			Cooldown = cooldown;
		}
		/// <summary>
		/// The cooldown.
		/// </summary>
		public TimeSpan Cooldown { get; set; }
		/// <summary>
		/// Blocks the calling thread until the cooldown expires. Any subsequent callers will have to wait for <see cref="Cooldown"/> milliseconds.
		/// </summary>
		public void Wait()
		{
			TimeSpan waitingTime;
			lock (stopwatch)
			{
				// The difference between how long we have to wait and how long we've actually waited
				waitingTime = currentWaitTime - stopwatch.Elapsed;
				stopwatch.Restart();
			}
			if (waitingTime <= TimeSpan.Zero)
			{
				// If we've waited enough, then all is well; set the waiting time to the cooldown issued and restart the Stopwatch
				currentWaitTime = Cooldown;
			}
			else
			{
				// If we haven't waited enough, then we need to issue a delay to the thread that requested this, and then the new waiting time compounds
				// Specifically, the new wait time should be the 
				// e.g. If we've only waited 2/5 seconds but we want another 5 second proc, then the current caller needs to wait 3 seconds (waitingTime), and the next Proc will have to wait for 3 + 5 seconds.
				// The waiting time compounds for every thing that wants to use this. We add on the cooldown plus the waiting time we're doling out
				currentWaitTime = Cooldown + waitingTime;
				Thread.Sleep(waitingTime);
			}
		}
		/// <summary>
		/// Delays the calling task until the cooldown expires. Any subsequent callers will have to wait for <see cref="Cooldown"/> milliseconds.
		/// </summary>
		public Task WaitAsync()
		{
			TimeSpan waitingTime;
			lock (stopwatch)
			{
				// The difference between how long we have to wait and how long we've actually waited
				waitingTime = currentWaitTime - stopwatch.Elapsed;
				stopwatch.Restart();
			}
			if (waitingTime <= TimeSpan.Zero)
			{
				// If we've waited enough, then all is well; set the waiting time to the cooldown issued and restart the Stopwatch
				currentWaitTime = Cooldown;
				return Task.CompletedTask;
			}
			else
			{
				// If we haven't waited enough, then we need to issue a delay to the thread that requested this, and then the new waiting time compounds
				// Specifically, the new wait time should be the 
				// e.g. If we've only waited 2/5 seconds but we want another 5 second proc, then the current caller needs to wait 3 seconds (waitingTime), and the next Proc will have to wait for 3 + 5 seconds.
				// The waiting time compounds for every thing that wants to use this. We add on the cooldown plus the waiting time we're doling out
				currentWaitTime = Cooldown + waitingTime;
				return Task.Delay(waitingTime);
			}
		}
	}
}
