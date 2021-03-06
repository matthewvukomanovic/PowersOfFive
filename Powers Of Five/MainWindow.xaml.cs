﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Powers_Of_Five
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IMainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = DataContext as MainWindowViewModel;
            if (vm != null)
            {
                vm.View = this;
            }
        }

        public void SelectTextboxText()
        {
            AnswerControl.SelectAll();
        }
    }

    public interface IMainWindow
    {
        void SelectTextboxText();
    }
}
