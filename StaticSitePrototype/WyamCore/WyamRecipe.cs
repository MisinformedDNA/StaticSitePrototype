using System.Threading.Tasks;

namespace StaticSitePrototype.WyamCore
{
    public interface IWyamRecipe
    {
        Task InvokeAsync();
    }
}