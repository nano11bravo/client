Imports System.Net.Sockets
Imports System.Text
Public Class fClient
    Dim clientSocket As New System.Net.Sockets.TcpClient()
    Dim serverStream As NetworkStream
    Dim readData As String
    Dim infiniteCounter As Integer
    Dim ctThread As Threading.Thread = New Threading.Thread(AddressOf getMessage)

    Private Sub btnSend_Click(ByVal sender As System.Object, _
        ByVal e As System.EventArgs) Handles btnSend.Click
        sendChat(txtMessage.Text)
    End Sub

    Private Sub msg()
        If Me.InvokeRequired Then
            Me.Invoke(New MethodInvoker(AddressOf msg))
        Else
            txtLog.Text = txtLog.Text + Environment.NewLine + Now() + " >> " + readData
            txtLog.SelectionStart = txtLog.Text.Length
            txtLog.ScrollToCaret()
            My.Computer.FileSystem.WriteAllText("C:\inetpub\wwwroot\client.html", "<html><body><pre>" & txtLog.Text & "</body></html>", False)
        End If
    End Sub
    Private Function log(ByVal text As String)
        txtLog.Text = txtLog.Text + Environment.NewLine + Now() + " >> " + text
        txtLog.SelectionStart = txtLog.Text.Length
        txtLog.ScrollToCaret()
        My.Computer.FileSystem.WriteAllText("C:\inetpub\wwwroot\client.html", "<html><body><pre>" & txtLog.Text & "</body></html>", False)
        log = True
    End Function

    Private Sub getMessage()
        If clientSocket.Connected Then
            For infiniteCounter = 1 To 2
                infiniteCounter = 1
                serverStream = clientSocket.GetStream()
                Dim buffSize As Integer
                Dim inStream(10024) As Byte
                buffSize = clientSocket.ReceiveBufferSize
                serverStream.Read(inStream, 0, buffSize)
                Dim returnData As String = System.Text.Encoding.ASCII.GetString(inStream)
                readData = returnData
                msg()
            Next
        End If
    End Sub

    Private Sub fClient_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Seven-Labs | TClient 0.0.0.0 | 2012
        Me.Text = My.Application.Info.CompanyName & " - TClient " & My.Application.Info.Version.ToString & " - Loaded at " & Now()

        ' Display loaded text
        log("Program loaded")
    End Sub

    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        Try
            ' Display connected text
            log("Connected to server...")

            ' Connection strings
            clientSocket.Connect("irishe.dyndns.org", 1337)
            'clientSocket.Connect("seven-labs.com", 14001)
            'clientSocket.Connect("192.168.1.2", 9734)
            'Label1.Text = "Client Socket Program - Server Connected ..."
            serverStream = clientSocket.GetStream()

            ' Send nickname
            sendData(txtName.Text)

            ' Start threading
            ctThread.Start()

            ' For GUI buttons
            btnDisconnect.Enabled = True
            btnConnect.Enabled = False

        Catch ex As Exception
            log(Err.Description)
        End Try
    End Sub

    Private Sub mnuSendTimer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuSendTimer.Click
        If tmrSendData.Enabled Then
            mnuSendTimer.Checked = False
            tmrSendData.Enabled = False
        Else
            mnuSendTimer.Checked = True
            tmrSendData.Enabled = True
        End If
    End Sub

    Private Sub tmrSendData_Tick(ByVal sender As Object, ByVal e As System.EventArgs) Handles tmrSendData.Tick
        If clientSocket.Connected Then
            sendChat(txtMessage.Text)
        End If
    End Sub
    Private Function sendData(ByVal data As String)
        Dim outStream As Byte() = System.Text.Encoding.ASCII.GetBytes(data + "$")
        serverStream.Write(outStream, 0, outStream.Length)
        serverStream.Flush()
        sendData = True
    End Function
    Private Function sendChat(ByVal text As String)
        Dim outStream As Byte() = System.Text.Encoding.ASCII.GetBytes(text + "$")
        serverStream.Write(outStream, 0, outStream.Length)
        serverStream.Flush()
        sendChat = True
    End Function

    Private Sub btnDisconnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDisconnect.Click
        ' stop threading
        ctThread.Abort()

        ' close sockets
        serverStream.Flush()
        serverStream.Close()
        clientSocket.Close()

        ' gui buttons
        btnDisconnect.Enabled = False
        btnConnect.Enabled = True
    End Sub

    Private Sub AboutToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles mnuAbout.Click
        MsgBox("About what?!?!")
    End Sub
End Class
