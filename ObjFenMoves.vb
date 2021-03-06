﻿Public Class ObjFenMoves

    'permet de controler les mouvements aux échecs
    'codé sans réflexion préalable
    'en rajoutant au fur et a mesure les bouts de code pour corriger les imprévus du départ
    'gestion complète des notations FEN en import et export
    '
    'le but était d'écrire un truc fonctinnel pour mon projet ARDUINO
    'http://www.cpe95.org/spip.php?rubrique128
    '
    'désolé pour le code très old school
    'je découvre VB.NET apres avoir laissé VB 6 il y a 10 ans
    'le but n'était pas d'abuser de collections et d'autres techniques avancées
    '
    'FredJust@gmail.com


    '***************************************************************************************
    '*****************************  VARIABLES **********************************************
    '***************************************************************************************
#Region "Déclaration des variables"

    'Fen correspondant à un début de partie
    Private Const IntitFEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"

    'le tableau 10x10 de 0 à 99 contient le tableau 8x8 de 11 à 88
    ' la case 11 est la case a1
    ' 90 91 92 93 94 95 96 97 98 99     =>      xx xx xx xx xx xx xx xx xx xx 
    ' 80 81 82 83 84 85 86 87 88 89     =>      xx a8 b8 c8 d8 e8 f8 g8 h8 xx
    ' 70 71 72 73 74 75 76 77 77 79     =>      xx a7 b7 c7 d7 e7 f7 g7 h7 xx
    ' 60 61 62 63 64 65 66 67 68 69     =>      xx a6 b6 c6 d6 e6 f6 g6 h6 xx
    ' 50 51 52 53 54 55 56 57 58 59     =>      xx a5 b5 c5 d5 e5 f5 g5 h5 xx
    ' 40 41 42 43 44 45 46 47 48 49     =>      xx a4 b4 c4 d4 e4 f4 g4 h4 xx
    ' 30 31 32 33 34 35 36 37 38 39     =>      xx a3 b3 c3 d3 e3 f3 g3 h3 xx
    ' 20 21 22 23 24 25 26 27 28 29     =>      xx a2 b2 c2 d2 e2 f2 g2 h2 xx
    ' 10 11 12 13 14 15 16 17 18 19     =>      xx a1 b1 c1 d1 e1 f1 g1 h1 xx
    ' 00 01 02 03 04 05 06 07 08 09     =>      xx xx xx xx xx xx xx xx xx xx 

    Public Board10x10(100) As Char

    'les direction possible pour le tableau 10x10    
    Private Enum dpl As Integer
        RIGHT = 1       'a droite de a1 c'est b1 index 2
        LEFT = -1
        UP = 10         'au dessus de a1 c'est a2 index 11
        DOWN = -10
        UP_LEFT = 9
        DOWN_LEFT = -11
        UP_RIGHT = 11
        DOWN_RIGHT = -9
    End Enum


    'utiliser pour connaitre la couleur d'une piece sur une case
    Private Enum ColorPiece
        Empty = 0
        White = 1
        Black = 2
        Border = 3      'en dehors de la place 11-88
        OutOfBounds = 4 'en dehors de la place 0-99
    End Enum

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

    'Traduction en variable des infos d un FEN
    Private Structure InfoFEN
        Dim ToPlay As ColorPiece            'le champ 2 w ou b
        Dim WhiteCanDoLittle As Boolean     'le champ 3 contient K
        Dim WhiteCanDoBig As Boolean        '                   Q
        Dim BlackCanDoLittle As Boolean     '                   k
        Dim BlackCanDoBig As Boolean        '                   q
        Dim sqiEnPassant As Byte            'le champ 4 index de la case si elle existe
        Dim nbSinceLastPawn As UInteger     'le champ 5 +1 a chaque mouvement sans prise ou mouvement de pion
        Dim nbMovesPlayed As Single         'le champ 6 on avance de 0.5 en 0.5 et on récupère la partie entière
    End Structure

    Private aFEN As New InfoFEN

    'pour traduire les pièces
    Private _LocalPIECE As String = "RNBQK"

#End Region

#Region "Fonctions privées"

