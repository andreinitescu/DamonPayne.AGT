
namespace DamonPayne.AGT.Design.Contracts
{
    public interface INameProvider
    {
        string GetUniqueName(INamingContainer container, IDesignableControl newChild);
    }
}
