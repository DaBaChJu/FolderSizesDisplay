#define ICONES_SUR_TREEVIEW //activer simultanément dans Tools.cs pour pouvoir avoir les icônes
#define MODE_RECHERCHE

#if MODE_RECHERCHE 

/* 
 * Initialisation des composants graphiques dans InitialiseComposantsRecherche()
 * Cela à cause des menus spécifiques créés pour la recherche je crois.
 * A revoir plus tard peut-être.
 * 
*/

#define AJOUT_CUMULATIF

#define MODE_NB_ELEMENTS //activer simultanément dans Tools.cs

#endif

///
/// TODO:
/// 

/// MODE_NB_ELEMENTS enregistre le nombre de fichiers.
/// Maintenant il faut créer le mode secondaire où ces
/// données sont utilisées, et mettre à jour les tris et
/// les recherches pour se baser dessus.

using System;
using System.IO;
using System.Windows.Forms;

namespace FolderSizesDisplay
{

	/// <summary>
	/// Type de tri pour les sous-noeuds d'un
	/// TreeNode.
	/// </summary>
	public enum TreeNodeTypeTri
	{
		tries_par_nom,
		tries_par_nom_inverse,
		tries_par_taille,
		tries_par_taille_inverse
	}

	/// <summary>
	/// Structure stockée dans le Tag de
	/// type Object d'un TreeNode pour 
	/// contenir des informations multiples.
	/// </summary>
	public struct TreeNodeTag
	{
		public Int64 taille;
#if MODE_NB_ELEMENTS  
		public Int64 nb_elem;
#endif
		public TreeNodeTypeTri tri;
	}

#if ICONES_SUR_TREEVIEW
	enum IconIndex : int
	{
		etoile=0,
		fichier=1,
		dossier_ferme=2,
		dossier_ouvert=3		
	}

#endif

	public partial class FolderSizesDisplayWindow : Form
	{

		#region Variables locales

		/// <summary>
		/// Variable stockant le noeud sujet d'un clic (généralement
		/// pour usage par les menus contextuels).
		/// </summary>
		private TreeNode noeud_cible_menu_contextuel = null;
	
#if MODE_RECHERCHE
		/// <summary>
		/// Variable stockant le noeud à partir duquel une
		/// recherche est effectuée (car le "noeud sélectionné"
		/// varie en cours de recherche).
		/// </summary>
		private TreeNode noeud_racine_recherche = null; 
#endif

		/// <summary>
		/// Nombre de dossiers dont tout les sous-dossiers
		/// ont été enregistrés.
		/// </summary>
		private int nombre_dossiers_traites = 0;

		/// <summary>
		/// Nombre de dossiers restants à traiter.
		/// </summary>
		private int nombre_dossiers_detectes = 0;

		/// <summary>
		/// Active l'affichage des exceptions dans des
		/// MessageBox si -exc est utilisé.
		/// </summary>
		private bool bDisplayExceptionsMBOX = false;

		/// <summary>
		/// Active le log des exceptions si 
		/// -logexc est utilisé.
		/// </summary>
		private bool bLogExceptions = false;

		/// <summary>
		/// Chemin du fichier de log où sont écrites
		/// les exceptions si -logexc est utilisé
		/// et fonctionne.
		/// </summary>
		private string strExceptionsLogFilename = null;

		/// <summary>
		/// StreamWriter qui écrit les exceptions dans
		/// un log.Seulement créé lorsque l'exception
		/// survient puisque ça devrait être rare.
		/// </summary>
		System.IO.StreamWriter swExceptionsLog = null;

		/// <summary>
		/// Enregistrment que l'utilisateur a confirmé
		/// vouloir ouvrir des fichiers depuis l'application.
		/// </summary>
		private bool bAutoriseOuvertureFichier = false;

		#endregion Variables locales

		#region Variables pour composants interface optionnels

#if MODE_RECHERCHE

		#region Elements extraits du Designer.cs

		System.Windows.Forms.ToolStripSeparator toolStripSeparator_menu_dossier = new ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem rechercheToolStripMenuItem_dossier = new ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator_menu_multi = new ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem rechercheToolStripMenuItem_multi = new ToolStripMenuItem();
		System.Windows.Forms.Panel GUI_panel_recherche = new Panel();
		System.Windows.Forms.TextBox GUI_textBox_recherche = new TextBox();
		System.Windows.Forms.Button GUI_button_recherche_ok = new Button();

		#endregion Elements extraits du Designer.cs

		#region Recherche depuis Parent (ajouts manuel après extraction)

		System.Windows.Forms.ToolStripMenuItem rechercheDepuisParentToolStripMenuItem = new ToolStripMenuItem();

		#endregion

#endif

#if AJOUT_CUMULATIF

		System.Windows.Forms.ToolStripSeparator toolStripSeparator_dossier_ajout = new ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem modeAjout_dossierToolStripMenuItem = new ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator_multi_ajout = new ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem modeAjout_multiToolStripMenuItem = new ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator_fichier_ajout = new ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem modeAjout_fichierToolStripMenuItem = new ToolStripMenuItem();

#endif

#if MODE_NB_ELEMENTS

		System.Windows.Forms.ToolStripSeparator toolStripSeparator_dossier_nbelem = new ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem modeNbelem_dossierToolStripMenuItem = new ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator_multi_nbelem = new ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem modeNbelem_multiToolStripMenuItem = new ToolStripMenuItem();
		System.Windows.Forms.ToolStripSeparator toolStripSeparator_fichier_nbelem = new ToolStripSeparator();
		System.Windows.Forms.ToolStripMenuItem modeNbelem_fichierToolStripMenuItem = new ToolStripMenuItem();

#endif

		#endregion Variables pour composants interface optionnels

		#region Constructeurs

		/// <summary>
		/// Constructeur récupérant des arguments de ligne
		/// de commande
		/// </summary>
		/// <param name="args">Arguments de ligne de commande
		/// de l'application.</param>
		public FolderSizesDisplayWindow(string[] args)
		{

			InitializeComponent();

#if MODE_RECHERCHE
			InitialiseComposantsRecherche();
#endif
#if ICONES_SUR_TREEVIEW
			InitialiseImagesTreeView(); 
#endif

			System.Collections.Generic.List<string> dossiers_ajoutes_au_demarrage = new System.Collections.Generic.List<string>(args.Length);

			foreach (string argument in args)
			{

				#region Gestion argument -exc pour afficher mbox avec les exceptions
				if (argument == "-exc")
					bDisplayExceptionsMBOX = true;
				#endregion Gestion argument -exc pour afficher mbox avec les exceptions
				else
				#region Gestion argument -logexc pour logger les exceptions

				if (argument == "-logexc")
				{

					strExceptionsLogFilename = Application.StartupPath;

					if (!strExceptionsLogFilename.EndsWith(Path.DirectorySeparatorChar.ToString()))
						strExceptionsLogFilename = strExceptionsLogFilename + Path.DirectorySeparatorChar + "FolderSizesDisplay.log";
					else
						strExceptionsLogFilename = strExceptionsLogFilename + "FolderSizesDisplay.log";

					try
					{

						DateTime date_courante = DateTime.Now;

						swExceptionsLog = File.AppendText(strExceptionsLogFilename);
						swExceptionsLog.Write(Environment.NewLine +
							"Démarré le " +
							string.Format("{0:00}/{1:00}/{2}", date_courante.Day, date_courante.Month, date_courante.Year) + " @ " +
							string.Format("{0:00}:{1:00}:{2:00}", date_courante.Hour, date_courante.Minute, date_courante.Second) +
							Environment.NewLine);
						swExceptionsLog.Close();

						bLogExceptions = true;

					}
					catch (Exception) { swExceptionsLog.Close(); }

					if (!bLogExceptions)
					{

						strExceptionsLogFilename = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

						if (!strExceptionsLogFilename.EndsWith(Path.DirectorySeparatorChar.ToString()))
							strExceptionsLogFilename = strExceptionsLogFilename + Path.DirectorySeparatorChar + "FolderSizesDisplay.log";
						else
							strExceptionsLogFilename = strExceptionsLogFilename + "FolderSizesDisplay.log";

						try
						{

							DateTime date_courante = DateTime.Now;

							swExceptionsLog = File.AppendText(strExceptionsLogFilename);
							swExceptionsLog.Write(Environment.NewLine +
								"Démarré le " +
								string.Format("{0:00}/{1:00}/{2}", date_courante.Day, date_courante.Month, date_courante.Year) + " à " +
								string.Format("{0:00}:{1:00}:{2:00}", date_courante.Hour, date_courante.Minute, date_courante.Second) +
								Environment.NewLine);
							swExceptionsLog.Close();

							bLogExceptions = true;

						}
						catch (Exception) { swExceptionsLog.Close(); }

					}

					if (!bLogExceptions)
					{

						strExceptionsLogFilename = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

						if (!strExceptionsLogFilename.EndsWith(Path.DirectorySeparatorChar.ToString()))
							strExceptionsLogFilename = strExceptionsLogFilename + Path.DirectorySeparatorChar +
													   "FolderSizesDisplay" + Path.DirectorySeparatorChar +
														"FolderSizesDisplay.log";
						else
							strExceptionsLogFilename = strExceptionsLogFilename +
														"FolderSizesDisplay" + Path.DirectorySeparatorChar +
														"FolderSizesDisplay.log";

						try
						{

							DateTime date_courante = DateTime.Now;

							swExceptionsLog = File.AppendText(strExceptionsLogFilename);
							swExceptionsLog.Write(Environment.NewLine +
								"Démarré le " +
								string.Format("{0:00}/{1:00}/{2}", date_courante.Day, date_courante.Month, date_courante.Year) + " à " +
								string.Format("{0:00}:{1:00}:{2:00}", date_courante.Hour, date_courante.Minute, date_courante.Second) +
								Environment.NewLine);
							swExceptionsLog.Close();

							bLogExceptions = true;

						}
						catch (Exception) { swExceptionsLog.Close(); }

					}

					if (bLogExceptions)
						MessageBox.Show(
							"Les exceptions seront enregistrées dans le fichier" + Environment.NewLine + strExceptionsLogFilename,
							"Enregistrement log",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information);
					else
						MessageBox.Show("L'application n'a pas trouvé d'emplacement où stocker son log.Il ne sera donc pas fait.",
							"Echec enregistrement log",
							MessageBoxButtons.OK,
							MessageBoxIcon.Warning);

				}

				#endregion Gestion argument -logexc pour logger les exceptions
				else
				#region Gestion des dossiers passés en paramètre
				if (Directory.Exists(argument))
					dossiers_ajoutes_au_demarrage.Add(argument);
				#endregion Gestion des dossiers passés en paramètre

			}

			if (dossiers_ajoutes_au_demarrage.Count > 0)
			{
				this.GUI_progressBar_invisible.Visible = true;
				backgroundWorker1.RunWorkerAsync(dossiers_ajoutes_au_demarrage.ToArray());
			}
			else
			{
				if (Environment.OSVersion.Version.Major > 5) //Vista or Never
				{
					try
					{
						System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
						System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
						if (principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator))
						{
							MessageBox.Show(
								"Le Drag&Drop ne fonctionne pas correctement en mode Administrateur.Pour lister des éléments protégés,\n"+
									"Définissez la propriété \"Exécuter en mode Administrateur\" de l'exécutable, puis faites un Drag&Drop\n"+
									"d'un ou plusieurs dossiers vers l'exécutable.Pensez à désactiver le mode Administrateur après cela pour\n"+
									"utiliser le programme dans des conditions normales.",
								"Problème Sécurité",
								MessageBoxButtons.OK,
								MessageBoxIcon.Stop	
							);
						}
					}
					catch (Exception e)
					{
						MessageBox.Show(
							"Le programme n'a pas réussi à déterminer s'il est actuellement exécuté en mode Administrateur.\n" +
								"Le Drag&Drop ne fonctionne pas correctement en mode Administrateur.Pour lister des éléments protégés,\n" +
								"définissez la propriété \"Exécuter en mode Administrateur\" de l'exécutable, puis faites un Drag&Drop\n" +
								"d'un ou plusieurs dossiers vers l'exécutable.Pensez à désactiver le mode Administrateur après cela pour\n" +
								"utiliser le programme dans des conditions normales.",
							"Information Sécurité",
							MessageBoxButtons.OK,
							MessageBoxIcon.Information
						);
					}
				}
			}

		}

