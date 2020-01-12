''FredJust@gmail.com

' IDEES DE 2015
'Définir un nouveau FORMAT : PGA (Portable Game Analyse)
'- lorsque deux variantes amène à la même position 
'   permettre de changer celle qui se trouve dans la ligne principale
'- permettre de retrouver la ligne principale si une variante arrive à la même position
'- affiche les coups suivant sous forme de fleche
'- un clic sur la fleche d'un coup pour le jouer
'- affiche les coups suivants dans la zone commentaires
'- différentier visuellement les bons coups des mauvais (fleches ou case en couleur) sans les confondre avec d'autres fleches
'- différentier le coup principale des variantes
'- pouvoir modifier un coup sans devoir tout ressaisir 
'- saisir les commentaire directement sans validation
'- enregistrer chaque modification automatiquement 
'- afficher le feuille de la variante completement sans commentaire
'- 3 zones échiquier commentaires et coups
'- ne pas melanger les commentaires avec les coups
'- un commentaire par position (possibilité d'afficher les commentaires suivants)
'- ne pas se limiter a 3 couleurs (rouge vert jaune
'bleu le coups possibles
'blanc noir ...(couleur standard CSS)
'- permettre la saisi d'un coup illégal
'- affiche des FEN => permet d'illustrer la structure des pions en supprimant les pièces
'et revenir à la position de la partie après

Option Explicit On

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Public Class frmMain

    Public pos_current As BoardPositions.aPosition

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

    Dim bboard As New Bitmap(My.Resources.board100)
    Dim bHaut As New Bitmap(My.Resources.bHaut)
    Dim bCote As New Bitmap(My.Resources.bcote)

    Dim greenCircle As New Bitmap(My.Resources.vert)
    Dim redCircle As New Bitmap(My.Resources.rouge)
    Dim blueCircle As New Bitmap(My.Resources.bleu)
    Dim GreenCross As New Bitmap(My.Resources.pg)
    Dim bmpSquare As New Bitmap(My.Resources.square)
#End Region


    Dim TheScreenBoard As New ScreenBoard

    Dim BoardPos As New BoardPositions

    Dim bmp_backBuffer As Bitmap  'New Bitmap(My.Resources.board100)
    Dim gr_chessboard As Graphics ' = Graphics.FromImage(bmp_backBuffer)

    'variable globale quelle horreur
    'Dim sqFrom As String
    'Dim sqTo As String
    Dim EffaceNoir As Boolean = False   'une rustine de dernière minute

    'mon objet pour gérer les mouvements
    Public ThePOS As ObjFenMoves

#Region "gestion des string list"
    'ajoute un symbole ou le remplace

    Public Sub AddOtherSymbol(ByVal str_square As String, ByVal str_color As String)
        Dim toDel As Integer

        If pos_current.Symbols.Contains(str_square & "-" & str_color) Then
            toDel = pos_current.Symbols.IndexOf(str_square)
            pos_current.Symbols = pos_current.Symbols.Remove(toDel, 5)
        Else
            If pos_current.Symbols.Contains(str_square) Then
                toDel = pos_current.Symbols.IndexOf(str_square)
                pos_current.Symbols = pos_current.Symbols.Remove(toDel, 5)
                pos_current.Symbols &= str_square & "-" & str_color & " "
            Else
                pos_current.Symbols &= str_square & "-" & str_color & " "
            End If
        End If
    End Sub

    Public Sub AddArrow(ByVal str_squares As String, ByVal str_color As String)
        Dim toDel As Integer

        'contient deja cette fleche avec cette couleur on l'efface
        If pos_current.Arrows.Contains(str_squares & "-" & str_color) Then
            toDel = pos_current.Arrows.IndexOf(str_squares)
            pos_current.Arrows = pos_current.Arrows.Remove(toDel, 7)
        Else 'contient que la fleche on change la couleur
            If pos_current.Arrows.Contains(str_squares) Then
                toDel = pos_current.Arrows.IndexOf(str_squares)
                pos_current.Arrows = pos_current.Arrows.Remove(toDel, 7)
                pos_current.Arrows &= str_squares & "-" & str_color & " "
            Else 'la fleche n'y est pas on l'ajoute
                pos_current.Arrows &= str_squares & "-" & str_color & " "
            End If
        End If
    End Sub

#End Region
    

