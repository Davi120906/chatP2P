using Avalonia.Controls;
using System.Threading.Tasks;  

namespace ChatP2P;

public partial class CaixaMensagem : Window
{
    public CaixaMensagem(string mensagem)
    {
        InitializeComponent();
        MensagemTexto.Text = mensagem;
        BtnOk.Click += (_, _) => this.Close();
    }
    
    //versao pra avisar que desconectou do host por isso e async
    public static Task ShowAsync(string mensagem, Window? owner = null)
    {
        var caixa = new CaixaMensagem(mensagem);

        if (owner != null)
            return caixa.ShowDialog(owner);

        caixa.Show();
        return Task.CompletedTask;
    }
    public static void Show(string mensagem, Window? owner = null)
    {
        var caixa = new CaixaMensagem(mensagem);

        if (owner != null)
            caixa.ShowDialog(owner);
        else
            caixa.Show();
    }
}
