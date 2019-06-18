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

//Felipe Melchior de Britto  RA:18200
//Gabrielle da Silva Barbosa RA:18200
namespace apCaminhosMarte
{
    public partial class Form1 : Form
    {
        Arvore<Cidade> cidades;                   //arvore com todas as cidades de Marte
        int[,] adjacencias;                       //matriz qque armazena as conexões(caminhos) entre cidades, possuindo em determinado campo [x,y] a quilometragem necessária a se percorrer
        PilhaLista<int> caminho;                  //pilha que guarda as ids de cidades que formam em sequência um caminho
        bool[] cidadesPercorridas;                //vetor booleano, no qual o index representa a id de uma cidade, portanto ao acessar determinado index saberemos se esta cidade já foi percorrida
        bool percorreuTodosOsCaminhosPossiveis;   //informa se todos os caminhos existentes foram percorridos
        List<PilhaLista<int>> caminhosPossiveis;  //lista que armazena todos os caminhosPossiveis
        PilhaLista<int> caminhoASerMostrado;      //variável que guarda o caminhoASerMostrado, no caso o caminho que se quer ser desenhado

        public Form1()
        {
            InitializeComponent();
        }


        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            //verificamos se duas cidades foram selecionadas, caso contrário avisamos o usuário
            if (lsbOrigem.SelectedItem != null && lsbDestino.SelectedItem != null)
            {
                //pegamos o índice das cidades de origem e destino disponíveis nos lsbs
                int idCidadeOrigem = Int32.Parse(lsbOrigem.SelectedIndex.ToString().Split('-')[0]);
                int idCidadeDestino = Int32.Parse(lsbDestino.SelectedIndex.ToString().Split('-')[0]);

                //verificamos se origem e destino são iguais
                if (idCidadeOrigem == idCidadeDestino)
                {
                    MessageBox.Show("Seu ponto de partida é o mesmo que o seu destino!");
                }
                else
                {
                    //instanciamos as variáveis que serão utilizadas neste método
                    cidadesPercorridas = new bool[cidades.QuantosDados];
                    caminhosPossiveis = new List<PilhaLista<int>>();
                    caminho = new PilhaLista<int>();
                    percorreuTodosOsCaminhosPossiveis = false;

                    //chamamos o método buscarCaminhos com base no destino desejado e na cidade de origem
                    BuscarCaminhos(idCidadeOrigem, idCidadeDestino, 0);

                    //enquanto houver caminhos a serem percorridos, voltamos uma posição(uma cidade) e chamamos o método Retornar
                    //este método chamará novamente BuscarCaminhos, desempilhando uma cidade
                    //ou seja, voltando uma posição para verificar a existência de outros caminhos para o mesmo local
                    while (!percorreuTodosOsCaminhosPossiveis)
                    {
                        int idOrigem = caminho.Desempilhar();
                        Retornar(idOrigem, idCidadeDestino);
                    }
                    //ordenamos os caminhos encontrados do melhor para o pior 
                    OrdenarCaminhos();
                    //exibimos os caminhos no gridView
                    ExibirCaminhos();
                    //o caminho a ser mostrado(desenhado) por padrão será o do index 0 do dgv, pois de acordo com a ordenação, este trata-se do melhor caminho
                    caminhoASerMostrado = caminhosPossiveis[0].Clone();
                    //permitimos que o mapa seja redesenhado
                    pbMapa.Invalidate();
                }
            }
            else
            {
                MessageBox.Show("Por favor selecione uma origem e um destino");
            }
        }

