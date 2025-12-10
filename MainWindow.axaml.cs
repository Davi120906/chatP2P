using Avalonia.Controls;

namespace ChatP2P;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        BtnServidor.Click += BtnServidor_Click;
        BtnCliente.Click += BtnCliente_Click;
    }
    private void BtnServidor_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var hostWindow = new HostWindow();
        hostWindow.Show();
        this.Close();
    }
    private void BtnCliente_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var clientWindow = new ClientWindow();
        clientWindow.Show();
        this.Close();
    }
}