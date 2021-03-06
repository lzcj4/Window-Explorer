﻿using MongoMQTest.Model;
using MongoMQTest.ViewModel;
using System.Windows;

namespace MongoMQTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new AnalysisViewModel();
            this.Closing += (sender, e) =>
            {
                StrategyFactory.Instance.Get();
                (this.DataContext as AnalysisViewModel).Dispose();
            };
        }

    }
}