        //método que busca todos os caminhos possíveis para um determinado destino
        private void BuscarCaminhos(int idOrigem, int idDestino, int indiceInicial)  //parâmetros: id cidade origem, id cidade destino, índice inicial para percorrer na matriz de adjacências 
        {
            //se a origem for igual ao destino, por se tratar de um método recursivo com a origem mudando de acordo com as cidades percorridas
            //empilhamos o id da cidade destino na pilha
            //adicionamos o caminho na lista de caminhos possíveis, clonando-o já que o reutilizamos para achar outros caminhos 
            if (idOrigem == idDestino)
            {
                caminho.Empilhar(idDestino);
                caminhosPossiveis.Add(caminho.Clone());
            }
            //caso a origem seja diferente do destino
            else
            {
                //variável que será utilizada para a realização de verificações, recebendo os índices das cidades
                int c = -1;

                //até achar uma adjacência (conexão entre cidades) ou acabar as cidades da linha da matriz
                for (int i = indiceInicial; i < adjacencias.GetLength(0); i++)
                {
                    //se há uma conexão e esta cidade não foi percorrida anteriormente neste mesmo caminho encontrado
                    if (adjacencias[idOrigem, i] != 0 && cidadesPercorridas[i] == false)
                    {
                        //"c" recebe o id da cidade a ser percorrida
                        c = i;
                        break;
                    }
                }
                //não é possível chegar ao destino no caminho encontrado ou todas as possibilidades de caminho por esta cidade já foram encontradas
                if (c == -1)
                {
                    //ele voltou para a cidade origem, portanto já encontrou todos os caminhos
                    if (caminho.EstaVazia())
                        percorreuTodosOsCaminhosPossiveis = true;

                    //se ainda existem opções, volta uma posição(uma cidade) para procurar outra possibilidade
                    else
                        Retornar(idOrigem, idDestino);
                }
                else
                {
                    //definimos que a cidade foi percorrida
                    cidadesPercorridas[idOrigem] = true;
                    //empilhamos o id da cidade usada no método
                    caminho.Empilhar(idOrigem);
                    //buscamos um novo caminho a partir da última cidade encontrada
                    BuscarCaminhos(c, idDestino, 0);
                }
            }
        }

        //método que faz com que a busca por caminho volte para a cidade anterior à posição atual
        private void Retornar(int idOrigem, int idDestino)
        {
            //desempilhamos uma cidade 
            int c = caminho.Desempilhar();
            //fazemos com que a cidade de 'idOrigem' em cidadesPercorridas fique false, pois ao voltar é como se ela nunca tivesse sido percorrida
            cidadesPercorridas[idOrigem] = false;
            //buscamos novamente o caminho
            BuscarCaminhos(c, idDestino, idOrigem + 1);
        }

        //ao iniciar o programa
        private void Form1_Load(object sender, EventArgs e)
        {
            //limpamos os itens existentes nos lsbs
            lsbOrigem.Items.Clear();
            lsbDestino.Items.Clear();
            //instanciamos variáveis
            caminho = new PilhaLista<int>();
            cidades = new Arvore<Cidade>();

            //leitura do arquivo de cidades de acordo com o método feito na classe Cidade
            //incluimos as cidades na árvore de cidades
            StreamReader arq = new StreamReader("CidadesMarte.txt");
            while (!arq.EndOfStream)
            {
                Cidade cid = Cidade.LerArquivo(arq);
                cidades.Incluir(cid);
            }
            arq.Close();

            //lemos o arquivo ordenado para inclusão das cidades nos lsbs de origem e destino 
            arq = new StreamReader("CidadesMarteOrdenado.txt");
            while (!arq.EndOfStream)
            {
                Cidade cid = Cidade.LerArquivo(arq);
                lsbOrigem.Items.Add(cid.IdCidade + "-" + cid.NomeCidade);
                lsbDestino.Items.Add(cid.IdCidade + "-" + cid.NomeCidade);
            }
            arq.Close();

            //criamos uma matriz de cidades de acordo com a quantidade de cidades existentes 
            adjacencias = new int[cidades.QuantosDados, cidades.QuantosDados];
            //lemos o arquivo que nos passa os caminhos entre as cidades
            arq = new StreamReader("CaminhosEntreCidadesMarte.txt");

            //cria-se uma variável que recebe o caminho entre as cidades
            //guarda-se a distância do caminho na matriz de acordo com a cidade de origem e cidade de destino
            //[idOrigem, idDestino] == [x,y]
            while (!arq.EndOfStream)
            {
                CaminhoEntreCidades caminho = CaminhoEntreCidades.LerArquivo(arq);
                adjacencias[caminho.IdCidadeOrigem, caminho.IdCidadeDestino] = caminho.Distancia;
                adjacencias[caminho.IdCidadeDestino, caminho.IdCidadeOrigem] = caminho.Distancia;
            }
            arq.Close();

            //pedimos para que o mapa seja redesenhado
            pbMapa.Invalidate();
        }

