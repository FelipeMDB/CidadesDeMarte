using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace apCaminhosMarte
{
    class Cidade : IComparable<Cidade>
    {
        int idCidade;
        const int tamanhoIdCidade = 3;
        const int inicioIdCidade = 0;
        string nomeCidade;
        const int tamanhoNomeCidade = 15;
        const int inicioNomeCidade = tamanhoIdCidade + inicioIdCidade;
        int coordenadaX;
        const int tamanhoCoordenadaX = 5;
        const int inicioCoordenadaX = inicioNomeCidade + tamanhoNomeCidade;
        int coordenadaY;
        const int tamanhoCoordenadaY = 5;
        const int inicioCoordenadaY = inicioCoordenadaX + tamanhoCoordenadaX;
             
        public Cidade(int id, string nome, int x, int y)
        {
            idCidade = id;
            nomeCidade = nome;
            coordenadaX = x;
            coordenadaY = y;
        }

        public static Cidade LerArquivo(StreamReader arq)
        {
            string linha = arq.ReadLine();
            int idCidade = int.Parse(linha.Substring(inicioIdCidade, tamanhoIdCidade));
            string nomeCidade = linha.Substring(inicioNomeCidade, tamanhoNomeCidade);
            int coordenadaX = int.Parse(linha.Substring(inicioCoordenadaX, tamanhoCoordenadaX));
            int coordenadaY = int.Parse(linha.Substring(inicioCoordenadaY, tamanhoCoordenadaY));
            return new Cidade(idCidade, nomeCidade, coordenadaX, coordenadaY);
        }

        public int CompareTo(Cidade other)
        {
            return this.idCidade - other.idCidade;
        }
    }
}
