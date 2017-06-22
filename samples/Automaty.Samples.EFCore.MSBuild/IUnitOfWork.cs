namespace Automaty.Samples.EFCore.MSBuild
{
	using System.Linq;

	public interface IUnitOfWork
	{
		void Add<T>(T entity) where T : class;

		T Create<T>() where T : class;

		void Delete<T>(T entity) where T : class;

		IQueryable<T> GetEntitySet<T>() where T : class;

		void RevertChanges();

		void Save();
	}
}