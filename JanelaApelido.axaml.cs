using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ChatP2P;

public partial class JanelaApelido : Window
{
    private int porta;
    public JanelaApelido(int porta)
    {
        InitializeComponent();
        BtnConfirmar.Click += BtnConfirmar_Click;
        this.porta = porta;
    }

    private void BtnConfirmar_Click(object? sender, RoutedEventArgs e)
    {
        string apelido = TxtApelido.Text.Trim();

        if (string.IsNullOrWhiteSpace(apelido))
        {
            CaixaMensagem.Show("Por favor, digite um apelido.", this);
            return;
        }

        var chatWindow = new ChatWindow(apelido, porta);
        chatWindow.Show();
        this.Close();
    }
}
