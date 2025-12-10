using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Text.RegularExpressions;

namespace ChatP2P
{
    public partial class ClientWindow : Window
    {
        public ClientWindow()
        {
            InitializeComponent();

            BtnConfirmar.Click += BtnConfirmar_Click;
            BtnVoltar.Click += BtnVoltar_Click;
        }

        private void BtnConfirmar_Click(object? sender, RoutedEventArgs e)
        {
            string ip = TxtIP.Text ?? "";
            string porta_texto = TxtPorta.Text ?? "";

            if (!Regex.IsMatch(ip, @"^[0-9.]+$"))
            {
                CaixaMensagem.Show("O IP deve conter apenas números e pontos.", this);
                return;
            }

            bool ipValido = Regex.IsMatch(ip, @"^((25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)\.){3}(25[0-5]|2[0-4]\d|1\d\d|[1-9]?\d)$");
            if (!ipValido)
            {
                CaixaMensagem.Show("O IP digitado não é válido.", this);
                return;
            }

            if (!int.TryParse(porta_texto, out int porta))
            {
                CaixaMensagem.Show("A porta deve ser um número inteiro.", this);
                return;
            }

            if (porta < 1 || porta > 65535)
            {
                CaixaMensagem.Show("A porta deve estar entre 1 e 65535.", this);
                return;
            }

        }

        private void BtnVoltar_Click(object? sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
