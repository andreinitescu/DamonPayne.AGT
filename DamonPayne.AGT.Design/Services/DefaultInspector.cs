using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AGT.Design.Types;

namespace DamonPayne.AGT.Design.Services
{
    public class DefaultInspector : IDesignableControlInspector
    {
        public System.Collections.Generic.List<DesignablePropertyDescriptor> Inspect(IDesignableControl idt)
        {
            if (null != idt)
            {
                return idt.GetDesignProperties();
            }
            return null;
        }
    }
}
