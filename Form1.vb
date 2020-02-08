''FredJust@gmail.com

' IDEES DE 2015
'Définir un nouveau FORMAT : PGA (Portable Game Analyse)
'- lorsque deux variantes amène à la même position 
'   permettre de changer celle qui se trouve dans la ligne principale
'- permettre de retrouver la ligne principale si une variante arrive à la même position
'- affiche les coups suivant sous forme de fleche FAIT
'- un clic sur la fleche d'un coup pour le jouer FAIT
'- affiche les coups suivants dans la zone commentaires
'- différentier visuellement les bons coups des mauvais (fleches ou case en couleur) sans les confondre avec d'autres fleches
'- différentier le coup principale des variantes 
'- pouvoir modifier un coup sans devoir tout ressaisir 
'- saisir les commentaire directement sans validation
'- enregistrer chaque modification automatiquement 
'- afficher le feuille de la variante completement sans commentaire
'- 3 zones échiquier commentaires et coups
'- ne pas melanger les commentaires avec les coups FAIT SUR LECHIQUIER
'- un commentaire par position (possibilité d'afficher les commentaires suivants)
'- ne pas se limiter a 3 couleurs (rouge vert jaune FAIT
'bleu le coups possibles
'blanc noir ...(couleur standard CSS)
'- permettre la saisi d'un coup illégal
'- affiche des FEN => permet d'illustrer la structure des pions en supprimant les pièces
'et revenir à la position de la partie après

