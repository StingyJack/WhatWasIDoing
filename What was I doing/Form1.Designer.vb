<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.lblCurrentTime = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnRecord = New System.Windows.Forms.Button()
        Me.sbrStatus = New System.Windows.Forms.StatusStrip()
        Me.timNotify = New System.Windows.Forms.Timer(Me.components)
        Me.txtInterval = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtLogLoc = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtOpacity = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.ckDontUseSystemBeep = New System.Windows.Forms.CheckBox()
        Me.rTxtDoingThis = New System.Windows.Forms.RichTextBox()
        Me.btnBul = New System.Windows.Forms.Button()
        Me.btnOpenFolder = New System.Windows.Forms.Button()
        Me.btnLoad = New System.Windows.Forms.Button()
        Me.SuspendLayout
        '
        'lblCurrentTime
        '
        Me.lblCurrentTime.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.lblCurrentTime.AutoSize = true
        Me.lblCurrentTime.Location = New System.Drawing.Point(116, 290)
        Me.lblCurrentTime.Name = "lblCurrentTime"
        Me.lblCurrentTime.Size = New System.Drawing.Size(13, 13)
        Me.lblCurrentTime.TabIndex = 1
        Me.lblCurrentTime.Text = "1"
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(12, 290)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(97, 13)
        Me.Label1.TabIndex = 2
        Me.Label1.Text = "Last recorded time:"
        '
        'btnRecord
        '
        Me.btnRecord.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btnRecord.Location = New System.Drawing.Point(15, 317)
        Me.btnRecord.Name = "btnRecord"
        Me.btnRecord.Size = New System.Drawing.Size(129, 23)
        Me.btnRecord.TabIndex = 3
        Me.btnRecord.Text = "Record action"
        Me.btnRecord.UseVisualStyleBackColor = true
        '
        'sbrStatus
        '
        Me.sbrStatus.Location = New System.Drawing.Point(0, 402)
        Me.sbrStatus.Name = "sbrStatus"
        Me.sbrStatus.Size = New System.Drawing.Size(484, 22)
        Me.sbrStatus.TabIndex = 4
        Me.sbrStatus.Text = "Loaded"
        '
        'timNotify
        '
        '
        'txtInterval
        '
        Me.txtInterval.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.txtInterval.Location = New System.Drawing.Point(317, 317)
        Me.txtInterval.Name = "txtInterval"
        Me.txtInterval.Size = New System.Drawing.Size(38, 20)
        Me.txtInterval.TabIndex = 5
        Me.txtInterval.Text = "60"
        '
        'Label2
        '
        Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(199, 320)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(99, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Notification minutes"
        '
        'txtLogLoc
        '
        Me.txtLogLoc.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.txtLogLoc.Location = New System.Drawing.Point(88, 346)
        Me.txtLogLoc.Name = "txtLogLoc"
        Me.txtLogLoc.Size = New System.Drawing.Size(267, 20)
        Me.txtLogLoc.TabIndex = 7
        Me.txtLogLoc.Text = " %USERPROFILE%\my documents\WTFWID"
        '
        'Label3
        '
        Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(11, 349)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(71, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Save logs in :"
        '
        'txtOpacity
        '
        Me.txtOpacity.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.txtOpacity.Location = New System.Drawing.Point(292, 372)
        Me.txtOpacity.Name = "txtOpacity"
        Me.txtOpacity.Size = New System.Drawing.Size(31, 20)
        Me.txtOpacity.TabIndex = 9
        Me.txtOpacity.Text = "100"
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(243, 375)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(43, 13)
        Me.Label4.TabIndex = 10
        Me.Label4.Text = "Opacity"
        '
        'Label5
        '
        Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(329, 375)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(15, 13)
        Me.Label5.TabIndex = 11
        Me.Label5.Text = "%"
        '
        'ckDontUseSystemBeep
        '
        Me.ckDontUseSystemBeep.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.ckDontUseSystemBeep.AutoSize = true
        Me.ckDontUseSystemBeep.Checked = true
        Me.ckDontUseSystemBeep.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ckDontUseSystemBeep.Location = New System.Drawing.Point(15, 374)
        Me.ckDontUseSystemBeep.Name = "ckDontUseSystemBeep"
        Me.ckDontUseSystemBeep.Size = New System.Drawing.Size(136, 17)
        Me.ckDontUseSystemBeep.TabIndex = 12
        Me.ckDontUseSystemBeep.Text = "Dont Use System Beep"
        Me.ckDontUseSystemBeep.UseVisualStyleBackColor = true
        '
        'rTxtDoingThis
        '
        Me.rTxtDoingThis.AcceptsTab = true
        Me.rTxtDoingThis.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.rTxtDoingThis.AutoWordSelection = true
        Me.rTxtDoingThis.BulletIndent = 2
        Me.rTxtDoingThis.Font = New System.Drawing.Font("Calibri", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.rTxtDoingThis.Location = New System.Drawing.Point(15, 12)
        Me.rTxtDoingThis.Name = "rTxtDoingThis"
        Me.rTxtDoingThis.Size = New System.Drawing.Size(457, 258)
        Me.rTxtDoingThis.TabIndex = 13
        Me.rTxtDoingThis.Text = ""
        '
        'btnBul
        '
        Me.btnBul.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btnBul.Location = New System.Drawing.Point(292, 280)
        Me.btnBul.Name = "btnBul"
        Me.btnBul.Size = New System.Drawing.Size(38, 23)
        Me.btnBul.TabIndex = 14
        Me.btnBul.Text = "Bul"
        Me.btnBul.UseVisualStyleBackColor = true
        '
        'btnOpenFolder
        '
        Me.btnOpenFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btnOpenFolder.Location = New System.Drawing.Point(361, 346)
        Me.btnOpenFolder.Name = "btnOpenFolder"
        Me.btnOpenFolder.Size = New System.Drawing.Size(54, 23)
        Me.btnOpenFolder.TabIndex = 15
        Me.btnOpenFolder.Text = "Open"
        Me.btnOpenFolder.UseVisualStyleBackColor = true
        '
        'btnLoad
        '
        Me.btnLoad.Location = New System.Drawing.Point(422, 346)
        Me.btnLoad.Name = "btnLoad"
        Me.btnLoad.Size = New System.Drawing.Size(50, 23)
        Me.btnLoad.TabIndex = 16
        Me.btnLoad.Text = "Load"
        Me.btnLoad.UseVisualStyleBackColor = true
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(484, 424)
        Me.Controls.Add(Me.btnLoad)
        Me.Controls.Add(Me.btnOpenFolder)
        Me.Controls.Add(Me.btnBul)
        Me.Controls.Add(Me.rTxtDoingThis)
        Me.Controls.Add(Me.ckDontUseSystemBeep)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.txtOpacity)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.txtLogLoc)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtInterval)
        Me.Controls.Add(Me.sbrStatus)
        Me.Controls.Add(Me.btnRecord)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblCurrentTime)
        Me.Name = "Form1"
        Me.Text = "What are you doing?"
        Me.TopMost = true
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents lblCurrentTime As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents btnRecord As System.Windows.Forms.Button
    Friend WithEvents sbrStatus As System.Windows.Forms.StatusStrip
    Friend WithEvents timNotify As System.Windows.Forms.Timer
    Friend WithEvents txtInterval As System.Windows.Forms.TextBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents txtLogLoc As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents txtOpacity As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents ckDontUseSystemBeep As System.Windows.Forms.CheckBox
    Friend WithEvents rTxtDoingThis As System.Windows.Forms.RichTextBox
    Friend WithEvents btnBul As System.Windows.Forms.Button
    Friend WithEvents btnOpenFolder As Button
    Friend WithEvents btnLoad As Button
End Class
