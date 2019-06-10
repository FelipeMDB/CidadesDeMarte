using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Arvore<Dado> : IComparable<Arvore<Dado>> where Dado : IComparable<Dado>
{
    protected NoArvore<Dado> raiz, atual, antecessor;
    protected int quantosNos;

    public Panel painelArvore;
    public Panel OndeExibir
    {
        get => painelArvore;
        set => painelArvore = value;
    }

    public NoArvore<Dado> Raiz
    {
        get { return raiz; }
        set { raiz = value; }
    }
    public NoArvore<Dado> Atual
    {
        get { return atual; }
        set { atual = value; }
    }
    public NoArvore<Dado> Antecessor
    {
        get { return antecessor; }
        set { antecessor = value; }
    }

    public bool Existe(Dado procurado)  // pesquisa binária não recursiva
    {
        antecessor = null;
        atual = Raiz;
        while (atual != null)
        {
            if (atual.Info.CompareTo(procurado) == 0)
                return true;
            else
            {
                antecessor = atual;
                if (procurado.CompareTo(atual.Info) < 0)
                    atual = atual.Esq; // Desloca à esquerda
                else
                    atual = atual.Dir; // Desloca à direita
            }
        }
        return false; // Se atual == null, a chave não existe mas antecessor aponta o pai 
    }

    public void Incluir(Dado incluido)    // inclusão usando o método de pesquisa binária
    {
        if (Existe(incluido))
            throw new Exception("Informação repetida");
        else
        {
            var novoNo = new NoArvore<Dado>(incluido);
            if (incluido.CompareTo(antecessor.Info) < 0)
                antecessor.Esq = novoNo;
            else
                antecessor.Dir = novoNo;
            quantosNos++;
        }
    }

    /// exercício 1
    private bool Eq(NoArvore<Dado> atualA, NoArvore<Dado> atualB)
    {
        if (atualA == null && atualB == null)
            return true;
        if ((atualA == null) != (atualB == null)) // apenas um dos nós é
            return false;                          // nulo
                                                   // os dois nós não são nulos
        if (atualA.Info.CompareTo(atualB.Info) != 0)
            return false; // infos diferentes
        return Eq(atualA.Esq, atualB.Esq) && Eq(atualA.Dir, atualB.Dir);
    }

    public bool EquivaleA(Arvore<Dado> outraArvore)
    {
        /* . ambas são vazias
        ou
        .. Info(A) = Info(B) e
        ... Esq(A) eq Esq(B) e Dir(A) eq Dir(B)
        */
        return Eq(this.Raiz, outraArvore.Raiz);
    }

    // if (arvoreA.Equivale(arvoreB)) ...   NA APLICAÇÂO


    // Exercício 2 – Contagem de nós em árvore, com função recursiva
    private int QtosNos(NoArvore<Dado> noAtual)
    {
        if (noAtual == null)
            return 0;
        return 1 +                    // conta o nó atual
               QtosNos(noAtual.Esq) + // conta nós da subárvore esquerda
               QtosNos(noAtual.Dir);  // conta nós da subárvore direita
    }
    public int QuantosDados { get => this.QtosNos(this.Raiz); }
    // Exercício 3 – Contagem de folhas de árvore, com função recursiva
    private int QtasFolhas(NoArvore<Dado> noAtual)
    {
        if (noAtual == null)
            return 0;
        if (noAtual.Esq == null && noAtual.Dir == null) // noAtual é folha
            return 1;
        // noAtual não é folha, portanto procuramos as folhas de cada ramo e as contamos
        return QtasFolhas(noAtual.Esq) + // conta folhas da subárvore esquerda
               QtasFolhas(noAtual.Dir);  // conta folhas da subárvore direita
    }
    public int QuantasFolhas { get => this.QtasFolhas(this.Raiz); }

    // na aplicação : ... if ( minhaArvore.QuantasFolhas > 0 ) ...

    // Exercício 4 – Árvore Estritamente Binária
    private bool EstritamenteBinaria(NoArvore<Dado> noAtual)
    {
        if (noAtual == null)
            return true;
        // noAtual não é nulo
        if (noAtual.Esq == null && noAtual.Dir == null)
            return true;
        // um dos descendentes é nulo e o outro não é
        if (noAtual.Esq == null && noAtual.Dir != null)
            return false;
        if (noAtual.Esq != null && noAtual.Dir == null)
            return false;
        // se chegamos aqui, nenhum dos descendentes é nulo, dai testamos a
        // "estrita binariedade" das duas subárvores descendentes do nó atual
        return EstritamenteBinaria(noAtual.Esq) && EstritamenteBinaria(noAtual.Dir);
    }

    public bool EstritamenteBinaria()
    {
        return this.EstritamenteBinaria(this.Raiz);
    }

    // Exercício 5 – Altura de uma árvore binária
    private int Altura(NoArvore<Dado> noAtual)
    {
        int alturaEsquerda,
            alturaDireita;

        if (noAtual == null)
            return 0;

        alturaEsquerda = Altura(noAtual.Esq);
        alturaDireita = Altura(noAtual.Dir);

        if (alturaEsquerda >= alturaDireita)
            return 1 + alturaEsquerda;
        return 1 + alturaDireita;
    }

    public int Altura()
    {
        return Altura(Raiz);
    }

    // Exercício 7 – Troca ramos esquerdo e direito entre si
    private void Espelhar(NoArvore<Dado> noAtual)
    {
        if (noAtual != null)
        {
            NoArvore<Dado> Auxiliar = noAtual.Esq;
            noAtual.Esq = noAtual.Dir;
            noAtual.Dir = Auxiliar;
            Espelhar(noAtual.Esq);
            Espelhar(noAtual.Dir);
        }
    }

    public void Espelhar()
    {
        this.Espelhar(this.Raiz);
    }

    // Exercício 8 – Percurso por níveis
    /*public void EscreverPorNiveis()
    {
        var umaFila = new FilaLista<NoArvore<Dado>>();

        NoArvore<Dado> noAtual = Raiz;
        while (noAtual != null)
        {
            if (noAtual.Esq != null)
                umaFila.Enfileirar(noAtual.Esq);
            if (noAtual.Dir != null)
                umaFila.Enfileirar(noAtual.Dir);
            Console.WriteLine(noAtual.Info);
            if (umaFila.EstaVazia())
                noAtual = null;
            else
                noAtual = umaFila.Retirar();
        }
    }

    public int CompareTo(Arvore<Dado> other)
    {
        throw new NotImplementedException();
    }*/
}
