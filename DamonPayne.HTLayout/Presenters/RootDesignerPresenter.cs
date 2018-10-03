using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Unity;
using DamonPayne.HTLayout.ViewContracts;
using DamonPayne.AGT.Design.Types;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.HTLayout.Controls;

namespace DamonPayne.HTLayout.Presenters
{
    /// <summary>
    /// Global logic for the designer application
    /// </summary>
    public class RootDesignerPresenter
    {
        public RootDesignerPresenter(IRootView view)
        {
            _view = view;
        }

        private IRootView _view;

        [Dependency]
        public IToolboxService Toolbox { get; set; }

        public void InitializeDesignableControls()
        {
            ToolboxItem couchItem = new ToolboxItem("Couch", "A comfy place to watch movies with your S/O", typeof(Couch));
            Toolbox.AddItem(couchItem, "Furniture");

            ToolboxItem chairItm = new ToolboxItem("Chair", "A nice chair courtesy of Adobe products", typeof(Chair));
            Toolbox.AddItem(chairItm, "Furniture");

            ToolboxItem rugItm = new ToolboxItem("Area Rug", "A rug, which really ties the room together", typeof(AreaRug));
            Toolbox.AddItem(rugItm, "Furniture");

            ToolboxItem speakItem = new ToolboxItem("Tower speaker", "This is a generic 2-way tower speaker with horn-loaded mid & high range", typeof(HornSpeaker));
            Toolbox.AddItem(speakItem, "Audio");

            ToolboxItem centerItem = new ToolboxItem("Center channel", "This is a generic 2-way center channel speaker with horn-loaded mid & high range", typeof(CenterChannelSpeaker));
            Toolbox.AddItem(centerItem, "Audio");

            ToolboxItem db = new ToolboxItem("Button", "A nice dummy button", typeof(DummyButton));
            Toolbox.AddItem(db);
        }

    }
}
