﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
    Inherits System.Windows.Forms.Form

    'Form remplace la méthode Dispose pour nettoyer la liste des composants.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Requise par le Concepteur Windows Form
    Private components As System.ComponentModel.IContainer

    'REMARQUE : la procédure suivante est requise par le Concepteur Windows Form
    'Elle peut être modifiée à l'aide du Concepteur Windows Form.  
    'Ne la modifiez pas à l'aide de l'éditeur de code.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.pbReduire = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.lvMoves = New System.Windows.Forms.ListView()
        Me.chCoup = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chWhite = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chBlack = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        CType(Me.pbReduire, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'pbReduire
        '
        Me.pbReduire.BackgroundImage = Global.TestGraphic.My.Resources.Resources.reduire0
        Me.pbReduire.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.pbReduire.Location = New System.Drawing.Point(864, 12)
        Me.pbReduire.Name = "pbReduire"
        Me.pbReduire.Size = New System.Drawing.Size(34, 28)
        Me.pbReduire.TabIndex = 12
        Me.pbReduire.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(444, 447)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'lvMoves
        '
        Me.lvMoves.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chCoup, Me.chWhite, Me.chBlack})
        Me.lvMoves.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lvMoves.FullRowSelect = True
        Me.lvMoves.GridLines = True
        Me.lvMoves.HideSelection = False
        Me.lvMoves.Location = New System.Drawing.Point(484, 12)
        Me.lvMoves.MultiSelect = False
        Me.lvMoves.Name = "lvMoves"
        Me.lvMoves.Size = New System.Drawing.Size(214, 332)
        Me.lvMoves.TabIndex = 13
        Me.lvMoves.UseCompatibleStateImageBehavior = False
        Me.lvMoves.View = System.Windows.Forms.View.Details
        '
        'chCoup
        '
        Me.chCoup.Text = "n°"
        '
        'chWhite
        '
        Me.chWhite.Text = "White"
        '
        'chBlack
        '
        Me.chBlack.Text = "Black"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(910, 530)
        Me.Controls.Add(Me.lvMoves)
        Me.Controls.Add(Me.pbReduire)
        Me.Controls.Add(Me.PictureBox1)
        Me.MinimumSize = New System.Drawing.Size(768, 480)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "PGN Export"
        CType(Me.pbReduire, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents pbReduire As System.Windows.Forms.PictureBox
    Friend WithEvents lvMoves As System.Windows.Forms.ListView
    Friend WithEvents chCoup As System.Windows.Forms.ColumnHeader
    Friend WithEvents chWhite As System.Windows.Forms.ColumnHeader
    Friend WithEvents chBlack As System.Windows.Forms.ColumnHeader

End Class