Option Explicit On

Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Public Class frmMain

    'TODO A PLACER DANS BoardPositions

    Public Enum enu_Arrow_Type
        Arrow_Default = 0
        Arrow_Next = 1
        Arrow_Last = 2
    End Enum




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


    Public TheScreenBoard As New ScreenBoard

    Public WithEvents BoardPos As New BoardPositions

    Dim bmp_backBuffer As Bitmap

    Dim gr_chessboard As Graphics


    'variable globale quelle horreur
    'Dim sqFrom As String
    'Dim sqTo As String
    Dim EffaceNoir As Boolean = False   'une rustine de dernière minute

    'mon objet pour gérer les mouvements
    Public ThePOS As ObjFenMoves




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


    'dessine un cercle dans un carré sur une case 
    Private Sub PutReg(ByVal sqIndex As Byte, ByVal cColor As String)
        Dim rect As Rectangle
        Dim grPath As New GraphicsPath

        rect.X = TheScreenBoard.rect_square(sqIndex).X
        rect.Y = TheScreenBoard.rect_square(sqIndex).Y
        rect.Width = TheScreenBoard.size_square
        rect.Height = TheScreenBoard.size_square

        Dim aBrush As New SolidBrush(ColorFromChar(cColor, 128))
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
            .Height = Me.ClientSize.Height - .Top - 10 - lbl_status.Height
            .Width = Me.ClientSize.Width - (PictureBox1.Width + 30)
        End With

        With lbl_status
            .Top = TabControl1.Top + TabControl1.Height
            .Left = TabControl1.Left
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

        'supprime le dernier espace génant 
        tempo = Trim(TheScreenBoard.MoveSymbols).Split(" ")

        For i = 0 To tempo.Count - 1
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

        'pas de symbols on quitte
        If BoardPos.Symbols = "" Then Exit Sub

        'supprime le dernier espace génant 
        tempo = Trim(BoardPos.Symbols).Split(" ")

        For i = 0 To tempo.Count - 1
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

        If BoardPos.Arrows = "" Then Exit Sub

        tempo = Trim(BoardPos.Arrows).Split(" ")



        For i = 0 To tempo.Count - 1
            If tempo(i).Length < 5 Then Exit Sub
            str_from = tempo(i).Substring(0, 2)
            str_to = tempo(i).Substring(2, 2)
            chr_Color = tempo(i).Substring(5, 1)
            'TODO rajouter le décallage
            DrawArrow(str_from, str_to, chr_Color) ' TheScreenBoard.pt_center(str_from), TheScreenBoard.pt_center(str_to), chr_Color)
        Next
    End Sub

    Private Sub DrawAllNextMovesArrows()
        Dim tempo() As String
        Dim str_from, str_to As String


        If BoardPos.next_pos = "" Then Exit Sub

        tempo = Trim(BoardPos.next_pos).Split(" ")

        For i = 0 To tempo.Count - 1
            str_from = tempo(i).Substring(0, 2)
            str_to = tempo(i).Substring(2, 2)
            'TODO rajouter le décallage

            DrawArrow(str_from, str_to, "N", 15, enu_Arrow_Type.Arrow_Next)
        Next
    End Sub

    Private Sub DrawAllLastMovesArrows()
        Dim tempo() As String
        Dim str_from, str_to As String


        If BoardPos.last_pos = "" Then Exit Sub

        tempo = Trim(BoardPos.last_pos).Split(" ")

        For i = 0 To tempo.Count - 1

            str_from = tempo(i).Substring(0, 2)
            str_to = tempo(i).Substring(2, 2)

            DrawArrow(str_from, str_to, "W", 15, enu_Arrow_Type.Arrow_Last)
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

    Public Sub DrawArrow(ByVal str_from As String, ByVal str_to As String, _
                          ByVal str_Color As String, Optional ByVal pc_size As Integer = 0, Optional ByVal arrow_Type As Byte = 0)


        Dim pt_from, pt_to As Point

        pt_from = TheScreenBoard.pt_center(str_from)
        pt_to = TheScreenBoard.pt_center(str_to)

        Dim px_size As Integer

        If pc_size = 0 Then
            px_size = TheScreenBoard.size_square / 100 * 20
        Else
            px_size = TheScreenBoard.size_square / 100 * pc_size
        End If


        pt_to = pt_between(pt_to, pt_from, 0, TheScreenBoard.size_square / 5)


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

        Dim aBrush As Brush

        If pc_size = 15 Then
            aBrush = New SolidBrush(ColorFromChar(str_Color, 64))
        Else
            aBrush = New SolidBrush(ColorFromChar(str_Color, 128))
        End If

        grpt = New GraphicsPath

        grpt.AddPolygon(pts_array)

        With TheScreenBoard
            If arrow_Type = enu_Arrow_Type.Arrow_Next Then
                .ToId_arrow(.nb_arrow) = str_Between(BoardPos.next_pos, str_from & str_to & "-", " ")
                .path_arrow(.nb_arrow) = New GraphicsPath
                .path_arrow(.nb_arrow).AddPolygon(pts_array)
                .nb_arrow += 1 'pour sauvegarder les gr path des flèches
            End If
            If arrow_Type = enu_Arrow_Type.Arrow_Last Then
                .ToId_arrow(.nb_arrow) = str_Between(BoardPos.last_pos, str_from & str_to & "-", " ")
                .path_arrow(.nb_arrow) = New GraphicsPath
                .path_arrow(.nb_arrow).AddPolygon(pts_array)
                .nb_arrow += 1 'pour sauvegarder les gr path des flèches
            End If


        End With

        grpt.FillMode = 1
        grpt.AddEllipse(rect_from)

        gr_chessboard.FillPath(aBrush, grpt)




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

        If Me.WindowState = FormWindowState.Minimized Then Exit Sub

        TheScreenBoard.nb_arrow = 0
        TheScreenBoard.nb_comment = 0

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


        DrawAllLastMovesArrows()
        DrawAllNextMovesArrows()

        'dessine les commentaires
        DrawAllComments()

        'raffraichit l'affichage
        PictureBox1.Image = bmp_backBuffer

        ShowNode(BoardPos.Id)

        Debug.Print("DRAW EVERY " & BoardPos.Id)

        

    End Sub




    'dessine les commentaires
    Public Sub DrawAllComments()
        Dim tempo() As String
        Dim str_Top, str_Bottom As String 'nom des cases sur lesquelles se trouve la zone
        Dim str_comment As String 'le commentaire avec des \n pour retour chariot
        Dim strBorderColor As String 'la couleur du contour
        Dim strForeColor As String 'la couleur du texte
        Dim strBackColor As String 'la couleur de la zone

        If BoardPos.Comments = "" Then Exit Sub

        tempo = Trim(BoardPos.Comments).Split("|")

        For i = 0 To tempo.Count - 2

            str_Top = tempo(i).Substring(0, 2)
            str_Bottom = tempo(i).Substring(2, 2)
            strForeColor = tempo(i).Substring(4, 1)
            strBackColor = tempo(i).Substring(5, 1)
            strBorderColor = tempo(i).Substring(6, 1)
            str_comment = tempo(i).Substring(8)

            DrawComment(str_Top, str_Bottom, str_comment, "Tahoma", strForeColor, strBackColor, strBorderColor)
        Next

    End Sub


    'dessine un commentaire
    Public Sub DrawComment(ByVal Topleft As String, ByVal BottomRight As String, _
                           ByVal aComment As String, Optional ByVal NameFont As String = "Tahoma",
                           Optional ByVal ForeColor As String = "W", Optional ByVal BackColor As String = "N", Optional ByVal BorderColor As String = "W")

        Dim ForeBrush As New SolidBrush(ColorFromChar(ForeColor, 192))
        Dim SizeFont As Single = 1
        Dim rectText As SizeF

        With TheScreenBoard
            .path_comment(.nb_comment) = New GraphicsPath
            .Idstr_comment(.nb_comment) = Topleft & BottomRight
        End With

        DrawRoundedRectangle(TheScreenBoard.rect_squares(Topleft, BottomRight), BackColor, BorderColor)

        rectText = gr_chessboard.MeasureString(aComment.Replace("\n", vbCrLf), New Font(NameFont, SizeFont))

        While rectText.Width < TheScreenBoard.rect_squares(Topleft, BottomRight).Width - TheScreenBoard.size_square / 2 And _
            rectText.Height < TheScreenBoard.rect_squares(Topleft, BottomRight).Height - TheScreenBoard.size_square / 2
            SizeFont += 1
            rectText = gr_chessboard.MeasureString(aComment.Replace("\n", vbCrLf), New Font(NameFont, SizeFont))
        End While

        Dim pT As Point

        pT.X = TheScreenBoard.rect_squares(Topleft, BottomRight).X + (TheScreenBoard.rect_squares(Topleft, BottomRight).Width - rectText.Width) / 2
        pT.Y = TheScreenBoard.rect_squares(Topleft, BottomRight).Y + (TheScreenBoard.rect_squares(Topleft, BottomRight).Height - rectText.Height) / 2

        gr_chessboard.DrawString(aComment.Replace("\n", vbCrLf), New Font(NameFont, SizeFont), ForeBrush, pT)
    End Sub

    Public Sub DrawRoundedRectangle(ByVal aRect As Rectangle, Optional ByVal BackColor As String = "N", Optional ByVal BorderColor As String = "W")
        Dim rt As Rectangle = aRect
        Dim radius As Integer = TheScreenBoard.size_square / 3
        Dim BorderPen As Pen
        Dim gp As New GraphicsPath
        Dim BackBrush As Brush

        'ajouter le path 

        With TheScreenBoard
            .path_comment(.nb_comment).AddRectangle(aRect)
            .nb_comment += 1

        End With



        rt.Inflate(-2, -2)

        gp.AddLine(rt.X + radius, rt.Y, rt.X + rt.Width - (radius * 2), rt.Y)
        gp.AddArc(rt.X + rt.Width - (radius * 2), rt.Y, radius * 2, radius * 2, 270, 90)
        gp.AddLine(rt.X + rt.Width, rt.Y + radius, rt.X + rt.Width, rt.Y + rt.Height - (radius * 2))
        gp.AddArc(rt.X + rt.Width - (radius * 2), rt.Y + rt.Height - (radius * 2), radius * 2, radius * 2, 0, 90)
        gp.AddLine(rt.X + rt.Width - (radius * 2), rt.Y + rt.Height, rt.X + radius, rt.Y + rt.Height)
        gp.AddArc(rt.X, rt.Y + rt.Height - (radius * 2), radius * 2, radius * 2, 90, 90)
        gp.AddLine(rt.X, rt.Y + rt.Height - (radius * 2), rt.X, rt.Y + radius)
        gp.AddArc(rt.X, rt.Y, radius * 2, radius * 2, 180, 90)
        gp.CloseFigure()

        BackBrush = New SolidBrush(ColorFromChar(BackColor, 192))
        BorderPen = New Pen(ColorFromChar(BorderColor, 192), 4)

        gr_chessboard.FillPath(BackBrush, gp)
        gr_chessboard.DrawPath(BorderPen, gp)
        gp.Dispose()

    End Sub

