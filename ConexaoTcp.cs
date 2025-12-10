using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatP2P
{
    public class ConexaoTcp
    {
        private readonly string chaveXor = "COLTEC2025";
        private TcpListener? servidor;
        private TcpClient? cliente;
        private NetworkStream? fluxo;

        public event Action<string>? AoReceberMensagem;
        public event Action<string>? AoClienteConectar;

        public async Task IniciarServidorAsync(int porta)
        {
            servidor = new TcpListener(IPAddress.Any, porta);
            servidor.Start();
            cliente = await servidor.AcceptTcpClientAsync();
            fluxo = cliente.GetStream();

           
            AoClienteConectar?.Invoke("Cliente");

            _ = LoopRecebimentoAsync();
        }

        public async Task ConectarAoServidorAsync(string ip, int porta, string apelido)
        {
            cliente = new TcpClient();
            await cliente.ConnectAsync(ip, porta);
            fluxo = cliente.GetStream();

            // Dispara evento de cliente conectado no servidor enviando apelido
            await EnviarMensagemAsync($"{apelido} entrou na conversa");

            _ = LoopRecebimentoAsync();
        }

        public async Task EnviarMensagemAsync(string mensagem)
        {
            if (fluxo == null) return;
            byte[] dados = CriptografarDescriptografar(Encoding.UTF8.GetBytes(mensagem));
            await fluxo.WriteAsync(dados, 0, dados.Length);
        }

        private async Task LoopRecebimentoAsync()
        {
            if (fluxo == null) return;
            byte[] buffer = new byte[4096];
            while (true)
            {
                try
                {
                    int bytesLidos = await fluxo.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesLidos == 0) break;
                    byte[] msgBytes = new byte[bytesLidos];
                    Array.Copy(buffer, msgBytes, bytesLidos);
                    string msg = Encoding.UTF8.GetString(CriptografarDescriptografar(msgBytes));
                    AoReceberMensagem?.Invoke(msg);
                }
                catch
                {
                    break;
                }
            }
        }

        private byte[] CriptografarDescriptografar(byte[] dados)
        {
            byte[] chaveBytes = Encoding.UTF8.GetBytes(chaveXor);
            byte[] resultado = new byte[dados.Length];
            for (int i = 0; i < dados.Length; i++)
                resultado[i] = (byte)(dados[i] ^ chaveBytes[i % chaveBytes.Length]);
            return resultado;
        }

        public void Fechar()
        {
            fluxo?.Close();
            cliente?.Close();
            servidor?.Stop();
        }
    }
}
