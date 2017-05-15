/*
 * Creado por SharpDevelop.
 * Usuario: tetra
 * Fecha: 15/05/2017
 * Hora: 14:50
 * Licencia GNU GPL V3
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

namespace RomHackingWebConnector
{
	/// <summary>
	/// Description of Connector.
	/// </summary>
	public abstract class Connector:UserControl
	{
		protected Uri perfilUsuario;
		protected Bitmap imgPerfil;
		protected System.Windows.Forms.WebBrowser WbLogin{ get; private set; }
		public Uri WebRomHacking{ get; private set; }
		
		
		public  bool EstaConectado{ get; protected set; }
		
		public event EventHandler Conectado;
		public event EventHandler Desconectado;
		public Connector(Uri webRomHacking)
		{
			WindowsFormsHost winForms;
			winForms = new WindowsFormsHost();
			WbLogin = new System.Windows.Forms.WebBrowser();
			winForms.Child = WbLogin;
	    	this.AddChild(winForms);
			WbLogin.ScriptErrorsSuppressed = true;
			WbLogin.DocumentCompleted += PaginaCargada;
			WebRomHacking = webRomHacking;
			Hide();
			WbLogin.Navigate(webRomHacking);
			imgPerfil=null;
			EstaConectado=false;
			
		}
		public virtual Uri PerfilUsuario{
			get{
				return perfilUsuario;
			}
			protected set { perfilUsuario = value; }
		}
		public virtual Bitmap ImagenPerfil{
			get{
				return imgPerfil;
			}
			protected set { imgPerfil = value; }

		}
		public string User {
			get {
				return PerfilUsuario.Segments[PerfilUsuario.Segments.Length - 1];
			}
			
		}
		public void Hide()
		{
			WbLogin.Hide();
		}
		public void  Show()
		{
			WbLogin.Show();
		}
		void PaginaCargada(object sender, System.Windows.Forms.WebBrowserDocumentCompletedEventArgs e)
		{
			
			PaginaCargada();
			
			EstaConectado=ComprobarQueEstaConectado();
			
			if(Conectado!=null&&EstaConectado)
				Conectado(this,new EventArgs());
			
			else if(Desconectado!=null&&!EstaConectado)
				Desconectado(this,new EventArgs());
		}
		
		protected abstract void PaginaCargada();
		
		public void Salir()
		{
			if (EstaConectado) {
				SalirForo();
				Hide();
				//el usuario quizas rechaza algun mensaje lanzado por la web para evitar que se desconecte y al final no se desconecta
				EstaConectado = ComprobarQueEstaConectado();
			}
		}
		protected abstract bool ComprobarQueEstaConectado();
		protected abstract void SalirForo();
		
		public void Entrar()
		{
			if (!EstaConectado) {
				Visibility =System.Windows.Visibility.Visible; 
				WbLogin.Navigate(WebRomHacking);
			}
		}
		
		protected System.Windows.Forms.HtmlDocument GetHtmlDocument(string html)
		{//sacado de http://stackoverflow.com/questions/4935446/string-to-htmldocument
			System.Windows.Forms.WebBrowser browser = new System.Windows.Forms.WebBrowser();
			browser.ScriptErrorsSuppressed = true;
			browser.DocumentText = html;
			browser.Document.OpenNew(true);
			browser.Document.Write(html);
			browser.Refresh();
			return browser.Document;
		}
		

	}
}