		#endregion Constructeurs

		#region Drag&Drop

		/// <summary>
		/// Entrée d'un drag&drop dans le treeview, enregistrement
		/// des éléments s'ils sont de type FileDrop.
		/// </summary>
		/// <param name="sender">Par défaut.</param>
		/// <param name="e">Variable contenant les données du
		/// drag&drop.</param>
		private void treeView1_DragEnter(object sender, DragEventArgs e)
		{
			if(!backgroundWorker1.IsBusy)
				if(e.Data.GetDataPresent(DataFormats.FileDrop))
					e.Effect=DragDropEffects.Copy;
		}

		/// <summary>
		/// Exécution d'un drag&drop dans le treeview, lance le
		/// backgroundworker avec les données du drag&drop si
		/// le backgroundworker ne travaille pas déjà.
		/// </summary>
		/// <param name="sender">Par défaut.</param>
		/// <param name="e">Variable contenant les données du
		/// drag&drop.</param>
		private void treeView1_DragDrop(object sender, DragEventArgs e)
		{
			if (!backgroundWorker1.IsBusy)
			{
				this.GUI_treeView.Cursor = Cursors.WaitCursor;
				this.GUI_treeView.Focus();
				this.GUI_progressBar_invisible.Visible = true;
				backgroundWorker1.RunWorkerAsync(e.Data.GetData(DataFormats.FileDrop));
			}
		}

		#endregion Drag&Drop

		#region Méthodes backgroundWorker

