using MGSimpleForms;
using MGSimpleForms.MVVM;
using MGSimpleFormsExamples.EditExamples;
using MGSimpleFormsExamples.FormExamples;
using MGSimpleFormsExamples.GridFormExamples;
using System;
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

namespace MGSimpleFormsExamples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private static void Display(FormViewModel viewModel)
        {
            viewModel.ShowInWindowDialog();
        }

        private void Textbox_Click(object sender, RoutedEventArgs e)
        {
            Display(new TextBoxExample());
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {

            Display(new ButtonExample());
        }

        private void dpcb_Click(object sender, RoutedEventArgs e)
        {
            Display(new DatePickerCheckBoxExample());
        }

        private void cbo_Click(object sender, RoutedEventArgs e)
        {
            Display(new ComboBoxExample());
        }


        private void gl_Click(object sender, RoutedEventArgs e)
        {
            Display(new GridListExample());
        }

        private void lbl_Click(object sender, RoutedEventArgs e)
        {
            Display(new LabelExample());
        }

        private void File_Folder_Picker_Click(object sender, RoutedEventArgs e)
        {
            Display(new FolderFilePicker());
        }

        private void Question_Click(object sender, RoutedEventArgs e)
        {
            QuestionBox.Show("Test");
            QuestionBox.Show("Test", "Test 2");
            QuestionBox.Show("Test", "Test 3", QuestionBoxButtons.YesNoCancel);
            QuestionBox.Show("Test", "Test 3", new QuestionBoxButton[] { 
                new QuestionBoxButton("Why Do this"),
                "Do This",
                new QuestionBoxButton("Surprise?!?", ()=>{
                    QuestionBox.Show("A question Box in a Question BOX?");
                    return false;
                })
            });

            QuestionBox.Show("Testal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkgTestal;sjdhgflksjfzhpoiltjkjhdfxv kjsdfghkj drn;lkdh filujdr hfg;usd glkjernjarhg lkjuarhntg; idhfrg liarnl harlkg", "Test 4");

        }

        private void Item_Click(object sender, RoutedEventArgs e)
        {
            Display(new ShowItemExample());
        }

        private void Replace_Click(object sender, RoutedEventArgs e)
        {
            Display(new testingFull());
        }

        private void Progress_Click(object sender, RoutedEventArgs e)
        {
            Display(new ProgressBarExample());
        }
    }
}
