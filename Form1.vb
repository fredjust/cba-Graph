Option Explicit On

Imports System.Drawing.Drawing2D

Public Class frmMain

    'FredJust@gmail.com

#Region "Les Images PNG"
    Dim wp As New Bitmap(My.Resources.wp)
    Dim wr As New Bitmap(My.Resources.wr)
    Dim wn As New Bitmap(My.Resources.wn)
    Dim wb As New Bitmap(My.Resources.wb)
    Dim wq As New Bitmap(My.Resources.wq)
    Dim wk As New Bitmap(My.Resources.wk)

    Dim bp As New Bitmap(My.Resources.bp)
    Dim br As New Bitmap(My.Resources.br)
    Dim bn As New Bitmap(My.Resources.bn)
    Dim bb As New Bitmap(My.Resources.bb)
    Dim bq As New Bitmap(My.Resources.bq)
    Dim bk As New Bitmap(My.Resources.bk)

    Dim bboard As New Bitmap(My.Resources.board90)
    Dim bHaut As New Bitmap(My.Resources.bHaut)
    Dim bCote As New Bitmap(My.Resources.bcote)

    Dim greenCircle As New Bitmap(My.Resources.vert)
    Dim redCircle As New Bitmap(My.Resources.rouge)
    Dim blueCircle As New Bitmap(My.Resources.bleu)
    Dim GreenCross As New Bitmap(My.Resources.pg)
#End Region


    Dim TheScreenBoard As New ScreenBoard


    'graphic sur la picturebox
    'je ne comprends pas trop comment cela fonctionne vraiment
    Dim bmp_backBuffer As New Bitmap(My.Resources.board90)
    Dim gr_chessboard As Graphics = Graphics.FromImage(bmp_backBuffer)

    'variable globale quelle horreur
    Dim sqFrom As String
    Dim sqTo As String
    Dim EffaceNoir As Boolean = False   'une rustine de dernière minute

    'mon objet pour gérer les mouvements
    Public ThePOS As ObjFenMoves


