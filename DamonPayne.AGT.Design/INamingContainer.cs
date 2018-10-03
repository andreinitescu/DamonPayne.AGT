using System.Collections.Generic;

namespace DamonPayne.AGT.Design
{
    /// <summary>
    /// Represents something that has children needing unique names
    /// </summary>
    public interface INamingContainer
    {
        IList<IDesignableControl> Children{get;}
    }
}
