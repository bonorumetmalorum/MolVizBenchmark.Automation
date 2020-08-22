
To be able to run this project follow the steps as described:

1. retrieve the unreal engine source code from the following repository:

2. Setup the UE4 project as described in its ReadMe for your target platform.

3. Compile in Visual Studio the PerfReportTool by navigating to it its sln in the "UnrealEngine\Engine\Source\Programs\CSVTools" directory

4. Clone this repo into the UE4 source directory "UnrealEngine\Engine\Source\Programs"

5. Run the Setup.bat file once more and reopen the UE4 sln to compile the automation project; it should now be listed as a project in Visual Studio.

6. Now that the automation project has been compiled run the following command from the "UnrealEngine\Engine\Build\BatchFiles" directory

.\RunUAT.bat RunUnreal -project="...\MolViz\MolViz.uproject" -test=MyFirstTest -build="...\Benchmark" -uploaddir="...\Benchmark\Perf"