#Region "Fonction de dessin"


    'retourne le bitmap correspondant à la piece d'une notation FEN
    Private Function bmpPiece(ByVal name As Char) As Bitmap
        Select Case name
            Case "P"
                Return wp
            Case "R"
                Return wr
            Case "N"
                Return wn
            Case "B"
                Return wb
            Case "Q"
                Return wq
            Case "K"
                Return wk

            Case "p"
                Return bp
            Case "r"
                Return br
            Case "n"
                Return bn
            Case "b"
                Return bb
            Case "q"
                Return bq
            Case "k"
                Return bk

            Case "1"
                Return redCircle
            Case "2"
                Return greenCircle
            Case "3"
                Return blueCircle
            Case "4"
                Return GreenCross

        End Select
    End Function

    'dessine une pièce sur une case
    Private Sub PutPiece(ByVal sqIndex As Byte, ByVal Initiale As Char)

        gr_chessboard.DrawImage(bmpPiece(Initiale), TheScreenBoard.rect_square(sqIndex))

    End Sub

    'dessine un symbole sur une case
    Private Sub PutSymbol(ByVal sqIndex As Byte, ByVal Initiale As Char)

        Dim p As Graphics = PictureBox1.CreateGraphics

        p.DrawImage(bmpPiece(Initiale), TheScreenBoard.rect_square(sqIndex))

    End Sub

    'dessine un cercle sur une case TODO test pour la suite
    Private Sub PutCircle(ByVal sqIndex As Byte)
        Dim rect As Rectangle
        Dim p As Graphics = PictureBox1.CreateGraphics
        rect.X = TheScreenBoard.rect_square(sqIndex).X '+ TheScreenBoard.size_square
        rect.Y = TheScreenBoard.rect_square(sqIndex).Y '+ TheScreenBoard.size_square
        rect.Width = TheScreenBoard.size_square '- TheScreenBoard.size_square
        rect.Height = TheScreenBoard.size_square '- TheScreenBoard.size_square

        Dim trnsRedBrush As New SolidBrush(Color.FromArgb(128, 255, 0, 0))
        ' Create pen.
        Dim aPen As New Pen(trnsRedBrush, 1) '  TheScreenBoard.size_square / 10)

        p.DrawEllipse(aPen, rect)

    End Sub

    'dessine un rectangle sur une case 
    Private Sub PutRect(ByVal sqIndex As Byte)
        Dim rect As Rectangle
        Dim p As Graphics = PictureBox1.CreateGraphics
        rect.X = TheScreenBoard.rect_square(sqIndex).X '+ TheScreenBoard.size_square
        rect.Y = TheScreenBoard.rect_square(sqIndex).Y '+ TheScreenBoard.size_square
        rect.Width = TheScreenBoard.size_square '- TheScreenBoard.size_square
        rect.Height = TheScreenBoard.size_square '- TheScreenBoard.size_square

        Dim trnsRedBrush As New SolidBrush(Color.FromArgb(128, 0, 255, 255))
        ' Create pen.
        Dim aPen As New Pen(trnsRedBrush, 1) '  TheScreenBoard.size_square / 10)

        p.FillRectangle(trnsRedBrush, rect)
        'Dim aReg As New Region(rect)


    End Sub

    'dessine un cercle dans un carré sur une case 
    Private Sub PutReg(ByVal sqIndex As Byte)
        Dim rect As Rectangle
        Dim grPath As New GraphicsPath
        Dim p As Graphics = PictureBox1.CreateGraphics
        rect.X = TheScreenBoard.rect_square(sqIndex).X '+ TheScreenBoard.size_square
        rect.Y = TheScreenBoard.rect_square(sqIndex).Y '+ TheScreenBoard.size_square
        rect.Width = TheScreenBoard.size_square '- TheScreenBoard.size_square
        rect.Height = TheScreenBoard.size_square '- TheScreenBoard.size_square

        Dim trnsRedBrush As New SolidBrush(Color.FromArgb(128, 0, 255, 255))
        ' Create pen.
        Dim aPen As New Pen(trnsRedBrush, 1) '  TheScreenBoard.size_square / 10)

        'p.FillRectangle(trnsRedBrush, rect)
        Dim aRegC As New Region(rect)
        'Dim aRegC As New Region(grPath)

        grPath.AddEllipse(rect)

        aRegC.Xor(grPath)




        p.FillRegion(trnsRedBrush, aRegC)

    End Sub

    'dessine l'échiquier
    Private Sub DrawChessBoard()

        Dim rect As Rectangle
        Dim pt As Point

        'place la pb dans le coin
        PictureBox1.Top = 10
        PictureBox1.Left = 10

        'dimension de la pb en fonction de la hauteur
        PictureBox1.Height = Me.ClientSize.Height - 20
        PictureBox1.Width = Me.ClientSize.Height - 20


        PictureBox1.Image = New Bitmap(PictureBox1.Width, PictureBox1.Height)

        bmp_backBuffer = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        gr_chessboard = Graphics.FromImage(bmp_backBuffer)

        'dessine le bord haut
        pt.X = 1 : pt.Y = 1 : gr_chessboard.DrawImage(bHaut, pt)
        'dessine le bord bas
        pt.X = 1 : pt.Y = PictureBox1.Height - (TheScreenBoard.size_border - 1) : gr_chessboard.DrawImage(bHaut, pt)
        'dessine le bord gauche
        pt.X = 1 : pt.Y = 1 : gr_chessboard.DrawImage(bCote, pt)
        'dessine le bord droit
        pt.X = PictureBox1.Width - (TheScreenBoard.size_border - 1) : pt.Y = 1 : gr_chessboard.DrawImage(bCote, pt)
        'dessine le bord haut
        pt.X = bHaut.Width - 1 : pt.Y = 1 : gr_chessboard.DrawImage(bHaut, pt)
        'dessine le bord bas
        pt.X = bHaut.Width - 1 : pt.Y = PictureBox1.Height - (TheScreenBoard.size_border - 1) : gr_chessboard.DrawImage(bHaut, pt)
        'dessine le bord gauche
        pt.X = 1 : pt.Y = bCote.Height - 1 : gr_chessboard.DrawImage(bCote, pt)
        'dessine le bord droit
        pt.X = PictureBox1.Width - (TheScreenBoard.size_border - 1) : pt.Y = bCote.Height - 1 : gr_chessboard.DrawImage(bCote, pt)

        'dessine l'échiquier
        rect.X = TheScreenBoard.size_border : rect.Y = TheScreenBoard.size_border
        rect.Width = PictureBox1.Width - TheScreenBoard.size_border * 2 : rect.Height = PictureBox1.Height - TheScreenBoard.size_border * 2
        gr_chessboard.DrawImage(bboard, rect)

        'dessine le filet exterieur 
        rect.X = 0 : rect.Y = 0
        rect.Width = PictureBox1.Width - 1 : rect.Height = PictureBox1.Height - 1
        gr_chessboard.DrawRectangle(Pens.Brown, rect)

        'dessine le filet intérieur
        rect.X = TheScreenBoard.size_border - 1 : rect.Y = TheScreenBoard.size_border - 1
        rect.Width = PictureBox1.Width - (TheScreenBoard.size_border * 2 - 1) : rect.Height = PictureBox1.Height - (TheScreenBoard.size_border * 2 - 1)
        gr_chessboard.DrawRectangle(Pens.Black, rect)

        TheScreenBoard.size_square = (PictureBox1.Height - TheScreenBoard.size_border * 2) / 8

        With TabControl1
            .Top = 10
            .Left = PictureBox1.Left + PictureBox1.Width + 10
            .Height = Me.ClientSize.Height - .Top - 10
            .Width = Me.ClientSize.Width - (PictureBox1.Width + 30)
        End With

    End Sub


    'dessine les pièces
    Private Sub DrawPiece()

        If Me.WindowState <> FormWindowState.Minimized Then

            DrawChessBoard()

            PictureBox1.Image = bmp_backBuffer

            Dim LaPiece As Char
            For i = 11 To 88
                LaPiece = ThePOS.Board10x10(i)
                If LaPiece <> " " And LaPiece <> "*" Then
                    PutPiece(i, LaPiece)
                End If
            Next
        End If

    End Sub

    'place les symboles de déplacement d'une pièce
    Public Sub DrawMove()

        Dim sqMoves() As String
        Dim sq As String
        Dim txtMoves As String

        txtMoves = ThePOS.GetMoves(sqFrom)

        If txtMoves.Length > 0 Then
            sqMoves = txtMoves.Split(" ")

            For i = 0 To sqMoves.Count - 1
                sq = sqMoves(i)
                If sq.Substring(0, 1) <> "x" Then
                    If Not ThePOS.IsValidMove(sqFrom & sq) Then
                        PutSymbol(ThePOS.SquareIndex(sq), "3")
                    Else
                        PutSymbol(ThePOS.SquareIndex(sq), "2")
                    End If
                Else
                    sq = sqMoves(i).Substring(1, 2)
                    If Not ThePOS.IsValidMove(sqFrom & sq) Then
                        PutSymbol(ThePOS.SquareIndex(sq), "3")
                    Else
                        PutSymbol(ThePOS.SquareIndex(sq), "4")
                    End If
                End If
            Next
        End If

        Dim txtCanTake As String = ThePOS.WhoCanTake(sqFrom)

        If txtCanTake.Length > 0 Then
            sqMoves = txtCanTake.Split(" ")
            For i = 0 To sqMoves.Count - 1
                sq = sqMoves(i)
                PutSymbol(ThePOS.SquareIndex(sq), "1")
            Next
        End If

    End Sub


