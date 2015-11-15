Public Class frmMain

    'FredJust@gmail.com
    'avril 2015
    'les graphiques ne font que 88x88
    'pour des écrans de 768 de haut max
    'a tester en Full HD
    'il faudrait peut etre changer les PNG qui existent en tout les tailles sur le web

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

    'variable global quelle horreur
    Dim sqFrom As String
    Dim sqTo As String
    Dim MoveByClic As Boolean
    Dim EffaceNoir As Boolean = False   'une rustine de dernière minute
    Public ThePOS As ObjFenMoves

    Public Const lv_num As Byte = 0
    Public Const lv_rec As Byte = 1
    Public Const lv_time As Byte = 2
    Public Const lv_off As Byte = 3
    Public Const lv_on As Byte = 4
    Public Const lv_FEN As Byte = 5
    Public Const lv_Nb As Byte = 6


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

    'retourne la position X de la case
    Private Function Xsqi(ByVal sqi As Byte) As Integer
        Dim colonne As Byte
        Dim ligne As Byte

        colonne = sqi Mod 10
        ligne = sqi \ 10

        Return 18 + (colonne - 1) * PieceSize

    End Function

    'retourne la position Y de la case
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

    'dessine l'échiquier puis  les pièces
    Private Sub DrawPiece()
        Dim rect As Rectangle
        Dim pt As Point

        Debug.Print("DrawPiece")

        If Me.WindowState <> FormWindowState.Minimized Then

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

            rect.X = 0 : rect.Y = 0
            rect.Width = PictureBox1.Width - 1 : rect.Height = PictureBox1.Height - 1
            g.DrawRectangle(Pens.Brown, rect)                                   'dessine le filet exterieur 

            rect.X = 17 : rect.Y = 17
            rect.Width = PictureBox1.Width - 35 : rect.Height = PictureBox1.Height - 35
            g.DrawRectangle(Pens.Black, rect)                                   'dessine le filet intérieur

            rect.X = 18 : rect.Y = 18
            rect.Width = PictureBox1.Width - 36 : rect.Height = PictureBox1.Height - 36
            g.DrawImage(bboard, rect)                                           'dessine l'échiquier

            PieceSize = (PictureBox1.Height - 36) / 8

            'With lvMoves
            '    .Top = 50
            '    .Left = PictureBox1.Left + PictureBox1.Width + 10
            '    .Height = Me.ClientSize.Height - lvMoves.Top - 10
            '    .Width = Me.ClientSize.Width - (PictureBox1.Width + 30)
            'End With

            With TabControl1
                .Top = 10
                .Left = PictureBox1.Left + PictureBox1.Width + 10
                .Height = Me.ClientSize.Height - .Top - 10
                .Width = Me.ClientSize.Width - (PictureBox1.Width + 30)
            End With



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
        Debug.Print("DrawMove:" & sqFrom)


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

    Private Sub frmMain_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown
        If e.Button = MouseButtons.Right Then
            mnFrm.Show(Control.MousePosition)
        End If
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

            Debug.Print("MouseUp: " & sqI & "-" & sqFrom & "-" & sqTo)

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


        If e.Button = MouseButtons.Right Then
            mnFrm.Show(Control.MousePosition)
        Else

            'récupère le nom de la case sur lequel le curseur se trouve
            sqI = Chr(97 + Math.Truncate((e.X - 18) / (PictureBox1.Width - 36) * 8)) _
                    + (8 - Math.Truncate((e.Y - 18) / (PictureBox1.Height - 36) * 8)).ToString

            Debug.Print("MouseUp: " & sqI & "-" & sqFrom & "-" & sqTo)

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

