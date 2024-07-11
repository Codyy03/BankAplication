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
using System.Windows.Shapes;

namespace Projekt
{
    /// <summary>
    /// Logika interakcji dla klasy Rejestracion.xaml
    /// </summary>
    public partial class Rejestracion : Window
    {
        public Rejestracion()
        {
            InitializeComponent();
        }

        private void RegistracionName_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
            {
                e.Handled = true; // Ignorowanie wprowadzonego tekstu, jeśli nie jest literą
            }
        }
    }
}