#Region "Fonction de dessin"


    'retourne le bitmap correspondant à la piece d'une notation FEN
    'TODO
    'a completer avec plein d'images utiles pour expliquer les positions
    'des cibles
    'des murailles
    'des pièges
    'chaque image étant affecté a une lettre autre que R N B Q K P
    'TODO utiliser une collection de bitmap avec comme key la lettre !
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
            Case "5"
                Return bmpSquare

        End Select
    End Function

    'dessine un symbole ou une piece sur une case
    Private Sub DrawSymbol(ByVal sqIndex As Byte, ByVal Initiale As Char)
        gr_chessboard.DrawImage(bmpPiece(Initiale), TheScreenBoard.rect_square(sqIndex))
    End Sub

    'dessine un cercle sur une case 
    Private Sub PutCircle(ByVal sqIndex As Byte)
        Dim rect As Rectangle
        rect.X = TheScreenBoard.rect_square(sqIndex).X + TheScreenBoard.size_square / 3
        rect.Y = TheScreenBoard.rect_square(sqIndex).Y + TheScreenBoard.size_square / 3
        rect.Width = TheScreenBoard.size_square / 3
        rect.Height = TheScreenBoard.size_square / 3

        Dim trnsRedBrush As New SolidBrush(Color.FromArgb(128, 255, 0, 0))

        Dim aPen As New Pen(trnsRedBrush, 1)

        gr_chessboard.FillEllipse(trnsRedBrush, rect)

    End Sub

    'dessine un rectangle sur une case 
    Private Sub PutRect(ByVal sqIndex As Byte)
        Dim rect As Rectangle
        'Dim p As Graphics = PictureBox1.CreateGraphics
        rect.X = TheScreenBoard.rect_square(sqIndex).X '+ TheScreenBoard.size_square
        rect.Y = TheScreenBoard.rect_square(sqIndex).Y '+ TheScreenBoard.size_square
        rect.Width = TheScreenBoard.size_square '- TheScreenBoard.size_square
        rect.Height = TheScreenBoard.size_square '- TheScreenBoard.size_square


        Dim aBrush As New SolidBrush(Color.FromArgb(128, TheScreenBoard.Color_square.Alt, TheScreenBoard.Color_square.Shift, TheScreenBoard.Color_square.Control))
        ' Create pen.
        Dim aPen As New Pen(aBrush, 1)

        gr_chessboard.FillRectangle(aBrush, rect)

    End Sub

    'dessine un cercle dans un carré sur une case 
    Private Sub PutReg(ByVal sqIndex As Byte, ByVal cColor As String)
        Dim rect As Rectangle
        Dim grPath As New GraphicsPath

        rect.X = TheScreenBoard.rect_square(sqIndex).X
        rect.Y = TheScreenBoard.rect_square(sqIndex).Y
        rect.Width = TheScreenBoard.size_square
        rect.Height = TheScreenBoard.size_square

        Dim aBrush As New SolidBrush(str2color(cColor))
        ' Create pen.
        Dim aPen As New Pen(aBrush, 1)

        Dim aRegC As New Region(rect)

        'le cercle a supprimer du rectangle
        rect.X = TheScreenBoard.rect_square(sqIndex).X '+ TheScreenBoard.size_square / 10
        rect.Y = TheScreenBoard.rect_square(sqIndex).Y ' + TheScreenBoard.size_square / 10
        rect.Width = TheScreenBoard.size_square '- TheScreenBoard.size_square / 5
        rect.Height = TheScreenBoard.size_square ' - TheScreenBoard.size_square / 5

        grPath.AddEllipse(rect)

        aRegC.Xor(grPath)

        gr_chessboard.FillRegion(aBrush, aRegC)

    End Sub

    'dessine l'échiquier
    'dans gr_chessboard 
    Private Sub DrawChessBoard()


        Dim rect As Rectangle
        Dim pt As Point

        'place la pb dans le coin
        PictureBox1.Top = 10
        PictureBox1.Left = 10

        'dimension de la pb en fonction de la hauteur
        PictureBox1.Height = Me.ClientSize.Height - 20
        PictureBox1.Width = Me.ClientSize.Height - 20

        'change la taille de la PictureBox pour que l'échiquier soit un multiple de 8
        While ((PictureBox1.Height - 36) Mod 8) <> 0
            PictureBox1.Height = PictureBox1.Height - 1
        End While
        PictureBox1.Width = PictureBox1.Height

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

        With lvMoves
            .Top = 0
            .Left = 0
            .Height = TabControl1.Height - 30
            .Width = 225
        End With

    End Sub

    'dessine les symboles contenus dans la liste
    Public Sub DrawAllMoveSymbol()
        Dim tempo() As String
        Dim str_sq As String
        Dim num_sym As Char

        If TheScreenBoard.MoveSymbols = "" Then Exit Sub

        tempo = TheScreenBoard.MoveSymbols.Split(" ")

        For i = 0 To tempo.Count - 2
            str_sq = tempo(i).Substring(0, 2)
            num_sym = tempo(i).Substring(3, 1)
            DrawSymbol(TheScreenBoard.index_Square(str_sq), num_sym)
        Next


    End Sub

    'dessine les symboles contenus dans la liste
    Public Sub DrawAllOtherSymbol()
        Dim tempo() As String
        Dim str_sq As String
        Dim chr_Color As Char

        If pos_current.Symbols = "" Then Exit Sub

        tempo = pos_current.Symbols.Split(" ")

        For i = 0 To tempo.Count - 2
            str_sq = tempo(i).Substring(0, 2)
            chr_Color = tempo(i).Substring(3, 1)
            PutReg(TheScreenBoard.index_Square(str_sq), chr_Color)
        Next


    End Sub

    'dessine les pièces
    'dans gr_chessboard 
    Private Sub DrawAllPieces()

        Dim LaPiece As Char
        For i = 11 To 88
            LaPiece = ThePOS.Board10x10(i)
            If LaPiece <> " " And LaPiece <> "*" Then
                DrawSymbol(i, LaPiece)
            End If
        Next

    End Sub

    'dessine les fleches
    'dans gr_chessboard 
    Private Sub DrawAllArrows()
        Dim tempo() As String
        Dim str_from, str_to As String
        Dim chr_Color As Char

        If pos_current.Arrows = "" Then Exit Sub

        tempo = pos_current.Arrows.Split(" ")

        For i = 0 To tempo.Count - 2
            str_from = tempo(i).Substring(0, 2)
            str_to = tempo(i).Substring(2, 2)
            chr_Color = tempo(i).Substring(5, 1)
            'TODO rajotuer le décallage
            DrawArrow(TheScreenBoard.pt_center(str_from), TheScreenBoard.pt_center(str_to), chr_Color)
        Next
    End Sub

    'renvoie le point situé entre deux points 
    'à la position relative pos_rel (0.5 pour le milieu)
    'ou au nombre de pixel pos_px
    Public Function pt_between(ByVal pt_from As Point, ByVal pt_to As Point, _
                               Optional ByVal pos_rel As Double = 0, Optional ByVal pos_px As Integer = 0) As Point
        Dim pt_vect As Point 'vecteur directeur
        Dim norm_vect As Double 'norme du vecteur

        Dim pt_return As Point

        'vecteur directeur
        With pt_vect
            .X = pt_to.X - pt_from.X
            .Y = pt_to.Y - pt_from.Y
            'norme du vecteur
            norm_vect = Math.Sqrt(.X * .X + .Y * .Y)
        End With


        If pos_rel <> 0 Then
            With pt_return
                .X = pt_from.X + pos_rel * pt_vect.X
                .Y = pt_from.Y + pos_rel * pt_vect.Y
            End With
        End If

        If pos_px <> 0 Then
            With pt_return
                .X = pt_from.X + pos_px * pt_vect.X / norm_vect
                .Y = pt_from.Y + pos_px * pt_vect.Y / norm_vect
            End With
        End If


        Return pt_return

    End Function

    'ajoute les 7 points formant une fleche autour de deux points
    'de largeur de dist_px pixel
    'dans la liste de points  pt_ortho
    Public Sub Pts_for_Arrow(ByVal pt_from As Point, ByVal pt_to As Point, ByVal dist_px As Integer, _
                                  ByRef pts_arrow As List(Of Point))

        Dim pt_vect, pt_tempo, pti As Point
        Dim norm_vect As Double

        pts_arrow.Clear()

        'vecteur orthogonal (-b,a)
        With pt_vect
            .X = -(pt_to.Y - pt_from.Y)
            .Y = (pt_to.X - pt_from.X)
            'norme du vecteur
            norm_vect = Math.Sqrt(.X * .X + .Y * .Y)
        End With

        'point 0
        With pt_tempo
            .X = pt_from.X + 0.5 * dist_px * pt_vect.X / norm_vect
            .Y = pt_from.Y + 0.5 * dist_px * pt_vect.Y / norm_vect
        End With

        pts_arrow.Add(pt_tempo)
        'ptc(0) = pt_tempo
        'ptc(1) = pt_between(pt_from, pt_to, 0, -dist_px / 2)

        'point 1
        With pt_tempo
            .X = pt_from.X - 0.5 * dist_px * pt_vect.X / norm_vect
            .Y = pt_from.Y - 0.5 * dist_px * pt_vect.Y / norm_vect
        End With
        'ptc(2) = pt_tempo
        pts_arrow.Add(pt_tempo)

        'bout de la fleche
        pti = pt_between(pt_to, pt_from, 0, dist_px * 2)

        'point 2
        With pt_tempo
            .X = pti.X - 0.5 * dist_px * pt_vect.X / norm_vect
            .Y = pti.Y - 0.5 * dist_px * pt_vect.Y / norm_vect
        End With

        pts_arrow.Add(pt_tempo)

        'point 3
        With pt_tempo
            .X = pti.X - 1.5 * dist_px * pt_vect.X / norm_vect
            .Y = pti.Y - 1.5 * dist_px * pt_vect.Y / norm_vect
        End With

        pts_arrow.Add(pt_tempo)

        'point 4
        With pt_tempo
            .X = pt_to.X
            .Y = pt_to.Y
        End With

        pts_arrow.Add(pt_tempo)


        'point 5
        With pt_tempo
            .X = pti.X + 1.5 * dist_px * pt_vect.X / norm_vect
            .Y = pti.Y + 1.5 * dist_px * pt_vect.Y / norm_vect
        End With

        pts_arrow.Add(pt_tempo)

        'point 6
        With pt_tempo
            .X = pti.X + 0.5 * dist_px * pt_vect.X / norm_vect
            .Y = pti.Y + 0.5 * dist_px * pt_vect.Y / norm_vect
        End With

        pts_arrow.Add(pt_tempo)

    End Sub

    Public Sub DrawArrow(ByVal pt_from As Point, ByVal pt_to As Point, _
                          ByVal str_Color As String)

        Const px_size As Byte = 16
        Dim rect_from As Rectangle

        Dim pts_list As New List(Of Point)
        Dim pts_array() As Point
        Dim ptype() As Byte
        Dim grpt As GraphicsPath

        With rect_from
            .X = pt_from.X - px_size / 2
            .Y = pt_from.Y - px_size / 2
            .Width = px_size
            .Height = px_size
        End With

        'Dim aColor As Color = str2color(str_Color)
        'Dim aPen As New Pen(aColor)

        Pts_for_Arrow(pt_from, pt_to, px_size, pts_list)

        Array.Resize(pts_array, pts_list.Count)
        Array.Resize(ptype, pts_list.Count)

        ptype(0) = 0
        For i = 0 To pts_list.Count - 1
            pts_array(i) = pts_list(i)
            ptype(i) = 1
        Next

        Dim aBrush As New SolidBrush(str2color(str_Color))

        grpt = New GraphicsPath

        grpt.AddPolygon(pts_array)
        'gr_chessboard1.DrawPath(Pens.Black, grpt)

        grpt.FillMode = 1
        grpt.AddEllipse(rect_from)

        gr_chessboard.FillPath(aBrush, grpt)
        'gr_chessboard1.FillPath(aBrush, grpt)

    End Sub

    'calcule les symboles de déplacement d'une pièce
    'et les places dans la list TheScreenBoard.MoveSymbols
    Public Sub GetMovesSymbols(ByVal str_square As String)

        Dim sqMoves() As String
        Dim sq As String
        Dim txtMoves As String

        'efface la list des anciens symbol
        TheScreenBoard.MoveSymbols = ""

        txtMoves = ThePOS.GetMoves(str_square)

        If txtMoves.Length > 0 Then
            sqMoves = txtMoves.Split(" ")

            For i = 0 To sqMoves.Count - 1
                sq = sqMoves(i)
                If sq.Substring(0, 1) <> "x" Then
                    If Not ThePOS.IsValidMove(str_square & sq) Then
                        TheScreenBoard.MoveSymbols &= sq & "-3 "
                    Else
                        TheScreenBoard.MoveSymbols &= sq & "-2 "
                    End If
                Else
                    sq = sqMoves(i).Substring(1, 2)
                    If Not ThePOS.IsValidMove(str_square & sq) Then
                        TheScreenBoard.MoveSymbols &= sq & "-3 "
                    Else
                        TheScreenBoard.MoveSymbols &= sq & "-4 "

                    End If
                End If
            Next
        End If

        Dim txtCanTake As String = ThePOS.WhoCanTake(str_square)

        If txtCanTake.Length > 0 Then
            sqMoves = txtCanTake.Split(" ")
            For i = 0 To sqMoves.Count - 1
                sq = sqMoves(i)
                TheScreenBoard.MoveSymbols &= sq & "-1 "
            Next
        End If

    End Sub



    'dessine tout une fois pour toutes
    Public Sub DrawEveryThing()

        Debug.Print("drawEVERY")
        If Me.WindowState = FormWindowState.Minimized Then Exit Sub
        'dessine le fond et les bordures
        DrawChessBoard()
        'dessine les mouvements possibles
        DrawAllMoveSymbol()

        'dessine les mouvements possibles
        DrawAllOtherSymbol()

        'dessine les fleches
        DrawAllArrows()

        'dessine les pieces
        DrawAllPieces()

        'raffraichit l'affichage
        PictureBox1.Image = bmp_backBuffer
    End Sub