#Region "Gestion des mouvements et de la ListView"

    'efface tous les items suivant dans la listview
    Public Sub deletenextitem()
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








    Private Sub GetRecToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetRecToolStripMenuItem.Click
        MsgBox(ThePOS.GetRec)
    End Sub

    Private Sub GetMovesToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles GetMovesToolStripMenuItem.Click
        MsgBox(ThePOS.GetMoves(sqFrom))
    End Sub


    Private Sub GetFenToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles GetFenToolStripMenuItem.Click
        MsgBox(ThePOS.GetFEN)
    End Sub


    Private Sub GetAllRecToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles GetAllRecToolStripMenuItem.Click
        MsgBox(ThePOS.GetRecs(sqFrom))
    End Sub



    Public Function NbPieceColonne(y) As Byte
        Dim a As Byte = 0
        Dim x As Byte
        Dim z As Byte
        Dim e As Byte
        z = y
        Do
            x = Int(z / 2)
            If z = 2 * x + 1 Then a = a + 1
            e = e + 1
            z = x
        Loop While z > 0
        NbPieceColonne = a
    End Function

    Public Function NbPiece(rec As String)
        Dim colonnes() As String
        Dim tempo As Byte = 0

        colonnes = rec.Split(".")
        For i = 0 To 7
            tempo = tempo + NbPieceColonne(colonnes(i))
        Next
        Return tempo
    End Function

    'charge lvREC avec les données d'un fichier
    Private Sub LoadToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles LoadToolStripMenuItem.Click

        Dim nomfichier As String = ""

        '___________________________________________________________________________________________________________
        'ouverture du fichier
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
        '___________________________________________________________________________________________________________

        Dim RecTempo() As String
        Dim RecordsInFile As String

        On Error GoTo ErrorHandler

        RecordsInFile = My.Computer.FileSystem.ReadAllText(nomfichier)

        RecTempo = RecordsInFile.Split(Chr(10) + Chr(13)) 'sépare les différentes lignes

        Dim i As Integer = 0
        Dim j As Integer = 0
        Dim NewLine As String
        Dim LastLine As String
        Dim sqOn As String = ""
        Dim sqOff As String = ""

        LastLine = RecTempo(0).Substring(0, RecTempo(0).Length - 2)

        lvRec.Items.Clear()
        '___________________________________________________________________________________________________________
        'première ligne
        lvRec.Items.Add(0)
        lvRec.Items(0).SubItems.Add(LastLine)
        lvRec.Items(0).SubItems.Add(RecTempo(j + 1))
        lvRec.Items(0).SubItems.Add(sqOff)
        lvRec.Items(0).SubItems.Add(sqOn)
        lvRec.Items(0).SubItems.Add("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1")
        lvRec.Items(0).SubItems.Add("32")
        lvRec.Items(0).ForeColor = Color.Green
        '___________________________________________________________________________________________________________    
        j = j + 2
        i = i + 1
        '___________________________________________________________________________________________________________
        'rempli lvREC
        While j < RecTempo.Length - 2
            lvRec.Items.Add(i)
            NewLine = RecTempo(j).Substring(0, RecTempo(j).Length - 2) 'supprimer l . en fin de ligne
            lvRec.Items(i).SubItems.Add(NewLine)
            lvRec.Items(i).SubItems.Add(RecTempo(j + 1)) 'ajoute le temps de stabilité
            FindOnOff(NewLine, LastLine, sqOn, sqOff) 'cherche les différence            
            If sqOn <> "" And sqOff <> "" Then
                lvRec.Items(i).ForeColor = Color.Purple
            End If
            lvRec.Items(i).SubItems.Add(sqOff)
            lvRec.Items(i).SubItems.Add(sqOn)
            lvRec.Items(i).SubItems.Add("")
            lvRec.Items(i).SubItems.Add(NbPiece(NewLine))

            LastLine = NewLine
            j = j + 2
            i = i + 1


        End While
        '___________________________________________________________________________________________________________
        Exit Sub
