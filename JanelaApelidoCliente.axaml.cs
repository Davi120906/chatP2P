using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ChatP2P;

public partial class JanelaApelidoCliente : Window
{
    private readonly ConexaoTcp conexao;

    public JanelaApelidoCliente(ConexaoTcp conexao)
    {
        InitializeComponent();
        BtnConfirmar.Click += BtnConfirmar_Click;
        this.conexao = conexao;
    }

    private async void BtnConfirmar_Click(object? sender, RoutedEventArgs e)
    {
        string apelido = TxtApelido.Text.Trim();

        if (string.IsNullOrWhiteSpace(apelido))
        {
            CaixaMensagem.Show("Por favor, digite um apelido.", this);
            return;
        }
        var chatWindow = new ChatWindowCliente(apelido, conexao);
        chatWindow.Show();
        this.Close();
       
    }
}