#End Region

    'renvoie le texte entre strAfter et strBefore dans strMain
    ' strAfter & str_Between & strBefore
    Public Function str_Between(ByVal strMain As String, ByVal strAfter As String, ByVal strBefore As String) As String
        Dim iAfter, iBefore As Integer
        Dim tempo As String

        If strMain = "" Then Return "-1"

        iAfter = strMain.IndexOf(strAfter)
        tempo = strMain.Substring(iAfter + strAfter.Length)
        iBefore = tempo.IndexOf(strBefore)

        tempo = tempo.Substring(0, iBefore)

        Return tempo

    End Function


    Public Sub ShowCommentBox()

        TheScreenBoard.Name2coord("c5f4")
        frmComment.Show()

    End Sub

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

    Private Sub frmMain_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        'Debug.Print(e.KeyCode)
        Select Case e.KeyCode
            Case 27
                CloseInputBox()

        End Select
    End Sub

    Private Sub frmMain_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles Me.KeyPress
        'Debug.Print(e.KeyChar)
        Select Case e.KeyChar
            Case "R"
                TheScreenBoard.Reversed = Not TheScreenBoard.Reversed
                DrawEveryThing()
            Case "C"
                ShowCommentBox()
            Case "D"
                'efface la position
                BoardPos.col_Positions(BoardPos.Id).FEN = ""
                UpdateDATA()
        End Select
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        PictureBox1.Top = 10
        PictureBox1.Left = 10
        pbReduire.Top = 10

        ThePOS = New ObjFenMoves()
        ThePOS.LocalPiece = "TCFDR"

        'initialise la position 
        Add_Pos_LV(0)

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

    Private Sub PictureBox1_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles PictureBox1.DoubleClick

        'si on est au dessus d'une zone
        If TheScreenBoard.Comment_Over <> 255 Then
            TheScreenBoard.Name2coord(TheScreenBoard.Idstr_comment(TheScreenBoard.Comment_Over))
            TheScreenBoard.str_Comment_Over = BoardPos.CommentAt(TheScreenBoard.Idstr_comment(TheScreenBoard.Comment_Over))
            frmComment.Show()
        End If
        'entre en edition de la zone



    End Sub

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

            If TheScreenBoard.Reversed Then
                .id_square = ThePOS.SquareIndex(.str_Square, True)
                .str_Square = ThePOS.SquareName(.id_square, False)
            End If

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

            If TheScreenBoard.Arrow_Over <> 255 Then Exit Sub
            If TheScreenBoard.Comment_Over <> 255 Then Exit Sub


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

            If TheScreenBoard.Reversed Then
                .id_square = ThePOS.SquareIndex(.str_Square, True)
                .str_Square = ThePOS.SquareName(.id_square, False)
            End If

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

        If TheScreenBoard.Arrow_Over <> 255 Then Exit Sub
        'If TheScreenBoard.Comment_Over <> 255 Then Exit Sub

        Dim sqFrom, sqTo As String
        sqFrom = TheScreenBoard.clicDown.str_Square
        sqTo = TheScreenBoard.clicUp.str_Square

        If sqFrom <> sqTo Then 'si on a changé de case depuis mousedown
            If TheScreenBoard.clicUp.RightClic Then 'si clic droit
                'on trace une fleche
                BoardPos.AddArrow(sqFrom & sqTo, cur2str())
                update_Pos_LV()
            Else 'clic normal
                'on tente de déplacer une piece
                If ThePOS.IsValidMove(sqFrom & sqTo) Then
                    If AddMove(sqFrom, sqTo) Then
                        'le mouvement c'est fait on change de position à la suite d'un mouvement
                        BoardPos.change_Pos_move(sqFrom, sqTo, ThePOS.GetFEN)
                    End If
                End If
            End If
        Else 'on ne change pas de case
            If TheScreenBoard.clicUp.RightClic Then 'si clic droit
                'on place un symbole
                BoardPos.AddOtherSymbol(sqTo, cur2str())
                update_Pos_LV()
            End If
        End If

        DrawEveryThing()
    End Sub