        //desenha uma cidade no mapa de acordo com a cidade passada como parâmetro
        private void DesenharCidade(NoArvore<Cidade> c, Graphics gr)
        {
            if (c != null)
            {
                SolidBrush caneta = new SolidBrush(Color.Black);
                DesenharCidade(c.Esq, gr);
                DesenharCidade(c.Dir, gr);

                //conversão das coordenadas da cidade de acordo com o tamnho do mapa
                int x = (c.Info.CoordenadaX * pbMapa.Width) / 4096;
                int y = (c.Info.CoordenadaY * pbMapa.Height) / 2048;

                //desenha-se um ponto preto representando a localização da cidade
                gr.FillEllipse(caneta, x, y, 6, 6);
                //escreve no mapa nos seus respectivos locais o nome da cidade
                gr.DrawString(c.Info.NomeCidade, new Font("Bauhaus 93", 11),
                              new SolidBrush(Color.Black), x - 20, y - 15);
            }


        }

        //método para desenhar a árvore de cidades com base no método fornecido a nós pelo professor
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

                DesenharArvore(false, raiz.Esq, xf, yf, Math.PI / 2 + incremento,
                                                 incremento * 0.60, comprimento * 0.8, g);
                DesenharArvore(false, raiz.Dir, xf, yf, Math.PI / 2 - incremento,
                                                  incremento * 0.60, comprimento * 0.8, g);

