Public Class ScreenBoard

    'classe regroupant toutes les variables nécessaires pour gérer l'échiquier gaphique

    Public size_square As Integer 'taille d'une case en pixel
    Public size_border As Byte 'taille du contour de l'échiquier

    Public Structure color_sq
        Dim Alt As Byte
        Dim Shift As Byte
        Dim Control As Byte
    End Structure

    'pas indispensable mais pratique pour refaire tout proprement
    Public Structure aBoardClic
        Dim X As Integer 'position X du clic en pixel
        Dim Y As Integer 'position Y du clic en pixel
        Dim id_square As Byte 'index de la case cliquée : 23
        Dim str_Square As String 'nom de la case cliquée : c2
        Dim RightClic As Boolean 'est ce un clic droit sinon clic normal
        Dim Alt As Boolean 'bouton alt enfoncé ?
        Dim Shift As Boolean 'bouton Shift enfoncé ?
        Dim Control As Boolean 'bouton Control enfoncé ?
    End Structure

    Public clicDown, clicUp As aBoardClic

    Public Color_square As Color_sq

    'list des symboles de déplacement à dessiner sur l'échiquier sous la forme "e4" & "t"
    'e4t signifie sur la case e4 se trouve le symbole t (correspondant à un bitmap)
    Public MoveSymbols As String  'As New List(Of String)

    ''list des symboles de déplacement à dessiner sur l'échiquier sous la forme "e4" & "t"
    ''e4t signifie sur la case e4 se trouve le symbole t (correspondant à un bitmap)
    'Public OtherSymbols As String ' As New List(Of String)

    'list des fleches a dessiner sur l'échiquier sous la forme e2e4-R
    'les deux cases suivit de l'initiale de la couleurs
    'Red Green Blue Cyan Magenta Yelow White Noir
    Public Arrows As New List(Of String)


    'renvoie l'INDEX d'une case a partir de son nom 
    'SquareIndex(xy)=10*y+x
    ' exemple :
    '   SquareIndex(a1)=11
    '   SquareIndex(h8)=88
    'ou 0 en cas d'erreur
    Public Function index_Square(ByVal sqName As String) As Byte
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
    Public Function pt_center(ByVal square) As Point
        Dim pt_tempo As Point
        Dim id_square As Byte

        If TypeOf square Is String Then
            id_square = index_square(square)
        Else
            id_square = CByte(square)
        End If

        With pt_tempo
            .X = rect_square(id_square).X + size_square \ 2
            .Y = rect_square(id_square).Y + size_square \ 2

        End With

        Return pt_tempo

    End Function

    Public Sub New()
        size_border = 18
    End Sub
End Class