		/// <summary>
		/// Travail de BackgroundWorker qui constitue un TreeNode 
		/// servant de racine au treeview, soit à partir d'un 
		/// dossier unique, soit en parent des multiples dossiers 
		/// passés.
		/// </summary>
		/// <param name="sender">Par défaut.</param>
		/// <param name="e">Arguments du backgroundworker, contenant
		/// les dossiers (peut-être 1 seul) à traiter, et le noeud
		/// servant à produire le treeview généré à la fin du travail.</param>
		private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
		{

#if AJOUT_CUMULATIF
			string[] liste_chemins_dragndrop = (string[])e.Argument;
			TreeNode noeud_principal = null;

			if (GUI_treeView.BackColor == System.Drawing.Color.White || this.GUI_treeView.Nodes.Count==0)
			{

				noeud_principal = new TreeNode();
#else
			string[] liste_chemins_dragndrop = (string[])e.Argument;
			TreeNode noeud_principal = new TreeNode();
#endif

				this.nombre_dossiers_traites = 0;
				this.nombre_dossiers_detectes = 0;

				if (liste_chemins_dragndrop.Length == 1)
				{

					TreeNodeTag tag_dossier_principal = new TreeNodeTag();

					this.nombre_dossiers_detectes++;

#if MODE_NB_ELEMENTS
					BackgroundWork_AjouteSousRepertoires(ref noeud_principal, ref tag_dossier_principal, liste_chemins_dragndrop[0]);
#else
					tag_dossier_principal.taille =
						BackgroundWork_AjouteSousRepertoires(ref noeud_principal, liste_chemins_dragndrop[0]); 
#endif

					this.nombre_dossiers_traites++;

					noeud_principal.Text =
						liste_chemins_dragndrop[0].Replace(
							Path.GetDirectoryName(liste_chemins_dragndrop[0]) + Path.DirectorySeparatorChar,
							""
						)
						+ ' ' + Tools.Generer_taille_fichier_lisible(tag_dossier_principal.taille, Application.CurrentCulture);
					noeud_principal.ToolTipText = liste_chemins_dragndrop[0];
					if (tag_dossier_principal.taille != -1)
					{
						noeud_principal.ContextMenuStrip = contextMenuStrip_dossier;
						tag_dossier_principal.tri = TreeNodeTypeTri.tries_par_nom;
					}
					noeud_principal.Tag = tag_dossier_principal;
#if ICONES_SUR_TREEVIEW
					if (noeud_principal.Nodes.Count > 0)
					{
						noeud_principal.ImageIndex = (int)IconIndex.dossier_ouvert;
						noeud_principal.SelectedImageIndex = (int)IconIndex.dossier_ouvert;
					}
					else
					{
						noeud_principal.ImageIndex = (int)IconIndex.dossier_ferme;
						noeud_principal.SelectedImageIndex = (int)IconIndex.dossier_ferme;
					}
#endif
				}
				else
				{

					TreeNodeTag tag_noeud_principal = new TreeNodeTag();
					TreeNodeTag tag_noeud_courant = new TreeNodeTag();
					TreeNode noeud_courant;

					tag_noeud_principal.taille = 0;

					foreach (string chemin_de_dragdrop in liste_chemins_dragndrop)
					{

						noeud_courant = new TreeNode();

						this.nombre_dossiers_detectes++;

#if MODE_NB_ELEMENTS
						BackgroundWork_AjouteSousRepertoires(ref noeud_courant, ref tag_noeud_courant, chemin_de_dragdrop); 
#else
						tag_noeud_courant.taille = BackgroundWork_AjouteSousRepertoires(ref noeud_courant, chemin_de_dragdrop); 
#endif

						this.nombre_dossiers_traites++;

						noeud_courant.Text =
							chemin_de_dragdrop.Replace(
								Path.GetDirectoryName(chemin_de_dragdrop),
								""
							)
							 + ' ' + Tools.Generer_taille_fichier_lisible(tag_noeud_courant.taille, Application.CurrentCulture);
						noeud_courant.ToolTipText = chemin_de_dragdrop;
						if (tag_noeud_courant.taille != -1)
						{
							noeud_courant.ContextMenuStrip = contextMenuStrip_dossier;
							tag_noeud_courant.tri = TreeNodeTypeTri.tries_par_nom;
						}
						tag_noeud_principal.taille += tag_noeud_courant.taille;
						noeud_courant.Tag = tag_noeud_courant;
#if ICONES_SUR_TREEVIEW
						if (noeud_courant.Nodes.Count > 0)
						{
							noeud_courant.ImageIndex = (int)IconIndex.dossier_ouvert;
							noeud_courant.SelectedImageIndex = (int)IconIndex.dossier_ouvert;
						}
						else
						{
							noeud_courant.ImageIndex = (int)IconIndex.dossier_ferme;
							noeud_courant.SelectedImageIndex = (int)IconIndex.dossier_ferme;
						}
#endif
						noeud_principal.Nodes.Add(noeud_courant);

					}

					noeud_principal.Text = "* " + Tools.Generer_taille_fichier_lisible(tag_noeud_principal.taille, Application.CurrentCulture);
					noeud_principal.ToolTipText = "Taille totale des dossiers sélectionnés";
					if (tag_noeud_principal.taille != -1)
					{
						noeud_principal.ContextMenuStrip = contextMenuStrip_multi;
						tag_noeud_principal.tri = TreeNodeTypeTri.tries_par_nom;
					}
					noeud_principal.Tag = tag_noeud_principal;

				}

#if AJOUT_CUMULATIF
			}
			else //<=> if !(GUI_treeView.BackColor == System.Drawing.Color.White || this.GUI_treeView.Nodes.Count == 0)
			{

				TreeNode noeud_clef_ajouts = new TreeNode();

				if (this.GUI_treeView.Nodes[0].Text.StartsWith("*"))
					noeud_principal = this.GUI_treeView.Nodes[0].Clone() as TreeNode;
				else
				{
					noeud_principal = new TreeNode("*");
					noeud_principal.Nodes.Add(this.GUI_treeView.Nodes[0].Clone() as TreeNode);
					noeud_principal.Tag = null;
				}

				#region Version modifiée de l'ajout ordinaire

				if (liste_chemins_dragndrop.Length == 1)
				{

					TreeNodeTag tag_noeud_clef_ajouts = new TreeNodeTag();
					TreeNodeTag tag_noeud_principal;

					this.nombre_dossiers_detectes++;

#if MODE_NB_ELEMENTS
					BackgroundWork_AjouteSousRepertoires(ref noeud_clef_ajouts, ref tag_noeud_clef_ajouts, liste_chemins_dragndrop[0]);
#else
					tag_noeud_clef_ajouts.taille =
						BackgroundWork_AjouteSousRepertoires(ref noeud_clef_ajouts, liste_chemins_dragndrop[0]); 
#endif

					this.nombre_dossiers_traites++;

					noeud_clef_ajouts.Text =
						liste_chemins_dragndrop[0].Replace(
							Path.GetDirectoryName(liste_chemins_dragndrop[0]) + Path.DirectorySeparatorChar,
							""
						)
						+ ' ' + Tools.Generer_taille_fichier_lisible(tag_noeud_clef_ajouts.taille, Application.CurrentCulture);
					noeud_clef_ajouts.ToolTipText = liste_chemins_dragndrop[0];
					if (tag_noeud_clef_ajouts.taille != -1)
					{
						noeud_clef_ajouts.ContextMenuStrip = contextMenuStrip_dossier;
						tag_noeud_clef_ajouts.tri = TreeNodeTypeTri.tries_par_nom;
					}
					noeud_clef_ajouts.Tag = tag_noeud_clef_ajouts;
#if ICONES_SUR_TREEVIEW
					if (noeud_clef_ajouts.Nodes.Count > 0)
					{
						noeud_clef_ajouts.ImageIndex = (int)IconIndex.dossier_ouvert;
						noeud_clef_ajouts.SelectedImageIndex = (int)IconIndex.dossier_ouvert;
					}
					else
					{
						noeud_clef_ajouts.ImageIndex = (int)IconIndex.dossier_ferme;
						noeud_clef_ajouts.SelectedImageIndex = (int)IconIndex.dossier_ferme;
					}
#endif

					noeud_principal.Nodes.Add(noeud_clef_ajouts);

					if (noeud_principal.Tag != null) //noeud principal réutilisé comme noeud multi (étoile)
					{
						tag_noeud_principal = (TreeNodeTag)noeud_principal.Tag;
						tag_noeud_principal.taille += tag_noeud_clef_ajouts.taille;
					}
					else							//nouveau noeud principal utilisé comme noeud multi (étoile),
					{
						tag_noeud_principal = new TreeNodeTag();
						//ajoute les tailles de l'élément drag&droppé et de l'élément pré-existant (car this.GUI_treeView.Nodes.Count!=0))
						tag_noeud_principal.taille = tag_noeud_clef_ajouts.taille + ((TreeNodeTag)this.GUI_treeView.Nodes[0].Tag).taille;
					}

					noeud_principal.Text = "* " + Tools.Generer_taille_fichier_lisible(tag_noeud_principal.taille, Application.CurrentCulture);
					noeud_principal.ToolTipText = "Taille totale des dossiers sélectionnés";
					if (tag_noeud_principal.taille != -1)
						noeud_principal.ContextMenuStrip = contextMenuStrip_multi;
					noeud_principal.Tag = tag_noeud_principal;

				}
				else
				{

					///
					///TO UPDATE
					///

					TreeNodeTag tag_noeud_principal = new TreeNodeTag();
					TreeNodeTag tag_noeud_courant = new TreeNodeTag();
					TreeNode noeud_courant;

					tag_noeud_principal.taille = 0;

					foreach (string chemin_de_dragdrop in liste_chemins_dragndrop)
					{

						noeud_courant = new TreeNode();

						this.nombre_dossiers_detectes++;

#if MODE_NB_ELEMENTS
						BackgroundWork_AjouteSousRepertoires(ref noeud_courant, ref tag_noeud_courant, chemin_de_dragdrop);
#else
						tag_noeud_courant.taille = BackgroundWork_AjouteSousRepertoires(ref noeud_courant, chemin_de_dragdrop); 
#endif

						this.nombre_dossiers_traites++;

						noeud_courant.Text =
							chemin_de_dragdrop.Replace(
								Path.GetDirectoryName(chemin_de_dragdrop),
								""
							)
							 + ' ' + Tools.Generer_taille_fichier_lisible(tag_noeud_courant.taille, Application.CurrentCulture);
						noeud_courant.ToolTipText = chemin_de_dragdrop;
						if (tag_noeud_courant.taille != -1)
						{
							noeud_courant.ContextMenuStrip = contextMenuStrip_dossier;
							tag_noeud_courant.tri = TreeNodeTypeTri.tries_par_nom;
						}
						tag_noeud_principal.taille += tag_noeud_courant.taille;
						noeud_courant.Tag = tag_noeud_courant;
#if ICONES_SUR_TREEVIEW
						if (noeud_courant.Nodes.Count > 0)
						{
							noeud_courant.ImageIndex = (int)IconIndex.dossier_ouvert;
							noeud_courant.SelectedImageIndex = (int)IconIndex.dossier_ouvert;
						}
						else
						{
							noeud_courant.ImageIndex = (int)IconIndex.dossier_ferme;
							noeud_courant.SelectedImageIndex = (int)IconIndex.dossier_ferme;
						}
#endif
						noeud_principal.Nodes.Add(noeud_courant);

					}

					noeud_principal.Text = "* " + Tools.Generer_taille_fichier_lisible(tag_noeud_principal.taille, Application.CurrentCulture);
					noeud_principal.ToolTipText = "Taille totale des dossiers sélectionnés";
					if (tag_noeud_principal.taille != -1)
					{
						noeud_principal.ContextMenuStrip = contextMenuStrip_multi;
						tag_noeud_principal.tri = TreeNodeTypeTri.tries_par_nom;
					}
					noeud_principal.Tag = tag_noeud_principal;

				}

				#endregion Version modifiée de l'ajout ordinaire

			}
#endif

			e.Result = noeud_principal;

		}

		/// <summary>
		/// Affiche la progression du BackgroundWorkeravec le nombre 
		/// de dossiers traités et détectés.
		/// </summary>
		/// <param name="sender">Par défaut.</param>
		/// <param name="e">Par défaut.Inutilisé.</param>
		private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
		{
			this.Text = "Folders sizes display " + this.nombre_dossiers_traites + '/' + this.nombre_dossiers_detectes;
			this.GUI_progressBar_invisible.Value = this.nombre_dossiers_traites;
			this.GUI_progressBar_invisible.Maximum = this.nombre_dossiers_detectes;
		}

		/// <summary>
		/// Fin de travail du BackgroundWorker.
		/// Met à jour le treeview en lui fournissant le noeud produit.
		/// </summary>
		/// <param name="sender">Par défaut.</param>
		/// <param name="e">Arguments contenant le TreeNode produit.</param>
		/// <remarks>Actualise une dernière fois le nombre de dossiers traités
		/// et détectés, permettant éventuellement de détecter une anomalie.</remarks>
		private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
		{

#if AJOUT_CUMULATIF
			TreeNode temp_selected_node = GUI_treeView.SelectedNode;
#endif

			this.GUI_treeView.BeginUpdate();

			this.GUI_treeView.Nodes.Clear();
			this.GUI_treeView.Nodes.Add((TreeNode)e.Result);

			this.GUI_treeView.EndUpdate();

			this.GUI_treeView.Cursor = Cursors.Default;

			this.Text = "Folders sizes display " + this.nombre_dossiers_traites + '/' + this.nombre_dossiers_detectes;

			this.GUI_progressBar_invisible.Visible = false;

#if AJOUT_CUMULATIF

			if (GUI_treeView.BackColor != System.Drawing.Color.White)
				this.modeAjout_ToolStripMenuItem(null, null);

			if (temp_selected_node != null)
			{
				// /!\      /!\      /!\
				// /!\ NE MARCHE PAS /!\
				// /!\      /!\      /!\
				this.GUI_treeView.SelectedNode = this.GUI_treeView.Nodes[temp_selected_node.Text];
			}

#endif

		}