#End Region

#Region "Gestion des couleurs avec les touches ALT SHIT CTRL"

    ''renvoie la couleur correspondant aux dernières touches lors du clic
    'Public Function cur2color() As Color

    '    With TheScreenBoard.clicUp

    '        If .Alt Then
    '            If .Shift Then
    '                Return ColorFromChar("M")
    '            End If
    '            If .Control Then
    '                Return ColorFromChar("C")
    '            End If
    '            Return ColorFromChar("B")
    '        End If

    '        If .Shift Then
    '            Return ColorFromChar("R")
    '        End If

    '        If .Control Then
    '            Return ColorFromChar("Y")
    '        End If

    '    End With

    '    Return ColorFromChar("G")

    'End Function

    'Renvoie l'initiale de la couleur
    Public Function cur2str() As String
        With TheScreenBoard.clicDown

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
    Public Function ColorFromChar(ByVal str As String, ByVal alpha As Byte) As Color
        Select Case str
            Case "R"
                Return Color.FromArgb(alpha, 255, 0, 0)
            Case "G"
                Return Color.FromArgb(alpha, 0, 255, 0)
            Case "B"
                Return Color.FromArgb(alpha, 0, 0, 255)
            Case "Y"
                Return Color.FromArgb(alpha, 255, 255, 0)
            Case "M"
                Return Color.FromArgb(alpha, 255, 0, 255)
            Case "C"
                Return Color.FromArgb(alpha, 0, 255, 255)
            Case "W"
                Return Color.FromArgb(alpha, 255, 255, 255)
            Case "N"
                Return Color.FromArgb(alpha, 0, 0, 0)
            Case "W"
                Return Color.FromArgb(alpha, 255, 255, 255)
        End Select
    End Function

#End Region

