using System.Linq;

namespace CrossExchange_Project
{
    public interface IPortfolioRepository : IGenericRepository<Portfolio>
    {
        IQueryable<Portfolio> GetAll();
    }
}