		#endregion Méthodes backgroundWorker

		#region Récupération du noeud cliqué pour usage avec menus contextuels
		/// <summary>Récupération du noeud cliqué pour usage avec menus contextuels</summary>
		private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{

			this.noeud_cible_menu_contextuel = e.Node;

#if MODE_RECHERCHE
			if (e.Node.Parent == null)
				this.rechercheDepuisParentToolStripMenuItem.Enabled = false;
			else
				this.rechercheDepuisParentToolStripMenuItem.Enabled = true; 
#endif

		}
		#endregion Récupération du noeud cliqué pour usage avec menus contextuels

		#region Menus contextuels

		#region Méthodes d'ouverture de fichier/dossier

		/// <summary>
		/// Ouvre le fichier décrit par le noeud pour lequel la
		/// méthode est appelée.
		/// Demande une confirmation pour permettre les ouvertures
		/// de fichier.
		/// Si le chemin n'est plus valide, affiche un message en
		/// conséquence.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void ouvrirFichierToolStripMenuItem1_Click(object sender, EventArgs e)
		{

			if (this.bAutoriseOuvertureFichier)
			{

				if (File.Exists(this.noeud_cible_menu_contextuel.ToolTipText))
				{

					DialogResult resultat = MessageBox.Show(
						"Voulez-vous vraiment ouvrir le fichier suivant:" + Environment.NewLine + this.noeud_cible_menu_contextuel.ToolTipText,
						"Ouvrir fichier",
						MessageBoxButtons.YesNo,
						MessageBoxIcon.None,
						MessageBoxDefaultButton.Button2
						);

					if (resultat == DialogResult.Yes)
						System.Diagnostics.Process.Start(this.noeud_cible_menu_contextuel.ToolTipText);

				}
				else
					MessageBox.Show("Le fichier n'est plus accessible par le chemin enregistré.");

			}
			else
			{

				DialogResult resultat = MessageBox.Show(
					"Cette fonction est désactivée par défaut.L'activer?",
					"Fonction Ouvrir fichier",
					MessageBoxButtons.YesNo,
					MessageBoxIcon.None,
					MessageBoxDefaultButton.Button2);

				if (resultat == DialogResult.Yes)
				{
					this.bAutoriseOuvertureFichier = true;
					ouvrirFichierToolStripMenuItem.ForeColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
					ouvrirFichierToolStripMenuItem1_Click(sender, e);
				}

			}

		}

		/// <summary>
		/// Ouvre le dossier décrit par le noeud pour lequel la
		/// méthode est appelée.
		/// Si le chemin n'est plus valide, affiche un message en
		/// conséquence.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void ouvrirDossierToolStripMenuItem1_Click(object sender, EventArgs e)
		{

			if (Directory.Exists(this.noeud_cible_menu_contextuel.ToolTipText))
				System.Diagnostics.Process.Start(this.noeud_cible_menu_contextuel.ToolTipText);
			else
				MessageBox.Show("Le dossier n'est plus accessible par le chemin enregistré.");

		}

		/// <summary>
		/// Ouvre le dossier contenant le fichier décrit par le 
		/// noeud pour lequel la méthode est appelée.
		/// Si le chemin n'est plus valide, affiche un message en
		/// conséquence.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void ouvrirDossierContenantToolStripMenuItem_Click(object sender, EventArgs e)
		{

			string dossier_contenant = Path.GetDirectoryName(this.noeud_cible_menu_contextuel.ToolTipText);

			if (Directory.Exists(dossier_contenant))
				System.Diagnostics.Process.Start(dossier_contenant);
			else
				MessageBox.Show("Le dossier n'est plus accessible par le chemin enregistré.");

		}

		#endregion Méthodes d'ouverture de fichier/dossier

		#region Méthodes de tri du TreeView

		/// <summary>
		/// Tri les noeuds enfants du noeud_cible_menu_contextuel
		/// courant par leur nom de fichier.
		/// Inverse le tri s'ils sont déjà triés par nom croissant.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void triParNomToolStripMenuItem_Click(object sender, EventArgs e)
		{

			if (this.GUI_treeView.Cursor != Cursors.WaitCursor)
			{

				System.Collections.Generic.LinkedList<TreeNode> nouvelle_collection = new System.Collections.Generic.LinkedList<TreeNode>();
				System.Collections.Generic.LinkedListNode<TreeNode> iterateur;
				TreeNodeTag tag_infos = (TreeNodeTag)this.noeud_cible_menu_contextuel.Tag;

				this.GUI_treeView.Cursor = Cursors.WaitCursor;

				if (this.noeud_cible_menu_contextuel.Nodes.Count > 0)
					nouvelle_collection.AddFirst(this.noeud_cible_menu_contextuel.Nodes[0]);

				if (!(tag_infos.tri == TreeNodeTypeTri.tries_par_nom))
				{

					for (int i = 1; i < this.noeud_cible_menu_contextuel.Nodes.Count; i++)
					{

						iterateur = nouvelle_collection.First;

						while (iterateur.Next != null && (iterateur.Value.Text).CompareTo(this.noeud_cible_menu_contextuel.Nodes[i].Text) < 0)
							iterateur = iterateur.Next;

						if ((iterateur.Value.Text).CompareTo(this.noeud_cible_menu_contextuel.Nodes[i].Text) < 0)
							nouvelle_collection.AddAfter(iterateur, this.noeud_cible_menu_contextuel.Nodes[i]);
						else
							nouvelle_collection.AddBefore(iterateur, this.noeud_cible_menu_contextuel.Nodes[i]);

					}

					tag_infos.tri = TreeNodeTypeTri.tries_par_nom;
					this.noeud_cible_menu_contextuel.Tag = tag_infos;

				}
				else
				{

					for (int i = 1; i < this.noeud_cible_menu_contextuel.Nodes.Count; i++)
					{

						iterateur = nouvelle_collection.First;

						while (iterateur.Next != null && (iterateur.Value.Text).CompareTo(this.noeud_cible_menu_contextuel.Nodes[i].Text) > 0)
							iterateur = iterateur.Next;

						if ((iterateur.Value.Text).CompareTo(this.noeud_cible_menu_contextuel.Nodes[i].Text) > 0)
							nouvelle_collection.AddAfter(iterateur, this.noeud_cible_menu_contextuel.Nodes[i]);
						else
							nouvelle_collection.AddBefore(iterateur, this.noeud_cible_menu_contextuel.Nodes[i]);

					}

					tag_infos.tri = TreeNodeTypeTri.tries_par_nom_inverse;
					this.noeud_cible_menu_contextuel.Tag = tag_infos;

				}

				this.GUI_treeView.BeginUpdate();
				this.noeud_cible_menu_contextuel.Nodes.Clear();
				foreach (TreeNode noeud in nouvelle_collection)
					this.noeud_cible_menu_contextuel.Nodes.Add(noeud);
				this.GUI_treeView.EndUpdate();

				this.GUI_treeView.Cursor = Cursors.Default;

			}

		}

		/// <summary>
		/// Tri les noeuds enfants du noeud_cible_menu_contextuel
		/// courant par leur taille de fichier.
		/// Inverse le tri s'ils sont déjà triés par taille 
		/// croissante.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void triParTailleToolStripMenuItem_Click(object sender, EventArgs e)
		{

			if (this.GUI_treeView.Cursor != Cursors.WaitCursor)
			{

				System.Collections.Generic.LinkedList<TreeNode> nouvelle_collection = new System.Collections.Generic.LinkedList<TreeNode>();
				System.Collections.Generic.LinkedListNode<TreeNode> iterateur;
				TreeNodeTag tag_infos = (TreeNodeTag)this.noeud_cible_menu_contextuel.Tag;

				this.GUI_treeView.Cursor = Cursors.WaitCursor;

				if (this.noeud_cible_menu_contextuel.Nodes.Count > 0)
					nouvelle_collection.AddFirst(this.noeud_cible_menu_contextuel.Nodes[0]);

				if (!(tag_infos.tri == TreeNodeTypeTri.tries_par_taille))
				{

					for (int i = 1; i < this.noeud_cible_menu_contextuel.Nodes.Count; i++)
					{

						iterateur = nouvelle_collection.First;

						while (
							iterateur.Next != null && 
							(((TreeNodeTag)iterateur.Value.Tag)).taille < ((TreeNodeTag)this.noeud_cible_menu_contextuel.Nodes[i].Tag).taille
							)
							iterateur = iterateur.Next;

						if ((((TreeNodeTag)iterateur.Value.Tag)).taille < ((TreeNodeTag)this.noeud_cible_menu_contextuel.Nodes[i].Tag).taille)
							nouvelle_collection.AddAfter(iterateur, this.noeud_cible_menu_contextuel.Nodes[i]);
						else
							nouvelle_collection.AddBefore(iterateur, this.noeud_cible_menu_contextuel.Nodes[i]);

					}

					tag_infos.tri = TreeNodeTypeTri.tries_par_taille;
					this.noeud_cible_menu_contextuel.Tag = tag_infos;

				}
				else
				{

					for (int i = 1; i < this.noeud_cible_menu_contextuel.Nodes.Count; i++)
					{

						iterateur = nouvelle_collection.First;

						while (
							iterateur.Next != null &&
							(((TreeNodeTag)iterateur.Value.Tag)).taille > ((TreeNodeTag)this.noeud_cible_menu_contextuel.Nodes[i].Tag).taille
							)
							iterateur = iterateur.Next;

						if ((((TreeNodeTag)iterateur.Value.Tag)).taille > ((TreeNodeTag)this.noeud_cible_menu_contextuel.Nodes[i].Tag).taille)
							nouvelle_collection.AddAfter(iterateur, this.noeud_cible_menu_contextuel.Nodes[i]);
						else
							nouvelle_collection.AddBefore(iterateur, this.noeud_cible_menu_contextuel.Nodes[i]);

					}

					tag_infos.tri = TreeNodeTypeTri.tries_par_taille_inverse;
					this.noeud_cible_menu_contextuel.Tag = tag_infos;

				}

				this.GUI_treeView.BeginUpdate();
				this.noeud_cible_menu_contextuel.Nodes.Clear();
				foreach (TreeNode noeud in nouvelle_collection)
					this.noeud_cible_menu_contextuel.Nodes.Add(noeud);
				this.GUI_treeView.EndUpdate();

				this.GUI_treeView.Cursor = Cursors.Default;

			}

		}