#Region "Gestion des mouvements et de la ListView"

    'efface tous les items suivant l'item selectionné dans la listview


    'affiche le mouvement dans la listview lvmoves
    Private Function AddMove(ByVal sqFrom As String, ByVal sqTo As String) As Boolean
        Dim lvi As New ListViewItem




        If ThePOS.WhiteToPlay Then
            BoardPos.Last_SAN = ThePOS.MovesPlayed.ToString & "." & ThePOS.PGNmove(sqFrom & sqTo)
            lvi.Text = ThePOS.MovesPlayed.ToString & "."
            lvi.SubItems.Add(ThePOS.PGNmove(sqFrom & sqTo))
            ThePOS.MakeMove(sqFrom & sqTo)
            lvi.SubItems(1).Tag = ThePOS.GetFEN()
            lvMoves.Items.Add(lvi)

        Else
            BoardPos.Last_SAN = ThePOS.MovesPlayed.ToString & "... " & ThePOS.PGNmove(sqFrom & sqTo)
            'lvi = lvMoves.Items(lvMoves.Items.Count - 1)
            'lvi.SubItems.Add(ThePOS.PGNmove(sqFrom & sqTo))
            ThePOS.MakeMove(sqFrom & sqTo)
            'lvi.SubItems(2).Tag = ThePOS.GetFEN()
        End If

        'lvMoves.Items(lvMoves.Items.Count - 1).Selected = True
        Return True


        Return False
    End Function



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


    Private Sub updateLV()
        lvPositions.Items.Clear()

        For i = 0 To BoardPos.nb_pos - 1

            Add_Pos_LV(i)

        Next

        lvPositions.Items(BoardPos.Id).Selected = True
    End Sub

    Private Sub lvPositions_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvPositions.SelectedIndexChanged


        On Error Resume Next
        MoveToId(lvPositions.SelectedItems(0).Index)

        

    End Sub

    'modifie l'affichage d'une position (la courrante) dans la LV
    Public Sub update_Pos_LV()

        With lvPositions.Items(BoardPos.Id & "k")
            .SubItems(1).Text = BoardPos.SAN
            .SubItems(2).Text = BoardPos.next_pos
            .SubItems(3).Text = BoardPos.last_pos
            .SubItems(4).Text = BoardPos.Symbols
            .SubItems(5).Text = BoardPos.Arrows
            .SubItems(6).Text = BoardPos.FEN
            .SubItems(7).Text = BoardPos.Comments
        End With

    End Sub

    'a ne pas utiliser en cas de mise a jour
    'id de la pos doit correspondre a id de l'item pour une mise a jour
    Public Sub Add_Pos_LV(ByVal Id_Pos As Integer)
        Dim NbLigne As Integer



        NbLigne = lvPositions.Items.Count

        lvPositions.Items.Insert(NbLigne, CStr(BoardPos.col_Positions(Id_Pos).Id) & "k", CStr(BoardPos.col_Positions(Id_Pos).Id) & " id", 0)    'numéro de la position

        'Public Comments As String 'x|y|text||x|y|text||...

        With lvPositions.Items(NbLigne)
            .SubItems.Add(BoardPos.col_Positions(Id_Pos).SAN)
            .SubItems.Add(BoardPos.col_Positions(Id_Pos).next_pos)
            .SubItems.Add(BoardPos.col_Positions(Id_Pos).last_pos)
            .SubItems.Add(BoardPos.col_Positions(Id_Pos).Symbols)
            .SubItems.Add(BoardPos.col_Positions(Id_Pos).Arrows)
            .SubItems.Add(BoardPos.col_Positions(Id_Pos).FEN)
            .SubItems.Add(BoardPos.col_Positions(Id_Pos).Comments)
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




#Region "CORBEILLE Fonctions inutilisées "
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
#End Region



    Private Sub UpdateDATA() Handles BoardPos.PositionAdded
        updateLV()
        UpdateTreeView()

    End Sub

    Public Sub MoveToId(ByVal id_Pos As Integer)

        Debug.Print("MOVETO " & id_Pos)

        If id_Pos = 0 Then
            Debug.Print("ALERTE MOVETO " & id_Pos)
        End If

        With lvPositions.Items(id_Pos)
            BoardPos.Id = .SubItems(0).Text.Replace(" id", "")
            BoardPos.SAN = .SubItems(1).Text
            BoardPos.next_pos = .SubItems(2).Text
            BoardPos.last_pos = .SubItems(3).Text
            BoardPos.Symbols = .SubItems(4).Text
            BoardPos.Arrows = .SubItems(5).Text
            BoardPos.FEN = .SubItems(6).Text
            BoardPos.Comments = .SubItems(7).Text
            ThePOS.SetFEN(BoardPos.FEN)
        End With

        


        'If tvPositions.Nodes(id_Pos & "k").IsSelected Then
        'Else
        '    tvPositions.Nodes(id_Pos).ForeColor = Color.Red
        'End If

        DrawEveryThing()
    End Sub