#Region "Gestion du tableau et des index"

    '***************************************************************************************
    '               REMPLI Board10x10 A PARTIR DU CHAMP 1 D UN FEN
    '---------------------------------------------------------------------------------------
    'CODE ERREUR
    '0 OK
    '10 on a pas 8 lignes
    '10 + NumLigne cette ligne ne contient pas 8 char
    Private Function ToBoard(ByVal FEN_Champ1 As String) As Byte
        Dim lignes() As String
        Dim NumLigne As Integer
        Dim NumCol As Integer
        Dim LaLigne As String

        'on place des bords partout
        For NumLigne = 0 To 99
            Board10x10(NumLigne) = "*"
        Next

        'sépare les lignes
        lignes = FEN_Champ1.Split("/")

        'a t on 8 lignes ?
        If lignes.Count <> 8 Then
            Return 10
        End If

        For NumLigne = 0 To 7

            LaLigne = lignes(NumLigne)

            'remplace les nombres par leur équivalent en espace
            LaLigne = LaLigne.Replace("1", " ")
            LaLigne = LaLigne.Replace("2", "  ")
            LaLigne = LaLigne.Replace("3", "   ")
            LaLigne = LaLigne.Replace("4", "    ")
            LaLigne = LaLigne.Replace("5", "     ")
            LaLigne = LaLigne.Replace("6", "      ")
            LaLigne = LaLigne.Replace("7", "       ")
            LaLigne = LaLigne.Replace("8", "        ")

            'rempli le tableau avec les lettres du fen ou un espace si vide
            If LaLigne.Length = 8 Then
                For NumCol = 0 To 7
                    Board10x10((8 - NumLigne) * 10 + (NumCol + 1)) = LaLigne.Substring(NumCol, 1)
                Next
            Else
                Return 11 + NumLigne
            End If

        Next

        'tout est OK
        Return 0

    End Function




    'renvoie l'INDEX d'une case a partir de son nom 
    'SquareIndex(xy)=10*y+x
    ' exemple :
    '   SquareIndex(a1)=11
    '   SquareIndex(h8)=88
    'ou 0 en cas d'erreur
    Public Function SquareIndex(ByVal sqName As String, Optional ByVal reversed As Boolean = False) As Byte
        Dim lettre As Char
        Dim colonne As Byte
        Dim ligne As Byte

        If sqName.Length <> 2 Then Return 0

        lettre = sqName.Substring(0, 1) 'recupere la lettre
        colonne = Asc(lettre) - 96 'recupere le numero de colonne

        If colonne < 1 Or colonne > 8 Then Return 0

        ligne = sqName.Substring(1, 1) 'recupere le numero de ligne

        If ligne < 1 Or ligne > 8 Then Return 0

        If reversed Then
            Return (9 - ligne) * 10 + (9 - colonne)
        Else
            Return ligne * 10 + colonne
        End If

        Return ligne * 10 + colonne

    End Function

    'renvoie le nom d'une case sous la forme e2
    'ou un caractère vide en cas d'erreur
    'SquareName(11)=a1
    Public Function SquareName(ByVal sqIndex As Byte, Optional ByVal reversed As Boolean = False) As String
        Dim lettre As Char
        Dim colonne As Byte
        Dim ligne As Byte

        If sqIndex > 10 And sqIndex < 89 Then
            colonne = sqIndex Mod 10

            If colonne < 1 Or colonne > 8 Then Return ""

            ligne = sqIndex \ 10

            If ligne < 1 Or ligne > 8 Then Return ""

            If reversed Then
                ligne = 9 - ligne
                colonne = 9 - colonne
            End If

            lettre = Chr(colonne + 96) 'recupere le numero de colonne


            Return lettre & ligne.ToString
        Else
            Return ""
        End If


    End Function

    'Renvoie la couleur de la piece sur la case 1 blanc 2 noire 0 vide ou 3 bord
    Private Function ColorOf(ByVal sqIndex As Byte) As ColorPiece

        If sqIndex >= 0 And sqIndex < 100 Then

            If Board10x10(sqIndex) = " " Then Return ColorPiece.Empty
            If Board10x10(sqIndex) = "*" Then Return ColorPiece.Border

            If UCase(Board10x10(sqIndex)) = Board10x10(sqIndex) Then    'les majuscules sont des pieces blanches
                Return ColorPiece.White
            Else
                Return ColorPiece.Black
            End If

        Else
            Return ColorPiece.OutOfBounds
        End If

    End Function

    'Contraire de la fonction colorOf
    Private Function ColorOpponent(ByVal sqIndex As Byte) As Byte

        Select Case ColorOf(sqIndex)

            Case ColorPiece.White
                Return ColorPiece.Black
            Case ColorPiece.Black
                Return ColorPiece.White
            Case ColorPiece.Border
                Return ColorPiece.Border
            Case ColorPiece.Empty
                Return ColorPiece.Empty
            Case Else
                Return ColorPiece.OutOfBounds
        End Select

    End Function

    'renvoie la couleur opposée a ToPlay
    Private Function ToNotPlay() As ColorPiece

        Select Case aFEN.ToPlay

            Case ColorPiece.White
                Return ColorPiece.Black
            Case ColorPiece.Black
                Return ColorPiece.White
            Case Else
                Return ColorPiece.OutOfBounds
        End Select

    End Function

#End Region

    '***************************************************************************************
    '********************** FONCTION SUR LES MOUVELENTS ************************************
    '***************************************************************************************

