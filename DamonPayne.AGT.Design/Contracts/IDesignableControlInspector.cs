using System.Collections.Generic;
using DamonPayne.AGT.Design.Types;

namespace DamonPayne.AGT.Design.Contracts
{
    public interface IDesignableControlInspector
    {
        List<DesignablePropertyDescriptor> Inspect(IDesignableControl idt);
    }
}