		#endregion Méthodes de tri du TreeView

		#endregion Menus contextuels

		#region Gestion fermeture fenêtre/application

		/// <summary>
		/// Fermeture de la fenêtre de l'application.
		/// Assure la fin du backgroundworker et la
		/// suppression de swExceptionsLog si
		/// nécessaire.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.Inutilisé.</param>
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			backgroundWorker1.CancelAsync();
			while (backgroundWorker1.IsBusy)
				Application.DoEvents();
			if (swExceptionsLog != null)
				swExceptionsLog.Dispose();
		}

		#endregion Gestion fermeture fenêtre/application

		#region Méthode BackgroundWork_AjouteSousRepertoires

		/// <summary>
		/// Méthode clef pour le travail du BackgroundWorker.
		/// Ajoute les sous-repertoires correspondant au noeud
		/// passé au noeud en question, par récursivité pour
		/// inclure tous les sous-répertoires successifs.
		/// Peut être interrompu par backgroundWorker1.CancelAsync().
		/// </summary>
		/// <param name="noeud">Noeud auquel ajouter les noeuds
		/// enfants correspondants à ses sous-dossiers.
		/// Passé par référence pour mise à jour du noeud réel.</param>
#if MODE_NB_ELEMENTS
		/// <param name="tag_noeud">Tag du noeud qui contiendra la
		/// taille et le nombre d'élements pour le noeud en cours
		/// (le tag est ajouté en-dehors de cette méthode).</param>
#endif
		/// <param name="chemin">Chemin auquel correspond le
		/// noeud.</param>
#if !MODE_NB_ELEMENTS
		/// <returns>Taille du dossier décrit par le noeud, incluant
		/// la taille de ses sous-dossiers et fichiers.</returns>
#endif
#if MODE_NB_ELEMENTS
		void BackgroundWork_AjouteSousRepertoires(ref TreeNode noeud, ref TreeNodeTag tag_noeud, string chemin)
#else
		Int64 BackgroundWork_AjouteSousRepertoires(ref TreeNode noeud, string chemin) 
#endif
		{

			string[] liste_dossiers = null;
			string[] liste_fichiers = null;

#if MODE_NB_ELEMENTS
			tag_noeud.nb_elem = 0;
			tag_noeud.taille = 0;
#else
			Int64 taille_retournee = 0; 
#endif

			bool bExceptionOccured = false;

			try
			{

				liste_dossiers = Directory.GetDirectories(chemin);
				liste_fichiers = Directory.GetFiles(chemin);

			}
			catch (Exception exc)
			{

				if (bDisplayExceptionsMBOX)
					MessageBox.Show(exc.Message, "Exception");

				if (bLogExceptions)
				{
					swExceptionsLog = File.AppendText(strExceptionsLogFilename);
					swExceptionsLog.Write(Environment.NewLine + exc.GetType().ToString() + " : " + exc.Message + Environment.NewLine);
					swExceptionsLog.Close();
				}

				bExceptionOccured = true;
#if MODE_NB_ELEMENTS
				tag_noeud.taille = -1;
				tag_noeud.nb_elem = -1;
#else
				taille_retournee = -1; 
#endif

			}

			if (!bExceptionOccured)
			{

				this.nombre_dossiers_detectes += liste_dossiers.Length;

				backgroundWorker1.ReportProgress(1);

				foreach (string dossier_interne in liste_dossiers)
				{

					if (!backgroundWorker1.CancellationPending)
					{

						TreeNodeTag tag_noeud_ajoute = new TreeNodeTag();
						TreeNode noeud_ajoute = new TreeNode(
							dossier_interne.Replace(
								Path.GetDirectoryName(dossier_interne),
								""
							)
						);

#if MODE_NB_ELEMENTS
						BackgroundWork_AjouteSousRepertoires(ref noeud_ajoute, ref tag_noeud_ajoute, dossier_interne);
#else
						tag_noeud_ajoute.taille = BackgroundWork_AjouteSousRepertoires(ref noeud_ajoute, dossier_interne); 
#endif

						if (tag_noeud_ajoute.taille != -1)
						{
#if MODE_NB_ELEMENTS
							tag_noeud.taille += tag_noeud_ajoute.taille;
							tag_noeud.nb_elem += tag_noeud_ajoute.nb_elem;
#else
							taille_retournee += tag_noeud_ajoute.taille; 
#endif
							noeud_ajoute.ContextMenuStrip = contextMenuStrip_dossier;
							tag_noeud_ajoute.tri = TreeNodeTypeTri.tries_par_nom;
						}

						noeud_ajoute.Text = noeud_ajoute.Text + ' ' + Tools.Generer_taille_fichier_lisible(tag_noeud_ajoute.taille, Application.CurrentCulture);
						noeud_ajoute.ToolTipText = dossier_interne;
						noeud_ajoute.Tag = tag_noeud_ajoute;
#if ICONES_SUR_TREEVIEW
						if (noeud_ajoute.Nodes.Count > 0)
						{
							noeud_ajoute.ImageIndex = (int)IconIndex.dossier_ouvert;
							noeud_ajoute.SelectedImageIndex = (int)IconIndex.dossier_ouvert;
						}
						else
						{
							noeud_ajoute.ImageIndex = (int)IconIndex.dossier_ferme;
							noeud_ajoute.SelectedImageIndex = (int)IconIndex.dossier_ferme;
						} 
#endif
						noeud.Nodes.Add(noeud_ajoute);

						this.nombre_dossiers_traites++;

					}

				}
				if (!backgroundWorker1.CancellationPending)
				{
					foreach (string fichier_interne in liste_fichiers)
					{
						if (!backgroundWorker1.CancellationPending)
						{

							TreeNode noeud_ajoute = new TreeNode();
							TreeNodeTag tag_noeud_ajoute = new TreeNodeTag();
							bool bExceptionOccured2 = false;

							try
							{
								tag_noeud_ajoute.taille = new FileInfo(fichier_interne).Length;
							}
							catch (Exception exc)
							{

								if (bDisplayExceptionsMBOX)
									MessageBox.Show(exc.Message, "Exception");

								if (bLogExceptions)
								{
									swExceptionsLog = File.AppendText(strExceptionsLogFilename);
									swExceptionsLog.Write(Environment.NewLine + exc.GetType().ToString() + " : " + exc.Message + Environment.NewLine);
									swExceptionsLog.Close();
								}

								bExceptionOccured2 = true;

								tag_noeud_ajoute.taille = -1;

							}

							if (!bExceptionOccured2)
							{
#if MODE_NB_ELEMENTS
								tag_noeud.taille += tag_noeud_ajoute.taille; 
#else
								taille_retournee += tag_noeud_ajoute.taille; 
#endif
								noeud_ajoute.ContextMenuStrip = contextMenuStrip_fichier;
							}

#if MODE_NB_ELEMENTS
							tag_noeud.nb_elem++; 
#endif

							noeud_ajoute.Text =
								Path.GetFileName(fichier_interne) +
								' ' + Tools.Generer_taille_fichier_lisible(tag_noeud_ajoute.taille, Application.CurrentCulture);
							noeud_ajoute.ToolTipText = fichier_interne;
							noeud_ajoute.Tag = tag_noeud_ajoute;
#if ICONES_SUR_TREEVIEW
							noeud_ajoute.ImageIndex = (int)IconIndex.fichier;
							noeud_ajoute.SelectedImageIndex = (int)IconIndex.fichier; 
#endif
							noeud.Nodes.Add(noeud_ajoute);

						}
					}
				}

			}

#if !MODE_NB_ELEMENTS
			return taille_retournee; 
#endif

		}

		#endregion Méthode AjouteSousRepertoires

		#region Génération des menus contextuels (prise en compte du tri enregistré pour la sélection)

		/// <summary>
		/// Ouverture du menu contextuel pour un dossier.
		/// Coche l'élément correspondant au tri utilisé pour ce
		/// dossier grâce au TreeNodeTag.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.Inutilisé.</param>
		private void contextMenuStrip_dossier_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{

			TreeNodeTag noeud_cible_tag = (TreeNodeTag)this.noeud_cible_menu_contextuel.Tag;

			switch(noeud_cible_tag.tri) {

				case TreeNodeTypeTri.tries_par_nom:
					triParTailleToolStripMenuItem.CheckState = CheckState.Unchecked;
					triParNomToolStripMenuItem.CheckState = CheckState.Checked;
					break;

				case TreeNodeTypeTri.tries_par_nom_inverse:
					triParTailleToolStripMenuItem.CheckState = CheckState.Unchecked;
					triParNomToolStripMenuItem.CheckState = CheckState.Indeterminate;
					break;

				case TreeNodeTypeTri.tries_par_taille:
					triParNomToolStripMenuItem.CheckState = CheckState.Unchecked;
					triParTailleToolStripMenuItem.CheckState = CheckState.Checked;
					break;

				case TreeNodeTypeTri.tries_par_taille_inverse:
					triParNomToolStripMenuItem.CheckState = CheckState.Unchecked;
					triParTailleToolStripMenuItem.CheckState = CheckState.Indeterminate;
					break;

				default:
					triParNomToolStripMenuItem.CheckState = CheckState.Unchecked;
					triParTailleToolStripMenuItem.CheckState = CheckState.Unchecked;
					break;

			}

		}

		/// <summary>
		/// Ouverture du menu contextuel pour la racine du
		/// treeview si créé par multiples dossiers.
		/// Coche l'élément correspondant au tri utilisé pour cet
		/// élément grâce au TreeNodeTag.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.Inutilisé.</param>
		private void contextMenuStrip_multi_Opening(object sender, System.ComponentModel.CancelEventArgs e)
		{

			TreeNodeTag noeud_cible_tag = (TreeNodeTag)this.noeud_cible_menu_contextuel.Tag;

			switch (noeud_cible_tag.tri)
			{

				case TreeNodeTypeTri.tries_par_nom:
					triParTailleToolStripMenuItemMulti.CheckState = CheckState.Unchecked;
					triParNomToolStripMenuItemMulti.CheckState = CheckState.Checked;
					break;

				case TreeNodeTypeTri.tries_par_nom_inverse:
					triParTailleToolStripMenuItemMulti.CheckState = CheckState.Unchecked;
					triParNomToolStripMenuItemMulti.CheckState = CheckState.Indeterminate;
					break;

				case TreeNodeTypeTri.tries_par_taille:
					triParNomToolStripMenuItemMulti.CheckState = CheckState.Unchecked;
					triParTailleToolStripMenuItemMulti.CheckState = CheckState.Checked;
					break;

				case TreeNodeTypeTri.tries_par_taille_inverse:
					triParNomToolStripMenuItemMulti.CheckState = CheckState.Unchecked;
					triParTailleToolStripMenuItemMulti.CheckState = CheckState.Indeterminate;
					break;

				default:
					triParNomToolStripMenuItemMulti.CheckState = CheckState.Unchecked;
					triParTailleToolStripMenuItemMulti.CheckState = CheckState.Unchecked;
					break;

			}

		}

		#endregion Génération des menus contextuels (prise en compte du tri enregistré pour la sélection)

		#region Clic sur une Recherche dans un menu contextuel
