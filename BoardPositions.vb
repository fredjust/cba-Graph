Public Class BoardPositions

    Public Const InitFEN As String = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"

    'Type pour les données brutes
    Public Structure aPosition
        Public SAN As String 'premier coup joué pour arrivé à cette position
        Public next_pos As String 'les UCI et id des positions atégnables Cf3-g1f3-1 a1a8-2 ...
        Public last_pos As String 'les UCI et id des positions memant a celle ci b2b3-12 5 3 ...
        Public Arrows As String  'les fleches  e2e4-R d7d5-G ...
        Public Symbols As String  'les symboles e2-t d7-1 ...
        Public FEN As String 'la FEN de la position INDEX UNIQUE
        Public Comments As String 'x|y|text||x|y|text||...
    End Structure

    'les 6 champs d'un FEN
    'pour simplifier la lecture
    Private Structure SplitFEN
        Dim c1Pieces As String
        Dim c2AuTrait As String
        Dim c3DroitRoque As String
        Dim c4CaseEnPassant As String
        Dim c5nbDemiCoup As String
        Dim c6nbMouvement As String
    End Structure

    'pour identifier simplement les champs STRING d'un FEN
    Private ChampsDuFEN As New SplitFEN

    'tableau des positions (en collection je galère trop sur les affectations)
    'a redemensionner tous les 100 positions
    Public col_Positions(110) As aPosition

    'l'id de la position courante 
    Private pos_current As Integer

    Public nb_pos As Integer
    Public Last_SAN As String

    Public Event PositionAdded(ByVal id_pos As Integer)


    ''le premier champs du fen sert de clé
    ''TODO rend impossible de faire plusieurs ecran avec les meme pieces :-(
    'Private Function keyFEN(ByVal aFen As String) As String
    '    Dim tempo = Split(aFen, " ")
    '    Return tempo(0) & tempo(1)
    'End Function


    'efface la contenue d'une position 
    Private Sub clear_pos(ByRef id_pos As Integer)
        With col_Positions(id_pos)
            .SAN = ""
            .last_pos = ""
            .next_pos = ""
            .FEN = ""
            .Arrows = ""
            .Symbols = ""
            .Comments = ""
        End With
    End Sub

    'initialise la position 0
    Private Sub init_first_pos()
        With col_Positions(0)
            .SAN = ""
            .last_pos = ""
            .next_pos = ""
            .FEN = InitFEN
            .Arrows = ""
            .Symbols = ""
            .Comments = ""
        End With
    End Sub

    Public Sub New()

        init_first_pos()
        pos_current = 0

        nb_pos = 1
        clear_pos(1)

    End Sub

#Region "gestion des string list"

    'ajoute un symbole, le remplace ou l'efface dans pos_current
    Public Sub AddOtherSymbol(ByVal str_square As String, ByVal str_color As String, _
                              Optional ByVal id_pos As Integer = -1)
        Dim toDel As Integer

        If id_pos = -1 Then id_pos = pos_current

        With col_Positions(pos_current)

            If .Symbols.Contains(str_square & "-" & str_color) Then
                'il existe avec la meme couleur on l'efface
                toDel = .Symbols.IndexOf(str_square)
                .Symbols = .Symbols.Remove(toDel, 5)
            Else
                If .Symbols.Contains(str_square) Then
                    'il existe avec une couleur différente on le remplace
                    toDel = .Symbols.IndexOf(str_square)
                    .Symbols = .Symbols.Remove(toDel, 5)
                    .Symbols &= str_square & "-" & str_color & " "
                Else
                    'il existe pas on l'ajoute
                    .Symbols &= str_square & "-" & str_color & " "
                End If
            End If

        End With

    End Sub

    'ajoute une fèche, la remplace ou l'efface dans pos_current
    Public Sub AddArrow(ByVal str_squares As String, ByVal str_color As String, _
                        Optional ByVal id_pos As Integer = -1)
        Dim toDel As Integer

        If id_pos = -1 Then id_pos = pos_current

        With col_Positions(id_pos)
            If .Arrows.Contains(str_squares & "-" & str_color) Then
                'contient deja cette fleche avec cette couleur on l'efface
                toDel = .Arrows.IndexOf(str_squares)
                .Arrows = .Arrows.Remove(toDel, 7)
            Else
                If .Arrows.Contains(str_squares) Then
                    'contient que la fleche on change la couleur
                    toDel = .Arrows.IndexOf(str_squares)
                    .Arrows = .Arrows.Remove(toDel, 7)
                    .Arrows &= str_squares & "-" & str_color & " "
                Else
                    'la fleche n'y est pas on l'ajoute
                    .Arrows &= str_squares & "-" & str_color & " "
                End If
            End If
        End With

    End Sub

    'ajoute un deplacement NEXT
    Private Sub AddNext(ByVal str_squares As String, ByVal str_Id As String, _
                        Optional ByVal id_pos As Integer = -1)

        If id_pos = -1 Then id_pos = pos_current

        With col_Positions(id_pos)
            If Not .next_pos.Contains(str_squares) Then
                .next_pos &= str_squares & "-" & str_Id & " "
            End If
        End With

    End Sub

    'ajoute un deplacement NEXT
    Private Sub AddLast(ByVal str_squares As String, ByVal str_Id As String, _
                        Optional ByVal id_pos As Integer = -1)


        If id_pos = -1 Then id_pos = pos_current

        With col_Positions(id_pos)
            If Not .last_pos.Contains(str_squares) Then
                .last_pos &= str_squares & "-" & str_Id & " "
            End If
        End With

    End Sub

#End Region

    Private Function Id_Of_FEN(ByVal aFen As String) As Integer
        For i = 0 To nb_pos
            If col_Positions(i).FEN = aFen Then
                Return i
            End If
        Next
        Return nb_pos
    End Function


    'changement de position suite a un mouvement 
    Public Sub change_Pos_move(ByVal sqFrom As String, ByVal sqTo As String, ByVal new_FEN As String)

        Dim id1, id2 As Integer
        'on veut mettre à jour la pos_current menant à la position2
        'la pos_current existe
        '   on doit ajouter next_pos et trouver l'id de la position suivante
        'la position2 peut existe ou pas
        '   on doit y rajouter last_pos avec l'ancien id

        '---------------------------------------------------------------
        ' MAJ DE POSITION 1
        '---------------------------------------------------------------
        'on arrive pos_current contient toute la postion1 sauf next et l'id de position 2

        'sauvegarde de la position courante
        id1 = pos_current

        '---------------------------------------------------------------
        ' AJOUT DE POSITION 2
        '---------------------------------------------------------------

        'récupérer les infos de la position suivante si elle existe
        id2 = Id_Of_FEN(new_FEN)

        If id2 < nb_pos Then
            'rajoute le coups au coup précédant possible
            AddLast(sqFrom & sqTo, id1, id2)

        Else 'elle n existe pas => nouvelle position
            col_Positions(id2).FEN = new_FEN
            col_Positions(id2).SAN = Last_SAN
            col_Positions(id2).last_pos = sqFrom & sqTo & "-" & id1 & " "
            nb_pos += 1
            clear_pos(nb_pos)
        End If



        'on ajoute dans pos1 le coup aux coups suivants memant vers pos2
        AddNext(sqFrom & sqTo, id2, id1)

        pos_current = id2

        RaiseEvent PositionAdded(id2)


    End Sub


    Public Function LoadFromFile(ByVal NameFile As String) As Boolean
        Dim RecTempo() As String                'tableau de toute les lignes
        Dim DataLine() As String
        Dim RecordsInFile As String = ""        'texte complet du fichier
        Dim iLigne As Integer = 0              'compteur de ligne

        If NameFile = "" Then Return False

        If Not My.Computer.FileSystem.FileExists(NameFile) Then Return False

        Try
            RecordsInFile = My.Computer.FileSystem.ReadAllText(NameFile)
        Catch ex As Exception
            Return False
        End Try

        RecTempo = RecordsInFile.Split(vbCrLf) 'sépare les différentes lignes

        For iLigne = 0 To RecTempo.Count - 2
            DataLine = RecTempo(iLigne).Split("¤")
            With col_Positions(iLigne)
                .SAN = DataLine(1)
                .next_pos = DataLine(2)
                .last_pos = DataLine(3)

                .Symbols = DataLine(4)
                .Arrows = DataLine(5)
                .FEN = DataLine(6)
                .Comments = DataLine(7)
            End With
        Next

        nb_pos = iLigne
        pos_current = iLigne - 1
        Return True
    End Function


#Region "Property"

    Property Id() As Integer
        Get
            Return pos_current
        End Get
        Set(ByVal Value As Integer)
            pos_current = Value
        End Set
    End Property

    Property Arrows() As String
        Get
            If Id = -1 Then Id = pos_current
            Return col_Positions(Id).Arrows
        End Get
        Set(ByVal Value As String)
            If Id = -1 Then Id = pos_current
            col_Positions(Id).Arrows = Value
        End Set
    End Property

    Property SAN() As String
        Get
            If Id = -1 Then Id = pos_current
            Return col_Positions(Id).SAN
        End Get
        Set(ByVal Value As String)
            If Id = -1 Then Id = pos_current
            col_Positions(Id).SAN = Value
        End Set
    End Property

    Property next_pos(Optional ByVal Id As Integer = -1) As String
        Get
            If Id = -1 Then Id = pos_current
            Return col_Positions(Id).next_pos
        End Get
        Set(ByVal Value As String)
            If Id = -1 Then Id = pos_current
            col_Positions(Id).next_pos = Value
        End Set
    End Property

    Property last_pos(Optional ByVal Id As Integer = -1) As String
        Get
            If Id = -1 Then Id = pos_current
            Return col_Positions(Id).last_pos
        End Get
        Set(ByVal Value As String)
            If Id = -1 Then Id = pos_current
            col_Positions(Id).last_pos = Value
        End Set
    End Property

    Property Symbols(Optional ByVal Id As Integer = -1) As String
        Get
            If Id = -1 Then Id = pos_current
            Return col_Positions(Id).Symbols
        End Get
        Set(ByVal Value As String)
            If Id = -1 Then Id = pos_current
            col_Positions(Id).Symbols = Value
        End Set
    End Property

    Property FEN(Optional ByVal Id As Integer = -1) As String
        Get
            If Id = -1 Then Id = pos_current
            Return col_Positions(Id).FEN
        End Get
        Set(ByVal Value As String)
            If Id = -1 Then Id = pos_current
            col_Positions(Id).FEN = Value
        End Set
    End Property

    Property Comments(Optional ByVal Id As Integer = -1) As String
        Get
            If Id = -1 Then Id = pos_current
            Return col_Positions(Id).Comments
        End Get
        Set(ByVal Value As String)
            If Id = -1 Then Id = pos_current
            col_Positions(Id).Comments = Value
        End Set
    End Property

#End Region

End Class
