Public Class BoardPositions

    Public Const InitFEN As String = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"

    'Type pour les données brutes
    Public Structure aPosition
        Public id As Integer 'numéro de la position
        Public next_pos As String 'les UCI et id des positions atégnables e2e4-1 a1a8-2 ...
        Public last_pos As String 'les UCI et id des positions memant a celle ci b2b3-12 5 3 ...
        Public Arrows As String  'les fleches  e2e4-R d7d5-G ...
        Public Symbols As String  'les symboles e2-t d7-1 ...
        Public FEN As String 'la FEN de la position
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

    'collection des positions
    Public col_Positions As Collection

    'la position courante 
    Public pos_current As aPosition

    Public Event PositionAdded()


    'le premier champs du fen sert de clé
    'TODO rend impossible de faire plusieurs ecran avec les meme pieces :-(
    Private Function keyFEN(ByVal aFen As String) As String
        Dim tempo = Split(aFen, " ")
        Return tempo(0) & tempo(1)
    End Function


    Public Function IdOfFen(ByVal aFen12 As String) As Integer

        If col_Positions.Contains(aFen12) Then
            Return col_Positions(aFen12).id
        Else
            Return col_Positions.Count + 1
        End If

    End Function

    Private Sub clear_pos(ByRef aPos As aPosition)
        With aPos
            .id = 0
            .last_pos = ""
            .next_pos = ""
            .FEN = ""
            .Arrows = ""
            .Symbols = ""
            .Comments = ""
        End With
    End Sub

    Private Sub init_pos_current()
        With pos_current
            .id = 1
            .last_pos = ""
            .next_pos = ""
            .FEN = InitFEN
            .Arrows = ""
            .Symbols = ""
            .Comments = ""
        End With
    End Sub


    Public Sub New()

        col_Positions = New Collection

        init_pos_current()

        col_Positions.Add(pos_current, keyFEN(InitFEN))
        
    End Sub

#Region "gestion des string list"

    'ajoute un symbole, le remplace ou l'efface dans pos_current
    Public Sub AddOtherSymbol(ByVal str_square As String, ByVal str_color As String)
        Dim toDel As Integer


        With pos_current

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
    Public Sub AddArrow(ByVal str_squares As String, ByVal str_color As String)
        Dim toDel As Integer



        With pos_current
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
    Private Sub AddNext(ByRef aPos As aPosition, ByVal str_squares As String, ByVal str_Id As String)

        With aPos
            If Not .next_pos.Contains(str_squares) Then
                .next_pos &= str_squares & "-" & str_Id & " "
            End If
        End With

    End Sub

    'ajoute un deplacement NEXT
    Private Sub AddLast(ByRef aPos As aPosition, ByVal str_squares As String, ByVal str_Id As String)

        With aPos
            If Not .last_pos.Contains(str_squares) Then
                .last_pos &= str_squares & "-" & str_Id & " "
            End If
        End With

    End Sub

#End Region

    'efface la position Num et la remplace par la pos_current
    Private Sub col_PositionsAdd(ByVal apos As aPosition)
        If col_Positions.Contains(keyFEN(apos.FEN)) Then
            col_Positions.Remove(keyFEN(apos.FEN))
        End If
        col_Positions.Add(apos, keyFEN(apos.FEN))
    End Sub


    'changement de position suite a un mouvement 
    Public Sub change_Pos_move(ByVal sqFrom As String, ByVal sqTo As String, ByVal new_FEN As String)
        'on veut mettre à jour la position1 menant à la position2
        'la position1 peut exister ou pas
        '   on doit ajouter next_pos et trouver l'id de la position suivante
        'la position2 peut existe ou pas
        '   on doit y rajouter last_pos avec l'ancien id

        Dim pos1, pos2, pos_tempo As New aPosition
        Dim Key1, Key2 As String

        'sauvegarde la position courrante 
        Key1 = keyFEN(pos_current.FEN)


        '---------------------------------------------------------------
        ' MAJ DE POSITION 1
        '---------------------------------------------------------------
        'on arrive pos_current contient toute la postion1 sauf next et l'id de position 2

        'si la position courante existe la supprime
        If col_Positions.Contains(Key1) Then
            pos_tempo = col_Positions(Key1)
            col_Positions.Remove(Key1)
        End If


        'sauvegarde de la position courante
        pos1 = pos_current

        '---------------------------------------------------------------
        ' AJOUT DE POSITION 2
        '---------------------------------------------------------------

        'récupérer les infos de la position suivante si elle existe
        Key2 = keyFEN(new_FEN)

        If col_Positions.Contains(Key2) Then
            'elle existe on la place dans pos_current et pos2
            pos_current = col_Positions(Key2)
            pos2 = col_Positions(Key2)


            'rajoute le coups au coup précédant possible
            AddLast(pos2, sqFrom & sqTo, pos1.id)

        Else 'elle n existe pas => nouvelle position
            clear_pos(pos2)
            pos2.FEN = new_FEN
            pos2.last_pos = sqFrom & sqTo & "-" & pos1.id & " "
            pos2.id = col_Positions.Count + 2

        End If

        'on ajoute dans pos1 le coup aux coups suivants memant vers pos2
        AddNext(pos1, sqFrom & sqTo, pos2.id)

        col_PositionsAdd(pos1)
        col_PositionsAdd(pos2)

        pos_current = pos2

        RaiseEvent PositionAdded()


    End Sub

   
End Class