#if MODE_RECHERCHE

		/// <summary>
		/// Ouverture de la zone de recherche de fichier/dossier.
		/// Definit le noeud racine de recherche comme le noeud
		/// depuis lequel la recherche est lancée pour rechercher
		/// dans ses noeuds enfants.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void rechercheToolStripMenuItem_Click(object sender, EventArgs e)
		{
			GUI_panel_recherche.Visible = true;
			GUI_textBox_recherche.Focus();
			this.noeud_racine_recherche = this.noeud_cible_menu_contextuel;
		}

		/// <summary>
		/// Ouverture de la zone de recherche de fichier/dossier.
		/// Definit le noeud racine de recherche comme le noeud
		/// parent de celui depuis lequel la recherche est lancée, 
		/// pour rechercher l'élément suivant correspondant aux
		/// critères de recherche dans ses noeuds enfants.
		/// Si pas de parent, recherche dans les noeuds enfants
		/// du noeud depuis lequel la recherche est lancée.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void rechercheDepuisParentToolStripMenuItem_Click(object sender, EventArgs e)
		{

			GUI_panel_recherche.Visible = true;
			GUI_textBox_recherche.Focus();

			if (this.noeud_cible_menu_contextuel.Parent != null)
				this.noeud_racine_recherche = this.noeud_cible_menu_contextuel.Parent;
			else
				this.noeud_racine_recherche = this.noeud_cible_menu_contextuel;

			this.GUI_treeView.SelectedNode = this.noeud_cible_menu_contextuel;

		} 

#endif
		#endregion Clic sur une Recherche dans un menu contextuel

		#region Clic sur l'option Mode Ajout du menu contextuel

#if AJOUT_CUMULATIF

		/// <summary>
		/// Changement du fond du treeview pour indiquer que l'ajout
		/// cumulatif est activé, et que les éléments glissés vers
		/// l'application seront ajoutés plutôt que de remplacer
		/// l'actuelle liste.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void modeAjout_ToolStripMenuItem(object sender, EventArgs e)
		{

			if (this.GUI_treeView.BackColor == System.Drawing.Color.White)
			{
				this.GUI_treeView.BackColor = System.Drawing.Color.Honeydew;
				this.modeAjout_dossierToolStripMenuItem.Checked = true;
				this.modeAjout_multiToolStripMenuItem.Checked = true;
			}
			else
			{
				this.GUI_treeView.BackColor = System.Drawing.Color.White;
				this.modeAjout_dossierToolStripMenuItem.Checked = false;
				this.modeAjout_multiToolStripMenuItem.Checked = false;
			}

		}

#endif

		#endregion Clic sur l'option Mode Ajout du menu contextuel

		#region Clic sur l'option Mode Nombre d'éléments du menu contextuel

#if MODE_NB_ELEMENTS

		/// <summary>
		/// Active le mode "nombre d'éléments", qui utilise le nombre de
		/// fichiers plutôt que leur taille totale comme élément.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void modeNbelem_ToolStripMenuItem(object sender, EventArgs e)
		{

			MessageBox.Show("En construction");

		}

#endif

		#endregion Clic sur l'option Mode Nombre d'éléments du menu contextuel

		#region Recherche: clic sur ok ou clic en-dehors de la zone de recherche

#if MODE_RECHERCHE //GUI_button_recherche_ok_Click

		/// <summary>
		/// Lance la recherche dans les noeuds enfants de noeud_racine_recherche.
		/// Sélectionne le noeud suivant contenant le texte de la textbox de
		/// recherche, et l'affiche écrit en bleu jusqu'au retour au treeview.
		/// Joue un son si rien n'est trouvé.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void GUI_button_recherche_ok_Click(object sender, EventArgs e)
		{

			TreeNode noeud_lu = null;
			string texte_recherche = GUI_textBox_recherche.Text.ToLower();

			if (texte_recherche != GUI_textBox_recherche.Text)
				GUI_textBox_recherche.Text = texte_recherche;

			//attempt to continue an existing search
			if (this.GUI_treeView.SelectedNode != null && this.GUI_treeView.SelectedNode.Parent == this.noeud_racine_recherche)
			{

				this.GUI_treeView.SelectedNode.ForeColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlText);

				noeud_lu = this.GUI_treeView.SelectedNode.NextNode;

				while (noeud_lu != null && !noeud_lu.Text.ToLower().Contains(texte_recherche))
					noeud_lu = noeud_lu.NextNode;

			}

			//nothing found when tried to continue an existing search
			//try again from the first node of the parent of selection
			if (noeud_lu == null)
			{

				noeud_lu = this.noeud_racine_recherche.FirstNode;

				while (noeud_lu != null && !noeud_lu.Text.ToLower().Contains(texte_recherche))
					noeud_lu = noeud_lu.NextNode;

			}

			//nothing is found containing the search text
			if (noeud_lu == null)
			{
				if (this.GUI_treeView.SelectedNode != this.noeud_racine_recherche)
					this.GUI_treeView.SelectedNode = this.noeud_racine_recherche;
				System.Media.SystemSounds.Beep.Play();
			}
			else
			{
				this.GUI_treeView.SelectedNode = noeud_lu;
				this.GUI_treeView.SelectedNode.ForeColor = System.Drawing.Color.Blue;
			}

		} 

#endif

