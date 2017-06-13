Imports System.IO
Imports System.Linq

Public Class Form1

    Private DEFAULT_BUTTON_COLOR As Color

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        DEFAULT_BUTTON_COLOR = btnBul.BackColor

        Me.BringToFront()
        lblCurrentTime.Text = DateTime.Now.ToString()

    End Sub


    Private Sub btnRecord_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRecord.Click
        Dim strMsg As String
        strMsg = rTxtDoingThis.Text.Trim()
        timNotify.Stop()
        timNotify.Enabled = False
        btnRecord.Enabled = False

        If String.IsNullOrEmpty(strMsg) Then
            WriteStatus("No action data to save")
            Exit Sub
        End If

        logEvent(strMsg)
        lblCurrentTime.Text = DateTime.Now.ToString()

        Dim timeInterval As Integer
        If Integer.TryParse(txtInterval.Text, timeInterval) Then
            timNotify.Interval = timeInterval * 1000 * 60
        Else
            MessageBox.Show("Fix yer interval. Its gotta be an integer.", Me.Text)
            btnRecord.Enabled = True
            Exit Sub
        End If

        timNotify.Enabled = True
        timNotify.Start()
        Me.WindowState = FormWindowState.Minimized
        btnRecord.Enabled = True
    End Sub

    Private Sub WriteStatus(ByVal message As String)
        If String.IsNullOrEmpty(message) Then
            Exit Sub
        End If

        sbrStatus.Text = message
    End Sub

    Private Sub logEvent(ByVal message As String)

        Dim strFileName As String
        Dim strFolder As String
        Dim tWriter As System.IO.TextWriter
        strFolder = Environment.ExpandEnvironmentVariables(txtLogLoc.Text)

        If String.IsNullOrEmpty(strFolder) Then
            strFolder = "C:\TEMP\"
            txtLogLoc.Text = strFolder
            WriteStatus("Defaulting to c:\temp")
        End If

        If IO.Directory.Exists(strFolder) = False Then
            IO.Directory.CreateDirectory(strFolder)
        End If

        If strFolder.EndsWith("\", StringComparison.InvariantCultureIgnoreCase) = False Then
            strFolder = strFolder & "\"
        End If

        strFileName = strFolder & CStr(DateTime.Now.Year) & "_Week_" & CStr(CInt(System.Math.Floor(DateTime.Now.DayOfYear / 7))) + ".txt"
        WriteStatus("Saving data to " & strFileName)
        tWriter = New IO.StreamWriter(strFileName, True)
        tWriter.WriteLine(New String(CChar("-"), 80))
        tWriter.WriteLine(DateTime.Now.ToString() & System.Environment.NewLine & message)
        tWriter.WriteLine(New String(CChar("-"), 80))
        tWriter.Flush()
        tWriter.Close()

        WriteStatus("Data Saved to " & strFileName)
    End Sub

    Private Sub timNotify_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles timNotify.Tick

        Dim originalColor As Color
        Dim altColor As Color
        originalColor = Me.BackColor
        altColor = Color.Red

        Me.BringToFront()
        Me.WindowState = FormWindowState.Normal
        btnRecord.Focus()
        AppActivate(Me.Text)
        Me.BackColor = altColor

        If ckDontUseSystemBeep.Checked Then
            Dim _sp As New Media.SoundPlayer(".\21STCNTR.WAV")
            _sp.Play()
            _sp.Dispose()
        End If

        For X As Integer = 0 To 4

            If ckDontUseSystemBeep.Checked = False Then
                System.Console.Beep()
            End If

            System.Threading.Thread.Sleep(500)

            If X Mod 2 = 0 Then
                Me.BackColor = originalColor
            Else
                Me.BackColor = altColor
            End If
            Application.DoEvents()
        Next

        Me.BackColor = originalColor
        Application.DoEvents()

        Me.Focus()
    End Sub

    Private Sub txtOpacity_Leave(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtOpacity.Leave
        SetOpacity(txtOpacity.Text)
    End Sub

    Private Sub txtOpacity_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtOpacity.LostFocus
        SetOpacity(txtOpacity.Text)
    End Sub

    Private Sub SetOpacity(ByVal val As String)
        If String.IsNullOrEmpty(val) Then
            Exit Sub
        End If

        If IsNumeric(val) = False Then
            Exit Sub
        End If

        Dim opacity As Double

        opacity = (CDbl(val) / 100)

        If opacity < 0.5 Or opacity > 1.0 Then
            WriteStatus("Opacity is invalid.")
            Exit Sub
        End If

        Me.Opacity = opacity
    End Sub

    Private Sub btnBul_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBul.Click

        If rTxtDoingThis.SelectionBullet = False Then
            btnBul.BackColor = Color.Aqua
        Else
            btnBul.BackColor = DEFAULT_BUTTON_COLOR
        End If

        rTxtDoingThis.SelectionBullet = Not rTxtDoingThis.SelectionBullet
    End Sub

    Private Sub btnOpenFolder_Click(sender As Object, e As EventArgs) Handles btnOpenFolder.Click
        Try
            Process.Start("explorer.exe", Environment.ExpandEnvironmentVariables(txtLogLoc.Text))
        Catch ex As Exception
            MessageBox.Show(ex.ToString())
        End Try
    End Sub

    Private Sub btnLoad_Click(sender As Object, e As EventArgs) Handles btnLoad.Click


        Dim logPath = Environment.ExpandEnvironmentVariables("%USERPROFILE%\my documents\WTFWID")

        Dim di as New DirectoryInfo(logPath)
        Dim files = di.GetFileSystemInfos().OrderByDescending(Function(f) f.LastWriteTime).Select(Function(f) f.FullName).ToList()
        Dim newestFile = files(0)
        

        If (newestFile Is Nothing) Then

            Debug.WriteLine("No newest file")
            Return
        End If

        Debug.WriteLine($"Found {newestFile}")

        Dim fileLines = File.ReadAllLines(newestFile)
        Array.Reverse(fileLines)

        Dim scrapedReversedLines As New List(Of String)
        Dim lineBreak = "--------------------------------------------------------------------------------"

        Dim endingBreakFound = False
        Dim startingBreakFound = False

        For Each fileLine As String In fileLines
            If fileLine.IndexOf(lineBreak) >= 0 Then

                If startingBreakFound = False AndAlso endingBreakFound = False Then

                    endingBreakFound = True

                    Continue For
                ElseIf startingBreakFound = False AndAlso endingBreakFound = True Then
                    Exit For
                End If
            End If

            If startingBreakFound = False AndAlso endingBreakFound = True Then
                scrapedReversedLines.Add(fileLine)
            End If
        Next
        scrapedReversedLines.RemoveAt(scrapedReversedLines.Count -1)

        Dim scrapedForwardLines = scrapedReversedLines.ToArray()
        Array.Reverse(scrapedForwardLines)

        For Each scrapedForwardLine As String In scrapedForwardLines
            rTxtDoingThis.Text += $"{scrapedForwardLine}{Environment.NewLine}"
        Next

        
    End Sub
End Class
