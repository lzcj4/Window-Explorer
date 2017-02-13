﻿using FileExplorer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window
    {
        public Test()
        {
            InitializeComponent();
            this.Loaded += (sender, e) =>
            {
                var i = IconAttachedProp.GetCheckedIcon(this.radioButton);
                var i2 = IconAttachedProp.GetUncheckedIcon(this.radioButton);
                string sbName = "RotateBorderStoryboard";
                bool isContained = this.Resources.Contains(sbName);
                var sb = this.Resources[sbName] as Storyboard;
               

                //Storyboard.SetTargetName(this.border, "border");
                //sb.RepeatBehavior = RepeatBehavior.Forever;
                //sb.Begin();

            };
        }
    }
}