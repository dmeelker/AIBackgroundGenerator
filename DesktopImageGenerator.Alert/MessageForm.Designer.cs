namespace DesktopImageGenerator.Alert;

partial class MessageForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        label1 = new Label();
        SuspendLayout();
        // 
        // label1
        // 
        label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        label1.Location = new Point(12, 9);
        label1.Name = "label1";
        label1.Size = new Size(410, 115);
        label1.TabIndex = 0;
        label1.Text = "label1";
        label1.TextAlign = ContentAlignment.MiddleCenter;
        label1.Click += label1_Click;
        // 
        // MessageForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        ClientSize = new Size(434, 133);
        Controls.Add(label1);
        FormBorderStyle = FormBorderStyle.None;
        Location = new Point(20, 20);
        Name = "MessageForm";
        StartPosition = FormStartPosition.Manual;
        Text = "Daily AI background";
        ResumeLayout(false);
    }

    #endregion

    private Label label1;
}