#End Region

#Region "Evenement de la form1"

    Private Sub Form1_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        Debug.Print("DRAW Form1_Activated")
        DrawEveryThing()
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
        'initialise la position 
        BoardPos.first_pos(pos_current)
        Add_Pos_LV(pos_current)
        'TODO remplacer les ressources par des images chargeables
        'wn = Image.FromFile(Application.StartupPath & "\images\bn.png")
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Static MustRedraw As Boolean
        pbReduire.Visible = False

        Debug.Print(Me.WindowState)


        Select Case Me.WindowState
            'si la feuille est agrandie on passe en full screen
            'et on affiche le bouton de réduction
            Case FormWindowState.Maximized
                Me.FormBorderStyle = Windows.Forms.FormBorderStyle.None
                MustRedraw = True 'en passant en maximisé 
                pbReduire.Visible = True
                If pbReduire.Left <> Me.ClientSize.Width - pbReduire.Width - 10 Then
                    pbReduire.Left = Me.ClientSize.Width - pbReduire.Width - 10
                    DrawEveryThing()
                End If

                Debug.Print("SIZE FULL")
            Case FormWindowState.Minimized
                'MustRedraw = True
            Case FormWindowState.Normal
                If MustRedraw Then
                    DrawEveryThing()
                    MustRedraw = False
                End If
                Me.FormBorderStyle = Windows.Forms.FormBorderStyle.Sizable
                Debug.Print("SIZE NORMAL")
        End Select
    End Sub

    Private Sub Form1_ResizeEnd(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.ResizeEnd
        Static WindowsSize As Integer

        'vérifie que la form a bien changer de taille avant de redessiner tout
        If Me.WindowState = FormWindowState.Normal Then
            If WindowsSize <> Me.Width + Me.Height Then
                Debug.Print("DRAW Form1_ResizeEnd")
                DrawEveryThing()
                WindowsSize = Me.Width + Me.Height
            End If
        End If
    End Sub

#End Region



#Region "Evenement de la picturebox1"

    'recupère les infos du clic puis la case que l'on a cliqué
    'clic normal
    '   deplacement de la pièce qui s'y trouve
    '   modifier le curseur
    'clic droit
    '   debut d'un tracé ou d'un placement de symbole
    Private Sub PictureBox1_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseDown

        'récupère les infos du clic et les place dans .clicDown
        With TheScreenBoard.clicDown
            .X = e.X
            .Y = e.Y
            .id_square = (Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8) + 1) + _
                (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)) * 10
            .str_Square = Chr(97 + Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8)) _
                + (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)).ToString
            .RightClic = (e.Button = MouseButtons.Right)

            .Alt = False
            .Shift = False
            .Control = False

            If Control.ModifierKeys = Keys.Alt Then .Alt = True
            If Control.ModifierKeys = Keys.Shift Then .Shift = True
            If Control.ModifierKeys = Keys.Control Then .Control = True

            If Control.ModifierKeys = (Keys.Alt + Keys.Shift) Then
                .Alt = True
                .Shift = True
            End If

            If Control.ModifierKeys = (Keys.Alt + Keys.Control) Then
                .Alt = True
                .Control = True
            End If

            If Control.ModifierKeys = (Keys.Shift + Keys.Control) Then
                .Shift = True
                .Control = True
            End If


            If .RightClic Then 'on trace quelque chose

            Else 'on tente de déplacer une pièce
                'si la case contient une piece
                If ThePOS.Board10x10(.id_square) <> " " Then

                    'on replace le curseur au centre de la case
                    Dim pt_square_clicked As Point 'case cliqué dans le sens de l'écran
                    pt_square_clicked.X = Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8)
                    pt_square_clicked.Y = Math.Truncate((e.Y - 18) / (PictureBox1.Width - 36) * 8)

                    Dim pt_center_square_clicked As Point 'centre de la case en pixel
                    With pt_center_square_clicked
                        .X = 18 + (pt_square_clicked.X + 0.5) * TheScreenBoard.size_square
                        .Y = 18 + (pt_square_clicked.Y + 0.5) * TheScreenBoard.size_square
                    End With

                    Dim pt_diff_with_center As Point 'différence du clic avec le centre

                    With pt_diff_with_center
                        .X = e.X - pt_center_square_clicked.X
                        .Y = e.Y - pt_center_square_clicked.Y
                    End With

                    Dim cursor_pos As Point 'position du curseur sur l'écran au moment du clic

                    cursor_pos.X = Cursor.Position.X - pt_diff_with_center.X
                    cursor_pos.Y = Cursor.Position.Y - pt_diff_with_center.Y

                    Cursor.Position = cursor_pos
                    'on change le curseur 
                    Cursor = New Cursor(CType(New Bitmap(bmpPiece(ThePOS.Board10x10(.id_square)).GetThumbnailImage(TheScreenBoard.size_square, TheScreenBoard.size_square, Nothing, IntPtr.Zero)), Bitmap).GetHicon())

                    'on calcule les symboles
                    GetMovesSymbols(.str_Square)
                    DrawEveryThing()
                Else
                    'on remet le curseur main
                    Cursor = Cursors.Hand
                End If
            End If

        End With







        'If e.Button = MouseButtons.Left Then

        'Else

        '    TheScreenBoard.Color_square.Alt = 0
        '    TheScreenBoard.Color_square.Shift = 0
        '    TheScreenBoard.Color_square.Control = 0

        '    If Control.ModifierKeys = Keys.Alt Then TheScreenBoard.Color_square.Alt = 255
        '    If Control.ModifierKeys = Keys.Shift Then TheScreenBoard.Color_square.Shift = 255
        '    If Control.ModifierKeys = Keys.Control Then TheScreenBoard.Color_square.Control = 255

        '    If Control.ModifierKeys = (Keys.Alt + Keys.Shift) Then
        '        TheScreenBoard.Color_square.Alt = 255
        '        TheScreenBoard.Color_square.Shift = 255
        '    End If

        '    If Control.ModifierKeys = (Keys.Alt + Keys.Control) Then
        '        TheScreenBoard.Color_square.Alt = 255
        '        TheScreenBoard.Color_square.Control = 255
        '    End If

        '    If Control.ModifierKeys = (Keys.Shift + Keys.Control) Then
        '        TheScreenBoard.Color_square.Shift = 255
        '        TheScreenBoard.Color_square.Control = 255
        '    End If

        '    ' PutReg(b_index_square)
        '        End If
    End Sub

    'récupère les infos du clic
    'clic gauche
    '   deplace la piece
    'clic droit
    '   place le symbole ou la fleche
    Private Sub PictureBox1_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseUp

        Cursor = Cursors.Hand
        TheScreenBoard.MoveSymbols = ""

        'récupère les infos du clic et les place dans .clicDown
        With TheScreenBoard.clicUp
            .X = e.X
            .Y = e.Y
            .id_square = (Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8) + 1) + _
                (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)) * 10
            .str_Square = Chr(97 + Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8)) _
                + (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)).ToString
            .RightClic = (e.Button = MouseButtons.Right)

            .Alt = False
            .Shift = False
            .Control = False

            If Control.ModifierKeys = Keys.Alt Then .Alt = True
            If Control.ModifierKeys = Keys.Shift Then .Shift = True
            If Control.ModifierKeys = Keys.Control Then .Control = True

            If Control.ModifierKeys = (Keys.Alt + Keys.Shift) Then
                .Alt = True
                .Shift = True
            End If

            If Control.ModifierKeys = (Keys.Alt + Keys.Control) Then
                .Alt = True
                .Control = True
            End If

            If Control.ModifierKeys = (Keys.Shift + Keys.Control) Then
                .Shift = True
                .Control = True
            End If

        End With

        Dim sqFrom, sqTo As String
        sqFrom = TheScreenBoard.clicDown.str_Square
        sqTo = TheScreenBoard.clicUp.str_Square

        If sqFrom <> sqTo Then 'si on a changé de case depuis mousedown
            If TheScreenBoard.clicUp.RightClic Then 'si clic droit
                'on trace une fleche
                AddArrow(sqFrom & sqTo, cur2str())
            Else 'clic normal
                'on tente de déplacer une piece
                If ThePOS.IsValidMove(sqFrom & sqTo) Then
                    AddMove(sqFrom, sqTo)

                End If
            End If
        Else 'on ne change pas de case
            If TheScreenBoard.clicUp.RightClic Then 'si clic droit
                'on place un symbole
                AddOtherSymbol(sqTo, cur2str())

            End If
        End If



        DrawEveryThing()
    End Sub