#Region "Gestion des mouvements"

    'AJOUTE LES COUPS POSSIBLE DANS UNE DIRECTION et AJOUTE LA PRISE A LA FIN
    Private Sub AddMoves(ByVal sqIndex As Byte, ByVal Direction As dpl, ByRef TheMoves As String)
        Dim NextSquare As Byte

        NextSquare = sqIndex + Direction

        While Board10x10(NextSquare) = " "

            TheMoves &= SquareName(NextSquare) & " "

            If UCase(Board10x10(sqIndex)) = "K" Then 'si c'est un roi on s'arette 
                Exit Sub
            End If

            NextSquare = NextSquare + Direction
        End While

        If ColorOf(NextSquare) = ColorOpponent(sqIndex) Then

            TheMoves &= "x" & SquareName(NextSquare) & " "

        End If

    End Sub

    'AJOUTE UNE PRISE EN VERIFIANT QUE LA CASE DE DESTINATION EST PRENABLE UNIQUEMENT POUR LES PIONS
    Private Sub AddTake(ByVal sqiTO As Byte, ByVal sqiFROM As Byte, ByRef TheMoves As String)
        If ColorOf(sqiTO) = ColorOpponent(sqiFROM) Then
            TheMoves &= "x" & SquareName(sqiTO) & " "
        End If
        'GESTION DE LA PRISE EN PASSANT
        If sqiTO = aFEN.sqiEnPassant Then
            If aFEN.ToPlay = ColorOf(sqiFROM) Then 'si la piece de départ est de la couleur de celui qui doit jouer
                TheMoves &= "x" & SquareName(sqiTO) & " "
            End If
        End If
    End Sub

    'AJOUTE UN MOUVEMENT UNIQUE POUR LE CAVALIER ET LES PIONS
    Private Sub AddMove(ByVal sqiTO As Integer, ByVal sqiFROM As Byte,
                        ByRef TheMoves As String, Optional ByVal AndTake As Boolean = True)

        If sqiTO > 10 And sqiTO < 89 Then

            If Board10x10(sqiTO) = " " Then
                TheMoves &= SquareName(sqiTO) & " "
            End If

            If AndTake Then
                If ColorOf(sqiTO) = ColorOpponent(sqiFROM) Then
                    TheMoves &= "x" & SquareName(sqiTO) & " "
                End If
            End If

        End If

    End Sub

    'GENERE TOUS LES MOUVEMENTS ET PRISE POSSIBLE POUR UNE DAME
    'LES STOCKS DANS UN STRING SOUS LA FORME : a1 xb1 e4 c5 
    Private Function Qmoves(ByVal sqIndex As Byte) As String
        Dim TempoMoves As String = ""

        AddMoves(sqIndex, dpl.DOWN, TempoMoves)
        AddMoves(sqIndex, dpl.UP, TempoMoves)
        AddMoves(sqIndex, dpl.LEFT, TempoMoves)
        AddMoves(sqIndex, dpl.RIGHT, TempoMoves)
        AddMoves(sqIndex, dpl.DOWN_LEFT, TempoMoves)
        AddMoves(sqIndex, dpl.UP_LEFT, TempoMoves)
        AddMoves(sqIndex, dpl.DOWN_RIGHT, TempoMoves)
        AddMoves(sqIndex, dpl.UP_RIGHT, TempoMoves)

        Return Trim(TempoMoves)
    End Function

    'GESTION DES DEPLACEMENTS DU ROI
    'LES ROQUES SONT GERES 
    Private Function Kmoves(ByVal sqIndex As Byte) As String
        Dim TempoMoves As String = ""

        AddMoves(sqIndex, dpl.DOWN, TempoMoves)
        AddMoves(sqIndex, dpl.UP, TempoMoves)
        AddMoves(sqIndex, dpl.LEFT, TempoMoves)
        AddMoves(sqIndex, dpl.RIGHT, TempoMoves)

        AddMoves(sqIndex, dpl.DOWN_LEFT, TempoMoves)
        AddMoves(sqIndex, dpl.UP_LEFT, TempoMoves)
        AddMoves(sqIndex, dpl.DOWN_RIGHT, TempoMoves)
        AddMoves(sqIndex, dpl.UP_RIGHT, TempoMoves)

        'ROQUE
        If SquareName(sqIndex) = "e1" Then

            If aFEN.WhiteCanDoLittle Then
                If Board10x10(sqIndex + dpl.RIGHT) = " " _
                    And Board10x10(sqIndex + dpl.RIGHT * 2) = " " Then
                    'VERIFIER QUE LE CASE f1  g1 ne sont pas en échecs
                    AddMoves(sqIndex, dpl.RIGHT * 2, TempoMoves)
                End If
            End If

            If aFEN.WhiteCanDoBig Then
                If Board10x10(sqIndex + dpl.LEFT) = " " _
                    And Board10x10(sqIndex + dpl.LEFT * 2) = " " _
                    And Board10x10(sqIndex + dpl.LEFT * 3) = " " Then
                    'VERIFIER QUE LES CASES d1 c1 ne sont pas en echecs
                    AddMoves(sqIndex, dpl.LEFT * 2, TempoMoves)
                End If
            End If

        End If

        If SquareName(sqIndex) = "e8" Then

            If Board10x10(sqIndex + dpl.RIGHT) = " " _
                And Board10x10(sqIndex + dpl.RIGHT * 2) = " " Then
                If aFEN.BlackCanDoLittle Then
                    'VERIFIER QUE LE CASE f8  g8 ne sont pas en échecs
                    AddMoves(sqIndex, dpl.RIGHT * 2, TempoMoves)
                End If
            End If

            If aFEN.BlackCanDoBig Then
                If Board10x10(sqIndex + dpl.LEFT) = " " _
                And Board10x10(sqIndex + dpl.LEFT * 2) = " " _
                And Board10x10(sqIndex + dpl.LEFT * 3) = " " Then
                    'VERIFIER QUE LE CASE d8 c8 ne sont pas en echecs
                    AddMoves(sqIndex, dpl.LEFT * 2, TempoMoves)
                End If
            End If

        End If

        Return Trim(TempoMoves)

    End Function

    'DEPLACEMENTS DU FOU
    Private Function Bmoves(ByVal sqIndex As Byte) As String
        Dim TempoMoves As String = ""

        AddMoves(sqIndex, dpl.DOWN_LEFT, TempoMoves)
        AddMoves(sqIndex, dpl.UP_LEFT, TempoMoves)
        AddMoves(sqIndex, dpl.DOWN_RIGHT, TempoMoves)
        AddMoves(sqIndex, dpl.UP_RIGHT, TempoMoves)

        Return Trim(TempoMoves)
    End Function

    'DEPLACEMENTS DU CAVALIER
    Private Function Nmoves(ByVal sqIndex As Byte) As String
        Dim TempoMoves As String = ""

        AddMove(sqIndex + 12, sqIndex, TempoMoves)
        AddMove(sqIndex - 12, sqIndex, TempoMoves)
        AddMove(sqIndex + 21, sqIndex, TempoMoves)
        AddMove(sqIndex - 21, sqIndex, TempoMoves)
        AddMove(sqIndex + 8, sqIndex, TempoMoves)
        AddMove(sqIndex - 8, sqIndex, TempoMoves)
        AddMove(sqIndex + 19, sqIndex, TempoMoves)
        AddMove(sqIndex - 19, sqIndex, TempoMoves)

        Return Trim(TempoMoves)

    End Function

    'DEPLACEMENT D UNE TOUR
    Private Function Rmoves(ByVal sqIndex As Byte) As String
        Dim TempoMoves As String = ""

        AddMoves(sqIndex, dpl.DOWN, TempoMoves)
        AddMoves(sqIndex, dpl.UP, TempoMoves)
        AddMoves(sqIndex, dpl.LEFT, TempoMoves)
        AddMoves(sqIndex, dpl.RIGHT, TempoMoves)

        Return Trim(TempoMoves)

    End Function


    'DEPLACEMENT D UN PION
    'LA PRISE EN PASSANT EST GEREE
    Private Function Pmoves(ByVal sqIndex As Byte) As String
        Dim TempoMoves As String = ""

        If ColorOf(sqIndex) = 1 Then
            AddMove(sqIndex + dpl.UP, sqIndex, TempoMoves, False)


            If sqIndex < 29 And TempoMoves <> "" Then
                'la case devant le pion doit etre vide pour qu'il puisse avancer de deux !
                If Board10x10(sqIndex + dpl.UP) = " " Then
                    AddMove(sqIndex + 2 * dpl.UP, sqIndex, TempoMoves, False)
                End If

            End If

            AddTake(sqIndex + dpl.UP_LEFT, sqIndex, TempoMoves)
            AddTake(sqIndex + dpl.UP_RIGHT, sqIndex, TempoMoves)

        End If

        If ColorOf(sqIndex) = 2 Then
            AddMove(sqIndex + dpl.DOWN, sqIndex, TempoMoves, False)
            If sqIndex > 70 And TempoMoves <> "" Then
                'la case devant le pion doit etre vide pour qu'il puisse avancer de deux !
                If Board10x10(sqIndex + dpl.DOWN) = " " Then
                    AddMove(sqIndex + 2 * dpl.DOWN, sqIndex, TempoMoves, False)
                End If
            End If

            AddTake(sqIndex + dpl.DOWN_LEFT, sqIndex, TempoMoves)
            AddTake(sqIndex + dpl.DOWN_RIGHT, sqIndex, TempoMoves)

        End If

        Return Trim(TempoMoves)

    End Function

