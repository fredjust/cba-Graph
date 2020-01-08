Public Class ScreenBoard

    'classe regroupant toutes les variables nécessaires pour gérer l'échiquier gaphique

    Public size_square As Integer 'taille d'une case en pixel
    Public size_border As Byte 'taille du contour de l'échiquier

    'renvoie l'INDEX d'une case a partir de son nom 
    'SquareIndex(xy)=10*y+x
    ' exemple :
    '   SquareIndex(a1)=11
    '   SquareIndex(h8)=88
    'ou 0 en cas d'erreur
    Private Function index_Square(ByVal sqName As String) As Byte
        Dim lettre As Char
        Dim colonne As Byte
        Dim ligne As Byte

        If sqName.Length <> 2 Then Return 0

        lettre = sqName.Substring(0, 1) 'recupere la lettre
        colonne = Asc(lettre) - 96 'recupere le numero de colonne

        If colonne < 1 Or colonne > 8 Then Return 0

        ligne = sqName.Substring(1, 1) 'recupere le numero de ligne

        If ligne < 1 Or ligne > 8 Then Return 0

        Return ligne * 10 + colonne

    End Function

    'renvoie un rectangle correspondant à une case
    'square peut etre une coordonné a1 ou un index 11
    Public Function rect_square(ByVal square) As Rectangle
        Dim colonne As Byte
        Dim ligne As Byte
        Dim rect_tempo As Rectangle
        Dim id_square As Byte

        If TypeOf square Is String Then
            id_square = index_Square(square)
        Else
            id_square = CByte(square)
        End If

        colonne = id_square Mod 10
        ligne = id_square \ 10

        With rect_tempo
            .X = size_border + (colonne - 1) * size_square
            .Y = size_border + (8 - ligne) * size_square
            .Width = size_square
            .Height = size_square
        End With

        Return rect_tempo

    End Function

    'renvoie le centre d'une case
    Public Function pt_center(ByVal index_square As Byte) As Point
        Dim pt_tempo As Point

        With pt_tempo
            .X = rect_square(index_square).X + size_square \ 2
            .Y = rect_square(index_square).Y + size_square \ 2

        End With

        Return pt_tempo

    End Function

    Public Sub New()
        size_border = 18
    End Sub
End Class
