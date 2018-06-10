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

    Dim PieceSize As Integer

    'graphic sur la picturebox
    'je ne comprends pas trop comment cela fonctionne vraiment
    Dim backBuffer As New Bitmap(My.Resources.board90)
    Dim g As Graphics = Graphics.FromImage(backBuffer)

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

    'retourne la position X en pixel de la case dans la fenetre a partir d'un numero de case
    Private Function Xsqi(ByVal sqi As Byte) As Integer
        Dim colonne As Byte
        Dim ligne As Byte

        colonne = sqi Mod 10
        ligne = sqi \ 10

        Return 18 + (colonne - 1) * PieceSize

    End Function

    'retourne la position Y en pixel de la case dans la fenetre a partir d'un numero de case
    Private Function Ysqi(ByVal sqi As Byte) As Integer
        Dim colonne As Byte
        Dim ligne As Byte

        colonne = sqi Mod 10
        ligne = sqi \ 10

        Return 18 + (8 - ligne) * PieceSize

    End Function

    'dessine une pièce sur une case
    Private Sub PutPiece(ByVal sqIndex As Byte, ByVal Initiale As Char)
        Dim rect As Rectangle
        rect.X = Xsqi(sqIndex)
        rect.Y = Ysqi(sqIndex)
        rect.Width = PieceSize
        rect.Height = PieceSize

        g.DrawImage(bmpPiece(Initiale), rect)

    End Sub

    'dessine un symbole sur une case
    Private Sub PutSymbol(ByVal sqIndex As Byte, ByVal Initiale As Char)
        Dim rect As Rectangle
        Dim p As Graphics = PictureBox1.CreateGraphics
        rect.X = Xsqi(sqIndex)
        rect.Y = Ysqi(sqIndex)
        rect.Width = PieceSize
        rect.Height = PieceSize

        p.DrawImage(bmpPiece(Initiale), rect)

    End Sub

    'dessine un cercle sur une case TODO test pour la suite
    Private Sub PutCircle(ByVal sqIndex As Byte, ByVal Initiale As Char)
        Dim rect As Rectangle
        Dim p As Graphics = PictureBox1.CreateGraphics
        rect.X = Xsqi(sqIndex) + PieceSize / 20
        rect.Y = Ysqi(sqIndex) + PieceSize / 20
        rect.Width = PieceSize - PieceSize / 10
        rect.Height = PieceSize - PieceSize / 10

        Dim trnsRedBrush As New SolidBrush(Color.FromArgb(128, 255, 0, 0))
        ' Create pen.
        Dim aPen As New Pen(trnsRedBrush, PieceSize / 10)

        p.DrawEllipse(aPen, rect)

    End Sub

    'dessine l'échiquier
    Private Sub DrawChessBoard()

        Dim rect As Rectangle
        Dim pt As Point

        PictureBox1.Top = 10
        PictureBox1.Left = 10

        PictureBox1.Height = Me.ClientSize.Height - 20
        PictureBox1.Width = Me.ClientSize.Height - 20

        PictureBox1.Image = New Bitmap(PictureBox1.Width, PictureBox1.Height)

        backBuffer = New Bitmap(PictureBox1.Width, PictureBox1.Height)
        g = Graphics.FromImage(backBuffer)

        pt.X = 1 : pt.Y = 1 : g.DrawImage(bHaut, pt)                        'dessine le bord haut
        pt.X = 1 : pt.Y = PictureBox1.Height - 17 : g.DrawImage(bHaut, pt)  'dessine le bord bas
        pt.X = 1 : pt.Y = 1 : g.DrawImage(bCote, pt)                        'dessine le bord gauche
        pt.X = PictureBox1.Width - 17 : pt.Y = 1 : g.DrawImage(bCote, pt)   'dessine le bord droit

        pt.X = bHaut.Width - 1 : pt.Y = 1 : g.DrawImage(bHaut, pt)                        'dessine le bord haut
        pt.X = bHaut.Width - 1 : pt.Y = PictureBox1.Height - 17 : g.DrawImage(bHaut, pt)  'dessine le bord bas
        pt.X = 1 : pt.Y = bCote.Height - 1 : g.DrawImage(bCote, pt)                        'dessine le bord gauche
        pt.X = PictureBox1.Width - 17 : pt.Y = bCote.Height - 1 : g.DrawImage(bCote, pt)   'dessine le bord droit

        rect.X = 18 : rect.Y = 18
        rect.Width = PictureBox1.Width - 36 : rect.Height = PictureBox1.Height - 36
        g.DrawImage(bboard, rect)                                           'dessine l'échiquier

        rect.X = 0 : rect.Y = 0
        rect.Width = PictureBox1.Width - 1 : rect.Height = PictureBox1.Height - 1
        g.DrawRectangle(Pens.Brown, rect)                                   'dessine le filet exterieur 

        rect.X = 17 : rect.Y = 17
        rect.Width = PictureBox1.Width - 35 : rect.Height = PictureBox1.Height - 35
        g.DrawRectangle(Pens.Black, rect)                                   'dessine le filet intérieur

        PieceSize = (PictureBox1.Height - 36) / 8

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

            PictureBox1.Image = backBuffer

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

        Select Case Me.WindowState
            Case FormWindowState.Maximized
                Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
                DrawPiece()
                MustRedraw = True
                pbReduire.Visible = True
                pbReduire.Left = Me.ClientSize.Width - pbReduire.Width - 10
            Case FormWindowState.Minimized
                MustRedraw = True
            Case FormWindowState.Normal
                If MustRedraw Then
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
                DrawPiece()
                WindowsSize = Me.Width + Me.Height
            End If
        End If
    End Sub

#End Region

#Region "Evenement de la picturebox1"

    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown
        Dim sqI As String

        If e.Button = MouseButtons.Left Then

            'récupère le nom de la case sur lequel le curseur se trouve
            sqI = Chr(97 + Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8)) _
                    + (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)).ToString

            If sqFrom = "" Then
                sqFrom = sqI
                DrawMove()
            Else
                If sqFrom <> sqI Then
                    sqTo = sqFrom
                End If
            End If

        End If
    End Sub

    Private Sub PictureBox1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp
        Dim sqI As String

        'récupère le nom de la case sur lequel le curseur se trouve
        sqI = Chr(97 + Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8)) _
                    + (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)).ToString

        'si on a changé de case depuis mousedown
        'déplacement par drag&drop
        If sqI <> sqFrom Then
                sqTo = sqI
                If ThePOS.IsValidMove(sqFrom & sqTo) Then
                    AddMove()
                    DrawPiece()
                    sqFrom = ""
                    sqTo = ""
                Else
                    sqFrom = ""
                    sqTo = ""
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



                deletenextitem()

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
    Private Sub lvMoves_MouseClick1(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles lvMoves.MouseClick
        On Error Resume Next
        If e.X > lvMoves.Columns(0).Width + lvMoves.Columns(2).Width Then
            ThePOS.SetFEN(lvMoves.SelectedItems(0).SubItems(2).Tag)
            EffaceNoir = False
            DrawPiece()
        Else
            ThePOS.SetFEN(lvMoves.SelectedItems(0).SubItems(1).Tag)
            EffaceNoir = True
            DrawPiece()
        End If
    End Sub
#End Region


#Region "Evenement Bouton reduire"

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
