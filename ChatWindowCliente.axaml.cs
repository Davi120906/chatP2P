using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.Media;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ChatP2P
{
    public partial class ChatWindowCliente : Window
    {
        private readonly string apelido;
        private readonly ConexaoTcp conexao;

        private ObservableCollection<mensagemItem> mensagens = new();
        private Dictionary<string, IBrush> apelidoCores = new();

        private readonly List<IBrush> paletaCores = new()
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

        public ChatWindowCliente(string apelido, ConexaoTcp conexao)
        {
            InitializeComponent();

            this.apelido = apelido;
            this.conexao = conexao;

            LstMensagens.ItemsSource = mensagens;

            BtnEnviar.Click += BtnEnviar_Click;
            TxtMensagem.KeyDown += TxtMensagem_KeyDown;

            conexao.AoReceberMensagem += ReceberMensagem;
            _ = conexao.NotificarEntradaCliente(apelido);
            conexao.AoReceberMensagem += DetectarServidorDesligou;
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            FecharConnection();
            base.OnClosing(e);
        }

        private void FecharConnection()
        {
            try
            {
                conexao?.EnviarMensagemAsync($"DISCONNECT_CLIENT:{apelido}");
                typeof(ConexaoTcp)
                    .GetMethod("Fechar", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(conexao, null);
            }
            catch { }
        }

        private void TxtMensagem_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                EnviarMensagem();
        }

        private void BtnEnviar_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            EnviarMensagem();
        }

        private async void EnviarMensagem()
        {
            string msg = TxtMensagem.Text?.Trim() ?? "";
            if (msg == "") return;

            if (!apelidoCores.ContainsKey(apelido))
                apelidoCores[apelido] = paletaCores[apelidoCores.Count % paletaCores.Count];

            mensagens.Add(new mensagemItem
            {
                apelido = apelido,
                mensagem = msg,
                cor = apelidoCores[apelido]
            });

            LstMensagens.ScrollIntoView(mensagens[mensagens.Count - 1]);
            TxtMensagem.Text = "";

            await conexao.EnviarMensagemAsync($"{apelido}:{msg}");
        }

        private void ReceberMensagem(string texto)
        {
            if (texto.StartsWith("COLTEZAP AI:"))
            {
                string novoUser = texto.Replace("COLTEZAP AI:", "").Trim();

                Dispatcher.UIThread.Post(() =>
                {
                    mensagens.Add(new mensagemItem
                    {
                        apelido = "Sistema",
                        mensagem = $"{novoUser} entrou na conversa!",
                        cor = Brushes.Gray
                    });

                    LstMensagens.ScrollIntoView(mensagens[mensagens.Count - 1]);
                });

                return;
            }

            var partes = texto.Split(':', 2);
            if (partes.Length != 2) return;

            string remetente = partes[0];
            string mensagemTxt = partes[1];

            if (!apelidoCores.ContainsKey(remetente))
                apelidoCores[remetente] = paletaCores[apelidoCores.Count % paletaCores.Count];

            Dispatcher.UIThread.Post(() =>
            {
                mensagens.Add(new mensagemItem
                {
                    apelido = remetente,
                    mensagem = mensagemTxt,
                    cor = apelidoCores[remetente]
                });

                LstMensagens.ScrollIntoView(mensagens[mensagens.Count - 1]);
            });
        }

        private async void DetectarServidorDesligou(string texto)
{
        if (texto == "SERVER_DOWN")
        {
            await Dispatcher.UIThread.InvokeAsync(async () =>
            {
                await CaixaMensagem.ShowAsync("O host encerrou a conexão!", this);

                var menu = new MainWindow();
                menu.Show();
                this.Close();
            });
        }
    }


        private bool conexaoAtivada()
        {
            try
            {
                return conexao != null &&
                    typeof(ConexaoTcp)
                        .GetField("cliente", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.GetValue(conexao) is TcpClient cli &&
                    cli.Connected;
            }
            catch
            {
                return false;
            }
        }
    }
}
