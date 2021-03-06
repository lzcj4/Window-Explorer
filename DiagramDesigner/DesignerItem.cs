﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DiagramDesigner.Controls;
using DiagramDesigner.ViewModel;

namespace DiagramDesigner
{
    //These attributes identify the types of the named parts that are used for templating
    [TemplatePart(Name = "PART_DragThumb", Type = typeof(DragThumb))]
    [TemplatePart(Name = "PART_RotateThumb", Type = typeof(RotateThumb))]
    [TemplatePart(Name = "PART_ResizeDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ConnectorDecorator", Type = typeof(Control))]
    [TemplatePart(Name = "PART_ContentPresenter", Type = typeof(ContentPresenter))]
    public class DesignerItem : ContentControl, ISelectable, IGroupable
    {
        #region ID
        private Guid id;
        public Guid ID
        {
            get { return id; }
        }
        #endregion

        #region ParentID
        public Guid ParentID
        {
            get { return (Guid)GetValue(ParentIDProperty); }
            set { SetValue(ParentIDProperty, value); }
        }
        public static readonly DependencyProperty ParentIDProperty = DependencyProperty.Register("ParentID", typeof(Guid), typeof(DesignerItem));
        #endregion

        #region IsGroup
        public bool IsGroup
        {
            get { return (bool)GetValue(IsGroupProperty); }
            set { SetValue(IsGroupProperty, value); }
        }
        public static readonly DependencyProperty IsGroupProperty =
            DependencyProperty.Register("IsGroup", typeof(bool), typeof(DesignerItem));
        #endregion

        #region IsSelected Property

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }
        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register("IsSelected",
                                       typeof(bool),
                                       typeof(DesignerItem),
                                       new FrameworkPropertyMetadata(false));

        #endregion

        #region DragThumbTemplate Property

        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty DragThumbTemplateProperty =
            DependencyProperty.RegisterAttached("DragThumbTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetDragThumbTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(DragThumbTemplateProperty);
        }

