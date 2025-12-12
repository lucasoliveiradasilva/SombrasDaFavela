using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class OpcaoDeDialogo
{
    public string texto;
    public int proximo;
}

[Serializable]
public class LinhaDeDialogo
{
    public string idPersonagem;
    [TextArea] public string texto;
    public List<OpcaoDeDialogo> opcoes;

    [Header("Essa linha termina a cena?")]
    public bool ehFinalDaCena;   // checkbox no inspector
}

public class GerenciadorDeDialogo : MonoBehaviour
{
    [Header("Delay antes do diálogo começar")]
    public float atrasoInicial = 1.5f;

    [Header("Referências UI")]
    public TMP_Text textoDialogo;
    public Transform containerOpcoes;
    public Button prefabOpcao;

    public List<LinhaDeDialogo> linhas;

    public Action<string> QuandoTrocarPersonagem;

    int indice = 0;
    Coroutine typewriterRodando;

    void Start()
    {
        StartCoroutine(FluxoInicial());
    }

    IEnumerator FluxoInicial()
    {
        // cuidado: FindFirstObjectByType requer Unity versão compatível (2023+). 
        // Se dar erro, troque por FindObjectOfType<RealcadorDePersonagem>().
        var highlighter = FindFirstObjectByType<RealcadorDePersonagem>();
        if (highlighter != null)
            QuandoTrocarPersonagem += highlighter.Destacar;

        yield return new WaitForSeconds(atrasoInicial);

        MostrarLinha(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TentarAvancar();
        }
    }

    void TentarAvancar()
    {
        if (linhas == null || linhas.Count == 0) return;

        var linha = linhas[indice];

        // se typewriter rodando, termina imediatamente
        if (typewriterRodando != null)
        {
            StopCoroutine(typewriterRodando);
            typewriterRodando = null;

            textoDialogo.text = linha.texto;
            return;
        }

        // Se for final da cena → troca de cena
        if (linha.ehFinalDaCena)
        {
            IrParaProximaCena();
            return;
        }

        // Se houver opções, o jogador deve clicar nelas
        if (linha.opcoes != null && linha.opcoes.Count > 0)
            return;

        MostrarProximaLinha();
    }

    void MostrarProximaLinha()
    {
        int proximo = indice + 1;

        if (proximo >= linhas.Count)
        {
            // chegou ao fim da lista: vai para próxima cena
            IrParaProximaCena();
            return;
        }

        MostrarLinha(proximo);
    }

    void MostrarLinha(int i)
    {
        if (linhas == null || linhas.Count == 0) return;
        if (i < 0 || i >= linhas.Count) return;

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
            var textoBtn = b.GetComponentInChildren<TMP_Text>();
            if (textoBtn != null) textoBtn.text = opcao.texto;

            int proximo = opcao.proximo;
            b.onClick.AddListener(() => MostrarLinha(proximo));
        }
    }

    IEnumerator Typewriter(string frase)
    {
        textoDialogo.text = "";
        foreach (char c in frase)
        {
            textoDialogo.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        typewriterRodando = null;
    }

    void LimparOpcoes()
    {
        if (containerOpcoes == null) return;
        foreach (Transform t in containerOpcoes)
            Destroy(t.gameObject);
    }

    void IrParaProximaCena()
    {
        int cenaAtual = SceneManager.GetActiveScene().buildIndex;
        // opcional: verificar se existe cena seguinte no Build Settings
        if (cenaAtual + 1 < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(cenaAtual + 1);
        else
            Debug.LogWarning("Não há próxima cena no Build Settings.");
    }
}