#if MODE_RECHERCHE //GUI_panel_recherche_Leave

		/// <summary>
		/// Sortie du panel contenant les éléments de recherche,
		/// provoquant l'arrêt de la recherche.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Par Défaut.</param>
		private void GUI_panel_recherche_Leave(object sender, EventArgs e)
		{

			GUI_panel_recherche.Visible = false;

			if (this.GUI_treeView.SelectedNode != null && this.GUI_treeView.SelectedNode.ForeColor == System.Drawing.Color.Blue)
			{
				this.GUI_treeView.SelectedNode.ForeColor = System.Drawing.Color.FromKnownColor(System.Drawing.KnownColor.ControlText);
				this.rechercheDepuisParentToolStripMenuItem.Enabled = true;
			}

		} 

#endif

		#endregion Recherche: clic sur ok ou clic en-dehors de la zone de recherche

		#region Recherche: Entrée/Echap donnant le même effet que les clics précédents
#if MODE_RECHERCHE
		/// <summary>
		/// Vérifie les touches de la textbox de recherche pour lancer
		/// la recherche avec Entrée et arrêter la recherche avec Echap.
		/// Utilise SuppressKeyPress pour ne pas causer le son windows
		/// joué lorsqu'on appuie sur une touche incorrecte dans une 
		/// textbox.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Arguments contenant la touche appuyée
		/// et l'option SuppressKeyPress.</param>
		private void GUI_textBox_recherche_KeyDown(object sender, KeyEventArgs e)
		{

			if (e.KeyCode == Keys.Enter)
			{
				e.SuppressKeyPress = true;
				this.GUI_button_recherche_ok_Click(null, null);
			}
			else
			if (e.KeyCode == Keys.Escape)
			{
				e.SuppressKeyPress = true;
				this.GUI_treeView.Focus();
			}

		} 
#endif
		#endregion Recherche: Entrée/Echap donnant le même effet que les clics précédents

		#region Activation des raccourcis de menus contextuels depuis le treeview

		/// <summary>
		/// Vérifie les touches utilisées dans le treeview pour
		/// déclencher les actions de menu contextuel vu que
		/// leurs raccourcis claviers ne marchent pas directement
		/// sur une simple sélection.
		/// Utilise SuppressKeyPress pour ne pas causer le son windows
		/// joué lorsqu'on appuie sur une touche incorrecte dans un 
		/// treeview.
		/// </summary>
		/// <param name="sender">Par Défaut.</param>
		/// <param name="e">Arguments contenant la touche appuyée, si
		/// Control (nécessaire aux raccourcis) est pressé, et l'option 
		/// SuppressKeyPress.</param>
		private void GUI_treeView_KeyDown(object sender, KeyEventArgs e)
		{

			if (e.Control == true)
			{

				switch (e.KeyCode)
				{

					case Keys.O:

						if (this.GUI_treeView.SelectedNode != null)
						{

							if (this.GUI_treeView.SelectedNode.ContextMenuStrip == contextMenuStrip_dossier)
							{
								e.SuppressKeyPress = true;
								this.noeud_cible_menu_contextuel = this.GUI_treeView.SelectedNode;
								this.ouvrirDossierToolStripMenuItem1_Click(null, null);
							}
							else
								if (this.GUI_treeView.SelectedNode.ContextMenuStrip == contextMenuStrip_fichier)
								{
									e.SuppressKeyPress = true;
									this.noeud_cible_menu_contextuel = this.GUI_treeView.SelectedNode;
									this.ouvrirDossierContenantToolStripMenuItem_Click(null, null);
								}

						}
						break;

#if MODE_RECHERCHE
					case Keys.F:

						if (this.GUI_treeView.SelectedNode != null)
						{

							if (this.GUI_treeView.SelectedNode.ContextMenuStrip == contextMenuStrip_dossier ||
								this.GUI_treeView.SelectedNode.ContextMenuStrip == contextMenuStrip_multi)
							{

								e.SuppressKeyPress = true;
								this.noeud_cible_menu_contextuel = this.GUI_treeView.SelectedNode;
								this.rechercheToolStripMenuItem_Click(null, null);

							}

						}
						break;
#endif

#if AJOUT_CUMULATIF
					case Keys.Add:
						if (this.GUI_treeView.BackColor == System.Drawing.Color.White)
						{
							this.GUI_treeView.BackColor = System.Drawing.Color.Honeydew;
							this.modeAjout_dossierToolStripMenuItem.Checked = true;
							this.modeAjout_multiToolStripMenuItem.Checked = true;
						}
						else
						{
							this.GUI_treeView.BackColor = System.Drawing.Color.White;
							this.modeAjout_dossierToolStripMenuItem.Checked = false;
							this.modeAjout_multiToolStripMenuItem.Checked = false;
						}
						break;
#endif

					default:
						break;

				} //switch (e.KeyCode)

			} //if (e.Control == true)
			else
			if(e.KeyCode == Keys.Escape)
				if (backgroundWorker1.IsBusy)
					backgroundWorker1.CancelAsync();
		}

		#endregion Activation des raccourcis de menus contextuels depuis le treeview

		#region Initialisation des composants pour l'affichage d'icônes dans le treeview
#if ICONES_SUR_TREEVIEW
		/// <summary>
		/// Récupère des icônes pour les dossiers et fichiers du Treeview dans shell32.dll
		/// et les associe au Treeview
		/// </summary>
		/// <remarks>
		/// Les descriptions d'images en commentaires correspondent à Windows XP et varient
		/// selon les versions de Windows.
		/// </remarks>
		void InitialiseImagesTreeView()
		{

			this.GUI_treeView.ImageList = new ImageList();

			try
			{
				this.GUI_treeView.ImageList.ColorDepth = ColorDepth.Depth32Bit;
				this.GUI_treeView.ImageList.Images.Add(Tools.ExtractAssociatedIconEx("shell32.dll", 43));	//étoile
				this.GUI_treeView.ImageList.Images.Add(Tools.ExtractAssociatedIconEx("shell32.dll", 126));	//fichier sortant d'un dossier
				this.GUI_treeView.ImageList.Images.Add(Tools.ExtractAssociatedIconEx("shell32.dll", 3));	//dossier fermé
				this.GUI_treeView.ImageList.Images.Add(Tools.ExtractAssociatedIconEx("shell32.dll", 4));	//dossier ouvert
			}
			catch (Exception exc)
			{

				if (bDisplayExceptionsMBOX)
					MessageBox.Show(exc.Message, "Exception");

				if (bLogExceptions)
				{
					swExceptionsLog = File.AppendText(strExceptionsLogFilename);
					swExceptionsLog.Write(Environment.NewLine + exc.GetType().ToString() + " : " + exc.Message + Environment.NewLine);
					swExceptionsLog.Close();
				}

				this.GUI_treeView.ImageList.Images.Clear();
				this.GUI_treeView.ImageList = null;

			}

		} 
#endif
		#endregion Initialisation des composants pour l'affichage d'icônes dans le treeview

		#region Initialisation des composants graphiques optionnels

