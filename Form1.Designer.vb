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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.mnGetRec = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnGetMoves = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem3 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem4 = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem5 = New System.Windows.Forms.ToolStripMenuItem()
        Me.pbReduire = New System.Windows.Forms.PictureBox()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.mnFrm = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.GetRecToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetMovesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GetFenToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.lvMoves = New System.Windows.Forms.ListView()
        Me.chCoup = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chWhite = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.chBlack = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.TabPage3 = New System.Windows.Forms.TabPage()
        Me.GetAllRecToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        CType(Me.pbReduire, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.mnFrm.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.SuspendLayout()
        '
        'mnGetRec
        '
        Me.mnGetRec.Name = "mnGetRec"
        Me.mnGetRec.Size = New System.Drawing.Size(183, 22)
        Me.mnGetRec.Text = "GetRec"
        '
        'mnGetMoves
        '
        Me.mnGetMoves.Name = "mnGetMoves"
        Me.mnGetMoves.Size = New System.Drawing.Size(183, 22)
        Me.mnGetMoves.Text = "GetMoves"
        '
        'ToolStripMenuItem3
        '
        Me.ToolStripMenuItem3.Name = "ToolStripMenuItem3"
        Me.ToolStripMenuItem3.Size = New System.Drawing.Size(183, 22)
        Me.ToolStripMenuItem3.Text = "ToolStripMenuItem3"
        '
        'ToolStripMenuItem4
        '
        Me.ToolStripMenuItem4.Name = "ToolStripMenuItem4"
        Me.ToolStripMenuItem4.Size = New System.Drawing.Size(183, 22)
        Me.ToolStripMenuItem4.Text = "ToolStripMenuItem4"
        '
        'ToolStripMenuItem5
        '
        Me.ToolStripMenuItem5.Name = "ToolStripMenuItem5"
        Me.ToolStripMenuItem5.Size = New System.Drawing.Size(183, 22)
        Me.ToolStripMenuItem5.Text = "ToolStripMenuItem5"
        '
        'pbReduire
        '
        Me.pbReduire.BackgroundImage = Global.TestGraphic.My.Resources.Resources.reduire0
        Me.pbReduire.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center
        Me.pbReduire.Location = New System.Drawing.Point(832, 37)
        Me.pbReduire.Name = "pbReduire"
        Me.pbReduire.Size = New System.Drawing.Size(26, 17)
        Me.pbReduire.TabIndex = 12
        Me.pbReduire.TabStop = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Cursor = System.Windows.Forms.Cursors.Hand
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(12, 12)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(444, 447)
        Me.PictureBox1.TabIndex = 0
        Me.PictureBox1.TabStop = False
        '
        'mnFrm
        '
        Me.mnFrm.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GetRecToolStripMenuItem, Me.GetAllRecToolStripMenuItem, Me.GetMovesToolStripMenuItem, Me.GetFenToolStripMenuItem})
        Me.mnFrm.Name = "mnFrm"
        Me.mnFrm.Size = New System.Drawing.Size(153, 114)
        '
        'GetRecToolStripMenuItem
        '
        Me.GetRecToolStripMenuItem.Name = "GetRecToolStripMenuItem"
        Me.GetRecToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.GetRecToolStripMenuItem.Text = "GetRec"
        '
        'GetMovesToolStripMenuItem
        '
        Me.GetMovesToolStripMenuItem.Name = "GetMovesToolStripMenuItem"
        Me.GetMovesToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.GetMovesToolStripMenuItem.Text = "GetMoves"
        '
        'GetFenToolStripMenuItem
        '
        Me.GetFenToolStripMenuItem.Name = "GetFenToolStripMenuItem"
        Me.GetFenToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.GetFenToolStripMenuItem.Text = "GetFen"
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Location = New System.Drawing.Point(484, 12)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(323, 352)
        Me.TabControl1.TabIndex = 14
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.lvMoves)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(315, 326)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Formulaire"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'lvMoves
        '
        Me.lvMoves.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chCoup, Me.chWhite, Me.chBlack})
        Me.lvMoves.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lvMoves.Font = New System.Drawing.Font("Microsoft Sans Serif", 15.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lvMoves.FullRowSelect = True
        Me.lvMoves.GridLines = True
        Me.lvMoves.HideSelection = False
        Me.lvMoves.Location = New System.Drawing.Point(3, 3)
        Me.lvMoves.MultiSelect = False
        Me.lvMoves.Name = "lvMoves"
        Me.lvMoves.Size = New System.Drawing.Size(309, 320)
        Me.lvMoves.TabIndex = 14
        Me.lvMoves.UseCompatibleStateImageBehavior = False
        Me.lvMoves.View = System.Windows.Forms.View.Details
        '
        'chCoup
        '
        Me.chCoup.Text = "n°"
        Me.chCoup.Width = 40
        '
        'chWhite
        '
        Me.chWhite.Text = "White"
        Me.chWhite.Width = 100
        '
        'chBlack
        '
        Me.chBlack.Text = "Black"
        Me.chBlack.Width = 100
        '
        'TabPage2
        '
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(315, 326)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "TabPage2"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(315, 326)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "TabPage3"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'GetAllRecToolStripMenuItem
        '
        Me.GetAllRecToolStripMenuItem.Name = "GetAllRecToolStripMenuItem"
        Me.GetAllRecToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
        Me.GetAllRecToolStripMenuItem.Text = "Get All Rec"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(910, 530)
        Me.Controls.Add(Me.pbReduire)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.PictureBox1)
        Me.MinimumSize = New System.Drawing.Size(768, 480)
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "ChessboARDuino VB"
        CType(Me.pbReduire, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.mnFrm.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
    Friend WithEvents pbReduire As System.Windows.Forms.PictureBox
    Friend WithEvents mnGetRec As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnGetMoves As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem3 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem4 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem5 As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents mnFrm As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents GetRecToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GetMovesToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents lvMoves As System.Windows.Forms.ListView
    Friend WithEvents chCoup As System.Windows.Forms.ColumnHeader
    Friend WithEvents chWhite As System.Windows.Forms.ColumnHeader
    Friend WithEvents chBlack As System.Windows.Forms.ColumnHeader
    Friend WithEvents GetFenToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents GetAllRecToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem

End Class
