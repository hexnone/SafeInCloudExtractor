using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Microsoft.Diagnostics.Runtime;

namespace SafeInCloudExtractor
{
	class Program
	{
		static void Main(string[] args)
		{
			new Extractor().Extract();
			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}
	}
}
