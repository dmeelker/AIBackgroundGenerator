namespace DesktopImageGenerator.Alert;

public partial class MessageForm : Form
{
    public MessageForm(string text)
    {
        InitializeComponent();
        label1.Text = text;
    }

    private void label1_Click(object sender, EventArgs e)
    {
        Close();
    }
}
