Public Class BoardPositions

    Public Const IntitFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"

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

    ' Public pos_current As aPosition


    Private Function keyFEN(ByVal aFen As String)
        Dim tempo = Split(aFen, " ")
        Return tempo(0)
    End Function


    'ajoute une position à la collection 
    'ou met a jour ses infos si elle existe déjà
    Public Sub Add_Pos_Col(ByVal aPos As aPosition)
        Dim pos_tempo As aPosition
        If col_Positions.Contains(keyFEN(aPos.FEN)) Then
            'la position existe on met a jour les infos
            pos_tempo = col_Positions.Item(keyFEN(aPos.FEN))
            With pos_tempo

            End With
        Else
            col_Positions.Add(aPos, keyFEN(aPos.FEN))
        End If

    End Sub

    'initialise la position sur la première position
    Public Sub first_pos(ByRef aPos As aPosition)
        With aPos
            .id = 0
            .last_pos = ""
            .next_pos = ""
            .FEN = IntitFEN
            .Arrows = ""
            .Symbols = ""
            .Comments = ""
        End With
    End Sub

    Public Function IdOfFen(ByVal aFen As String) As Integer

        If col_Positions.Contains(keyFEN(aFen)) Then
            Return col_Positions.Item(keyFEN(aFen)).id
        Else
            Return col_Positions.Count
        End If



    End Function


    'Efface les infos d'une position
    Public Sub Clear_Pos(ByRef aPos As aPosition)
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


    Public Sub New()

        col_Positions = New Collection

        Dim pos_first As New aPosition

        first_pos(pos_first)

        col_Positions.Add(pos_first, keyFEN(pos_first.FEN))

    End Sub
End Class
