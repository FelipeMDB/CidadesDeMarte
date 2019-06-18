using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


//Felipe Melchior de Britto  RA:18200
//Gabrielle da Silva Barbosa RA:18200
namespace apCaminhosMarte
{
    class CaminhoEntreCidades
    {
        //declaração das variáveis que serão utilizadas para a leitura do arquivo contendo os caminhos entre as cidades 
        int idCidadeOrigem;
        const int inicioIdCidadeOrigem = 0;
        const int tamanhoIdCidadeOrigem = 3;

        int idCidadeDestino;
        const int inicioIdCidadeDestino = tamanhoIdCidadeOrigem;
        const int tamanhoIdCidadeDestino = 3;

        int distancia;
        const int inicioDistancia = inicioIdCidadeDestino + tamanhoIdCidadeDestino; 
        const int tamanhoDistancia = 5;

        int tempo;
        const int inicioTempo = inicioDistancia + tamanhoDistancia;
        const int tamanhoTempo = 4;

        int custo;
        const int inicioCusto = inicioTempo + tamanhoTempo;
        const int tamanhoCusto = 5;


        //getters e setters das variáveis globais da classe
        public int Custo { get => custo; set => custo = value; }
        public int Tempo { get => tempo; set => tempo = value; }
        public int Distancia { get => distancia; set => distancia = value; }
        public int IdCidadeDestino { get => idCidadeDestino; set => idCidadeDestino = value; }
        public int IdCidadeOrigem { get => idCidadeOrigem; set => idCidadeOrigem = value; }


        //construtor de um caminho entre duas cidades específicas
        public CaminhoEntreCidades(int idCidOrigem, int idCidDestino, int distancia, int tempo, int custo )
        {
            IdCidadeOrigem = idCidOrigem;
            IdCidadeDestino = idCidDestino;
            Distancia = distancia;
            Tempo = tempo;
            Custo = custo;
        }

        //método que realiza a leitura do arquivo, convertendo strings para as respectivas variáveis globais 
        public static CaminhoEntreCidades LerArquivo(StreamReader arq)
        {
            string linha = arq.ReadLine();
            int idCidadeOrigem = int.Parse(linha.Substring(inicioIdCidadeOrigem, tamanhoIdCidadeOrigem));
            int idCidadeDestino = int.Parse(linha.Substring(inicioIdCidadeDestino, tamanhoIdCidadeDestino));
            int distancia = int.Parse(linha.Substring(inicioDistancia, tamanhoDistancia));
            int tempo = int.Parse(linha.Substring(inicioTempo, tamanhoTempo));
            int custo = int.Parse(linha.Substring(inicioCusto, tamanhoCusto));
            return new CaminhoEntreCidades(idCidadeOrigem, idCidadeDestino, distancia, tempo, custo);
        }

    }
}
