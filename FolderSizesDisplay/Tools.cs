#define ICONES_SUR_TREEVIEW //activer simultanément dans FolderSizesDisplayWindow.cs pour avoir les icônes
#define MODE_NB_ELEMENTS //activer simultanément dans FolderSizesDisplayWindow.cs

namespace FolderSizesDisplay
{
	class Tools
	{

		#region Méthode accessoire Generer_taille_fichier_lisible

		public static string Generer_taille_fichier_lisible(System.Int64 taille_fichier, System.Globalization.CultureInfo app_culture )
		{

			string return_value;

			if (taille_fichier < 0)
				return_value = "(-1)";
			else
			if (taille_fichier < 1000)
				return_value = "(1 ko)";
			else
			{

				string local_NumberGroupSeparator;

				if (app_culture == null)
					local_NumberGroupSeparator = ",";
				else
					local_NumberGroupSeparator = app_culture.NumberFormat.NumberGroupSeparator;

				return_value = (taille_fichier / 1000).ToString();

				/*
				 (nCompteSeparateurs - 1)	== 
											nombre de séparateurs déjà ajoutés, à retrancher pour ne compter que les chiffres
				 (3 * nCompteSeparateurs)	== 
											nombre de chiffres au-delà duquel il faut un sépareur, augmenté après un passages.
				 ((nCompteSeparateurs * 3) + nCompteSeparateurs - 1) ==
											distance du dernier caractère à laquelle insérer le séparateur (donc retranché à length)
				*/

				for (int nCompteSeparateurs = 1; (return_value.Length - (nCompteSeparateurs - 1)) > (3 * nCompteSeparateurs); nCompteSeparateurs++)
				{
					return_value = return_value.Insert(
						return_value.Length - ((nCompteSeparateurs * 3) + nCompteSeparateurs - 1),
						local_NumberGroupSeparator
						);
				}

				return_value = "(" + return_value + " ko)";

			}

			return return_value;

		}

		#endregion Méthode accessoire Generer_taille_fichier_lisible

		#region Méthode accessoire Generer_nombre_fichiers_lisible

		public static string Generer_nombre_fichiers_lisible(System.Int64 nb_elem, System.Globalization.CultureInfo app_culture)
		{

			string return_value;

			if (nb_elem < 0)
				return_value = "(??? fichiers)";
			else
			{

				string local_NumberGroupSeparator;

				if (app_culture == null)
					local_NumberGroupSeparator = ",";
				else
					local_NumberGroupSeparator = app_culture.NumberFormat.NumberGroupSeparator;

				return_value = nb_elem.ToString();

				/*
				 (nCompteSeparateurs - 1)	== 
											nombre de séparateurs déjà ajoutés, à retrancher pour ne compter que les chiffres
				 (3 * nCompteSeparateurs)	== 
											nombre de chiffres au-delà duquel il faut un sépareur, augmenté après un passages.
				 ((nCompteSeparateurs * 3) + nCompteSeparateurs - 1) ==
											distance du dernier caractère à laquelle insérer le séparateur (donc retranché à length)
				*/

				for (int nCompteSeparateurs = 1; (return_value.Length - (nCompteSeparateurs - 1)) > (3 * nCompteSeparateurs); nCompteSeparateurs++)
				{
					return_value = return_value.Insert(
						return_value.Length - ((nCompteSeparateurs * 3) + nCompteSeparateurs - 1),
						local_NumberGroupSeparator
						);
				}

				return_value = "(" + return_value + " fichiers)";

			}

			return return_value;

		}

		#endregion Méthode accessoire Generer_nombre_fichiers_lisible

		#region Trucs utilisés pour obtenir des icônes

		/// <summary>
		/// The ExtractAssociatedIconEx function returns a copy of 
		/// an indexed icon found in a file or an icon found in an 
		/// associated executable file. 
		/// </summary>
		/// <param name="strIconPath">
		/// String that specifies the full 
		/// path and file name of the file that contains the icon. 
		/// The function extracts the icon from that file, or from an 
		/// executable file associated with that file. 
		/// </param>
		/// <param name="nIndex">
		/// Pointer to a WORD that specifies the index of the icon.
		/// </param>
		/// <returns>
		/// -Icon which is a copy of the one extracted by handle 
		/// from the file.
		/// -null if it didn't work.
		/// </returns>

#if ICONES_SUR_TREEVIEW

		public static System.Drawing.Icon ExtractAssociatedIconEx(string strIconPath, System.UInt16 nIndex)
		{

			System.Drawing.Icon return_value = null;
			System.IntPtr hIcon = System.IntPtr.Zero;

			hIcon = Fonctions_Win32.ExtractAssociatedIcon(System.IntPtr.Zero, strIconPath, out nIndex);

			try
			{
				return_value = (System.Drawing.Icon.FromHandle(hIcon)).Clone() as System.Drawing.Icon;
			}
			catch (System.Exception exc)
			{
				System.Windows.Forms.MessageBox.Show(exc.Message);
			}
			finally
			{
				if (hIcon != System.IntPtr.Zero)
					Fonctions_Win32.DestroyIcon(hIcon);
			}

			return return_value;

		}

		class Fonctions_Win32
		{

			[System.Runtime.InteropServices.DllImport("user32.dll")]
			public static extern bool DestroyIcon(System.IntPtr hIcon);

			[System.Runtime.InteropServices.DllImport("shell32.dll")]
			public static extern System.IntPtr ExtractAssociatedIcon(System.IntPtr hInst, string lpIconPath, out System.UInt16 lpiIconIndex);

		}

#endif

		#endregion Trucs utilisés pour obtenir des icônes

	}
}
