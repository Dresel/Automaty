// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Automaty.Core.Resolution
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using NuGet.Frameworks;
	using NuGet.Packaging.Core;
	using NuGet.ProjectModel;

	// See https://github.com/dotnet/sdk/blob/master/src/Tasks/Microsoft.NET.Build.Tasks/NuGetUtils.cs
	internal static class NuGetUtils
	{
		public static IEnumerable<LockFileItem> FilterPlaceHolderFiles(this IEnumerable<LockFileItem> files)
		{
			return files.Where(f => !IsPlaceholderFile(f.Path));
		}

		public static string GetLockFileLanguageName(string projectLanguage)
		{
			switch (projectLanguage)
			{
				case "C#": return "cs";
				case "F#": return "fs";

				default: return projectLanguage?.ToLowerInvariant();
			}
		}

		public static string GetPackageIdFromSourcePath(string sourcePath)
		{
			string packageId, unused;

			GetPackageParts(sourcePath, out packageId, out unused);

			return packageId;
		}

		public static void GetPackageParts(string fullPath, out string packageId, out string packageSubPath)
		{
			packageId = null;
			packageSubPath = null;

			try
			{
				// this method is just a temporary heuristic until we flow the NuGet metadata through the right items
				// https://github.com/dotnet/sdk/issues/1091
				for (DirectoryInfo dir = Directory.GetParent(fullPath); dir != null; dir = dir.Parent)
				{
					FileInfo[] nuspecs = dir.GetFiles("*.nuspec");

					if (nuspecs.Length > 0)
					{
						packageId = Path.GetFileNameWithoutExtension(nuspecs[0].Name);
						packageSubPath = fullPath.Substring(dir.FullName.Length + 1).Replace('\\', '/');
						break;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static bool IsPlaceholderFile(string path)
		{
			return string.Equals(Path.GetFileName(path), PackagingCoreConstants.EmptyFolder, StringComparison.Ordinal);
		}

		public static NuGetFramework ParseFrameworkName(string frameworkName)
		{
			return frameworkName == null ? null : NuGetFramework.Parse(frameworkName);
		}
	}
}