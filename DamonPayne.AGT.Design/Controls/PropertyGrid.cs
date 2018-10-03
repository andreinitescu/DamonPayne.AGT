using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AGT.Design.Types;
using DamonPayne.AGT.Design.Services;
using Microsoft.Practices.Unity;
using DamonPayne.AG.Core;


namespace DamonPayne.AGT.Design.Controls
{
    /// <summary>
    /// 
    /// </summary>    
    [TemplatePart(Name = ROOT, Type = typeof(Panel))]
    [TemplatePart(Name = HEADER_PART, Type = typeof(UIElement))]
    [TemplatePart(Name = PROPERTY_AREA_PART, Type = typeof(Grid))]
    [TemplatePart(Name = FOOTER_PART, Type = typeof(UIElement))]
    [TemplateVisualState(GroupName = "CommonStates", Name = "Normal")]
    public class PropertyGrid: Control, IDesignEditorService
    {
        public PropertyGrid()
        {
            DefaultStyleKey = typeof(PropertyGrid);
            Model = new PropertyGridModel();
            MouseLeftButtonDown += new MouseButtonEventHandler(PropertyGrid_MouseLeftButtonDown);
        }

        void PropertyGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        public PropertyGridModel Model { get; set; }

        [Dependency]
        public IDesignableControlInspector Inspector { get; set; }

        public const string ROOT = "LayoutRoot";
        public const string HEADER_PART = "PropertyGridHeaderPart";
        public const string PROPERTY_AREA_PART = "PropertyAreaPart";
        public const string FOOTER_PART = "PropertyGridFooterPart";

        private Panel _rootPart;
        private UIElement _headerPart;
        private Grid _propertyPart;
        private UIElement _footerPart;
        
        protected Style _descriptionTextBlockStyle;

        /// <summary>
        /// We can only edit one property at a time!
        /// </summary>
        private FrameworkElement _editElement;



        public object HeaderContent
        {
            get { return (object)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HeaderContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register("HeaderContent", typeof(object), typeof(PropertyGrid), new PropertyMetadata("Property grid header", OnHeaderContentChanged));

        protected static void OnHeaderContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) 
        {
        }


        public object PropertyAreaContent
        {
            get { return (object)GetValue(PropertyAreaContentProperty); }
            set { SetValue(PropertyAreaContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PropertyAreaContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyAreaContentProperty =
            DependencyProperty.Register("PropertyAreaContent", typeof(object), typeof(PropertyGrid), new PropertyMetadata("put a grid or something here!", OnPropertyAreaContentChanged));


        protected static void OnPropertyAreaContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }



        public object FooterContent
        {
            get { return (object)GetValue(FooterContentProperty); }
            set { SetValue(FooterContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FooterContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FooterContentProperty =
            DependencyProperty.Register("FooterContent", typeof(object), typeof(PropertyGrid), new PropertyMetadata("Put a footer here!", OnFooterContentChanged));

        protected static void OnFooterContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }



        public static object GetPropGridValue(DependencyObject obj)
        {
            return (object)obj.GetValue(PropGridValueProperty);
        }

        public static void SetPropGridValue(DependencyObject obj, object value)
        {
            obj.SetValue(PropGridValueProperty, value);
        }

        // Using a DependencyProperty as the backing store for PropGridValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropGridValueProperty =
            DependencyProperty.RegisterAttached("PropGridValue", typeof(object), typeof(PropertyGrid), new PropertyMetadata(OnPropGridValueChanged) );

        protected static void OnPropGridValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

    

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _rootPart = (Panel)GetTemplateChild(ROOT);
            _headerPart = (UIElement)GetTemplateChild(HEADER_PART);
            _propertyPart = (Grid)GetTemplateChild(PROPERTY_AREA_PART);
            _footerPart = (UIElement)GetTemplateChild(FOOTER_PART);


            PopulateStyles();
        }

        private void PopulateStyles()
        {
            _descriptionTextBlockStyle = (Style)Application.Current.Resources["PropertyGridTextBlockStyle"];
            _rootPart.Style = (Style)Application.Current.Resources["PropertyGridRootStyle"];
        }

        public Control Visual
        {
            get
            {
                return this;
            }
        }

        public virtual void Edit(IList<IDesignableControl> targets)
        {
            ClearProperties();

            if (null != targets && targets.Count > 0 && null != targets[0])
            {
                Model.Selection.AddRange(targets);
                if (Model.Selection.Count > 1)
                {
                    EditMultiple(Model.Selection);
                }
                else
                {
                    EditSingle(Model.Selection[0]);
                }
            }
        }

        protected virtual void EditSingle(IDesignableControl idt)
        {
            List<DesignablePropertyDescriptor> props = Inspector.Inspect(idt);
            Model.SetProperties(props);
            var propCount = 0;
            foreach (var descriptor in props)
            {
                _propertyPart.RowDefinitions.Add(new RowDefinition());
                TextBlock tb = new TextBlock();
                tb.Text = descriptor.DisplayName;                
                tb.SetValue(Grid.RowProperty, propCount);
                tb.SetValue(Grid.ColumnProperty, 0);
                tb.Style = _descriptionTextBlockStyle;
                //
                FrameworkElement displayElement = EditServiceHelper.GetDisplayInstance(idt, descriptor);

                if (null == displayElement)
                {
                    displayElement = new TextBlock
                    {
                        Text = "No display element found"
                    };
                }

                displayElement.SetValue(Grid.RowProperty, propCount);
                displayElement.SetValue(Grid.ColumnProperty, 1);
                displayElement.GotFocus += new RoutedEventHandler(displayElement_GotFocus);
                displayElement.MouseLeftButtonDown += new MouseButtonEventHandler(displayElement_MouseLeftButtonDown);

                Model.SetDisplayElement(descriptor, displayElement);

                _propertyPart.Children.Add(tb);
                _propertyPart.Children.Add(displayElement);
                ++propCount;
            }
        }

        protected virtual void EditMultiple(IList<IDesignableControl> targets)
        {
            //Get common properties and then do similar to edit single on the first one,
            //CheckPreviousEditInstance() will deal with sets
            List<DesignablePropertyDescriptor> allProps = new List<DesignablePropertyDescriptor>();
            foreach (var idt in targets)
            {
                allProps.AddRange(Inspector.Inspect(idt));
            }
            var hashes = from pd in allProps
                         group pd by pd.GetHashCode() into g
                         where g.Count() == targets.Count
                         select new { g.Key };
            var dups = (from pd in allProps 
                        join h in hashes on pd.GetHashCode() equals h.Key 
                        select pd).Distinct();


            List<DesignablePropertyDescriptor> common = dups.ToList<DesignablePropertyDescriptor>();

            var propCount = 0;
            foreach (var descriptor in common)
            {
                _propertyPart.RowDefinitions.Add(new RowDefinition());
                TextBlock tb = new TextBlock();
                tb.Text = descriptor.DisplayName;
                tb.SetValue(Grid.RowProperty, propCount);
                tb.SetValue(Grid.ColumnProperty, 0);
                tb.Style = _descriptionTextBlockStyle;
                //
                FrameworkElement displayElement = EditServiceHelper.GetDisplayInstance(targets[0], descriptor);

                if (null == displayElement)
                {
                    displayElement = new TextBlock
                    {
                        Text = "No display element found"
                    };
                }

                displayElement.SetValue(Grid.RowProperty, propCount);
                displayElement.SetValue(Grid.ColumnProperty, 1);
                displayElement.GotFocus += new RoutedEventHandler(displayElement_GotFocus);
                displayElement.MouseLeftButtonDown += new MouseButtonEventHandler(displayElement_MouseLeftButtonDown);

                Model.SetDisplayElement(descriptor, displayElement);

                _propertyPart.Children.Add(tb);
                _propertyPart.Children.Add(displayElement);
                ++propCount;
            }


            Model.SetProperties(common);

        }

        void displayElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            VerifyEditVisualization(sender);   
        }

        void displayElement_GotFocus(object sender, RoutedEventArgs e)
        {
            VerifyEditVisualization(sender);            
        }

        private void VerifyEditVisualization(object sender)
        {
            CheckPreviousEditInstance();   
            if (null != Model.Selection && Model.Selection.Count > 0)
            {
                FrameworkElement element = (FrameworkElement)sender;
                var desc = Model.GetDescriptorForDisplayElement(element);
                FrameworkElement editInstance = Model.GetEditElement(desc);
                if (null == editInstance)
                {
                    editInstance = EditServiceHelper.GetEditInstance(Model.Selection[0], desc);
                    if (null != editInstance)
                    {
                        
                    }
                    else
                    {
                        editInstance = new TextBlock
                        {
                            Text = "Unable to edit"
                        };
                    }
                    Model.SetEditElement(desc, editInstance);
                    Model.CurrentEditElement = editInstance;
                }
                    
                if (element != editInstance)
                {
                    SwapToEdit(desc, editInstance);
                }
            }
        }

        void editInstance_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;//Don't let this bubble
        }

