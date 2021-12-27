using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BuildScheduler
{

    // TODO: add reference to lambda scheduler

    public async Task<bool> ScheduleBuildCompletionTime(int buildTimeInSeconds)
    {
        // TODO: not sure which to use yet... depends on how the step function wants it
        DateTime localDate = DateTime.Now;
        DateTime utcDate = DateTime.UtcNow;

        TimeSpan buildDuration = new TimeSpan(0, buildTimeInSeconds, 0); // h,m,s
        return await WaitForBuildToComplete(buildTimeInSeconds);
    }

    private async Task<bool> WaitForBuildToComplete(int secondsToWait)
    {
        await Task.Delay(TimeSpan.FromSeconds(secondsToWait));
        return true;
        //yield return new WaitForSeconds(5);
    }
}