                SolidBrush preenchimento = new SolidBrush(Color.Cyan);
                g.FillEllipse(preenchimento, xf - 15, yf - 15, 30, 30);
                g.DrawString(raiz.Info.IdCidade + "-" + raiz.Info.NomeCidade, new Font("Comic Sans", 12),
                              new SolidBrush(Color.Blue), xf - 15, yf - 10);
            }
        }

        //evento que efetivamente chama os métodos que desenham as cidades no mapa e o caminho entre elas
        //permite a utilização de "Graphics" de pbMapa
        private void pbMapa_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            DesenharCidade(cidades.Raiz, gr);
            if (caminhoASerMostrado != null)
                DesenharCaminho(caminhoASerMostrado.Clone(), e.Graphics);

        }

        //evento que efetivamente chama o métodos para desenhar a árvore de cidades e nos permite a utilização da classe Graphics necessária para desenhara árvore de cidades
        private void tpArvore_Paint(object sender, PaintEventArgs e)
        {
            Graphics gr = e.Graphics;
            DesenharArvore(true, cidades.Raiz, (int)tpArvore.Width / 2, 0, Math.PI / 2,
                                 Math.PI / 2.5, 300, gr);
        }

        private void DesenharCaminho(PilhaLista<int> caminho, Graphics g) //desenha com base em um caminho selecionado nos DGVs ou com base no melhorCaminho
        {
            //classe que será utilizada para desenhar
            Pen caneta = new Pen(Color.Black);
            caneta.Width = 2.0F;

            //desempilhamos de caminho uma id de uma cidade, verificando sua existência na árvore de cidades
            // caso exista, a variável cidadeAnterior receberá todas as informações desta cidade
            int idCidadeAnterior = caminho.Desempilhar();
            cidades.Existe(new Cidade(idCidadeAnterior, " ", 0, 0));
            Cidade cidadeAnterior = cidades.Atual.Info;

            //enquanto houver cidades a serem percorridas
            while (!caminho.EstaVazia())
            {
                //desempilhamos de caminho uma id de uma cidade, verificando sua existência na árvore de cidades
                // caso exista, a variável cidade receberá todas as informações desta cidade
                int idCidade = caminho.Desempilhar();
                cidades.Existe(new Cidade(idCidade, " ", 0, 0));
                Cidade cidade = cidades.Atual.Info;

                //refazemos as proporções do mapa de acordo com as coordenadas da cidade
                int x1 = (cidadeAnterior.CoordenadaX * pbMapa.Width) / 4096;
                int y1 = (cidadeAnterior.CoordenadaY * pbMapa.Height) / 2048;

                //refazemos as proporções do mapa de acordo com as coordenadas da cidade
                int x2 = (cidade.CoordenadaX * pbMapa.Width) / 4096;
                int y2 = (cidade.CoordenadaY * pbMapa.Height) / 2048;

                if(x1-x2 > 0 && x1-x2 > (pbMapa.Width-x1)+x2)
                {
                    g.DrawLine(caneta, x1, y1, pbMapa.Width, (y1 + y2) / 2);
                    g.DrawLine(caneta, x2, y2, 0, (y1 + y2) / 2);
                }
                else if(x2 - x1 > 0 && x2 - x1 > (pbMapa.Width - x2) + x1)
                {
                    g.DrawLine(caneta, x1, y1, 0, (y1 + y2) / 2);
                    g.DrawLine(caneta, x2, y2, pbMapa.Width, (y1 + y2) / 2);
                }
                else
                {
                    //desenhamos uma linha entre as duas cidades, cidadeAnterior representando a posição atual, e cidade representando aonde é necessário ir 
                    g.DrawLine(caneta, x1, y1, x2, y2); //utilizando as localizações "x" e "y" das cidades
                }

                //fazemos com que o destino se torne a origem, para refazer o processo
                cidadeAnterior = cidade;
            }
        }


        private void ExibirCaminhos() //exibe todos os caminhos possíveis no dataGridView de caminhos 
        {
            //instanciamos variáveis que serão utilizadas para definir as linhas e colunas do dgv
            int linha = 0;
            int coluna;

            dataGridView1.RowCount = caminhosPossiveis.Count; //quantidade de linhas é igual à quantidade de caminhos existentes
            dataGridView1.ColumnCount = 1;// o mesmo será feito com as colunas, mas apenas ao encontrarmos o caminho com a maior quantidade de cidades

            //para cada caminho possível, criaremos clones das pilhas para não estragarmos as originais
            foreach (var caminho in caminhosPossiveis)
            {

                if (caminho.Tamanho() > dataGridView1.ColumnCount)
                    dataGridView1.ColumnCount = caminho.Tamanho();

                //instanciamos uma quantidade de colunas e clonamos o caminho atual para que o mesmo não seja desmontado
                coluna = 0;
                var aux = caminho.Clone();

                //desempilhamos a variável auxiliar em outra variável
                //pois ao desempilhar, invertemos a pilha
                //ao desempilhar novamente, voltamos a sua forma original
                PilhaLista<int> aux2 = new PilhaLista<int>();
                while (!aux.EstaVazia())
                    aux2.Empilhar(aux.Desempilhar());


                //enquanto houver cidades a serem mostradas
                //desempilhamos um código de cidade verificando sua existência na árvore de cidades
                //em seguida utilizamos a variável atual para recuperar o nome da cidade desempilhada e escrevê-lo na respectiva célula de acordo com "coluna" e "linha"
                while (!aux2.EstaVazia())
                {
                    cidades.Existe(new Cidade(aux2.Desempilhar(), "", 0, 0));
                    dataGridView1.Columns[coluna].HeaderText = "Cidade";
                    dataGridView1.Rows[linha].Cells[coluna].Value = cidades.Atual.Info.NomeCidade;
                    coluna++;
                }
                linha++;
            }


            //encontramos o melhorCaminho
            PilhaLista<int> melhorCaminho = caminhosPossiveis[0].Clone();
            var auxiliar = new PilhaLista<int>();

            //instanciamos as linhas e colunas do dgv
            dataGridView2.RowCount = 1;
            dataGridView2.ColumnCount = melhorCaminho.Tamanho();

            //desempilhamos as ids de cidade e as empilhamos em uma variável auxiliar
            while (!melhorCaminho.EstaVazia())
                auxiliar.Empilhar(melhorCaminho.Desempilhar());
            //enquanto houver cidades a serem mostradas
            //desempilhamos um código de cidade verificando sua existência na árvore de cidades
            //em seguida utilizamos a variável atual para recuperar o nome da cidade desempilhada e escrevê-lo na respectiva célula de acordo com sua "coluna"
            coluna = 0;
            while (!auxiliar.EstaVazia())
            {

                cidades.Existe(new Cidade(auxiliar.Desempilhar(), "", 0, 0));
                dataGridView2.Columns[coluna].HeaderText = "Cidade";
                dataGridView2.Rows[0].Cells[coluna].Value = cidades.Atual.Info.NomeCidade;
                coluna++;
            }


        }

        

        private void dataGridView1_CellEnter(object sender, DataGridViewCellEventArgs e) //quando uma célula específica do dgv receber foco de entrada
        {
            //utilizamos o index da linha para encontrar a qual caminho ela se refere
            //pois os index do dgv estão de acordo com a List caminhos possíveis
            caminhoASerMostrado = caminhosPossiveis[int.Parse(e.RowIndex.ToString())].Clone();
            //permissão para redesenhar o mapa
            pbMapa.Invalidate();
        }

        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)  //quando uma célula específica do dgv receber foco de entrada
        {
            //este dataGridView mostra apenas o melhor caminho
            caminhoASerMostrado = caminhosPossiveis[0].Clone();  //portanto o caminho a ser mostrado recebe a função que acha o melhor caminho
            pbMapa.Invalidate();                            //permite com que o pbMapa seja redesenhado, chamando o evento paint
        }

        private void OrdenarCaminhos()
        {
            int[] distancias = new int[caminhosPossiveis.Count];

            for (int i = 0; i < caminhosPossiveis.Count; i++)
            {
                PilhaLista<int> aux = caminhosPossiveis[i].Clone();
                int caminho = 0;
                int anterior = aux.Desempilhar();
                while (!aux.EstaVazia())
                {
                    int cidade = aux.Desempilhar();
                    caminho += adjacencias[anterior, cidade];
                    anterior = cidade;
                }
                distancias[i] = caminho;
            }

            int quantosCaminhosForam = 0;
            int indiceMenorCaminho = 0;
            while(quantosCaminhosForam < caminhosPossiveis.Count)
            {
                int menorCaminho = int.MaxValue;

                for (int i = quantosCaminhosForam; i < caminhosPossiveis.Count; i++)
                    if (distancias[i] < menorCaminho)
                    {
                        menorCaminho = distancias[i];
                        indiceMenorCaminho = i;
                    }

                PilhaLista<int> melhor = caminhosPossiveis[indiceMenorCaminho].Clone();
                caminhosPossiveis.RemoveAt(indiceMenorCaminho);
                caminhosPossiveis.Insert(quantosCaminhosForam, melhor);
                quantosCaminhosForam++;
            }

        }

        //método não é mais utilizado por conta de usarmos o ordenar caminhos, que deixa o melhor caminho sempre na primeira posição
        private PilhaLista<int> SelecionarMelhorCaminho()   //retorna um inteiro o qual é o índice do melhor caminho guardado para a cidade desejada
        {
            
            PilhaLista<int> melhorCaminho = new PilhaLista<int>();//criamos uma nova pilha a qual será retornada no final do método contendo o melhor caminho possível
            int distanciaTotal = 0;                               //distanciaTotal receberá a soma da distância percorrida em um caminho 
            int menorDistancia = int.MaxValue;                    //menorDistância será usada para definir qual caminho utilizar, comparando as distâncias entre dois caminhos

            foreach (var caminho in caminhosPossiveis)
            {
                distanciaTotal = 0;
                var aux = caminho.Clone();
                int anterior = aux.Desempilhar(); //variável que será utilizada como a origem, em qual cidade se está atualmente

                //enquanto ainda houver cidades a serem percorridas
                while (!aux.EstaVazia())
                { 
                    int cidade = aux.Desempilhar();//desempilharemos o id de uma cidade
                    //somaremos o valor de distância da cidade que teve o id desempilhado através da matriz adjacências
                    //que possue como valor a distância a ser percorrida de seu ponto de origem até seu ponto de destino
                    distanciaTotal += adjacencias[anterior, cidade];
                    anterior = cidade; //fazemos com que a variável de origem receba o destino, de certa forma como se o usuário tivesse percorrido o caminho até a cidade
                }


                //se a menorDistancia for maior do que a do caminhoPercorrido
                //o caminhoPercorrido pelo programa torna-se o melhorCaminho 
                if (menorDistancia > distanciaTotal) 
                {
                    menorDistancia = distanciaTotal;
                    melhorCaminho = caminho.Clone();
                }
            }
            return melhorCaminho;
        } 
    }
}