        protected virtual void CheckPreviousEditInstance()
        {
            if (null != Model.CurrentEditElement)
            {
                var editElement = Model.CurrentEditElement;
                var desc = Model.GetDescriptorForEditElement(editElement);
                SwapToDisplay(desc, editElement);

                if (Model.Selection.Count > 1)
                {
                    //we bound to 1st one, now set the new value on all the subsequent edit items
                    //TODO: this method exposes an implementation detail of EditServiceHelper, need to fix this
                    var propName = desc.PropertyInfo.Name;
                    object value = desc.PropertyInfo.GetValue(Model.Selection[0], null);
                    for (int i = 1; i < Model.Selection.Count; ++i)
                    {
                        var pi = Model.Selection[i].GetType().GetProperty(propName);
                        pi.SetValue(Model.Selection[i], value, null);
                    }
                } 
            }
        }


        protected virtual void SwapToEdit(DesignablePropertyDescriptor desc, FrameworkElement editElement)
        {
            var display = Model.GetDisplayElement(desc);
            if (null != editElement)
            {
                int row = display.GetValue<int>(Grid.RowProperty);
                int col = display.GetValue<int>(Grid.ColumnProperty);
                editElement.SetValue(Grid.RowProperty, row);
                editElement.SetValue(Grid.ColumnProperty, col);
                _propertyPart.Children.Remove(display);
                _propertyPart.Children.Add(editElement); 
            }
        }

        protected virtual void SwapToDisplay(DesignablePropertyDescriptor desc, FrameworkElement editElement)
        {
            var display = Model.GetDisplayElement(desc);
            if (null != display)//Safety
            {
                _propertyPart.Children.Remove(editElement);
                _propertyPart.Children.Add(display); 
            }
        }


        protected virtual void ClearProperties()
        {
            CheckPreviousEditInstance();
            _propertyPart.Children.Clear();
            foreach (FrameworkElement fe in Model.GetAllDisplayElements())//don't hang on to controls via event handlers
            {
                fe.GotFocus -= displayElement_GotFocus;
                fe.MouseLeftButtonDown -= displayElement_MouseLeftButtonDown;
            }
            Model.Reset();
        }


    }
}
