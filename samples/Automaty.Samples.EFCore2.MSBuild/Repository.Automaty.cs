﻿namespace Automaty.Samples.EFCore2.MSBuild
{
	using System;
	using Automaty.Common.Execution;
	using Automaty.Common.Output;
	using Automaty.Samples.EFCore2.MSBuild.Data;
	using Microsoft.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore.Metadata;
	using SmartFormat;

	// Automaty IncludeFiles StringExtension.cs;FirstCharacterToLowerFormatter.cs
	[AutomatyIncludeDirectory(Directory = "./Data")]
	public class Repository : IAutomatyHost
	{
		public void Execute(IScriptContext scriptContext)
		{
			Smart.Default.AddExtensions(new FirstCharacterToLowerFormatter());

			DbContextOptions<BloggingContext> options = new DbContextOptionsBuilder<BloggingContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			using (BloggingContext bloggingContext = new BloggingContext(options))
			{
				IModel model = bloggingContext.Model;

				foreach (IEntityType entityType in model.GetEntityTypes())
				{
					scriptContext.Output.CurrentGeneratedFileName.FileNameWithoutExtension = $"{entityType.ClrType.Name}";

					string template = @"//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Automaty
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Automaty.Samples.EFCore2.MSBuild
{{
	using System.Linq;

	public class {ClrType.Name}Repository
	{{
		public IUnitOfWork UnitOfWork {{ get; set; }}

		public {ClrType.Name}Repository(IUnitOfWork unitOfWork)
		{{
			UnitOfWork = unitOfWork;
		}}

		public virtual {ClrType.Namespace}.{ClrType.Name} Find({FindPrimaryKey().Properties:{ClrType.Namespace}.{ClrType.Name} {Name:fctl()}|, })
		{{
			return UnitOfWork.GetEntitySet<{ClrType.Namespace}.{ClrType.Name}>().Single(x => {FindPrimaryKey().Properties:x.{Name} == {Name:fctl()}| && });
		}}

		// TODO: Add a lot more functionality :)
	}}
}}";

					scriptContext.Output.Current.WriteLine(Smart.Format(template, entityType));
				}
			}
		}
	}
}