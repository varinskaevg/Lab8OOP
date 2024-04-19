using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace Lab8OOP
{

    public partial class MainWindow : Window
    {
        public static readonly RoutedUICommand AddImageCommand = new RoutedUICommand("AddImage", "AddImage", typeof(MainWindow));
        public MainWindow()
        {
            InitializeComponent();
            cmbFontFamily.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            cmbFontSize.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Open));
        }
        private void rtbEditor_SelectionChanged(object sender, RoutedEventArgs e)
        {
            object temp = rtbEditor.Selection.GetPropertyValue(Inline.FontWeightProperty);
            btnBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontStyleProperty);
            btnItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
            temp = rtbEditor.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
            btnUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontFamilyProperty);
            cmbFontFamily.SelectedItem = temp;
            temp = rtbEditor.Selection.GetPropertyValue(Inline.FontSizeProperty);
            cmbFontSize.Text = temp.ToString();
        }

        private void Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Open);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
            }
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Rich Text Format (*.rtf)|*.rtf|All files (*.*)|*.*";
            if (dlg.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(dlg.FileName, FileMode.Create);
                TextRange range = new TextRange(rtbEditor.Document.ContentStart, rtbEditor.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
            }
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbFontFamily.SelectedItem != null)
                rtbEditor.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, cmbFontFamily.SelectedItem);
        }

        private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        {
            rtbEditor.Selection.ApplyPropertyValue(Inline.FontSizeProperty, cmbFontSize.Text);
        }

        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif)|*.jpg;*.jpeg;*.png;*.gif|All files (*.*)|*.*";

            if (dlg.ShowDialog() == true)
            {
                string imagePath = dlg.FileName;

                try
                {
                    InsertImage(imagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error inserting image: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void InsertImage(string imagePath)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(imagePath));
            Image image = new Image();
            image.Source = bitmap;
            image.Width = 200;
            image.Height = 200;

            if (rtbEditor.Selection.IsEmpty)
            {
                Paragraph paragraph = new Paragraph(new InlineUIContainer(image));
                rtbEditor.Document.Blocks.Add(paragraph);
            }
            else
            {
                TextPointer insertionPosition = rtbEditor.Selection.End;
                insertionPosition.Paragraph?.Inlines.Add(new InlineUIContainer(image));
            }
        }

        private void NewEditorWindow(object sender, RoutedEventArgs e)
        {
            TabItem newTab = new TabItem();
            newTab.Header = "New    ";
            newTab.Tag = "new";
            newTab.Style = FindResource("CloseableTabItemStyle") as Style;
            RichTextBox newEditor = new RichTextBox();
            newEditor.SelectionChanged += rtbEditor_SelectionChanged;
            newTab.Content = newEditor;
            tabControl.Items.Add(newTab);
            tabControl.SelectedItem = newTab;
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Button closeButton = sender as Button;
            TabItem tabItem = closeButton?.TemplatedParent as TabItem;
            if (tabItem != null)
            {
                tabControl.Items.Remove(tabItem);
            }
        }
    }
}



