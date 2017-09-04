/*
 * Creado por SharpDevelop.
 * Usuario: pikachu240
 * Fecha: 15/05/2017
 * Hora: 14:17
 * Licencia GNU GPL V3
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Gabriel.Cat.Extension;
namespace RomHackingWebConnector
{
	/// <summary>
	/// Interaction logic for WahackConnector.xaml
	/// </summary>
	public partial class WahackConnector : Connector
	{
		public const string WEB = "https://wahackforo.com";
		const string ENCONTRARNOMBREUSUARIO = "/u-";
		const string ENCONTRARIMGPERFIL = "profilepic";
		
		bool mostraPaginaAlUsuario;

		public WahackConnector():base(new Uri(WEB))
		{
			mostraPaginaAlUsuario=false;
			InitializeComponent();
			base.Conectado+=(s,e)=>{
				if(mostraPaginaAlUsuario)
					Hide();
			};
			base.Desconectado+=(s,e)=>{
				if(mostraPaginaAlUsuario)
					Show();
			};

		}

		public bool MostraPaginaAlUsuario {
			get {
				return mostraPaginaAlUsuario;
			}
			set {
				mostraPaginaAlUsuario = value;
			}
		}
		public override Uri PerfilUsuario
		{
			get{
				System.Windows.Forms.HtmlElementCollection linksCollection;
				if (base.perfilUsuario == null && EstaConectado) {
					linksCollection = WbLogin.Document.GetElementById("blackbar").GetElementsByTagName("a");
					for (int i = 0; i < linksCollection.Count && base.perfilUsuario == null; i++)
						if (linksCollection[i].GetAttribute("href").Contains(ENCONTRARNOMBREUSUARIO))
							perfilUsuario = new Uri(new Uri(WEB), linksCollection[i].GetAttribute("href"));
					
				}
				return perfilUsuario;
			}
			
		}
		
		public override Bitmap ImagenPerfil {
			get {
				
				WebClient wcPerfil;
				System.Windows.Forms.HtmlDocument htmlDoc;
				System.Windows.Forms.HtmlElementCollection linksCollection;
				string paginaPerfil;
				if (imgPerfil == null && User != null) {
					wcPerfil = new WebClient();
					paginaPerfil = wcPerfil.DownloadString(PerfilUsuario);
					htmlDoc = GetHtmlDocument(paginaPerfil);
					linksCollection = htmlDoc.GetElementById("collapseobj_stats_mini").GetElementsByTagName("img");
					for (int i = 0; i < linksCollection.Count && imgPerfil == null; i++)
						if (linksCollection[i].OuterHtml.Contains(ENCONTRARIMGPERFIL)) {
						imgPerfil= (Bitmap)Bitmap.FromStream(new MemoryStream(new WebClient().DownloadData(linksCollection[i].GetAttribute("href"))));
					}
				
					
				}
				return imgPerfil;
			}
		}


		

    	protected override void SalirForo()
		{
			WbLogin.Document.GetElementById("salir").InvokeMember("click");
		}
		protected override bool ComprobarQueEstaConectado()
		{
			return WbLogin.Document.GetElementById("memberbar").OuterHtml.Contains("salir");
		}
	}
}