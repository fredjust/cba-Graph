Public Class frmComment

    Dim resizeForm As Boolean
    Dim pos1 As Point
    Dim pos2 As Point
    Dim InitPos As New Rectangle
    Dim DiffPos As New Rectangle

    Dim xd, yd As Integer

    Private Sub frmComment_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated

        xd = frmMain.Width - frmMain.ClientRectangle.Width + 2
        yd = frmMain.Height - frmMain.ClientRectangle.Height + 2
        With Me
            .Top = frmMain.Location.Y + yd + frmMain.TheScreenBoard.size_border + frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.Y
            .Left = frmMain.Location.X + xd + frmMain.TheScreenBoard.size_border + frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.X
            .Width = frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.Width
            .Height = frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.Height
        End With
        If frmMain.TheScreenBoard.str_Comment_Over <> "" Then
            txtComment.Text = frmMain.TheScreenBoard.str_Comment_Over.Replace("\n", vbCrLf)
        End If
    End Sub






    Private Sub frmComment_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDown

        If e.X > Me.Width - frmMain.TheScreenBoard.size_square / 4 And e.Y > Me.Height - frmMain.TheScreenBoard.size_square / 4 Then
            resizeForm = True
        Else
            resizeForm = False
        End If

        pos1 = Cursor.Position
    End Sub

    Private Sub frmComment_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseMove
        Dim NewPos As Point

        If e.Button = Windows.Forms.MouseButtons.Left Then

            pos2 = Cursor.Position

            If resizeForm Then
                Me.Width += (pos2.X - pos1.X)
                Me.Height += (pos2.Y - pos1.Y)
            Else
                NewPos.X = Location.X + (pos2.X - pos1.X)
                NewPos.Y = Location.Y + (pos2.Y - pos1.Y)
                Location = NewPos
            End If

        End If

        If e.X > Me.Width - frmMain.TheScreenBoard.size_square / 4 And e.Y > Me.Height - frmMain.TheScreenBoard.size_square / 4 Then 'si on est dans la case a8
            Cursor = Cursors.SizeNWSE
        Else
            Cursor = Cursors.Default
        End If


        pos1 = Cursor.Position
    End Sub

    Private Sub frmComment_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseUp
        'If e.X > Me.Width * 0.875 And e.Y < Me.Height * 0.125 Then 'si on est dans la case a8
        '    Me.Close()
        'End If

        InitPos.Y = frmMain.Location.Y + yd + frmMain.TheScreenBoard.size_border + frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.Y
        InitPos.X = frmMain.Location.X + xd + frmMain.TheScreenBoard.size_border + frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.X
        InitPos.Width = frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.Width
        InitPos.Height = frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.Height

        'DiffPos.X = (Me.Left - InitPos.X - frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        'DiffPos.Y = (Me.Top - InitPos.Y - frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        'DiffPos.Width = (Me.Width - InitPos.Width + frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        'DiffPos.Height = (Me.Height - InitPos.Height + frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square


        If (Me.Left - InitPos.X) < 0 Then
            DiffPos.X = (Me.Left - InitPos.X - frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        Else
            DiffPos.X = (Me.Left - InitPos.X + frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        End If

        If (Me.Top - InitPos.Y) < 0 Then
            DiffPos.Y = (Me.Top - InitPos.Y - frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        Else
            DiffPos.Y = (Me.Top - InitPos.Y + frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        End If

        If (Me.Width - InitPos.Width) < 0 Then
            DiffPos.Width = (Me.Width - InitPos.Width - frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        Else
            DiffPos.Width = (Me.Width - InitPos.Width + frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        End If

        If (Me.Height - InitPos.Height) < 0 Then
            DiffPos.Height = (Me.Height - InitPos.Height - frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        Else
            DiffPos.Height = (Me.Height - InitPos.Height + frmMain.TheScreenBoard.size_square / 2) \ frmMain.TheScreenBoard.size_square
        End If

        With frmMain.TheScreenBoard.InputBoxRect
            .X += DiffPos.X
            .Y += DiffPos.Y
            .Width += DiffPos.Width
            .Height += DiffPos.Height
            If .X < 0 Then .X = 0
            If .Y < 0 Then .Y = 0

            If .X + .Width > 8 Then
                .X = 8 - .Width
            End If

            If .Y + .Height > 8 Then
                .Y = 8 - .Height
            End If

            If .Height < 1 Then .Height = 1
            If .Width < 1 Then .Width = 1
        End With

        With Me
            .Top = frmMain.Location.Y + yd + frmMain.TheScreenBoard.size_border + frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.Y
            .Left = frmMain.Location.X + xd + frmMain.TheScreenBoard.size_border + frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.X
            .Width = frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.Width
            .Height = frmMain.TheScreenBoard.size_square * frmMain.TheScreenBoard.InputBoxRect.Height
        End With

    End Sub

    Private Sub txtComment_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles txtComment.MouseWheel
        Dim SizeFont As Integer
        SizeFont = txtComment.Font.Size
        SizeFont += e.Delta * SystemInformation.MouseWheelScrollLines / 360
        txtComment.Font = New Font("Tahoma", SizeFont)
    End Sub


    Private Sub frmComment_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        With txtComment
            .Top = frmMain.TheScreenBoard.size_square / 4
            .Left = frmMain.TheScreenBoard.size_square / 4
            .Width = Me.Width - frmMain.TheScreenBoard.size_square / 2
            .Height = Me.Height - frmMain.TheScreenBoard.size_square / 2
        End With
        With lblClose
            .Top = 0
            .Left = txtComment.Left + txtComment.Width - 1
            .Width = (Me.Width - txtComment.Width) / 2 + 1
            .Height = (Me.Height - txtComment.Height) / 2 + 1
        End With
    End Sub

    Private Sub lblClose_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblClose.Click
        Me.Close()
    End Sub

    Private Sub frmComment_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub frmComment_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = 27 Then
            frmMain.BoardPos.AddComment(frmMain.TheScreenBoard.Coord2name(), txtComment.Text.Replace(vbCrLf, "\n"))
            frmMain.update_Pos_LV()
            Me.Close()
        End If
    End Sub
End Class