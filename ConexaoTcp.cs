using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Numerics; 

namespace ChatP2P
{
    public class ConexaoTcp
    {
        private readonly string chaveXor = "COLTEC2025";
        private TcpListener? servidor;
        private TcpClient? cliente;
        private NetworkStream? fluxo;

        public event Action<string>? AoReceberMensagem;
        public event Action<string>? ClienteEntrou;


        public async Task IniciarServidorAsync(int porta)
        {
            servidor = new TcpListener(IPAddress.Any, porta);
            servidor.Start();
            cliente = await servidor.AcceptTcpClientAsync();
            fluxo = cliente.GetStream();
            _ = LoopRecebimentoAsync();
        }

        public async Task ConectarAoServidorAsync(string ip, int porta)
        {
            cliente = new TcpClient();
            await cliente.ConnectAsync(ip, porta);
            fluxo = cliente.GetStream();
            _ = LoopRecebimentoAsync();
        }

        public async Task EnviarMensagemAsync(string mensagem)
        {
            if (fluxo == null) return;
            byte[] dados = CriptografarDescriptografar(Encoding.UTF8.GetBytes(mensagem));
            await fluxo.WriteAsync(dados, 0, dados.Length);
        }

        public async Task NotificarEntradaCliente(string apelido)
        {
            if (fluxo == null) return;
            string mensagemEntrada = $"COLTEZAP AI:{apelido}";
            byte[] dados = CriptografarDescriptografar(Encoding.UTF8.GetBytes(mensagemEntrada));
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
            int len = dados.Length;
            byte[] resultado = new byte[len];

            int vectorSize = Vector<byte>.Count;
            int i = 0;

            // Processa blocos inteiros usando SIMD
            for (; i <= len - vectorSize; i += vectorSize)
            {
                var dadosVec = new Vector<byte>(dados, i);
                var chaveVec = new Vector<byte>(chaveBytes, i % chaveBytes.Length);
                var resultadoVec = dadosVec ^ chaveVec;
                resultadoVec.CopyTo(resultado, i);
            }

      
            for (; i < len; i++)
            {
                resultado[i] = (byte)(dados[i] ^ chaveBytes[i % chaveBytes.Length]);
            }

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