#End Region

#Region "Evenement de la form1"

    Private Sub Form1_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        Debug.Print("DRAW Form1_Activated")
        DrawPiece()
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        wp.Dispose()
        wr.Dispose()
        wn.Dispose()
        wb.Dispose()
        wq.Dispose()
        wk.Dispose()

        bp.Dispose()
        br.Dispose()
        bn.Dispose()
        bb.Dispose()
        bq.Dispose()
        bk.Dispose()

        bboard.Dispose()
        bHaut.Dispose()
        bCote.Dispose()

        greenCircle.Dispose()
        redCircle.Dispose()
        blueCircle.Dispose()
        GreenCross.Dispose()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        PictureBox1.Top = 10
        PictureBox1.Left = 10
        pbReduire.Top = 10
        ThePOS = New ObjFenMoves()
        ThePOS.LocalPiece = "TCFDR"
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Static MustRedraw As Boolean
        pbReduire.Visible = False

        If (Me.ClientSize.Height - 20 - 36) Mod 8 <> 0 Then
            Debug.Print(Me.ClientSize.Height - 20 - 36)
            Me.Height = Me.Height + ((Me.ClientSize.Height - 20 - 36) Mod 8)
        End If

        Select Case Me.WindowState
            Case FormWindowState.Maximized
                Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
                Debug.Print("DRAW Form1_Resize")
                DrawPiece()
                MustRedraw = True
                pbReduire.Visible = True
                pbReduire.Left = Me.ClientSize.Width - pbReduire.Width - 10
            Case FormWindowState.Minimized
                MustRedraw = True
            Case FormWindowState.Normal
                If MustRedraw Then
                    Debug.Print("DRAW Form1_Resize")
                    DrawPiece()
                    MustRedraw = False
                End If
                Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
        End Select
    End Sub

    Private Sub Form1_ResizeEnd(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ResizeEnd
        Static WindowsSize As Integer
        If Me.WindowState = FormWindowState.Normal Then
            If WindowsSize <> Me.Width + Me.Height Then
                Debug.Print("DRAW Form1_ResizeEnd")
                DrawPiece()
                WindowsSize = Me.Width + Me.Height
            End If
        End If
    End Sub

#End Region

#Region "Evenement de la picturebox1"

    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        Dim str_name_square As String
        Dim cursor_pos As Point
        Dim b_index_square As Byte

        'récupère le nom de la case sur lequel le curseur se trouve
        str_name_square = Chr(97 + Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8)) _
                + (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)).ToString

        b_index_square = (Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8) + 1) + (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)) * 10

        If e.Button = MouseButtons.Left Then

            Dim pt_square_clicked As Point
            pt_square_clicked.X = Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8)
            pt_square_clicked.Y = Math.Truncate((e.Y - 18) / (PictureBox1.Width - 36) * 8)

            Dim pt_center_square_clicked As Point
            With pt_center_square_clicked
                .X = 18 + (pt_square_clicked.X + 0.5) * TheScreenBoard.size_square
                .Y = 18 + (pt_square_clicked.Y + 0.5) * TheScreenBoard.size_square
            End With

            Dim pt_diff_with_center As Point

            With pt_diff_with_center
                .X = e.X - pt_center_square_clicked.X
                .Y = e.Y - pt_center_square_clicked.Y
            End With

            cursor_pos.X = Cursor.Position.X - pt_diff_with_center.X
            cursor_pos.Y = Cursor.Position.Y - pt_diff_with_center.Y

            Cursor.Position = cursor_pos






            If ThePOS.Board10x10(b_index_square) <> " " Then
                'Dim center_square As Point
                'center_square.X =
                'center_square.Y =
                'Cursor.Position =
                Cursor = New Cursor(CType(New Bitmap(bmpPiece(ThePOS.Board10x10(b_index_square)).GetThumbnailImage(TheScreenBoard.size_square, TheScreenBoard.size_square, Nothing, IntPtr.Zero)), Bitmap).GetHicon())
            Else
                Cursor = Cursors.Hand
            End If

            If sqFrom = "" Then
                sqFrom = str_name_square
                DrawMove()
            Else
                If sqFrom <> str_name_square Then
                    sqTo = sqFrom
                End If
            End If
        Else

            PutReg(b_index_square)
        End If
    End Sub

    Private Sub PictureBox1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        Dim sqI As String

        'récupère le nom de la case sur lequel le curseur se trouve
        sqI = Chr(97 + Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8)) _
                    + (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)).ToString

        Cursor = Cursors.Hand

        'si on a changé de case depuis mousedown
        'déplacement par drag&drop
        If sqI <> sqFrom Then
            sqTo = sqI
            If ThePOS.IsValidMove(sqFrom & sqTo) Then
                AddMove()
                Debug.Print("DRAW PictureBox1_MouseUp")
                DrawPiece()
                sqFrom = ""
                sqTo = ""
            Else
                sqFrom = ""
                sqTo = ""
                Debug.Print("DRAW PictureBox1_MouseUp")
                DrawPiece()

            End If
        End If



    End Sub

