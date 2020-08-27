using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using Gauntlet;
using EpicGame;
using System.Diagnostics;
using AutomationTool;

//taken from https://horugame.com/gauntlet-automated-testing-and-performance-metrics-in-ue4/

namespace UE4Game
{
	/// <summary>
	/// Runs Gauntlet automated tests.
	/// </summary>
	public class Benchmark : DefaultTest
	{
		private string PerfReportToolPath = "C:/Users/Govind/Documents/UnrealEngine/Engine/Binaries/DotNET/CsvTools/PerfreportTool.exe";
		private string GauntletController = "MolVizGauntletTestController";

		public Benchmark(Gauntlet.UnrealTestContext InContext)
			: base(InContext)
		{
		}

		public override UE4TestConfig GetConfiguration()
		{
			UE4TestConfig Config = base.GetConfiguration();
			UnrealTestRole ClientRole = Config.RequireRole(UnrealTargetRole.Client);
			ClientRole.Controllers.Add(GauntletController);
			Config.MaxDuration = 30 * 60; // 10 minutes: this is a time limit, not the time the tests will take
			return Config;
		}

		public override void TickTest()
		{
			base.TickTest();
		}

		public override void CreateReport(TestResult Result, UnrealTestContext Contex, UnrealBuildSource Build, IEnumerable<UnrealRoleArtifacts> Artifacts, string ArtifactPath)
		{
			UnrealRoleArtifacts ClientArtifacts = Artifacts.Where(A => A.SessionRole.RoleType == UnrealTargetRole.Client).FirstOrDefault();

			var SnapshotSummary = new UnrealSnapshotSummary<UnrealHealthSnapshot>(ClientArtifacts.AppInstance.StdOut);

			Log.Info("My First Performance Report");
			Log.Info(SnapshotSummary.ToString());

			base.CreateReport(Result, Contex, Build, Artifacts, ArtifactPath);
		}

		public override void SaveArtifacts_DEPRECATED(string OutputPath)
		{
			string uploadDir = Globals.Params.ParseValue("uploaddir", "");

			Log.Info("====== Upload directory: " + uploadDir);

			if (uploadDir.Count() > 0 && Directory.CreateDirectory(uploadDir).Exists)
			{
				string artifactDir = TestInstance.ClientApps[0].ArtifactPath;
				string profilingDir = Path.Combine(artifactDir, "Profiling");
				string csvDir = Path.Combine(profilingDir, "CSV");
				string targetDir = Path.Combine(uploadDir, "CSV");

				if (!Directory.Exists(targetDir))
					Directory.CreateDirectory(targetDir);

				string[] csvFiles = Directory.GetFiles(csvDir);
				foreach (string csvFile in csvFiles)
				{
					string targetCSVFile = Path.Combine(targetDir, Path.GetFileName(csvFile));
					File.Copy(csvFile, targetCSVFile);

					if (!File.Exists(PerfReportToolPath))
					{
						Log.Error("Can't find PerfReportTool.exe at " + PerfReportToolPath, ", aborting!");
						break;
					}

					ProcessStartInfo startInfo = new ProcessStartInfo();
					startInfo.FileName = PerfReportToolPath;
					startInfo.Arguments = "-csv ";
					startInfo.Arguments += targetCSVFile;
					startInfo.Arguments += " -o ";
					startInfo.Arguments += uploadDir;

					try
					{
						using (Process exeProcess = Process.Start(startInfo))
						{
							exeProcess.WaitForExit();
						}
					}
					catch
					{
						Log.Error("Error running PerfReportTool.exe, aborting!");
					}
				}
			}
			else
				Log.Error("No UploadDir specified, not copying performance report! Set one with -uploaddir=c:/path/to/dir");
		}
	}

	
}