#if MODE_RECHERCHE
		/// <summary>
		/// Initialise les composants graphiques nécessaires à la recherche.
		/// </summary>
		/// <remarks>Séparé car la recherche est désactivable via MODE_RECHERCHE.</remarks>
		void InitialiseComposantsRecherche()
		{

			this.SuspendLayout();

			#region Elements extraits du Designer.cs

			// 
			// toolStripSeparator_menu_dossier
			// 
			this.toolStripSeparator_menu_dossier.Name = "toolStripSeparator_menu_dossier";
			this.toolStripSeparator_menu_dossier.Size = new System.Drawing.Size(181, 6);
			// 
			// rechercheToolStripMenuItem_dossier
			// 
			this.rechercheToolStripMenuItem_dossier.Name = "rechercheToolStripMenuItem_dossier";
			this.rechercheToolStripMenuItem_dossier.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.rechercheToolStripMenuItem_dossier.Size = new System.Drawing.Size(181, 22);
			this.rechercheToolStripMenuItem_dossier.Text = "Recherche";
			this.rechercheToolStripMenuItem_dossier.Click += new System.EventHandler(this.rechercheToolStripMenuItem_Click);

			// 
			// toolStripSeparator_menu_multi
			// 
			this.toolStripSeparator_menu_multi.Name = "toolStripSeparator_menu_multi";
			this.toolStripSeparator_menu_multi.Size = new System.Drawing.Size(181, 6);
			// 
			// rechercheToolStripMenuItem_multi
			// 
			this.rechercheToolStripMenuItem_multi.Name = "rechercheToolStripMenuItem_multi";
			this.rechercheToolStripMenuItem_multi.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.rechercheToolStripMenuItem_multi.Size = new System.Drawing.Size(181, 22);
			this.rechercheToolStripMenuItem_multi.Text = "Recherche";
			this.rechercheToolStripMenuItem_multi.Click += new System.EventHandler(this.rechercheToolStripMenuItem_Click);
			// 
			// GUI_panel_recherche
			// 
			this.GUI_panel_recherche.AutoSize = true;
			this.GUI_panel_recherche.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.GUI_panel_recherche.Controls.Add(this.GUI_button_recherche_ok);
			this.GUI_panel_recherche.Controls.Add(this.GUI_textBox_recherche);
			this.GUI_panel_recherche.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.GUI_panel_recherche.Location = new System.Drawing.Point(0, 252);
			this.GUI_panel_recherche.Margin = new System.Windows.Forms.Padding(0);
			this.GUI_panel_recherche.Name = "GUI_panel_recherche";
			this.GUI_panel_recherche.Size = new System.Drawing.Size(292, 21);
			this.GUI_panel_recherche.TabIndex = 3;
			this.GUI_panel_recherche.Visible = false;
			this.GUI_panel_recherche.Leave += new System.EventHandler(this.GUI_panel_recherche_Leave);
			// 
			// GUI_button_recherche_ok
			// 
			this.GUI_button_recherche_ok.Location = new System.Drawing.Point(260, -1);
			this.GUI_button_recherche_ok.Margin = new System.Windows.Forms.Padding(0);
			this.GUI_button_recherche_ok.Name = "GUI_button_recherche_ok";
			this.GUI_button_recherche_ok.Size = new System.Drawing.Size(32, 23);
			this.GUI_button_recherche_ok.TabIndex = 1;
			this.GUI_button_recherche_ok.Text = "OK";
			this.GUI_button_recherche_ok.UseVisualStyleBackColor = true;
			this.GUI_button_recherche_ok.Click += new System.EventHandler(this.GUI_button_recherche_ok_Click);
			this.GUI_button_recherche_ok.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));

			// 
			// GUI_textBox_recherche
			// 
			this.GUI_textBox_recherche.Location = new System.Drawing.Point(1, 1);
			this.GUI_textBox_recherche.Margin = new System.Windows.Forms.Padding(0);
			this.GUI_textBox_recherche.Name = "GUI_textBox_recherche";
			this.GUI_textBox_recherche.Size = new System.Drawing.Size(258, 20);
			this.GUI_textBox_recherche.TabIndex = 0;
			this.GUI_textBox_recherche.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GUI_textBox_recherche_KeyDown);
			this.GUI_textBox_recherche.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));

			this.toolTip1.SetToolTip(this.GUI_textBox_recherche, "Recherche en minuscules uniquement.Conversion effectuée automatiquement.");

			this.Controls.Add(this.GUI_panel_recherche);

			this.contextMenuStrip_dossier.Items.Add(this.toolStripSeparator_menu_dossier);
			this.contextMenuStrip_dossier.Items.Add(this.rechercheToolStripMenuItem_dossier);

			this.contextMenuStrip_multi.Items.Add(this.toolStripSeparator_menu_multi);
			this.contextMenuStrip_multi.Items.Add(this.rechercheToolStripMenuItem_multi);

			#endregion Elements extraits du Designer.cs

			#region Ajouts manuels après extraction

			#region Recherche depuis parent

			// 
			// rechercheDepuisParentToolStripMenuItem
			// 
			this.rechercheDepuisParentToolStripMenuItem.Name = "rechercheDepuisParentToolStripMenuItem";
			this.rechercheDepuisParentToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.rechercheDepuisParentToolStripMenuItem.Text = "Rechercher depuis noeud parent";
			this.rechercheDepuisParentToolStripMenuItem.Click += new System.EventHandler(this.rechercheDepuisParentToolStripMenuItem_Click);

			this.contextMenuStrip_dossier.Items.Add(this.rechercheDepuisParentToolStripMenuItem);

			#endregion Ajouts manuels après extraction

			#endregion Recherche depuis parent

			#region Option Ajout cumulatif

#if AJOUT_CUMULATIF

			// 
			// toolStripSeparator_dossier_ajout
			// 
			this.toolStripSeparator_dossier_ajout.Name = "toolStripSeparator_dossier_ajout";
			this.toolStripSeparator_dossier_ajout.Size = new System.Drawing.Size(181, 6);

			// 
			// modeAjout_dossierToolStripMenuItem
			// 
			this.modeAjout_dossierToolStripMenuItem.Name = "modeAjout_dossierToolStripMenuItem";
			this.modeAjout_dossierToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Add)));
			this.modeAjout_dossierToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.modeAjout_dossierToolStripMenuItem.Text = "Mode Ajout";
			this.modeAjout_dossierToolStripMenuItem.Click += new System.EventHandler(this.modeAjout_ToolStripMenuItem);

			// 
			// toolStripSeparator_multi_ajout
			// 
			this.toolStripSeparator_multi_ajout.Name = "toolStripSeparator_multi_ajout";
			this.toolStripSeparator_multi_ajout.Size = new System.Drawing.Size(181, 6);

			// 
			// modeAjout_multiToolStripMenuItem
			// 
			this.modeAjout_multiToolStripMenuItem.Name = "modeAjout_multiToolStripMenuItem";
			this.modeAjout_multiToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Add)));
			this.modeAjout_multiToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.modeAjout_multiToolStripMenuItem.Text = "Mode Ajout";
			this.modeAjout_multiToolStripMenuItem.Click += new System.EventHandler(this.modeAjout_ToolStripMenuItem);

			// 
			// toolStripSeparator_fichier_ajout
			// 
			this.toolStripSeparator_fichier_ajout.Name = "toolStripSeparator_fichier_ajout";
			this.toolStripSeparator_fichier_ajout.Size = new System.Drawing.Size(181, 6);

			// 
			// modeAjout_fichierToolStripMenuItem
			// 
			this.modeAjout_fichierToolStripMenuItem.Name = "modeAjout_fichierToolStripMenuItem";
			this.modeAjout_fichierToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Add)));
			this.modeAjout_fichierToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.modeAjout_fichierToolStripMenuItem.Text = "Mode Ajout";
			this.modeAjout_fichierToolStripMenuItem.Click += new System.EventHandler(this.modeAjout_ToolStripMenuItem);

			this.contextMenuStrip_dossier.Items.Add(this.toolStripSeparator_dossier_ajout);
			this.contextMenuStrip_dossier.Items.Add(this.modeAjout_dossierToolStripMenuItem);

			this.contextMenuStrip_multi.Items.Add(this.toolStripSeparator_multi_ajout);
			this.contextMenuStrip_multi.Items.Add(this.modeAjout_multiToolStripMenuItem);

			this.contextMenuStrip_fichier.Items.Add(this.toolStripSeparator_fichier_ajout);
			this.contextMenuStrip_fichier.Items.Add(this.modeAjout_fichierToolStripMenuItem);


#endif

			#endregion Option Ajout cumulatif

			#region Mode Nombres d'éléments

#if MODE_NB_ELEMENTS

			// 
			// toolStripSeparator_dossier_nbelem
			// 
			this.toolStripSeparator_dossier_nbelem.Name = "toolStripSeparator_dossier_nbelem";
			this.toolStripSeparator_dossier_nbelem.Size = new System.Drawing.Size(181, 6);

			// 
			// modeNbelem_dossierToolStripMenuItem
			// 
			this.modeNbelem_dossierToolStripMenuItem.Name = "modeNbelem_dossierToolStripMenuItem";
			this.modeNbelem_dossierToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.modeNbelem_dossierToolStripMenuItem.Text = "Mode Nombre d'éléments";
			this.modeNbelem_dossierToolStripMenuItem.Click += new System.EventHandler(this.modeNbelem_ToolStripMenuItem);

			// 
			// toolStripSeparator_multi_nbelem
			// 
			this.toolStripSeparator_multi_nbelem.Name = "toolStripSeparator_multi_nbelem";
			this.toolStripSeparator_multi_nbelem.Size = new System.Drawing.Size(181, 6);

			// 
			// modeNbelem_multiToolStripMenuItem
			// 
			this.modeNbelem_multiToolStripMenuItem.Name = "modeNbelem_multiToolStripMenuItem";
			this.modeNbelem_multiToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.modeNbelem_multiToolStripMenuItem.Text = "Mode Nombre d'éléments";
			this.modeNbelem_multiToolStripMenuItem.Click += new System.EventHandler(this.modeNbelem_ToolStripMenuItem);

			// 
			// toolStripSeparator_fichier_nbelem
			// 
			this.toolStripSeparator_fichier_nbelem.Name = "toolStripSeparator_fichier_nbelem";
			this.toolStripSeparator_fichier_nbelem.Size = new System.Drawing.Size(181, 6);

			// 
			// modeNbelem_fichierToolStripMenuItem
			// 
			this.modeNbelem_fichierToolStripMenuItem.Name = "modeNbelem_fichierToolStripMenuItem";
			this.modeNbelem_fichierToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.modeNbelem_fichierToolStripMenuItem.Text = "Mode Nombre d'éléments";
			this.modeNbelem_fichierToolStripMenuItem.Click += new System.EventHandler(this.modeNbelem_ToolStripMenuItem);

			this.contextMenuStrip_dossier.Items.Add(this.toolStripSeparator_dossier_nbelem);
			this.contextMenuStrip_dossier.Items.Add(this.modeNbelem_dossierToolStripMenuItem);

			this.contextMenuStrip_multi.Items.Add(this.toolStripSeparator_multi_nbelem);
			this.contextMenuStrip_multi.Items.Add(this.modeNbelem_multiToolStripMenuItem);

			this.contextMenuStrip_fichier.Items.Add(this.toolStripSeparator_fichier_nbelem);
			this.contextMenuStrip_fichier.Items.Add(this.modeNbelem_fichierToolStripMenuItem);

#endif

			#endregion Mode Nombres d'éléments

			this.ResumeLayout(false);
			this.PerformLayout();

		}

#endif

		#endregion Initialisation des composants graphiques optionnels


	}

}
