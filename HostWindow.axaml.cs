using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Text.RegularExpressions;

namespace ChatP2P;

public partial class HostWindow : Window
{
    public HostWindow()
    {
        InitializeComponent();

        BtnConfirmar.Click += BtnConfirmar_Click;
        BtnVoltar.Click += BtnVoltar_Click;
    }

    private void BtnVoltar_Click(object? sender, RoutedEventArgs e)
    {
        var mainWindow = new MainWindow();
        mainWindow.Show();
        this.Close();
    }

    private void BtnConfirmar_Click(object? sender, RoutedEventArgs e)
    {
        string portaText = TxtPorta.Text.Trim();
        Regex numerosRegex = new Regex(@"^\d+$");
        if (!numerosRegex.IsMatch(portaText))
        {
            CaixaMensagem.Show("Erro: digite apenas números.", this);
            return;
        }

        int porta = int.Parse(portaText);
        if (porta < 1 || porta > 65535)
        {
            CaixaMensagem.Show("Porta inválida! Digite um número entre 1 e 65535.", this);
            return;
        }

        var janelaApelido = new JanelaApelido(porta);
        janelaApelido.Show();
        this.Close();
    }
}
