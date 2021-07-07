using Basket.API.Entities;
using System.Threading.Tasks;

namespace Basket.API.Repositories
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasket(string userName);
        Task UpdateBasket(ShoppingCart basket);
        Task DeleteBasket(string userName);
    }
}
