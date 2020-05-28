using System.Collections.Generic;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using Serilog.Core;
using Serilog.Events;

namespace Serilog.PerformanceTests
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.NetCoreApp21, baseline: true)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class LevelOverrideBenchmark
    {
        readonly LevelOverrideMap _levelOverrideMap;
        readonly string[] _contexts;

        public LevelOverrideBenchmark()
        {
            var overrides = new Dictionary<string, LoggingLevelSwitch>
            {
                ["MyApp"] = new LoggingLevelSwitch(LogEventLevel.Debug),
                ["MyApp.Api.Controllers"] = new LoggingLevelSwitch(LogEventLevel.Information),
                ["MyApp.Api.Controllers.HomeController"] = new LoggingLevelSwitch(LogEventLevel.Warning),
                ["MyApp.Api"] = new LoggingLevelSwitch(LogEventLevel.Error)
            };

            _levelOverrideMap = new LevelOverrideMap(overrides, LogEventLevel.Fatal, null);

            _contexts = new[]
            {
                "Serilog",
                "MyApp",
                "MyAppSomething",
                "MyOtherApp",
                "MyApp.Something",
                "MyApp.Api.Models.Person",
                "MyApp.Api.Controllers.AboutController",
                "MyApp.Api.Controllers.HomeController",
                "Api.Controllers.HomeController"
            };
        }

        [Benchmark]
        public void GetEffectiveLevel()
        {
            for (var i = 0; i < _contexts.Length; ++i)
            {
                _levelOverrideMap.GetEffectiveLevel(_contexts[i], out _, out _);
            }
        }
    }
}
