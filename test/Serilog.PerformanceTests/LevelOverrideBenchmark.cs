using System.Collections.Generic;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using Serilog.Core;
using Serilog.Events;
using Serilog.PerformanceTests.Support;

namespace Serilog.PerformanceTests
{
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.NetCoreApp21, baseline: true)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    public class LevelOverrideBenchmark
    {
        readonly LevelOverrideMap _levelOverrideMap;
        readonly Logger _logger;
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

            _logger = new LoggerConfiguration()
                .MinimumLevel.Fatal()
                .MinimumLevel.Override("MyApp", LogEventLevel.Debug)
                .MinimumLevel.Override("MyApp.Api.Controllers", LogEventLevel.Information)
                .MinimumLevel.Override("MyApp.Api.Controllers.HomeController", LogEventLevel.Warning)
                .MinimumLevel.Override("MyApp.Api", LogEventLevel.Error)
                .WriteTo.Sink<NullSink>().CreateLogger();
        }

        [Benchmark]
        public ILogger ForContext()
        {
            ILogger logger = null;
            for (var i = 0; i < _contexts.Length; ++i)
            {
                logger = _logger.ForContext(Constants.SourceContextPropertyName, _contexts[i]);
            }
            return logger;
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
