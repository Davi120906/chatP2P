using Avalonia.Controls;

namespace ChatP2P;

public partial class CaixaMensagem : Window
{
    public CaixaMensagem(string mensagem)
    {
        InitializeComponent();
        MensagemTexto.Text = mensagem;
        BtnOk.Click += (_, _) => this.Close();
    }

    // Exibe a caixa de mensagem
    public static void Show(string mensagem, Window? owner = null)
    {
        var caixa = new CaixaMensagem(mensagem);

        if (owner != null)
            caixa.ShowDialog(owner);
        else
            caixa.Show();
    }
}
