using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ChatP2P;

public partial class ChatWindow : Window
{
    private readonly string apelido;
    private ObservableCollection<mensagemItem> mensagens = new ObservableCollection<mensagemItem>();
    private Dictionary<string, IBrush> apelidoCores = new Dictionary<string, IBrush>();
    private int porta;
    private List<IBrush> paletaCores = new List<IBrush>
    {
        Brushes.MediumPurple,
        Brushes.Coral,
        Brushes.DarkOrange,
        Brushes.DeepSkyBlue,
        Brushes.LimeGreen,
        Brushes.Gold,
        Brushes.HotPink,
        Brushes.CadetBlue,
        Brushes.MediumVioletRed,
        Brushes.Turquoise
    };

    public ChatWindow(string apelido, int porta)
    {
        InitializeComponent();

        this.apelido = apelido;
        this.porta = porta;

        LstMensagens.ItemsSource = mensagens;

        BtnEnviar.Click += BtnEnviar_Click;
        TxtMensagem.KeyDown += TxtMensagem_KeyDown;
    }

    private void BtnEnviar_Click(object? sender, RoutedEventArgs e)
    {
        EnviarMensagem();
    }

    private void TxtMensagem_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            EnviarMensagem();
        }
    }

    private void EnviarMensagem()
    {
        string texto = TxtMensagem.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(texto))
            return;

        if (!apelidoCores.ContainsKey(apelido))
            apelidoCores[apelido] = paletaCores[apelidoCores.Count % paletaCores.Count];

        mensagens.Add(new mensagemItem
        {
            apelido = apelido,
            mensagem = texto,
            cor = apelidoCores[apelido]
        });

        TxtMensagem.Text = string.Empty;
        LstMensagens.ScrollIntoView(mensagens[mensagens.Count - 1]);
    }
}
