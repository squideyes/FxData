![GitHub Workflow Status](https://img.shields.io/github/workflow/status/squideyes/fxdata/Deploy%20to%20NuGet?label=build)
![NuGet Version](https://img.shields.io/nuget/v/squidEyes.fxData)
![License](https://img.shields.io/github/license/squideyes/FxData)

**SquidEyes.FxData** is an collection of high-performance C#/.NET 6.0 trading-data primatives with a set of full-coverage unit-tests.  This code is a hard-fork of the author's <a href="https://github.com/squideyes/Trading" target="_blank">SquidEyes.Trading</a> library, with the data-oriented classes (i.e. TickSet) tweaked to support day-trading scenarios.  The quote unquote trading code (i.e. indicators, order management objects, etc.) has been removed.

Be forewarned, though, that literally no effort has been made to make the various classes and methods suitable for a general audience.

SquidEyes.FxData includes **DukasFetch**, a rather basic downloader for Dukascopy tick-data.  Th

To run the program, download and compile the source code, then issue a command similar to:

**DukasFetch --conn=UseDevelopmentStorage=true --symbols=EURUSD USDJPY --minyear=2020 --replace=true**

|Parameter|Required|Example|Notes|
|---|---|---|---|
|--conn|No|UseDevelopmentStorage=true|A connection-string to the Azure-storage account you want to save tick-sets and bundles to.  The data will be saved to an "fxdata" container in the referenced account, using a well-known pathing / naming methodology.|
|--symbols|No|EURUSD&nbsp;USDJPY|A space-separated list of FX symbols to download tick-data files for.  If omitted, EURUSD , GBPUSD, and USDJPY ticks will be downloaded.|
|--replace|No|Yes|If true, .STS and .STB files will be replaced|
|--minyear|No|2020|The minimum year to download tick-data for.  Must be greater than or equal to 2016.



#

**Super-Duper Extra-Important Caveat**:  THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.

More to the point, your use of this code may (literally!) lead to your losing thousands of dollars and more.  Be careful, and remember: Caveat Emptor!!



