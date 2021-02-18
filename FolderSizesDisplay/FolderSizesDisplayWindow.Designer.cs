//#define MODE_RECHERCHE

namespace FolderSizesDisplay
{
	partial class FolderSizesDisplayWindow
	{
		/// <summary>
		/// Variable nécessaire au concepteur.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Nettoyage des ressources utilisées.
		/// </summary>
		/// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Code généré par le Concepteur Windows Form

		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.GUI_treeView = new System.Windows.Forms.TreeView();
			this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
			this.contextMenuStrip_dossier = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ouvrirDossierToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.triParNomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.triParTailleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_fichier = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.ouvrirFichierToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.ouvrirDossierContenantToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.contextMenuStrip_multi = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.triParNomToolStripMenuItemMulti = new System.Windows.Forms.ToolStripMenuItem();
			this.triParTailleToolStripMenuItemMulti = new System.Windows.Forms.ToolStripMenuItem();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.GUI_progressBar_invisible = new System.Windows.Forms.ProgressBar();
			this.contextMenuStrip_dossier.SuspendLayout();
			this.contextMenuStrip_fichier.SuspendLayout();
			this.contextMenuStrip_multi.SuspendLayout();
			this.SuspendLayout();
			// 
			// GUI_treeView
			// 
			this.GUI_treeView.AllowDrop = true;
			this.GUI_treeView.BackColor = System.Drawing.Color.White;
			this.GUI_treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.GUI_treeView.Location = new System.Drawing.Point(0, 0);
			this.GUI_treeView.Name = "GUI_treeView";
			this.GUI_treeView.ShowNodeToolTips = true;
			this.GUI_treeView.Size = new System.Drawing.Size(392, 273);
			this.GUI_treeView.TabIndex = 0;
			this.GUI_treeView.DragDrop += new System.Windows.Forms.DragEventHandler(this.treeView1_DragDrop);
			this.GUI_treeView.DragEnter += new System.Windows.Forms.DragEventHandler(this.treeView1_DragEnter);
			this.GUI_treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
			this.GUI_treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GUI_treeView_KeyDown);
			// 
			// backgroundWorker1
			// 
			this.backgroundWorker1.WorkerReportsProgress = true;
			this.backgroundWorker1.WorkerSupportsCancellation = true;
			this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
			this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
			this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
			// 
			// contextMenuStrip_dossier
			// 
			this.contextMenuStrip_dossier.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ouvrirDossierToolStripMenuItem1,
            this.toolStripSeparator1,
            this.triParNomToolStripMenuItem,
            this.triParTailleToolStripMenuItem});
			this.contextMenuStrip_dossier.Name = "contextMenuStrip_dossier";
			this.contextMenuStrip_dossier.Size = new System.Drawing.Size(182, 76);
			this.contextMenuStrip_dossier.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_dossier_Opening);
			// 
			// ouvrirDossierToolStripMenuItem1
			// 
			this.ouvrirDossierToolStripMenuItem1.Name = "ouvrirDossierToolStripMenuItem1";
			this.ouvrirDossierToolStripMenuItem1.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.ouvrirDossierToolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
			this.ouvrirDossierToolStripMenuItem1.Text = "Ouvrir dossier";
			this.ouvrirDossierToolStripMenuItem1.Click += new System.EventHandler(this.ouvrirDossierToolStripMenuItem1_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
			// 
			// triParNomToolStripMenuItem
			// 
			this.triParNomToolStripMenuItem.Name = "triParNomToolStripMenuItem";
			this.triParNomToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.triParNomToolStripMenuItem.Text = "Tri par nom";
			this.triParNomToolStripMenuItem.Click += new System.EventHandler(this.triParNomToolStripMenuItem_Click);
			// 
			// triParTailleToolStripMenuItem
			// 
			this.triParTailleToolStripMenuItem.Name = "triParTailleToolStripMenuItem";
			this.triParTailleToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.triParTailleToolStripMenuItem.Text = "Tri par taille";
			this.triParTailleToolStripMenuItem.Click += new System.EventHandler(this.triParTailleToolStripMenuItem_Click);
			// 
			// contextMenuStrip_fichier
			// 
			this.contextMenuStrip_fichier.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ouvrirFichierToolStripMenuItem,
            this.ouvrirDossierContenantToolStripMenuItem});
			this.contextMenuStrip_fichier.Name = "contextMenuStrip_fichier";
			this.contextMenuStrip_fichier.Size = new System.Drawing.Size(234, 48);
			// 
			// ouvrirFichierToolStripMenuItem
			// 
			this.ouvrirFichierToolStripMenuItem.ForeColor = System.Drawing.SystemColors.GrayText;
			this.ouvrirFichierToolStripMenuItem.Name = "ouvrirFichierToolStripMenuItem";
			this.ouvrirFichierToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.ouvrirFichierToolStripMenuItem.Text = "Ouvrir fichier";
			this.ouvrirFichierToolStripMenuItem.Click += new System.EventHandler(this.ouvrirFichierToolStripMenuItem1_Click);
			// 
			// ouvrirDossierContenantToolStripMenuItem
			// 
			this.ouvrirDossierContenantToolStripMenuItem.Name = "ouvrirDossierContenantToolStripMenuItem";
			this.ouvrirDossierContenantToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.ouvrirDossierContenantToolStripMenuItem.Size = new System.Drawing.Size(233, 22);
			this.ouvrirDossierContenantToolStripMenuItem.Text = "Ouvrir dossier contenant";
			this.ouvrirDossierContenantToolStripMenuItem.Click += new System.EventHandler(this.ouvrirDossierContenantToolStripMenuItem_Click);
			// 
			// contextMenuStrip_multi
			// 
			this.contextMenuStrip_multi.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.triParNomToolStripMenuItemMulti,
            this.triParTailleToolStripMenuItemMulti});
			this.contextMenuStrip_multi.Name = "contextMenuStrip_dossier";
			this.contextMenuStrip_multi.Size = new System.Drawing.Size(131, 48);
			this.contextMenuStrip_multi.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_multi_Opening);
			// 
			// triParNomToolStripMenuItemMulti
			// 
			this.triParNomToolStripMenuItemMulti.Name = "triParNomToolStripMenuItemMulti";
			this.triParNomToolStripMenuItemMulti.Size = new System.Drawing.Size(130, 22);
			this.triParNomToolStripMenuItemMulti.Text = "Tri par nom";
			this.triParNomToolStripMenuItemMulti.Click += new System.EventHandler(this.triParNomToolStripMenuItem_Click);
			// 
			// triParTailleToolStripMenuItemMulti
			// 
			this.triParTailleToolStripMenuItemMulti.Name = "triParTailleToolStripMenuItemMulti";
			this.triParTailleToolStripMenuItemMulti.Size = new System.Drawing.Size(130, 22);
			this.triParTailleToolStripMenuItemMulti.Text = "Tri par taille";
			this.triParTailleToolStripMenuItemMulti.Click += new System.EventHandler(this.triParTailleToolStripMenuItem_Click);
			// 
			// GUI_progressBar_invisible
			// 
			this.GUI_progressBar_invisible.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.GUI_progressBar_invisible.Location = new System.Drawing.Point(0, 250);
			this.GUI_progressBar_invisible.Name = "GUI_progressBar_invisible";
			this.GUI_progressBar_invisible.Size = new System.Drawing.Size(392, 23);
			this.GUI_progressBar_invisible.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.GUI_progressBar_invisible.TabIndex = 3;
			this.GUI_progressBar_invisible.Visible = false;
			// 
			// FolderSizesDisplayWindow
			// 
			this.AllowDrop = true;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(392, 273);
			this.Controls.Add(this.GUI_progressBar_invisible);
			this.Controls.Add(this.GUI_treeView);
			this.Name = "FolderSizesDisplayWindow";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Folders sizes display";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.contextMenuStrip_dossier.ResumeLayout(false);
			this.contextMenuStrip_fichier.ResumeLayout(false);
			this.contextMenuStrip_multi.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TreeView GUI_treeView;
		private System.ComponentModel.BackgroundWorker backgroundWorker1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_dossier;
		private System.Windows.Forms.ToolStripMenuItem ouvrirDossierToolStripMenuItem1;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_fichier;
		private System.Windows.Forms.ToolStripMenuItem ouvrirFichierToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ouvrirDossierContenantToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem triParNomToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem triParTailleToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip_multi;
		private System.Windows.Forms.ToolStripMenuItem triParNomToolStripMenuItemMulti;
		private System.Windows.Forms.ToolStripMenuItem triParTailleToolStripMenuItemMulti;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ProgressBar GUI_progressBar_invisible;
	}
}

