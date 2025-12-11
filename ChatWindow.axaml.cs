using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ChatP2P;

public partial class ChatWindow : Window
{
    private readonly string apelido;
    private ObservableCollection<mensagemItem> mensagens = new ObservableCollection<mensagemItem>();
    private Dictionary<string, IBrush> apelidoCores = new Dictionary<string, IBrush>();
    private int porta;
    private ConexaoTcp conexao = new ConexaoTcp();

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

    public event Action<string>? ClienteEntrou;

    public ChatWindow(string apelido, int porta)
    {
        InitializeComponent();
        this.apelido = apelido;
        this.porta = porta;

        LstMensagens.ItemsSource = mensagens;

        BtnEnviar.Click += BtnEnviar_Click;
        TxtMensagem.KeyDown += TxtMensagem_KeyDown;

        conexao.AoReceberMensagem += ReceberMensagem;

        _ = conexao.IniciarServidorAsync(porta);
    }

    protected override async void OnClosing(WindowClosingEventArgs e)
        {
            await FecharConnection();
            base.OnClosing(e);
        }


    private async Task FecharConnection()
{
    try
    {
        await conexao.EnviarMensagemAsync("SERVER_DOWN");
        await Task.Delay(150);
        typeof(ConexaoTcp)
            .GetMethod("Fechar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(conexao, null);
    }
    catch { }
}

    private void BtnEnviar_Click(object? sender, RoutedEventArgs e)
    {
        EnviarMensagem();
    }

    private void TxtMensagem_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            EnviarMensagem();
    }

    private async void EnviarMensagem()
    {
        string texto = TxtMensagem.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(texto))
            return;

        if (!apelidoCores.ContainsKey(apelido))
            apelidoCores[apelido] = paletaCores[apelidoCores.Count % paletaCores.Count];

        var msgItem = new mensagemItem
        {
            apelido = apelido,
            mensagem = texto,
            cor = apelidoCores[apelido]
        };

        mensagens.Add(msgItem);
        TxtMensagem.Text = string.Empty;
        LstMensagens.ScrollIntoView(mensagens[mensagens.Count - 1]);

        await conexao.EnviarMensagemAsync($"{apelido}:{texto}");
    }

    private void ReceberMensagem(string texto)
    {
        if (texto.StartsWith("DISCONNECT_CLIENT:"))
        {
            string apelidoSaiu = texto.Replace("DISCONNECT_CLIENT:", "");

            Dispatcher.UIThread.Post(() =>
            {
                mensagens.Add(new mensagemItem
                {
                    apelido = "Sistema",
                    mensagem = $"{apelidoSaiu} saiu da conversa.",
                    cor = Brushes.Gray
                });

                LstMensagens.ScrollIntoView(mensagens[mensagens.Count - 1]);
            });

            return;
        }

        if (texto.StartsWith("COLTEZAP AI:"))
        {
            string apelidoCliente = texto.Substring("COLTEZAP AI:".Length);
            ClienteEntrou?.Invoke(apelidoCliente);

            Dispatcher.UIThread.Post(() =>
            {
                if (!apelidoCores.ContainsKey("COLTEZAP AI"))
                    apelidoCores["COLTEZAP AI"] = Brushes.MediumPurple;

                var msgItem = new mensagemItem
                {
                    apelido = "COLTEZAP AI",
                    mensagem = $"{apelidoCliente} entrou na conversa!",
                    cor = apelidoCores["COLTEZAP AI"]
                };

                mensagens.Add(msgItem);
                LstMensagens.ScrollIntoView(mensagens[mensagens.Count - 1]);
            });

            return;
        }

        var partes = texto.Split(':', 2);
        if (partes.Length != 2) return;

        string remetente = partes[0];
        string mensagem = partes[1];

        if (!apelidoCores.ContainsKey(remetente))
            apelidoCores[remetente] = paletaCores[apelidoCores.Count % paletaCores.Count];

        var msgItemNormal = new mensagemItem
        {
            apelido = remetente,
            mensagem = mensagem,
            cor = apelidoCores[remetente]
        };

        Dispatcher.UIThread.Post(() =>
        {
            mensagens.Add(msgItemNormal);
            LstMensagens.ScrollIntoView(mensagens[mensagens.Count - 1]);
        });
    }
}