        public static void SetDragThumbTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(DragThumbTemplateProperty, value);
        }



        public static UIElement GetRealControl(DependencyObject obj)
        {
            return (UIElement)obj.GetValue(RealControlProperty);
        }

        public static void SetRealControl(DependencyObject obj, UIElement value)
        {
            obj.SetValue(RealControlProperty, value);
        }

        public static readonly DependencyProperty RealControlProperty =
            DependencyProperty.RegisterAttached("RealControl", typeof(UIElement), typeof(DesignerItem), new PropertyMetadata(null));




        // can be used to replace the default template for the DragThumb
        public static readonly DependencyProperty IsEditbleProperty =
            DependencyProperty.RegisterAttached("IsEditable", typeof(bool), typeof(DesignerItem));

        public static bool GetIsEditable(UIElement element)
        {
            return (bool)element.GetValue(IsEditbleProperty);
        }

        public static void SetIsEditable(UIElement element, bool value)
        {
            element.SetValue(IsEditbleProperty, value);
        }

        public static double GetRepeatIconWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(RepeatIconWidthProperty);
        }

        public static void SetRepeatIconWidth(DependencyObject obj, double value)
        {
            obj.SetValue(RepeatIconWidthProperty, value);
        }

        public static readonly DependencyProperty RepeatIconWidthProperty =
            DependencyProperty.RegisterAttached("RepeatIconWidth", typeof(double), typeof(DesignerItem), new PropertyMetadata(20.0));

        public static double GetRepeatIconHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(RepeatIconHeightProperty);
        }

        public static void SetRepeatIconHeight(DependencyObject obj, double value)
        {
            obj.SetValue(RepeatIconHeightProperty, value);
        }
        public static readonly DependencyProperty RepeatIconHeightProperty =
            DependencyProperty.RegisterAttached("RepeatIconHeight", typeof(double), typeof(DesignerItem), new PropertyMetadata(20.0));


        #endregion

        #region ConnectorDecoratorTemplate Property

        // can be used to replace the default template for the ConnectorDecorator
        public static readonly DependencyProperty ConnectorDecoratorTemplateProperty =
            DependencyProperty.RegisterAttached("ConnectorDecoratorTemplate", typeof(ControlTemplate), typeof(DesignerItem));

        public static ControlTemplate GetConnectorDecoratorTemplate(UIElement element)
        {
            return (ControlTemplate)element.GetValue(ConnectorDecoratorTemplateProperty);
        }

        public static void SetConnectorDecoratorTemplate(UIElement element, ControlTemplate value)
        {
            element.SetValue(ConnectorDecoratorTemplateProperty, value);
        }

        #endregion

        #region IsDragConnectionOver

        // while drag connection procedure is ongoing and the mouse moves over 
        // this item this value is true; if true the ConnectorDecorator is triggered
        // to be visible, see template
        public bool IsDragConnectionOver
        {
            get { return (bool)GetValue(IsDragConnectionOverProperty); }
            set { SetValue(IsDragConnectionOverProperty, value); }
        }
        public static readonly DependencyProperty IsDragConnectionOverProperty =
            DependencyProperty.Register("IsDragConnectionOver",
                                         typeof(bool),
                                         typeof(DesignerItem),
                                         new FrameworkPropertyMetadata(false));

        #endregion

        #region Text / ID / Fill / Stroke

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(DesignerItem), new PropertyMetadata(null));

        public string ControlId
        {
            get { return (string)GetValue(ControlIdProperty); }
            set { SetValue(ControlIdProperty, value); }
        }

        public static readonly DependencyProperty ControlIdProperty =
            DependencyProperty.Register("ControlId", typeof(string), typeof(DesignerItem), new PropertyMetadata(null));

        public double Rotate
        {
            get { return (double)GetValue(RotateProperty); }
            set { SetValue(RotateProperty, value); }
        }

        public static readonly DependencyProperty RotateProperty =
            DependencyProperty.Register("Rotate", typeof(double), typeof(DesignerItem), new PropertyMetadata(0.0));

        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }

        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(DesignerItem), new PropertyMetadata(Brushes.Transparent));


        public Brush Stroke
        {
            get { return (Brush)GetValue(StrokeProperty); }
            set { SetValue(StrokeProperty, value); }
        }

        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register("Stroke", typeof(Brush), typeof(DesignerItem), new PropertyMetadata(Brushes.Black));        

        #endregion

        static DesignerItem()
        {
            // set the key to reference the style for this control
            FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DesignerItem), new FrameworkPropertyMetadata(typeof(DesignerItem)));
        }

        public DesignerItem(Guid id)
        {
            this.id = id;
            this.Loaded += new RoutedEventHandler(DesignerItem_Loaded);
            //this.DataContext = new ControlViewModel();
        }

        public DesignerItem()
            : this(Guid.NewGuid())
        {
        }


        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            DesignerCanvas designer = VisualTreeHelper.GetParent(this) as DesignerCanvas;

            // update selection
            if (designer != null)
            {
                if ((Keyboard.Modifiers & (ModifierKeys.Shift | ModifierKeys.Control)) != ModifierKeys.None)
                    if (this.IsSelected)
                    {
                        designer.SelectionService.RemoveFromSelection(this);
                    }
                    else
                    {
                        designer.SelectionService.AddToSelection(this);
                    }
                else if (!this.IsSelected)
                {
                    designer.SelectionService.SelectItem(this);
                }
                Focus();
            }

            e.Handled = false;
        }

        void DesignerItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (base.Template != null)
            {
                ContentPresenter contentPresenter =
                    this.Template.FindName("PART_ContentPresenter", this) as ContentPresenter;
                if (contentPresenter != null)
                {
                    UIElement contentVisual = VisualTreeHelper.GetChild(contentPresenter, 0) as UIElement;
                    if (contentVisual != null)
                    {
                        DragThumb thumb = this.Template.FindName("PART_DragThumb", this) as DragThumb;
                        if (thumb != null)
                        {
                            ControlTemplate template =
                                DesignerItem.GetDragThumbTemplate(contentVisual) as ControlTemplate;
                            if (template != null)
                                thumb.Template = template;
                        }
                    }
                }
            }
        }
    }
}
