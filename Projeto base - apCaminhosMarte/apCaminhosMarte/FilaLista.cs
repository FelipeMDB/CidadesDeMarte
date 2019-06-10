using System;
class FilaLista<Dado> : ListaSimples<Dado> where Dado : IComparable<Dado>
{
  public void Enfileirar(Dado elemento) // inclui objeto “elemento”
  {
    NoLista<Dado> novoNo = new NoLista<Dado>(elemento, null);
    base.InserirAposFim(novoNo);
  }
  public Dado Retirar() // devolve objeto do início e o
  {                     // retira da fila
    if (!EstaVazia())
    {
      Dado elemento = base.Primeiro.Info;
      base.RemoverNo(null, Primeiro);
      return elemento;
    }
    throw new FilaVaziaException("Fila vazia");
  }
  public Dado OInicio() // devolve objeto do início
  {                     // sem retirá-lo da fila
    if (EstaVazia())
      throw new FilaVaziaException("Fila vazia");
    Dado o = base.Primeiro.Info; // acessa o 1o objeto genérico
    return o;
  }
  public Dado OFim() // devolve objeto do fim
  {                  // sem retirá-lo da fila
    if (EstaVazia())
      throw new FilaVaziaException("Fila vazia");

    Dado o = base.Ultimo.Info; // acessa o 1o objeto genérico
    return o;
  }
  public int Tamanho() // devolve número de elementos da fila
  {
    return base.QuantosNos;  // tamanho da lista ligada herdada
  }
  public bool EstaVazia()
  {
    return base.EstaVazia;
  }
  public Dado[] ToArray()
  {
    Dado[] itens = new Dado[this.Tamanho()];
    int indice = 0;
    base.IniciarPercursoSequencial();
    while (base.PodePercorrer())
          itens[indice++] = base.Atual.Info;

    return itens;
  }
}