﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FolderSizesDisplay
{
	static class Program
	{
		/// <summary>
		/// Point d'entrée principal de l'application.
		/// </summary>
		[STAThread]
		static void Main(string[] arguments)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new FolderSizesDisplayWindow(arguments));
		}
	}
}