#Region "Gestion TVposition"



    'procedure recursive 
    'trouve le noeud ayant pour clé "thekey" dans l'ensemble de tous les noeuds et le renvoie dans "FindNode"
    Public Sub Find_Node_By_Key(ByVal myNodes As TreeNodeCollection, ByVal thekey As String, _
                                ByRef FindNode As TreeNode, ByRef isFind As Boolean)
        Dim Child As TreeNode

        If myNodes.ContainsKey(thekey) Then
            FindNode = myNodes(thekey)
            isFind = True
        Else
            If isFind = False Then
                For Each Child In myNodes
                    Find_Node_By_Key(Child.Nodes, thekey, FindNode, isFind)
                Next
            End If
        End If

    End Sub

    Public Function myNodesContainsTag(ByVal myNodes As TreeNodeCollection, ByVal theTag As String, ByRef FindNode As TreeNode) As Integer
        Dim aNode As TreeNode
        Dim tempo As String
        For Each aNode In myNodes
            tempo = aNode.Tag

            If tempo.IndexOf(" " & theTag) > 0 Then
                FindNode = aNode
                Return True
            End If

            If tempo.IndexOf(theTag) = 0 Then

                FindNode = aNode
                Return True
            End If

        Next
        Return False
    End Function

    Public Sub ShowNode(ByVal byid As Integer)
        Dim aNode As New TreeNode
        Dim NewNode As New TreeNode
        Dim isFind As Boolean

        SwitchOffNode(tvPositions.Nodes)

        isFind = False

        Find_Node_By_Tag(tvPositions.Nodes, byid & "k", aNode, isFind)

        If isFind Then
            aNode.ForeColor = Color.White
            aNode.BackColor = Color.Chocolate

            Debug.Print("SHOW " & byid)
            If byid = 0 Then
                Debug.Print("ALERT " & byid)
            End If
            'aNode.Expand()
            'aNode.ExpandAll()
            'aNode.EnsureVisible()
        End If
    End Sub

    Public Sub SwitchOffNode(ByVal myNodes As TreeNodeCollection)
        Dim Child As TreeNode
        For Each Child In myNodes
            Child.ForeColor = Color.Black
            Child.BackColor = Color.AntiqueWhite
            'Child.Collapse()
            SwitchOffNode(Child.Nodes)
        Next
    End Sub


    'procedure recursive 
    'trouve le noeud ayant pour clé "thekey" dans l'ensemble de tous les noeuds et le renvoie dans "FindNode"
    Public Sub Find_Node_By_Tag(ByVal myNodes As TreeNodeCollection, ByVal theTag As String, _
                                ByRef FindNode As TreeNode, ByRef isFind As Boolean)
        Dim Child As TreeNode

        If myNodesContainsTag(myNodes, theTag, FindNode) Then
            isFind = True
        Else
            If isFind = False Then
                For Each Child In myNodes
                    Find_Node_By_Tag(Child.Nodes, theTag, FindNode, isFind)
                Next
            End If
        End If

    End Sub

    'ajoute un noeud dans la treeview sous le noeud ayant pour clé "keyParent"
    Public Sub Add_child_Node(ByRef tvw As TreeView, ByVal keyParent As String, _
                       ByVal key As String, ByVal text As String)
        Dim aNode As New TreeNode
        Dim NewNode As New TreeNode
        Dim isFind As Boolean

        isFind = False

        Find_Node_By_Tag(tvw.Nodes, keyParent, aNode, isFind)

        NewNode = aNode.Nodes.Add(key, text)
        NewNode.Tag = key


    End Sub


    'modifie un noeud dans la treeview en ajoutant le coup
    Public Sub update_Node(ByRef tvw As TreeView, ByVal keyParent As String, _
                       ByVal key As String, ByVal text As String)
        Dim aNode As New TreeNode
        Dim NewNode As New TreeNode
        Dim isFind As Boolean

        isFind = False

        Find_Node_By_Tag(tvw.Nodes, keyParent, aNode, isFind)

        If text.Contains("...") Then
            aNode.Text &= text.Substring(text.IndexOf(" ")) & "  "
        Else
            aNode.Text &= " " & text
        End If


        aNode.Tag &= " " & key


    End Sub

    Private Sub Add_Pos_TV(ByVal id_pos As Integer)

        Dim aText As String = ""
        Dim k As String
        Dim np() As String
        Dim nextId As Integer

        k = Trim(BoardPos.col_Positions(id_pos).last_pos)
        np = Trim(BoardPos.col_Positions(id_pos).next_pos).Split(" ")


        If k.IndexOf(" ") <> -1 Then
            If k <> "" Then k = k.Substring(5, k.IndexOf(" ") - 5)
        Else
            If k <> "" Then k = k.Substring(5)
        End If

        'si la position a plusieurs fils 
        'il faut les mmodifier 
        If np.Count > 1 Then
            For i = 0 To np.Count - 1
                nextId = np(i).Substring(5)
                BoardPos.col_Positions(nextId).HaveBrother = True
            Next
        End If

        
        If BoardPos.col_Positions(id_pos).HaveBrother Then
            'il y a deux suites possibles
            Add_child_Node(tvPositions, k & "k", CStr(BoardPos.col_Positions(id_pos).Id) & "k", BoardPos.col_Positions(id_pos).SAN)

        Else
            'on modifie 
            update_Node(tvPositions, k & "k", CStr(BoardPos.col_Positions(id_pos).Id) & "k", BoardPos.col_Positions(id_pos).SAN)
        End If






    End Sub

    Private Sub UpdateTreeView()

        tvPositions.Nodes.Clear()
        tvPositions.Nodes.Add("0k", "Start")
        tvPositions.Nodes(0).Tag = "0k"

        For i = 1 To BoardPos.nb_pos - 1
            If BoardPos.col_Positions(i).FEN <> "" Then
                Add_Pos_TV(i)
            End If
        Next

        tvPositions.Nodes(0).ExpandAll()
        'tvPositions.Nodes(0).EnsureVisible()
        tvPositions.Nodes(tvPositions.Nodes.Count - 1).EnsureVisible()

        'Debug.Print(tvPositions.Nodes.Count)
        'tvPositions.Nodes(0).ForeColor = Color.Blue

    End Sub




