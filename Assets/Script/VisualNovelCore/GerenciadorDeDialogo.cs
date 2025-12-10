using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class LinhaDeDialogo
{
    public string idPersonagem;
    [TextArea] public string texto;
    public List<OpcaoDeDialogo> opcoes;
}

[Serializable]
public class OpcaoDeDialogo
{
    public string texto;
    public int proximo;
}

public class GerenciadorDeDialogo : MonoBehaviour
{
    #region ==== CONFIGURAÇÕES ====
    [Header("Delay antes do diálogo começar")]
    public float atrasoInicial = 1.5f;
    #endregion

    #region ==== REFERÊNCIAS DE UI ====
    public TMP_Text textoDialogo;
    public Transform containerOpcoes;
    public Button prefabOpcao;
    #endregion

    #region ==== DADOS DO DIÁLOGO ====
    public List<LinhaDeDialogo> linhas;
    #endregion

    #region ==== EVENTOS ====
    public Action<string> QuandoTrocarPersonagem;
    #endregion

    int indice = 0;
    Coroutine typewriterRodando;

    void Start()
    {
        StartCoroutine(FluxoInicial());
    }

    #region ==== FLUXO INICIAL COM DELAY ====
    IEnumerator FluxoInicial()
    {
        var highlighter = FindFirstObjectByType<RealcadorDePersonagem>();
        QuandoTrocarPersonagem += highlighter.Destacar;

        yield return new WaitForSeconds(atrasoInicial);

        MostrarLinha(0);
    }
    #endregion

    #region ==== MOSTRAR LINHA ====
    void MostrarLinha(int i)
    {
        indice = i;
        LimparOpcoes();

        var linha = linhas[indice];

        QuandoTrocarPersonagem?.Invoke(linha.idPersonagem);

        if (typewriterRodando != null)
            StopCoroutine(typewriterRodando);

        typewriterRodando = StartCoroutine(Typewriter(linha.texto));

        if (linha.opcoes == null || linha.opcoes.Count == 0)
            return;

        foreach (var opcao in linha.opcoes)
        {
            var b = Instantiate(prefabOpcao, containerOpcoes);
            b.GetComponentInChildren<TMP_Text>().text = opcao.texto;

            int proximo = opcao.proximo;
            b.onClick.AddListener(() => MostrarLinha(proximo));
        }
    }
    #endregion

    #region ==== TYPEWRITER ====
    IEnumerator Typewriter(string frase)
    {
        textoDialogo.text = "";
        foreach (char c in frase)
        {
            textoDialogo.text += c;
            yield return new WaitForSeconds(0.02f);
        }
    }
    #endregion

    #region ==== LIMPAR OPÇÕES ====
    void LimparOpcoes()
    {
        foreach (Transform t in containerOpcoes)
            Destroy(t.gameObject);
    }
    #endregion
}