#End Region

    'renvoie la position de la seconde pièce 0 si elle n'est pas présente
    Private Function FindOther(ByVal sqIndex As Byte) As Byte
        Dim LaPiece As Char

        LaPiece = Board10x10(sqIndex)

        If LaPiece = " " Then Return 0

        For sqi = 11 To 88
            If Board10x10(sqi) = LaPiece Then
                If sqi <> sqIndex Then
                    Return sqi
                End If
            End If
        Next

        Return 0

    End Function

    'converti les initiales en en local
    ' LocalPIECE =  RNBQK in en
    '               TCFDR in fr
    '   R => T
    '   N => C
    '   B => F
    '   Q => D
    '   K => R
    Private Function ToLocal(ByVal InitialeEN As Char) As Char
        Select Case UCase(InitialeEN)
            Case "R"
                Return LocalPiece.Substring(0, 1)
            Case "N"
                Return LocalPiece.Substring(1, 1)
            Case "B"
                Return LocalPiece.Substring(2, 1)
            Case "Q"
                Return LocalPiece.Substring(3, 1)
            Case "K"
                Return LocalPiece.Substring(4, 1)
            Case Else
                Return " "
        End Select

    End Function

    'deplace une piece avec deux noms a1 h8
    Private Sub MovePiece(ByVal sqFrom As String, ByVal sqTo As String)
        MovePiece(SquareIndex(sqFrom), SquareIndex(sqTo))
    End Sub

    'deplace une piece avec  deux index
    Private Sub MovePiece(ByVal sqiFrom As Integer, ByVal sqiTo As Integer)
        'TODO NE MARCHE PAS
        'remetre à zero le compteur en cas de prise
        'If Board10x10(sqiTo) <> " " Then aFEN.nbSinceLastPawn = 0
        'place la piece de depart sur la case d'arrivé
        Board10x10(sqiTo) = Board10x10(sqiFrom)
        'efface la case de depart
        Board10x10(sqiFrom) = " "
    End Sub

    'supprime une piece 
    Private Sub RemovePiece(ByVal aSquare)
        If TypeOf (aSquare) Is String Then
            Board10x10(SquareIndex(aSquare)) = " "
        Else
            Board10x10(aSquare) = " "
        End If

    End Sub

    'vérifie si le déplacement correspond a un roque
    'renvoi la lettre correspondant a ce roque KQkq ou "" dans ThisRoque
    Private Function IsARoque(ByVal FromTo As String, ByRef ThisRoque As Char) As Boolean
        Dim sqNameFROM As String
        Dim sqNameTO As String
        Dim sqiFROM As Byte
        Dim sqiTO As Byte

        'recoi un mouvement du type e2e4
        sqNameFROM = FromTo.Substring(0, 2)
        sqNameTO = FromTo.Substring(2, 2)

        sqiFROM = SquareIndex(sqNameFROM)  'recupere l'index de la case
        sqiTO = SquareIndex(sqNameTO)

        If sqNameFROM = "e1" Then
            If Board10x10(sqiFROM) = "K" Then
                If sqNameTO = "g1" Then
                    ThisRoque = "K"
                    Return True
                End If
                If sqNameTO = "c1" Then
                    ThisRoque = "Q"
                    Return True
                End If
            End If
        End If

        If sqNameFROM = "e8" Then
            If Board10x10(sqiFROM) = "k" Then
                If sqNameTO = "g8" Then
                    ThisRoque = "k"
                    Return True
                End If
                If sqNameTO = "c8" Then
                    ThisRoque = "q"
                    Return True
                End If
            End If
        End If

        Return False

    End Function

    'inverse le joueur au trait
    'ajoute un demi mouvement
    Private Sub SwitchToPlay()
        If aFEN.ToPlay = ColorPiece.White Then
            aFEN.ToPlay = ColorPiece.Black
        Else
            aFEN.ToPlay = ColorPiece.White
        End If
        aFEN.nbMovesPlayed += 0.5
    End Sub

    'deplace le roi et la tour pour un roque donner sous forme d'un char du champ FEN
    Private Sub MovesForRoque(ByVal ThisRoque As Char)
        Select Case ThisRoque
            Case "K"
                aFEN.WhiteCanDoLittle = False
                aFEN.WhiteCanDoBig = False
                MovePiece("e1", "g1")
                MovePiece("h1", "f1")
            Case "Q"
                aFEN.WhiteCanDoBig = False
                aFEN.WhiteCanDoLittle = False
                MovePiece("e1", "c1")
                MovePiece("a1", "d1")
            Case "k"
                aFEN.BlackCanDoLittle = False
                aFEN.BlackCanDoBig = False
                MovePiece("e8", "g8")
                MovePiece("h8", "f8")
            Case "q"
                aFEN.BlackCanDoBig = False
                aFEN.BlackCanDoLittle = False
                MovePiece("e8", "c8")
                MovePiece("a8", "d8")
        End Select
        aFEN.sqiEnPassant = 0

        SwitchToPlay()
        aFEN.nbSinceLastPawn += 1 '

    End Sub

    'vérifie s'il s'agit d'une prise en Passant
    Private Function IsEnPassant(ByVal sqiFrom As Byte, ByVal sqiTo As Byte) As Boolean

        If aFEN.sqiEnPassant = sqiTo Then       'on arrive sur la case enpassant
            If Board10x10(sqiFrom) = "P" Then   'si c'est un pion blanc
                Return True
            End If
            If Board10x10(sqiFrom) = "p" Then   'si c'est un pion noir
                Return True
            End If
        End If

        Return False

    End Function

    'deplace le pion et enlève le pion adverse
    Private Sub MovesForEnPassant(ByVal sqiFrom As Byte, ByVal sqiTo As Byte)

        If Board10x10(sqiFrom) = "P" Then RemovePiece(aFEN.sqiEnPassant - 10)
        If Board10x10(sqiFrom) = "p" Then RemovePiece(aFEN.sqiEnPassant + 10)
        MovePiece(sqiFrom, sqiTo)

        aFEN.nbSinceLastPawn = 0
        aFEN.sqiEnPassant = 0
        SwitchToPlay()

    End Sub

    'Renvoi le FEN correspondant a l'état des variables aFEN