ErrorHandler:

        MsgBox("Error in LoadRecords : " & vbCrLf & Err.Description)



    End Sub

    ' cherche les cases qui s'allument et s'éteingnent entre deux enregistrements
    Private Sub FindOnOff(ByVal new_line As String, ByVal last_line As String, ByRef SquareOn As String, ByRef SquareOff As String)
        Dim ligne As Integer
        Dim colonne As Byte

        Dim new_pos(9) As String
        Dim last_pos(9) As String

        Dim last_byte As Byte
        Dim new_byte As Byte

        SquareOff = ""
        SquareOn = ""

        last_pos = Split(new_line, ".") 'recupère les 8 bytes de la positions précedente
        new_pos = Split(last_line, ".") 'récupère les 8 bytes de la position 

        For colonne = 1 To 8
            new_byte = new_pos(colonne - 1) 'bytes des colonnes
            last_byte = last_pos(colonne - 1)
            If (new_byte <> last_byte) Then 'si les deux colonnes sont différentes
                For ligne = 7 To 0 Step -1
                    If (new_byte And (2 ^ ligne)) <> (last_byte And (2 ^ ligne)) Then   'si la case est diffétente
                        If (new_byte And (2 ^ ligne)) Then  's'il s'agit d'une extinction
                            SquareOff = Chr(96 + colonne) & (ligne + 1).ToString
                        Else
                            SquareOn = Chr(96 + colonne) & (ligne + 1).ToString
                        End If
                    End If
                Next
            End If
        Next
    End Sub

    'si une position existe sur la ligne sélectionnée de lvREC appelle l'affichage
    Private Sub lvRec_ItemSelectionChanged(sender As Object, e As System.Windows.Forms.ListViewItemSelectionChangedEventArgs) Handles lvRec.ItemSelectionChanged
        Dim aFen As String
        If e.IsSelected Then
            aFen = lvRec.SelectedItems.Item(0).SubItems(lv_FEN).Text
            If aFen <> "" Then
                ThePOS.SetFEN(aFen)
                DrawPiece()
            End If
        End If
    End Sub


    'affiche le menu contextuelle de lvREC
    Private Sub lvRec_MouseUp(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles lvRec.MouseUp
        If e.Button = MouseButtons.Right Then
            mnLv.Show(Control.MousePosition)
        End If

    End Sub


    'renvoi l'index de la première ligne correspondant a une signature possible 
    'dans les lignes non déjà rejetées
    'relativement à l'index iLigne 
    'lorsque la case sqLift disparait
    Private Function indexNextRec(idep As Integer, sqLift As String, ByRef uci_move As String) As Byte
        Dim LesRecs As String
        Dim RecPossibles As String()
        Dim LeCoup As String()
        Dim iMin As Byte = 255

        LesRecs = ThePOS.GetRecs(sqLift) 'récupère les signatures possibles forme : 195.195...195 a1h8|195.195...195 a1h8
        RecPossibles = LesRecs.Split("|") 'sépare le rec

        For c = 0 To RecPossibles.Count - 1 'pour chaque signature possible
            LeCoup = RecPossibles(c).Split(" ") 'sépare la signature du coup
            For i = 0 To 10 'cherche dans les 10 signatures suivantes
                If lvRec.Items(idep + i).SubItems(lv_rec).Text = LeCoup(0) Then 'si la signature correspond
                    If lvRec.Items(idep + i).ForeColor <> Color.Red Then 'si la ligne n'a pas été rejeté
                        If lvRec.Items(idep + i).SubItems(lv_on).Text = LeCoup(1).Substring(2, 2) Then 'si la case d'arrivé du coup est celle qui vient de s'allumer
                            If i < iMin Then
                                uci_move = LeCoup(1)
                                iMin = i
                            End If
                        Else
                            If LeCoup(1) = "e1g1" Or LeCoup(1) = "e8g8" Then
                                If i < iMin Then
                                    uci_move = LeCoup(1)
                                    iMin = i
                                End If
                            End If
                        End If

                    Else
                        Debug.Print("ligne " & idep + i & " déjà rejeté")
                    End If
                End If
            Next
        Next
        Return iMin 'on a rien trouvé
    End Function

    'renvoi l'index de la première ligne correspondant a une signature possible 
    'dans les lignes non déjà rejetées
    'relativement à l'index iLigne 
    Private Function indexNextAllRec(idep As Integer, ByRef uci_move As String) As Byte
        Dim LesRecs As String
        Dim RecPossibles As String()
        Dim LeCoup As String()
        Dim iMin As Byte = 255

        LesRecs = ThePOS.GetAllRecs()  'récupère les signatures possibles forme : 195.195...195 a1h8|195.195...195 a1h8
        RecPossibles = LesRecs.Split("|") 'sépare le rec

        For c = 0 To RecPossibles.Count - 1 'pour chaque signature possible
            LeCoup = RecPossibles(c).Split(" ") 'sépare la signature du coup
            For i = 0 To 20 'cherche dans les signatures suivantes
                If Convert.ToByte(lvRec.Items(idep + i).SubItems(lv_Nb).Text) >= Convert.ToByte(lvRec.Items(idep + i).SubItems(lv_Nb).Text) Then 'si le nombre de pièce n'augmente pas 
                    If lvRec.Items(idep + i).SubItems(lv_rec).Text = LeCoup(0) Then 'si la signature correspond
                        If lvRec.Items(idep + i).ForeColor <> Color.Red Then 'si la ligne n'a pas été rejeté
                            lvRec.Items(idep + i).ForeColor = Color.Aqua
                            lvRec.Items(idep + i).SubItems(lv_FEN).Text = LeCoup(1)
                            If lvRec.Items(idep + i).SubItems(lv_on).Text = LeCoup(1).Substring(2, 2) Then 'si la case d'arrivé du coup est celle qui vient de s'allumer
                                If i < iMin Then
                                    uci_move = LeCoup(1)
                                    iMin = i
                                End If
                            Else
                                If LeCoup(1) = "e1g1" Or LeCoup(1) = "e8g8" Then
                                    If i < iMin Then
                                        uci_move = LeCoup(1)
                                        iMin = i
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Next
        Next
        Return iMin 'on a rien trouvé
    End Function


    'cherche une correspondance dans les signatures suivantes
    'Private Sub AddLineToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles mniFindRec.Click

    '    Dim idep As Integer
    '    Dim RecPossibles As String()
    '    Dim LesRecs As String
    '    Dim imax As Integer
    '    Dim sqFrom As String
    '    Dim sqTo As String
    '    Dim LeCoup As String()
    '    Dim OnJoue As String

    '    sqFrom = lvRec.SelectedItems.Item(0).SubItems(lv_off).Text 'récupère la case qui s'éteint
    '    LesRecs = ThePOS.GetRecs(sqFrom) 'récupère les signatures possibles forme : 195.195...195 a1h8|195.195...195 a1h8

    '    RecPossibles = LesRecs.Split("|")
    '    idep = lvRec.SelectedItems.Item(0).Index 'ligne de départ

    '    For c = 0 To RecPossibles.Count - 1 'pour chaque signature possible
    '        LeCoup = RecPossibles(c).Split(" ")
    '        For i = 1 To 10 'cherche dans les 10 signatures suivantes
    '            If lvRec.Items(idep + i).SubItems(lv_rec).Text = LeCoup(0) Then
    '                lvRec.Items(idep + imax).ForeColor = Color.Green
    '                If i >= imax Then
    '                    imax = i
    '                    OnJoue = LeCoup(1)
    '                End If

    '            End If
    '        Next
    '    Next
    '    lvRec.Items(idep + imax).ForeColor = Color.Blue
    '    sqTo = lvRec.Items(idep + imax).SubItems(lv_on).Text
    '    If ThePOS.IsValidMove(OnJoue) Then
    '        ThePOS.MakeMove(OnJoue)
    '    End If
    '    lvRec.Items(idep + imax).SubItems(lv_FEN).Text = ThePOS.GetFEN
    'End Sub

    Private Sub FindNextMove()
        Dim LeSuivant As Byte
        Dim idep As Integer

        Dim lecoup As String = ""
        Dim PasTrouve As Boolean = True
        Dim i As Integer
        Dim aFen As String

        i = lvRec.Items.Count - 1
        While PasTrouve
            i = i - 1
            If lvRec.Items(i).ForeColor = Color.Green Then
                aFen = lvRec.Items(i).SubItems(lv_FEN).Text
                ThePOS.SetFEN(aFen)

                PasTrouve = False
                idep = i + 1
            End If

        End While

        'idep = lvRec.SelectedItems.Item(0).Index + 1 'ligne de départ


        'LeSuivant = indexNextRec(idep, sqfrom, lecoup)
        LeSuivant = indexNextAllRec(idep, lecoup)
        If LeSuivant <> 255 Then
            lvRec.Items(idep + LeSuivant).ForeColor = Color.Green
            If ThePOS.IsValidMove(lecoup) Then
                'sqFrom = lecoup.Substring(0, 2)
                'sqTo = lecoup.Substring(2, 2)
                'AddMove()
                ThePOS.MakeMove(lecoup)
                DrawPiece()
                lvRec.Items(idep + LeSuivant).SubItems(lv_FEN).Text = ThePOS.GetFEN
            End If
        Else
            lvRec.Items(idep - 1).ForeColor = Color.Red
        End If

    End Sub

    Private Sub DelLineToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles DelLineToolStripMenuItem.Click

        Dim idep As Integer



        idep = lvRec.SelectedItems.Item(0).Index 'ligne de départ


        For l = 0 To 20
            lvRec.Items(idep + l).ForeColor = Color.White
            lvRec.Items(idep + l).SubItems(lv_FEN).Text = ""
        Next
    End Sub


    Private Sub lvRec_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lvRec.SelectedIndexChanged

    End Sub

    Private Sub sslbl1_Click(sender As System.Object, e As System.EventArgs) Handles sslbl1.Click
        FindNextMove()
    End Sub

    Private Sub PictureBox1_Click(sender As System.Object, e As System.EventArgs) Handles PictureBox1.Click

    End Sub

    Private Sub FindNextToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) Handles FindNextToolStripMenuItem.Click
        FindNextMove()
    End Sub
End Class
