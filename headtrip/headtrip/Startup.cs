/*
		  __ _/| _/. _  ._/__ /
		_\/_// /_///_// / /_|/
					   _/
		copyright (c) sof digital 2019
		written by michael rinderle <michael@sofdigital.net>

        mit license

        Permission is hereby granted, free of charge, to any person obtaining a copy
        of this software and associated documentation files (the "Software"), to deal
        in the Software without restriction, including without limitation the rights
        to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
        copies of the Software, and to permit persons to whom the Software is
        furnished to do so, subject to the following conditions:

        The above copyright notice and this permission notice shall be included in all
        copies or substantial portions of the Software.

        THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
        IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
        FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
        AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
        LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
        OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
        SOFTWARE.
*/

using headtrip.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Shiny;
using Shiny.Jobs;

namespace headtrip
{
    public class Startup : ShinyStartup
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            var job = new JobInfo
            {
                Identifier = "Sensor Job",
                Type = typeof(SensorJob),
                // these are criteria that must be met in order for your job to run
                BatteryNotLow = true,
                DeviceCharging = false,
                RequiredInternetAccess = InternetAccess.Any,
                Repeat = true //defaults to true, set to false to run once OR set it inside a job to cancel further execution
            };
            services.RegisterJob(job);
        }
    }
}