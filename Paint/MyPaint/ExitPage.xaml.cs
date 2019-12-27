/************************************************
 * Author: Tucker Myers
 * Date 5/10/2018
 * File ExitPage.xaml.cs
 * *********************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

/***********************************************
 * Overview: This window is designed to be an exit 
 * dialogue, prompting the user to verify that they
 * intend to leave the program
 * ********************************************/
namespace MyPaint
{
    /// <summary>
    /// Interaction logic for ExitPage.xaml
    /// </summary>
    public partial class ExitPage : Window
    {

        MainWindow m_mainwindow;

        //Pass in a reference to the main window
        public ExitPage(MainWindow mainWindow)
        {
            m_mainwindow = mainWindow;
            InitializeComponent();
        }

        //If the user clicks no, the dialogue window will close, but 
        //not the main window
        private void noClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        //If the user clicks yes, exit the main window and the exit dialogue
        private void yesClick(object sender, RoutedEventArgs e)
        {
            m_mainwindow.Close();
            Close();
        }
    }
}