#End Region

#Region "Gestion des mouvements et de la ListView"

    'efface tous les items suivant dans la listview
    Public Sub Deletenextitem()
        On Error GoTo err

        If lvMoves.SelectedIndices(0) <> lvMoves.Items.Count - 1 Then
            While lvMoves.SelectedIndices(0) <> lvMoves.Items.Count - 1
                lvMoves.Items.RemoveAt(lvMoves.Items.Count - 1)
            End While
            If EffaceNoir Then
                If lvMoves.Items(lvMoves.Items.Count - 1).SubItems.Count = 3 Then
                    lvMoves.Items(lvMoves.Items.Count - 1).SubItems.RemoveAt(2)
                End If
            End If
        End If
err:

    End Sub

    'affiche le mouvement dans la listview lvmoves
    Private Sub AddMove()
        Dim lvi As New ListViewItem

        If sqFrom <> sqTo Then

            If ThePOS.IsValidMove(sqFrom & sqTo) Then



                Deletenextitem()

                If ThePOS.WhiteToPlay Then
                    lvi.Text = ThePOS.MovesPlayed.ToString & "."
                    lvi.SubItems.Add(ThePOS.PGNmove(sqFrom & sqTo))
                    ThePOS.MakeMove(sqFrom & sqTo)
                    lvi.SubItems(1).Tag = ThePOS.GetFEN()
                    lvMoves.Items.Add(lvi)

                Else
                    lvi = lvMoves.Items(lvMoves.Items.Count - 1)
                    lvi.SubItems.Add(ThePOS.PGNmove(sqFrom & sqTo))
                    ThePOS.MakeMove(sqFrom & sqTo)
                    lvi.SubItems(2).Tag = ThePOS.GetFEN()
                End If



                lvMoves.Items(lvMoves.Items.Count - 1).Selected = True



            End If
        End If
    End Sub

#End Region

#Region "Evenement listView"
    Private Sub lvMoves_MouseClick1(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lvMoves.MouseClick
        On Error Resume Next
        If e.X > lvMoves.Columns(0).Width + lvMoves.Columns(1).Width Then
            ThePOS.SetFEN(lvMoves.SelectedItems(0).SubItems(2).Tag)
            EffaceNoir = False
            Debug.Print("DRAW lvMoves_MouseClick1")
            DrawPiece()
        Else
            ThePOS.SetFEN(lvMoves.SelectedItems(0).SubItems(1).Tag)
            EffaceNoir = True
            Debug.Print("DRAW lvMoves_MouseClick1")
            DrawPiece()
        End If
    End Sub
#End Region


#Region "Evenement Bouton reduire"
    'gestion d'un bouton réduire perso nécessaire en mode plein écran
    Private Sub pbReduire_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles pbReduire.Click
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub pbReduire_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles pbReduire.MouseEnter
        pbReduire.BackgroundImage = My.Resources.reduire
    End Sub

    Private Sub pbReduire_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles pbReduire.MouseLeave
        pbReduire.BackgroundImage = My.Resources.reduire0
    End Sub

#End Region


End Class
