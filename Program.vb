Imports System

Module Program
    Dim mineW As Integer
    Dim mineH As Integer
    Dim mineN As Integer
    Dim mineNLeft As Integer
    Dim flagN As Integer = 0
    Dim minefield(,) As Boolean
    Dim minefieldRevealed(,) As Boolean 'actually no need to reset per every DFS
    Dim minefieldMark(,) As Boolean

    Dim cursorPosX As Integer = 0
    Dim cursorPosY As Integer = 0
    Public Function RndInt(lo As Integer, hi As Integer) As Integer
        'https://stackoverflow.com/questions/18676/random-integer-in-vb-net
        Static Generator As Random = New Random()
        Return Generator.Next(lo, hi)
    End Function
    Function getCursorPosition(x As Integer, y As Integer) As (Integer, Integer)
        Return (x * 3 + 1, y + 2)
    End Function
    Function prepend(number As Integer, c As Char, d As Integer) As String
        'Input: 9, '0', 3
        'Output: 009
        Dim res As String = number.ToString()
        If res.Length >= d Then Return res
        Dim yettoFill = d - res.Length
        For i As Integer = 1 To yettoFill
            res = c + res
        Next
        Return res
    End Function
    Sub writeTopBar()
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.White
        Console.SetCursorPosition(0, 0)
        'clear
        Console.Write(New String(" ", Console.WindowWidth))
        Console.SetCursorPosition(0, 0)
        Console.WriteLine("# of marks left: " + (mineN - flagN).ToString()) 'prepend(mineN - flagN, "0", 3)
    End Sub
    Sub initialiseBoard()
        'leave two lines for info
        writeTopBar()
        Console.WriteLine()
        For i As Integer = 1 To mineH
            For j As Integer = 1 To mineW
                Console.Write("[.]")
            Next
            Console.WriteLine()
        Next
        Console.WriteLine()
        Console.WriteLine("WASD, arrow keys = move | Space = Reveal | x, m = Mark mine")
    End Sub
    Sub initialiseMinefield()
        Dim i As Integer = 0
        Do
            Dim newX As Integer = RndInt(0, mineW)
            Dim newY As Integer = RndInt(0, mineH)
            If Not minefield(newX, newY) Then
                minefield(newX, newY) = True
                i += 1
            End If
        Loop While i < mineN

        'Debug - show mines
        'For y As Integer = 0 To mineH - 1
        '    For x As Integer = 0 To mineW - 1
        '        Console.Write($"[{If(minefield(x, y), "x", ".")}]")
        '    Next
        '    Console.WriteLine()
        'Next
    End Sub
    Function mineCheck(x As Integer, y As Integer) As Boolean
        'space bar thingie
        'True: game end
        If minefieldRevealed(x, y) Then Return False
        minefieldRevealed(x, y) = True
        minefieldMark(x, y) = False
        If minefield(x, y) Then
            'Game end
            Return True
        Else
            Dim isEmpty As Boolean = True 'adjacent
            Dim adjN As Integer = 0
            For Each dx In {-1, 0, 1}
                For Each dy In {-1, 0, 1}
                    If x + dx < 0 Or y + dy < 0 Or mineW <= x + dx Or mineH <= y + dy Then Continue For
                    If minefield(x + dx, y + dy) Then
                        adjN += 1
                        isEmpty = False
                    End If
                Next
            Next
            Dim nextCursorPos As (Integer, Integer) = getCursorPosition(x, y)
            Console.SetCursorPosition(nextCursorPos.Item1, nextCursorPos.Item2)
            Select Case adjN
                Case 1
                    Console.ForegroundColor = ConsoleColor.Blue
                Case 2
                    Console.ForegroundColor = ConsoleColor.Green
                Case 3
                    Console.ForegroundColor = ConsoleColor.DarkYellow
                Case 4
                    Console.ForegroundColor = ConsoleColor.Magenta
                Case 5
                    Console.ForegroundColor = ConsoleColor.DarkMagenta
                Case 6
                    Console.ForegroundColor = ConsoleColor.Red
                Case 7
                    Console.ForegroundColor = ConsoleColor.DarkRed
                Case 8
                    Console.ForegroundColor = ConsoleColor.DarkBlue
                Case 9
                    Console.ForegroundColor = ConsoleColor.Yellow
                Case Else
                    Console.ForegroundColor = ConsoleColor.White
            End Select
            Console.Write(If(adjN, adjN, " "))
            Console.ForegroundColor = ConsoleColor.White
            If isEmpty Then
                For Each dx In {-1, 0, 1}
                    For Each dy In {-1, 0, 1}
                        If x + dx < 0 Or y + dy < 0 Or mineW <= x + dx Or mineH <= y + dy Then Continue For
                        mineCheck(x + dx, y + dy)
                    Next
                Next
            End If
        End If
        Return False
    End Function
    Sub mineMark()
        If minefieldMark(cursorPosX, cursorPosY) Then
            Console.ForegroundColor = ConsoleColor.White
            Console.Write(".")
            minefieldMark(cursorPosX, cursorPosY) = False
            flagN -= 1
            If minefield(cursorPosX, cursorPosY) Then
                mineNLeft += 1
            End If
        Else
            If flagN >= mineN Then Return
            Console.ForegroundColor = ConsoleColor.Cyan
            Console.Write("M")
            Console.ForegroundColor = ConsoleColor.White
            flagN += 1
            minefieldMark(cursorPosX, cursorPosY) = True
            If minefield(cursorPosX, cursorPosY) Then
                mineNLeft -= 1
            End If
        End If
    End Sub
    Sub revealMines()
        Console.ForegroundColor = ConsoleColor.Red
        For x As Integer = 0 To mineW - 1
            For y As Integer = 0 To mineH - 1
                If minefield(x, y) Then
                    Dim nextCursorPos As (Integer, Integer) = getCursorPosition(x, y)
                    Console.SetCursorPosition(nextCursorPos.Item1, nextCursorPos.Item2)
                    Console.Write("*")
                End If
            Next
        Next
        Console.ForegroundColor = ConsoleColor.White
    End Sub
    Sub gameplay()
        Dim nc As (Integer, Integer) = getCursorPosition(cursorPosX, cursorPosY)
        Console.SetCursorPosition(nc.Item1, nc.Item2)
        While True
            Dim keyIn As ConsoleKey = Console.ReadKey(True).Key
            Select Case keyIn
                Case ConsoleKey.LeftArrow, ConsoleKey.A
                    If 0 < cursorPosX Then
                        cursorPosX -= 1
                    End If
                Case ConsoleKey.RightArrow, ConsoleKey.D
                    If cursorPosX < mineW - 1 Then
                        cursorPosX += 1
                    End If
                Case ConsoleKey.UpArrow, ConsoleKey.W
                    If 0 < cursorPosY Then
                        cursorPosY -= 1
                    End If
                Case ConsoleKey.DownArrow, ConsoleKey.S
                    If cursorPosY < mineH - 1 Then
                        cursorPosY += 1
                    End If
                Case ConsoleKey.Spacebar
                    If mineCheck(cursorPosX, cursorPosY) Then
                        revealMines()
                        Console.SetCursorPosition(0, mineH + 5)
                        Console.WriteLine("You Lose!")
                        Exit While
                    End If
                Case ConsoleKey.X, ConsoleKey.M
                    If minefieldRevealed(cursorPosX, cursorPosY) Then Exit Select
                    mineMark()
                    writeTopBar()
            End Select

            'Debug
            'Console.SetCursorPosition(0, mineH + 2)
            'Console.WriteLine($"{cursorPosX} {cursorPosY} {mineNLeft}")

            Dim nextCursorPos As (Integer, Integer) = getCursorPosition(cursorPosX, cursorPosY)
            Console.SetCursorPosition(nextCursorPos.Item1, nextCursorPos.Item2)

            If mineNLeft = 0 Then
                Console.SetCursorPosition(0, mineH + 5)
                Console.WriteLine("You won!")
                Exit While
            End If
        End While
    End Sub
    Sub setup()
        Console.WriteLine("Welcome to minesweeper! Please configure your game.")
        Console.WriteLine("HINT: for easy mode, type 9/9/10")
        While True
            Dim nw As Integer
            Try
                Console.Write("Minefield width> ")
                nw = Console.ReadLine()
                If nw = 0 Then
                    Console.WriteLine("Please type an integer bigger than 0.")
                Else
                    mineW = nw
                    Exit While
                End If
            Catch ex As InvalidCastException
                Console.WriteLine("Please only type a number that is bigger than 0.")
            End Try
        End While
        While True
            Dim nh As Integer
            Try
                Console.Write("Minefield height> ")
                nh = Console.ReadLine()
                If nh = 0 Then
                    Console.WriteLine("Please type an integer bigger than 0.")
                Else
                    mineH = nh
                    Exit While
                End If
            Catch ex As InvalidCastException
                Console.WriteLine("Please only type a number that is bigger than 0.")
            End Try
        End While
        While True
            Dim nm As Integer
            Try
                Console.Write("Number of mines> ")
                nm = Console.ReadLine()
                If nm = 0 Then
                    Console.WriteLine("Please type an integer bigger than 0.")
                ElseIf nm >= mineW * mineH Then
                    Console.WriteLine("The number of mines should be less than number of cells in the minefield.")
                Else
                    mineN = nm
                    mineNLeft = nm
                    Exit While
                End If
            Catch ex As InvalidCastException
                Console.WriteLine("Please only type a number that is bigger than 0.")
            End Try
        End While
        ReDim minefield(mineW, mineH)
        ReDim minefieldRevealed(mineW, mineH)
        ReDim minefieldMark(mineW, mineH)
        Console.Clear()
    End Sub
    Sub Main(args As String())
        Console.BackgroundColor = ConsoleColor.Black
        Console.ForegroundColor = ConsoleColor.White

        setup()
        initialiseBoard()
        initialiseMinefield()
        gameplay()
    End Sub
End Module
