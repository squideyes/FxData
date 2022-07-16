// ********************************************************
// Copyright (C) 2022 Louis S. Berman (louis@squideyes.com)
//
// This file is part of SquidEyes.FxData
//
// The use of this source code is licensed under the terms
// of the MIT License (https://opensource.org/licenses/MIT)
// ********************************************************

using Fclp;
using SquidEyes.Basics;
using SquidEyes.FxData.DukasFetch;
using SquidEyes.FxData.Models;

if (!TryGetSettings(out Settings? settings))
    return;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((_, services) => services
        .AddSingleton(settings!)
        .AddHostedService<Worker>())
    .Build();

await host.RunAsync();

bool TryGetSettings(out Settings? settings)
{
    settings = null;

    var parser = new FluentCommandLineParser<Settings>();

    parser.Setup(x => x.ConnString)
        .As('c', "connstring")
        .SetDefault("UseDevelopmentStorage=true")
        .WithDescription("A storage conn-string (default = UseDevelopmentStorage=true)");

    parser.Setup(x => x.Symbols)
        .As('s', "symbols")
        .SetDefault(EnumList.FromAll<Symbol>())
        .WithDescription("Space-separated list of FX pairs (i.e. EURUSD USDJPY)");

    parser.Setup(x => x.Replace)
        .As('r', "replace")
        .SetDefault(true)
        .WithDescription("If present, existing tick-data files will be replaced");

    parser.Setup(x => x.MinYear)
         .As('y', "minyear")
         .SetDefault(2016)
         .WithDescription("The first year to download ticks for (min/default = 2016)");

    parser.SetupHelp("?", "help").Callback(text => Console.WriteLine(text));

    var result = parser.Parse(args);

    if (result.HasErrors)
    {
        Console.Write(result.ErrorText);

        parser.HelpOption.ShowHelp(parser.Options);

        return false;
    }

    settings = parser.Object;

    bool isValid = true;
    
    void IsInvalid(string message)
    {
        Console.WriteLine(message);

        isValid = false;
    }

    if (settings.MinYear < 2016)
        IsInvalid($"The \"MinYear\" argument must be >= 2016!");
    
    return isValid;
}