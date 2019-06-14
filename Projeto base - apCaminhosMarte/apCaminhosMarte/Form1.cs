using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace apCaminhosMarte
{
    public partial class Form1 : Form
    {
        Arvore<Cidade> cidades;
        List<PilhaLista<int>> caminhosPossiveis;
        int[,] adjacencias;
        PilhaLista<int> caminho;
        bool[] cidadesPercorridas;
        bool percorreuTodosOsCaminhosPossiveis;

        public Form1()
        {
            InitializeComponent();
        }

        private void TxtCaminhos_DoubleClick(object sender, EventArgs e)
        {

        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Buscar caminhos entre cidades selecionadas");
            int idCidadeOrigem = Int32.Parse(lsbOrigem.SelectedIndex.ToString().Split('-')[0]);
            int idCidadeDestino = Int32.Parse(lsbDestino.SelectedIndex.ToString().Split('-')[0]);

            cidadesPercorridas = new bool[23];
            caminhosPossiveis = new List<PilhaLista<int>>();

            BuscarCaminhos(idCidadeOrigem, idCidadeDestino, 0);
            while (!percorreuTodosOsCaminhosPossiveis)
            {
                //AcharOutroCaminhoPossivel();
                int idOrigem = caminho.Desempilhar();
                Retornar(idOrigem, idCidadeDestino);
            }

            string s="";
            while(!caminho.EstaVazia())
            {
                s+=caminho.Desempilhar().ToString()+" ";
            }
            MessageBox.Show(s);

        }

        private void BuscarCaminhos(int idOrigem, int idDestino, int indiceInicial)
        {
            if(idOrigem == idDestino)
            {
                caminho.Empilhar(idDestino);
                caminhosPossiveis.Add(caminho.Clone());
            }
            else 
            {
                int c = -1;
                for (int i = indiceInicial; i < adjacencias.GetLength(0); i++)
                {
                    if (adjacencias[idOrigem, i] != 0 && cidadesPercorridas[i] == false)
                    {
                        if (caminho.EstaVazia() || i != caminho.OTopo())
                        {
                            c = i;
                            break;
                        }
                    }
                }
                if (c == -1)
                {
                    if (caminho.EstaVazia())
                        percorreuTodosOsCaminhosPossiveis = true;
                    else
                        Retornar(idOrigem, idDestino);
                }
                else
                {
                    cidadesPercorridas[idOrigem] = true;
                    caminho.Empilhar(idOrigem);
                    BuscarCaminhos(c, idDestino, 0);
                }
            }
        }

        private void Retornar(int idOrigem, int idDestino)
        {
            int c = caminho.Desempilhar();
            BuscarCaminhos(c, idDestino, idOrigem + 1);
        }
        
        //private void AcharOutroCaminhoPossivel()
        //{
        //    int c = caminho.Desempilhar();
        //}

        private void Form1_Load(object sender, EventArgs e)
        {
            lsbOrigem.Items.Clear();
            lsbDestino.Items.Clear();
            caminho = new PilhaLista<int>();
            cidades = new Arvore<Cidade>();
            StreamReader arq = new StreamReader("CidadesMarte.txt");

            while (!arq.EndOfStream)
            {
                Cidade cid = Cidade.LerArquivo(arq);
                cidades.Incluir(cid);
            }
            arq.Close();

            arq = new StreamReader("CidadesMarteOrdenado.txt");
            while (!arq.EndOfStream)
            {
                Cidade cid = Cidade.LerArquivo(arq);
                lsbOrigem.Items.Add(cid.IdCidade + "-" + cid.NomeCidade);
                lsbDestino.Items.Add(cid.IdCidade + "-" + cid.NomeCidade);
            }
            arq.Close();

            adjacencias = new int[cidades.QuantosDados,cidades.QuantosDados];
            arq = new StreamReader("CaminhosEntreCidadesMarte.txt");
            while(!arq.EndOfStream)
            {
                CaminhoEntreCidades caminho = CaminhoEntreCidades.LerArquivo(arq);
                adjacencias[caminho.IdCidadeOrigem, caminho.IdCidadeDestino] = caminho.Distancia;
                adjacencias[caminho.IdCidadeDestino, caminho.IdCidadeOrigem] = caminho.Distancia;
            }
            arq.Close();

            pbMapa.Invalidate();
        }

        private void DesenharCidade(NoArvore<Cidade> c, Graphics gr)
        {
            if (c != null)
            {
                SolidBrush caneta = new SolidBrush(Color.Black);
                DesenharCidade(c.Esq, gr);
                DesenharCidade(c.Dir, gr);
                int x = (c.Info.CoordenadaX * pbMapa.Width) / 4096;
                int y = (c.Info.CoordenadaY * pbMapa.Height) / 2048;
                gr.FillEllipse(caneta,x,y,6,6);
                gr.DrawString(c.Info.NomeCidade, new Font("Bauhaus 93", 11),
                              new SolidBrush(Color.Black), x - 20, y - 15);
            }


        }

        private void DesenharArvore(bool primeiraVez, NoArvore<Cidade> raiz,
                           int x, int y, double angulo, double incremento,
                           double comprimento, Graphics g)
        {
            int xf, yf;
            if (raiz != null)
            {
                Pen caneta = new Pen(Color.Black);
                xf = (int)Math.Round(x + Math.Cos(angulo) * comprimento);
                yf = (int)Math.Round(y + Math.Sin(angulo) * comprimento);
                if (primeiraVez)
                    yf = 25;
                g.DrawLine(caneta, x, y, xf, yf);
                // sleep(100);
                DesenharArvore(false, raiz.Esq, xf, yf, Math.PI / 2 + incremento,
                                                 incremento * 0.60, comprimento * 0.8, g);
                DesenharArvore(false, raiz.Dir, xf, yf, Math.PI / 2 - incremento,
                                                  incremento * 0.60, comprimento * 0.8, g);
                // sleep(100);
                SolidBrush preenchimento = new SolidBrush(Color.Cyan);
                g.FillEllipse(preenchimento, xf - 15, yf - 15, 30, 30);
                g.DrawString(raiz.Info.NomeCidade, new Font("Comic Sans", 12),
                              new SolidBrush(Color.Blue), xf - 15, yf - 10);
            }
        }

        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            DesenharCidade(cidades.Raiz, gr);
        }

        private void tpArvore_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            DesenharArvore(true, cidades.Raiz, (int)tpArvore.Width / 2, 0, Math.PI / 2,
                                 Math.PI / 2.5, 300, gr);
        }
    }
}