#End Region

#Region "Fonctions publiques"

    Sub New(Optional ByVal InitFen As String = IntitFEN)
        SetFEN(InitFen)
    End Sub


    ''' <summary>
    ''' retourne le FEN correspondant à l'état actuel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFEN() As String

        Dim colonne As Int16
        Dim ligne As Int16
        Dim sqi As Byte
        Dim strFEN As String = ""
        Dim nb_espace As Byte = 0


        For ligne = 8 To 1 Step -1
            For colonne = 1 To 8
                sqi = ligne * 10 + colonne
                If Board10x10(sqi) <> " " Then
                    If nb_espace > 0 Then
                        strFEN = strFEN + nb_espace.ToString
                        nb_espace = 0
                    End If
                    strFEN = strFEN + Board10x10(sqi)
                Else
                    nb_espace = nb_espace + 1
                End If

            Next
            If nb_espace > 0 Then
                strFEN = strFEN + nb_espace.ToString
                nb_espace = 0
            End If
            nb_espace = 0
            If ligne <> 1 Then strFEN = strFEN + "/"
        Next

        'rajoute les infos de aFEN pour les 5 autres champs
        With aFEN
            strFEN &= " "
            strFEN &= IIf(.ToPlay = ColorPiece.White, "w", "b")
            strFEN &= " "
            strFEN &= IIf(.WhiteCanDoLittle, "K", "")
            strFEN &= IIf(.WhiteCanDoBig, "Q", "")
            strFEN &= IIf(.BlackCanDoLittle, "k", "")
            strFEN &= IIf(.BlackCanDoBig, "q", "")
            'si aucun roque n'est possible
            If Not .BlackCanDoBig Then
                If Not .BlackCanDoLittle Then
                    If Not .WhiteCanDoBig Then
                        If Not .WhiteCanDoLittle Then
                            strFEN &= "-"
                        End If
                    End If
                End If
            End If
            'strFEN &= IIf(.BlackCanDoBig Or .BlackCanDoLittle Or .WhiteCanDoBig Or .WhiteCanDoLittle, "", "-")
            strFEN &= " "
            strFEN &= IIf(aFEN.sqiEnPassant <> 0, SquareName(aFEN.sqiEnPassant), "-")
            strFEN &= " "
            strFEN &= "0" 'TODO FAIRE FONCTIONNER aFEN.nbSinceLastPawn
            strFEN &= " "
            strFEN &= Math.Truncate(aFEN.nbMovesPlayed)
        End With

        Return strFEN

    End Function

    ''' <summary>
    ''' retourne la signature correspondant à l'état actuel
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRec() As String
        Dim colonne As Int16
        Dim ligne As Int16
        Dim sqi As Byte
        Dim oRec As Byte
        Dim strRec As String

        strRec = ""

        For colonne = 1 To 8
            oRec = 0
            For ligne = 1 To 8
                sqi = ligne * 10 + colonne
                If Board10x10(sqi) <> " " Then
                    oRec = oRec + 2 ^ (ligne - 1)
                End If
            Next
            strRec = strRec & oRec.ToString & "."
        Next
        strRec = strRec.Substring(0, strRec.Length - 1)

        Return strRec


    End Function

    ''' <summary>
    ''' retourne toutes les signatures possibles après le soulevement d'une pièce
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetRecs(ByVal strSquare As String) As String
        Dim strTempo As String
        Dim CanGo() As String
        Dim CanFrom() As String
        Dim TempoRec As String = ""
        Dim BackupFen As String
        Dim SanMove As String

        If strSquare.Length = 2 Then
            If ColorOf(SquareIndex(strSquare)) = aFEN.ToPlay Then
                strTempo = GetValidMoves(strSquare)
                If strTempo <> "" Then
                    CanGo = Split(strTempo, " ")
                    For i = 0 To CanGo.Count - 1
                        If CanGo(i).Substring(0, 1) = "x" Then
                            strTempo = CanGo(i).Substring(1, 2)
                        Else
                            strTempo = CanGo(i)
                        End If
                        BackupFen = GetFEN()
                        sanmove = PGNmove(strSquare & strTempo)
                        MakeMove(strSquare & strTempo)
                        TempoRec = TempoRec & "|" & GetRec() & " " & strSquare & strTempo & " " & sanmove
                        SetFEN(BackupFen)
                    Next
                    Return Trim(TempoRec.Substring(1))
                End If
                Return ""
            Else
                strTempo = WhoCanTake(strSquare)
                CanFrom = Split(strTempo, " ")
                If strTempo <> "" Then
                    For i = 0 To CanFrom.Count - 1
                        strTempo = CanFrom(i)

                        BackupFen = GetFEN()
                        sanmove = PGNmove(strSquare & strTempo)
                        MakeMove(strTempo & strSquare)
                        TempoRec = TempoRec & "|" & GetRec() & " " & strTempo & strSquare & " " & sanmove
                        SetFEN(BackupFen)

                    Next
                    Return Trim(TempoRec.Substring(1))
                End If
            End If

        End If

        Return ""
    End Function

    ''' <summary>
    ''' retourne toutes les signatures possibles après un coup legal
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetAllRecs() As String
        Dim tempo As String = ""

        For iSquare = 11 To 88
            If ColorOf(iSquare) = aFEN.ToPlay Then
                tempo = tempo & "|" & GetRecs(SquareName(iSquare))
            End If
        Next

        Return Trim(tempo.Substring(1))

    End Function

    '****************************************************************************
    '           REMPLIT VARIABLE aFEN A PARTIR D UN STRING FEN
    '----------------------------------------------------------------------------
    'CODE ERREUR
    '0 pas d'erreur
    '1 le FEN ne contient pas 6 champs
    '2 il n'y a pas w ou b
    '>10 CODE ERREUR DE TOBOARD
    ''' <summary>
    ''' Place la position à partir du FEN envoyé
    ''' </summary>
    ''' <param name="strFEN">notation FEN complète</param>
    ''' <returns>0 si aucune erreur</returns>
    ''' <remarks></remarks>
    Public Function SetFEN(ByVal strFEN As String) As Byte
        Dim lesChamps() As String

        lesChamps = strFEN.Split(" ")

        'Si on a pas 6 champs le FEN est erroné
        If lesChamps.Count <> 6 Then
            Return 1
        End If

        With ChampsDuFEN
            .c1Pieces = lesChamps(0)
            .c2AuTrait = lesChamps(1)
            .c3DroitRoque = lesChamps(2)
            .c4CaseEnPassant = lesChamps(3)
            .c5nbDemiCoup = lesChamps(4)
            .c6nbMouvement = lesChamps(5)

            'détermination du trait
            If .c2AuTrait = "w" Then
                aFEN.ToPlay = ColorPiece.White
            ElseIf .c2AuTrait = "b" Then
                aFEN.ToPlay = ColorPiece.Black
            Else
                Return 2
            End If

            'dertemination des droits au roque
            aFEN.WhiteCanDoLittle = .c3DroitRoque.Contains("K")
            aFEN.WhiteCanDoBig = .c3DroitRoque.Contains("Q")
            aFEN.BlackCanDoLittle = .c3DroitRoque.Contains("k")
            aFEN.BlackCanDoBig = .c3DroitRoque.Contains("q")

            'dertermination de la case e.p.
            If .c4CaseEnPassant <> "-" Then
                aFEN.sqiEnPassant = SquareIndex(.c4CaseEnPassant)
            Else
                aFEN.sqiEnPassant = 0
            End If

            'determnation du nombre de 1/2 coup depuis la derniere prise ????
            aFEN.nbSinceLastPawn = CInt(.c5nbDemiCoup)

            'determination du nombre de coup joué
            aFEN.nbMovesPlayed = CInt(.c6nbMouvement) + IIf(aFEN.ToPlay = ColorPiece.Black, 0.5, 0)


        End With

        Return ToBoard(ChampsDuFEN.c1Pieces)

    End Function


    '***************************************************************************************
    '**************************** FONCTIONS PUBLIC *****************************************
    '***************************************************************************************
    'effectue un mouvement sur le plateau
    'et retourne le FEN apres ce mouvement
    ''' <summary>
    ''' Effectue un mouvement de pièce 
    ''' sans vérifier la validité du mouvement
    ''' </summary>
    ''' <param name="FromTo">Chaine de type e2e4</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MakeMove(ByVal FromTo As String) As Byte
        Dim sqNameFROM As String
        Dim sqNameTO As String
        Dim sqiFROM As Int16
        Dim sqiTO As Int16
        Dim ThisRoque As Char = ""

        If FromTo.Length <> 4 Then
            Return 1
        End If

        'recoi un mouvement du type e2e4
        sqNameFROM = FromTo.Substring(0, 2)
        sqNameTO = FromTo.Substring(2, 2)

        sqiFROM = SquareIndex(sqNameFROM)  'converti e2 en 25
        sqiTO = SquareIndex(sqNameTO) 'converti e4 en 45

        '------------------- ROQUE ---------------------
        If IsARoque(FromTo, ThisRoque) Then
            MovesForRoque(ThisRoque)
            Return 0
        End If

        '------------------- EN PASSANT ----------------
        If IsEnPassant(sqiFROM, sqiTO) Then
            MovesForEnPassant(sqiFROM, sqiTO)
            Return 0
        End If

        '------------------- AJOUTE LA CASE EN PASSANT ----------------------
        If Math.Abs(sqiFROM - sqiTO) = 20 And UCase(Board10x10(sqiFROM)) = "P" Then   'si un pion avance de deux cases
            aFEN.sqiEnPassant = (sqiFROM + sqiTO) / 2   'la case entre les deux et la case ep. ;-)
        Else
            aFEN.sqiEnPassant = 0
        End If

        '------------------- AUGMENTE OU REMET A 0 LE NB -----------------------
        'TODO A FAIRE AUSSI EN CAS DE PRISE
        If UCase(Board10x10(sqiFROM)) = "P" _
            Or UCase(sqiTO) = "P" Then
            aFEN.nbSinceLastPawn = 0
        Else
            aFEN.nbSinceLastPawn += 1
        End If

        '------------------ DROIT DU ROQUE -----------------------------
        'une tour blanche bouge
        If Board10x10(sqiFROM) = "R" Then
            If SquareName(sqiFROM) = "h1" Then
                aFEN.WhiteCanDoLittle = False
            End If
            If SquareName(sqiFROM) = "a1" Then
                aFEN.WhiteCanDoBig = False
            End If
        End If
        'le roi blanc bouge
        If Board10x10(sqiFROM) = "K" Then
            aFEN.WhiteCanDoLittle = False
            aFEN.WhiteCanDoBig = False
        End If
        'une tour noire bouge
        If Board10x10(sqiFROM) = "r" Then
            If SquareName(sqiFROM) = "h8" Then
                aFEN.BlackCanDoLittle = False
            End If
            If SquareName(sqiFROM) = "a8" Then
                aFEN.BlackCanDoBig = False
            End If
        End If
        'le roi noir bouge
        If Board10x10(sqiFROM) = "k" Then
            aFEN.BlackCanDoLittle = False
            aFEN.BlackCanDoBig = False
        End If

        MovePiece(sqiFROM, sqiTO)
        SwitchToPlay()


        Return 0

    End Function

    ''' <summary>
    ''' Renvoie la liste des prises possibles pour la case
    ''' séparées par des espaces
    ''' </summary>
    ''' <param name="strSquare">le nom d'une case ex : e4</param>
    ''' <returns>liste des cases prenables</returns>
    ''' <remarks></remarks>
    Public Function GetTakes(ByVal strSquare As String) As String
        Dim tmp As String = ""
        Dim LesPrises() As String
        Dim i As Integer

        'initialise les variables avec la position

        tmp = GetMoves(strSquare)   'recupère l'ensemble des coups
        If tmp <> "" Then
            LesPrises = tmp.Split(" ")
            tmp = ""                            'variable pour stocker temporairement les prises

            For i = 0 To LesPrises.Count - 1
                If LesPrises(i).Substring(0, 1) = "x" Then      'si c'est une prise
                    tmp &= LesPrises(i).Substring(1, 2) & " "   'on ajoute la case sans le x à la liste
                End If
            Next
        End If
        Return Trim(tmp)
    End Function

    ''' <summary>
    ''' Retourne tous les deplacements possibles pour la case
    ''' séparées par des espaces
    ''' </summary>
    ''' <param name="strSquare">le nom d'une case ex : e4</param>
    ''' <returns>liste des cases atteingnables</returns>
    ''' <remarks>ATTENTION PAS DE VERIFICATION SUR LE JOUEUR AU TRAIT</remarks>
    Public Function GetMoves(ByVal strSquare As String) As String
        Dim sqi As Byte

        sqi = SquareIndex(strSquare)

        Select Case UCase(Board10x10(sqi))
            Case "Q"
                Return Qmoves(sqi)
            Case "K"
                Return Kmoves(sqi)
            Case "B"
                Return Bmoves(sqi)
            Case "N"
                Return Nmoves(sqi)
            Case "R"
                Return Rmoves(sqi)
            Case "P"
                Return Pmoves(sqi)
        End Select

        Return ""

    End Function


    ''' <summary>
    ''' Retourne tous les déplacements valides pour la case
    ''' séparées par des espaces
    ''' </summary>
    ''' <param name="strSquare">le nom d'une case ex : e4</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetValidMoves(ByVal strSquare As String) As String
        Dim AllMoves As String
        Dim aMove() As String
        Dim tmp As String = ""

        AllMoves = GetMoves(strSquare)

        If AllMoves.Length > 0 Then
            aMove = AllMoves.Split(" ")
            For i = 0 To aMove.Count - 1
                If aMove(i).Substring(0, 1) = "x" Then
                    If IsValidMove(strSquare & aMove(i).Substring(1, 2)) Then
                        tmp &= aMove(i) & " "
                    End If
                Else
                    If IsValidMove(strSquare & aMove(i)) Then
                        tmp &= aMove(i) & " "
                    End If
                End If

            Next

        End If

        Return Trim(tmp)

    End Function


    ''' <summary>
    ''' retourne la liste des pieces pouvant prendre cette piece
    ''' </summary>
    ''' <param name="strSquare"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WhoCanTake(ByVal strSquare As String) As String
        Dim sqi As Byte
        Dim FindColor As Byte
        Dim tmp As String = ""

        sqi = SquareIndex(strSquare)
        If sqi = 0 Then Return ""



        If Board10x10(sqi) = " " Then
            FindColor = ToNotPlay() 'on vérifie pour le roque
        Else
            FindColor = ColorOpponent(sqi)  'on cherche les pieces de la couleur adverse a celle ci
        End If




        For i = 11 To 88
            If ColorOf(i) = FindColor Then ' pour chaque piece adverse 
                If GetTakes(SquareName(i)).Contains(strSquare) Then
                    tmp &= SquareName(i) & " "
                End If
            End If
        Next

        Return Trim(tmp)

    End Function

    ''' <summary>
    ''' Cherche les cases contenant un type de piece
    ''' </summary>
    ''' <param name="aPiece">un caractère FEN d'une pièce (en)</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WhereIs(ByVal aPiece As Char) As String
        Dim tmp As String = ""
        For sqi = 11 To 88
            If Board10x10(sqi) = aPiece Then
                tmp &= SquareName(sqi) & " "
            End If
        Next
        Return Trim(tmp)
    End Function

    ''' <summary>
    ''' vérifie la validité d'un coup 
    ''' </summary>
    ''' <param name="FromTo">un mouvement du type e2e4</param>
    ''' <param name="CheckCheck">Doit on vérifier les échecs</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function IsValidMove(ByVal FromTo As String, Optional ByVal CheckCheck As Boolean = True) As Boolean
        Dim FromSquare As String
        Dim ToSquare As String
        Dim BackupPiece As Char
        Dim sqiFrom As Byte
        Dim sqiTo As Byte
        Dim sqKing As String
        Dim Threat As String
        Dim ThisRoque As String = ""
        Dim CheckSquare As String = ""

        'recoi un mouvement du type e2e4
        If FromTo.Length <> 4 Then
            Return False
        End If

        FromSquare = FromTo.Substring(0, 2)  'converti e2 en 25
        ToSquare = FromTo.Substring(2, 2)

        sqiFrom = SquareIndex(FromSquare)
        sqiTo = SquareIndex(ToSquare)

        If ColorOf(sqiFrom) <> aFEN.ToPlay Then
            Return False
        End If

        If Not CheckCheck Then Return GetMoves(FromSquare).Contains(ToSquare)

        If GetMoves(FromSquare).Contains(ToSquare) Then 'si on peut atteindre la case

            If IsARoque(FromTo, ThisRoque) Then         'si c'est un roque
                Select Case ThisRoque
                    Case "K"
                        CheckSquare = "f1"
                    Case "Q"
                        CheckSquare = "d1"
                    Case "k"
                        CheckSquare = "f8"
                    Case "q"
                        CheckSquare = "d8"

                End Select

                If Not IsValidMove(FromSquare & CheckSquare) Then   'la case traversé par le roi est controlée
                    Return False
                End If


            End If


            'on fait le changement et on regarde si on peut se faire capturer son roi
            BackupPiece = Board10x10(sqiTo)
            MovePiece(sqiFrom, sqiTo)

            'on cherche l'emplacement de son roi 
            If aFEN.ToPlay = ColorPiece.Black Then
                sqKing = WhereIs("k")
            Else
                sqKing = WhereIs("K")
            End If

            'y a t il une piece qui peut le capturer ?
            Threat = WhoCanTake(sqKing)

            If Threat = "" Then
                MovePiece(sqiTo, sqiFrom)
                Board10x10(sqiTo) = BackupPiece
                Return True
            Else
                MovePiece(sqiTo, sqiFrom)
                Board10x10(sqiTo) = BackupPiece
                Return False
            End If

        Else
            Return False
        End If

    End Function

    'renvoie la string PGN d'un coup avec les Cbd7
    ''' <summary>
    ''' retourne le coup en notation algébrique
    ''' </summary>
    ''' <param name="FromTo">un mouvement du type g1f3</param>
    ''' <returns>le mouvement sous forme Cf3</returns>
    ''' <remarks></remarks>
    Public Function PGNmove(ByVal FromTo As String) As String
        Dim LaPiece As Char
        Dim OtherIn As String
        Dim sqiFROM As Byte
        Dim sqiTO As Byte
        Dim AutrePeutAller As String
        Dim NameCase1 As String
        Dim NameCase2 As String
        Dim Colonne1 As Char
        Dim Colonne2 As Char
        Dim Ligne1 As Char
        Dim Ligne2 As Char
        Dim SpecificMove As String
        Dim DefaultMove As String
        Dim Threat As String
        Dim sqKing As String
        Dim BackupPiece As Char

        'recoi un mouvement du type e2e4
        NameCase1 = FromTo.Substring(0, 2)
        NameCase2 = FromTo.Substring(2, 2)

        sqiFROM = SquareIndex(NameCase1)  'converti e2 en 25
        sqiTO = SquareIndex(NameCase2) 'converti e4 en 45

        'place la position précedent le mouvement dans board
        LaPiece = Board10x10(sqiFROM)       'récupère la piece qui bouge

        SpecificMove = ""
        DefaultMove = ToLocal(LaPiece) & IIf(Board10x10(sqiTO) = " ", "", "x") & NameCase2

        Select Case UCase(LaPiece)          'en fonction du type de pièce

            Case "R", "N" 'si c'est un cavalier ou une tour   ***************************************************
                OtherIn = SquareName(FindOther(sqiFROM))                'cherche la position de l'autre
                If OtherIn <> "" Then                                 'si elle est bien présente
                    AutrePeutAller = GetMoves(OtherIn)              'récupère les mouvements possible de l'autre piece
                    If AutrePeutAller.Contains(NameCase2) Then  'si la case d'arrivé fait partie des cases atteingnables

                        Colonne1 = NameCase1.Substring(0, 1)
                        Ligne1 = NameCase1.Substring(1, 1)

                        Colonne2 = OtherIn.Substring(0, 1)
                        Ligne2 = OtherIn.Substring(1, 1)

                        If Colonne1 <> Colonne2 Then
                            'si les deux pieces ne sont pas sur les memes colonnes ex : Cbd7
                            SpecificMove = ToLocal(LaPiece) & Colonne1 & IIf(Board10x10(sqiTO) = " ", "", "x") & NameCase2
                        Else
                            'sinon il faut rajouter la ligne ex: T8e4
                            SpecificMove = ToLocal(LaPiece) & Ligne1 & IIf(Board10x10(sqiTO) = " ", "", "x") & NameCase2
                        End If
                    End If
                End If

            Case "P"                                                    ' SI C EST UN PION *********************************************************************
                If Board10x10(sqiTO) = " " Then
                    'gérer la prise en passant
                    If Board10x10(sqiTO) = " " Then
                        If IsEnPassant(sqiFROM, sqiTO) Then
                            SpecificMove = NameCase1.Substring(0, 1) & "x" & NameCase2 & " ep"
                        Else
                            'SUPPRIMER LA CASE DE DEPART EN CAS DE POUSSE DE PION 
                            SpecificMove = NameCase2
                        End If
                    End If

                Else
                    'AJOUTER LA COLONNE DE DEPART EN CASE DE PRISE DE PION
                    SpecificMove = NameCase1.Substring(0, 1) & "x" & NameCase2
                End If

            Case "K"                                                    ' SI C EST UN ROI ***********************************************************************
                'VERIFIER QUE CE N EST PAS UN ROQUE
                Select Case FromTo
                    Case "e8g8", "e1g1"
                        SpecificMove = "O-O"
                    Case "e8c8", "e1c1"
                        SpecificMove = "O-O-O"
                End Select
        End Select

        BackupPiece = Board10x10(sqiTO)
        MovePiece(sqiFROM, sqiTO)

        'Rajoutons un + en cas d'échecs
        If aFEN.ToPlay = ColorPiece.White Then
            sqKing = WhereIs("k")
        Else
            sqKing = WhereIs("K")
        End If

        Threat = WhoCanTake(sqKing)

        If Threat <> "" Then
            If SpecificMove <> "" Then SpecificMove &= "+"
            DefaultMove &= "+"
        End If

        MovePiece(sqiTO, sqiFROM)
        Board10x10(sqiTO) = BackupPiece

        If SpecificMove = "" Then
            Return DefaultMove
        Else
            Return SpecificMove
        End If

    End Function

    ''' <summary>
    ''' retourne dans quel coup on se trouve
    ''' commence à 1 avant que les blancs ne jouent
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function MovesPlayed() As UInteger
        Return Math.Truncate(aFEN.nbMovesPlayed)
    End Function

    ''' <summary>
    ''' retourne TRUE si c'est au blanc de jouer
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function WhiteToPlay() As Boolean
        Return aFEN.ToPlay = ColorPiece.White
    End Function


#End Region

#Region "les différentes Property"

    ''' <summary>
    ''' initiales des pièces
    ''' TCFDQ (fr) ou RNBQK (en)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property LocalPiece As String
        Get
            Return _LocalPIECE
        End Get
        Set(ByVal value As String)
            If value.Length = 5 Then
                _LocalPIECE = value
            End If

        End Set
    End Property



#End Region


End Class
