using Avalonia.Media;

namespace ChatP2P
{
    public class mensagemItem
    {
        public string apelido { get; set; } = string.Empty;
        public string mensagem { get; set; } = string.Empty;
        public IBrush cor { get; set; } = Brushes.White;
    }
}
