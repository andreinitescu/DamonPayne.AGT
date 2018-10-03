using System.Collections.Generic;
using System.Windows.Controls;

namespace DamonPayne.AGT.Design.Contracts
{
    public interface IDesignEditorService
    {
        Control Visual { get; }
        void Edit(IList<IDesignableControl> targets);
    }
}