#End Region

    'renvoie la couleur correspondant aux dernières touches lors du clic
    Public Function cur2color() As Color

        With TheScreenBoard.clicUp

            If .Alt Then
                If .Shift Then
                    Return str2color("M")
                End If
                If .Control Then
                    Return str2color("C")
                End If
                Return str2color("B")
            End If

            If .Shift Then
                Return str2color("R")
            End If

            If .Control Then
                Return str2color("Y")
            End If

        End With

        Return str2color("G")

    End Function

    'Renvoie l'initiale de la couleur
    Public Function cur2str() As String
        With TheScreenBoard.clicUp

            If .Alt Then
                If .Shift Then
                    Return ("M")
                End If
                If .Control Then
                    Return ("C")
                End If
                Return ("B")
            End If

            If .Shift Then
                Return ("R")
            End If

            If .Control Then
                Return ("Y")
            End If

        End With

        Return ("G")
    End Function

    'renvoie la couleur correspondant à une initiale
    Public Function str2color(ByVal str As String) As Color
        Select Case str
            Case "R"
                Return Color.FromArgb(128, 255, 0, 0)
            Case "G"
                Return Color.FromArgb(128, 0, 255, 0)
            Case "B"
                Return Color.FromArgb(128, 0, 0, 255)
            Case "Y"
                Return Color.FromArgb(128, 255, 255, 0)
            Case "M"
                Return Color.FromArgb(128, 255, 0, 255)
            Case "C"
                Return Color.FromArgb(128, 0, 255, 255)
            Case "W"
                Return Color.FromArgb(128, 255, 255, 255)
        End Select
    End Function

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
    Private Sub AddMove(ByVal sqFrom As String, ByVal sqTo As String)
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
        Debug.Print("BLANC " & lvMoves.Columns(1).Width & " NOIRS " & lvMoves.Columns(2).Width)
        If e.X > lvMoves.Columns(0).Width + lvMoves.Columns(1).Width Then
            ThePOS.SetFEN(lvMoves.SelectedItems(0).SubItems(2).Tag)
            EffaceNoir = False

        Else
            ThePOS.SetFEN(lvMoves.SelectedItems(0).SubItems(1).Tag)
            EffaceNoir = True
        End If
        DrawEveryThing()
    End Sub
#End Region

#Region "Gestion LVposition"

    Public Sub Add_Pos_LV(ByVal aPos As BoardPositions.aPosition)
        Dim NbLigne As Integer

        NbLigne = lvPositions.Items.Count + 1
        lvPositions.Items.Insert(NbLigne - 1, NbLigne)    'numéro de la position

        'Public Comments As String 'x|y|text||x|y|text||...

        With lvPositions
            NbLigne = .Items.Count - 1
            .Items(NbLigne).SubItems.Add(aPos.next_pos)
            .Items(NbLigne).SubItems.Add(aPos.last_pos)
            .Items(NbLigne).SubItems.Add(aPos.Arrows)
            .Items(NbLigne).SubItems.Add(aPos.Symbols)
            .Items(NbLigne).SubItems.Add(aPos.FEN)
            .Items(NbLigne).SubItems.Add(aPos.Comments)
        End With

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





    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click

    End Sub
End Class