#End Region



    Private Function PGAline(ByVal id_pos As Integer) As String
        Dim tempo As String
        tempo = ""
        With BoardPos.col_Positions(id_pos)
            tempo &= .Id & "¤"
            tempo &= .SAN & "¤"
            tempo &= .next_pos & "¤"
            tempo &= .last_pos & "¤"
            tempo &= .Symbols & "¤"
            tempo &= .Arrows & "¤"
            tempo &= .FEN & "¤"
            tempo &= .Comments & "¤"
            tempo &= vbCrLf

        End With

        Return tempo
    End Function

    'ouvre une boite de dialogue et renvoie le nom d'un fichier
    Public Function NameFile() As String
        Dim nomfichier As String = ""

        On Error GoTo ErrorHandler

        With OpenFileDialog1
            'On spécifie l'extension de fichiers visibles
            .FileName = ""
            .Filter = "ARD Files (*.*) | *.*"
            'On affiche et teste le retour du dialogue
            If .ShowDialog = Windows.Forms.DialogResult.OK Then
                'On récupère le nom du fichier
                nomfichier = .FileName
            End If
        End With

        Return nomfichier

        Exit Function
ErrorHandler:

        MsgBox("Error in Function NameFile : " & vbCrLf & Err.Description)

    End Function


    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        Dim NumFile As Integer
        Dim TheFileName As String = ""

        While My.Computer.FileSystem.FileExists(Application.StartupPath & "\game" & NumFile.ToString & ".pga")
            NumFile += 1
        End While

        TheFileName = Application.StartupPath & "\game" & NumFile.ToString & ".pga"

        Try
            For i = 0 To BoardPos.nb_pos - 1
                If BoardPos.col_Positions(i).FEN <> "" Then
                    My.Computer.FileSystem.WriteAllText(TheFileName, PGAline(i), True)
                End If
            Next

        Catch ex As Exception

            MsgBox("Impossible de sauvegarder : " & TheFileName, MsgBoxStyle.Exclamation, "ERREUR")

        End Try
    End Sub

    Private Sub LoadToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadToolStripMenuItem.Click
        Dim NomFichier As String

        NomFichier = NameFile()
        If NomFichier <> "" Then

            BoardPos.LoadFromFile(NomFichier)

            UpdateDATA()

        End If
    End Sub


    Private Sub CloseInputBox()
        If frmComment.Visible Then
            BoardPos.AddComment(TheScreenBoard.Coord2name(), frmComment.txtComment.Text.Replace(vbCrLf, "\n"))
            frmComment.Close()
            update_Pos_LV()
            Exit Sub
        End If
    End Sub

    Private Sub PictureBox1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles PictureBox1.Click
        Dim aColor As String

        Debug.Print("CLIC : " & TheScreenBoard.Arrow_Over)

        CloseInputBox()

        'If TheScreenBoard.Comment_Over <> 255 Then
        '    aColor = cur2str()
        '    If TheScreenBoard.clicDown.RightClic Then
        '        BoardPos.ModifCommentBorder(TheScreenBoard.Idstr_comment(TheScreenBoard.Comment_Over), aColor)
        '    Else
        '        BoardPos.ModifCommentFore(TheScreenBoard.Idstr_comment(TheScreenBoard.Comment_Over), aColor)
        '    End If
        '    update_Pos_LV()
        '    DrawEveryThing()
        '    Exit Sub
        'End If

        If TheScreenBoard.Arrow_Over <> 255 Then
            If TheScreenBoard.ToId_arrow(TheScreenBoard.Arrow_Over) = -1 Then Exit Sub

            lbl_status.Text = TheScreenBoard.Arrow_Over

            With lvPositions.Items(TheScreenBoard.ToId_arrow(TheScreenBoard.Arrow_Over))

                BoardPos.Id = .SubItems(0).Text.Replace(" id", "")
                BoardPos.SAN = .SubItems(1).Text
                BoardPos.next_pos = .SubItems(2).Text
                BoardPos.last_pos = .SubItems(3).Text
                BoardPos.Symbols = .SubItems(4).Text
                BoardPos.Arrows = .SubItems(5).Text
                BoardPos.FEN = .SubItems(6).Text
                BoardPos.Comments = .SubItems(7).Text
                ThePOS.SetFEN(BoardPos.FEN)
            End With
        End If

        DrawEveryThing()
    End Sub

    Private Sub PictureBox1_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseMove


        Dim OnArrow As Boolean

        OnArrow = False


        If TheScreenBoard.Arrow_Over <> 255 Then
            If TheScreenBoard.path_arrow(TheScreenBoard.Arrow_Over).IsVisible(e.X, e.Y) Then
            Else
                Debug.Print("EXIT ARROW " & TheScreenBoard.Arrow_Over)
                TheScreenBoard.Arrow_Over = 255
                DrawEveryThing()
            End If
        Else
            For i = 0 To TheScreenBoard.nb_arrow - 1
                If TheScreenBoard.path_arrow(i).IsVisible(e.X, e.Y) Then
                    'on se trouve dans la fleche i
                    If TheScreenBoard.Arrow_Over <> i Then
                        'on se trouve dans une nouvelle fleche
                        DrawEveryThing()
                        gr_chessboard.DrawPath(Pens.Black, TheScreenBoard.path_arrow(i))
                        PictureBox1.Image = bmp_backBuffer
                        TheScreenBoard.Arrow_Over = i
                        Debug.Print("ENTER = " & i & " " & e.X & " - " & e.Y)
                        OnArrow = True
                    End If
                End If
            Next
        End If

        If TheScreenBoard.Comment_Over <> 255 Then
            If TheScreenBoard.path_comment(TheScreenBoard.Comment_Over).IsVisible(e.X, e.Y) Then
            Else
                Debug.Print("EXIT ZONE " & TheScreenBoard.Comment_Over)
                TheScreenBoard.Comment_Over = 255

            End If
        Else
            'regarde si on est au dessus d'une zone
            For i = 0 To TheScreenBoard.nb_comment - 1
                If TheScreenBoard.path_comment(i).IsVisible(e.X, e.Y) Then
                    'on se trouve dans la zone i
                    TheScreenBoard.Comment_Over = i
                    Debug.Print("ENTER ZONE " & TheScreenBoard.Comment_Over)
                    lbl_status.Text = TheScreenBoard.Idstr_comment(i)
                End If
            Next
        End If





    End Sub












    Private Sub tvPositions_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tvPositions.MouseWheel
        Dim SizeFont As Integer

        SizeFont = e.Delta * SystemInformation.MouseWheelScrollLines / 360


        Dim AllPos() As String

        Debug.Print(BoardPos.Id)

        If SizeFont < 0 Then

            If BoardPos.next_pos <> "" Then
                AllPos = Trim(BoardPos.next_pos).Split(" ")
                If AllPos.Count > 1 Then

                Else
                    MoveToId(AllPos(0).Substring(5))
                End If
            End If
        Else
            If BoardPos.last_pos <> "" Then
                AllPos = Trim(BoardPos.last_pos).Split(" ")
                If AllPos.Count > 1 Then

                Else
                    MoveToId(AllPos(0).Substring(5))
                End If
            End If
        End If
    End Sub

    Private Sub tvPositions_NodeMouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.TreeNodeMouseClickEventArgs) Handles tvPositions.NodeMouseClick
        lbl_status.Text = e.Node.Tag

        MoveToId(CInt(lbl_status.Text.Substring(0, lbl_status.Text.IndexOf("k"))))

    End Sub

    Private Sub tvPositions_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles tvPositions.AfterSelect

    End Sub

    Private Sub PictureBox1_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles PictureBox1.MouseWheel
        Dim SizeFont As Integer

        SizeFont = e.Delta * SystemInformation.MouseWheelScrollLines / 360


        Dim AllPos() As String

        Debug.Print(BoardPos.Id)

        If SizeFont < 0 Then

            If BoardPos.next_pos <> "" Then
                AllPos = Trim(BoardPos.next_pos).Split(" ")
                If AllPos.Count > 1 Then

                Else
                    MoveToId(AllPos(0).Substring(5))
                End If
            End If
        Else
            If BoardPos.last_pos <> "" Then
                AllPos = Trim(BoardPos.last_pos).Split(" ")
                If AllPos.Count > 1 Then

                Else
                    MoveToId(AllPos(0).Substring(5))
                End If
            End If
        End If
    End Sub
End Class
