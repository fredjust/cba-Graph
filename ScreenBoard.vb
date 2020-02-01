Imports System.Drawing.Drawing2D

Public Class ScreenBoard

    'classe regroupant toutes les variables nécessaires pour gérer l'échiquier gaphique

    Public size_square As Integer 'taille d'une case en pixel
    Public size_border As Byte 'taille du contour de l'échiquier
    Public Reversed As Boolean 'si l'échiquier est affiché a l'envers (les noirs en bas)

    Public nb_arrow As Byte = 0
    Public path_arrow(32) As GraphicsPath 'chemin des fleches
    Public ToId_arrow(32) As Integer 'id de la position vers laquelle pointe la fleche
    Public Arrow_Over As Byte

    Public nb_comment As Byte = 0
    Public path_comment(16) As GraphicsPath 'chemin des zones
    Public Idstr_comment(16) As String 'case et couleur de la zone pour la retrouver
    Public Comment_Over As Byte
    Public str_Comment_Over As String

    Public InputBoxRect As New Rectangle 'rectangle de la zone de saisie


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

    Public Color_square As color_sq

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

        If Reversed Then

            With rect_tempo
                .X = size_border + (8 - colonne) * size_square
                .Y = size_border + (ligne - 1) * size_square
                .Width = size_square
                .Height = size_square
            End With

        Else
            With rect_tempo
                .X = size_border + (colonne - 1) * size_square
                .Y = size_border + (8 - ligne) * size_square
                .Width = size_square
                .Height = size_square
            End With
        End If

        Return rect_tempo

    End Function

    'renvoie un rectangle entre 2 case
    'square peut etre une coordonné a1 ou un index 11
    Public Function rect_squares(ByVal square1, ByVal square2) As Rectangle
        Dim colonne1, colonne2 As Byte
        Dim ligne1, ligne2 As Byte
        Dim rect_tempo, errRect As Rectangle
        Dim id_square1, id_square2 As Byte

        On Error GoTo err


        If TypeOf square1 Is String Then
            id_square1 = index_Square(square1)
        Else
            id_square1 = CByte(square1)
        End If

        If TypeOf square2 Is String Then
            id_square2 = index_Square(square2)
        Else
            id_square2 = CByte(square2)
        End If

        colonne1 = id_square1 Mod 10
        ligne1 = id_square1 \ 10

        colonne2 = id_square2 Mod 10
        ligne2 = id_square2 \ 10

        If Reversed Then

            With rect_tempo
                .X = size_border + (8 - colonne2) * size_square
                .Y = size_border + (ligne2 - 1) * size_square
                .Width = size_square * (colonne2 - colonne1 + 1)
                .Height = size_square * (ligne1 - ligne2 + 1)
            End With

        Else
            With rect_tempo
                .X = size_border + (colonne1 - 1) * size_square
                .Y = size_border + (8 - ligne1) * size_square
                .Width = size_square * (colonne2 - colonne1 + 1)
                .Height = size_square * (ligne1 - ligne2 + 1)
            End With
        End If

        Return rect_tempo
err:
        With errRect
            .X = size_border
            .Y = size_border
            .Width = size_square * 2
            .Height = size_square * 1
        End With
        Return errRect

    End Function

    'renvoie le centre d'une case
    Public Function pt_center(ByVal square) As Point
        Dim pt_tempo As Point
        Dim id_square As Byte

        If TypeOf square Is String Then
            id_square = index_Square(square)
        Else
            id_square = CByte(square)
        End If

        With pt_tempo
            .X = rect_square(id_square).X + size_square \ 2
            .Y = rect_square(id_square).Y + size_square \ 2

        End With

        Return pt_tempo

    End Function

   
    'modifie le rectangle en fonction des coordonnées de 2 cases
    'EN COORDONNEES DE CASE
    Public Sub Name2coord(ByVal strSquares As String)
        Dim lettre1, lettre2 As Char
        Dim colonne1, colonne2 As Byte
        Dim ligne1, ligne2 As Byte

        'TODO SI L ECHIQUIER EST INVERSE

        If Reversed Then 'transforme e3g2 en b7d6

            'Je reprend le prog apres 2 selmaine je ne comprends plus rien
            'je fatigue c'est la merde
            'j'inverse comme je peux

            lettre1 = strSquares.Substring(0, 1)
            colonne1 = Asc(lettre1) - 96 'recupere le numero de colonne
            ligne1 = 9 - strSquares.Substring(1, 1)

            lettre2 = strSquares.Substring(2, 1)
            colonne2 = Asc(lettre2) - 96 'recupere le numero de colonne
            ligne2 = 9 - strSquares.Substring(3, 1)

            colonne1 = 9 - colonne1
            lettre1 = Chr(colonne1 + 96)

            colonne2 = 9 - colonne2
            lettre2 = Chr(colonne2 + 96)

            Dim tempo As String
            tempo = lettre2 & ligne2 & lettre1 & ligne1

            lettre1 = tempo.Substring(0, 1)
            colonne1 = Asc(lettre1) - 96 'recupere le numero de colonne
            ligne1 = 8 - tempo.Substring(1, 1)

            lettre2 = tempo.Substring(2, 1)
            colonne2 = Asc(lettre2) - 96 'recupere le numero de colonne
            ligne2 = 8 - tempo.Substring(3, 1)



        Else

            lettre1 = strSquares.Substring(0, 1)
            colonne1 = Asc(lettre1) - 96 'recupere le numero de colonne
            ligne1 = 8 - strSquares.Substring(1, 1)

            lettre2 = strSquares.Substring(2, 1)
            colonne2 = Asc(lettre2) - 96 'recupere le numero de colonne
            ligne2 = 8 - strSquares.Substring(3, 1)


        End If

        With InputBoxRect
            .X = colonne1 - 1
            .Y = ligne1
            .Width = colonne2 - colonne1 + 1
            .Height = ligne2 - ligne1 + 1
        End With



    End Sub

    'renvoie les cases correspondant aux coordonnées de InputBoxRect
    Public Function Coord2name() As String

        Dim lettre As Char
        Dim colonne As Byte
        Dim ligne As Byte

        Dim tempo As String = ""

        colonne = InputBoxRect.X + 1
        ligne = 8 - InputBoxRect.Y

        If colonne < 1 Or colonne > 8 Then Return ""
        If ligne < 1 Or ligne > 8 Then Return ""

        If Reversed Then
            ligne = 9 - ligne
            colonne = 9 - colonne
        End If

        lettre = Chr(colonne + 96) 'recupere le numero de colonne

        'ajoute la premiere case
        tempo = lettre & ligne.ToString

        colonne = InputBoxRect.X + InputBoxRect.Width
        ligne = 8 - (InputBoxRect.Y + InputBoxRect.Height) + 1

        If colonne < 1 Or colonne > 8 Then Return ""

        If ligne < 1 Or ligne > 8 Then Return ""

        If Reversed Then
            ligne = 9 - ligne
            colonne = 9 - colonne
        End If

        lettre = Chr(colonne + 96) 'recupere le numero de colonne

        'ajoute la seconde case
        tempo &= lettre & ligne.ToString

        'on inverse
        If Reversed Then
            tempo = tempo.Substring(2, 2) & tempo.Substring(0, 2)
        End If

        Return tempo

    End Function


    Public Sub New()
        size_border = 18
        Reversed = False
        Arrow_Over = 255
        Comment_Over = 255
        For i = 0 To 32
            ToId_arrow(i) = -1
        Next


        With InputBoxRect
            .X = 3
            .Y = 2
            .Width = 4
            .Height = 2
        End With
    End Sub
End Class
